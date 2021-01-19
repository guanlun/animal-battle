using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class InteractionController : MonoBehaviour
{
    public static Vector3 DIRECTION_UP = new Vector3(-1, 0, 1).normalized;
    public static Vector3 DIRECTION_LEFT = new Vector3(-1, 0, -1).normalized;
    public static Vector3 DIRECTION_DOWN = new Vector3(1, 0, -1).normalized;
    public static Vector3 DIRECTION_RIGHT = new Vector3(1, 0, 1).normalized;
    public static float CAMERA_MOVEMENT_SPEED = 0.1f;

    // properties for drag selection
    public RectTransform dragSelectionBox;
    private GameObject dragSelection;
    private MeshFilter dragSelectionMeshFilter;
    private bool isDragging = false;
    private float dragStartScreenPositionX;
    private float dragStartScreenPositionY;

    private List<GameObject> selectedUnits = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        this.dragSelection = GameObject.Find("DragSelectionMesh");
        this.dragSelectionMeshFilter = this.dragSelection.GetComponent<MeshFilter>();

        this.dragSelectionBox.gameObject.SetActive(false);

        // TODO: remove the initial values
        this.dragSelectionMeshFilter.mesh.vertices = new Vector3[5]
        {
            Camera.main.transform.position,
            new Vector3(1, 1, 1),
            new Vector3(1, 2, 1),
            new Vector3(2, 2, 1),
            new Vector3(2, 1, 1),
        };
        this.dragSelectionMeshFilter.mesh.triangles = new int[18]
        {
            0, 1, 2,
            0, 1, 3,
            0, 2, 4,
            0, 3, 4,
            1, 2, 3,
            2, 4, 3
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            Camera.main.transform.position += DIRECTION_UP * CAMERA_MOVEMENT_SPEED;
        }

        if (Input.GetKey(KeyCode.A))
        {
            Camera.main.transform.position += DIRECTION_LEFT * CAMERA_MOVEMENT_SPEED;
        }

        if (Input.GetKey(KeyCode.S))
        {
            Camera.main.transform.position += DIRECTION_DOWN * CAMERA_MOVEMENT_SPEED;
        }

        if (Input.GetKey(KeyCode.D))
        {
            Camera.main.transform.position += DIRECTION_RIGHT * CAMERA_MOVEMENT_SPEED;
        }

        bool leftMouseButtonDown = Input.GetMouseButtonDown(0);
        if (leftMouseButtonDown)
        {
            this.isDragging = true;
            this.dragStartScreenPositionX = Input.mousePosition.x;
            this.dragStartScreenPositionY = Input.mousePosition.y;
            
        }

        if (this.isDragging)
        {   
            float selectionBoxWidth = Input.mousePosition.x - this.dragStartScreenPositionX;
            float selectionBoxHeight = Input.mousePosition.y - this.dragStartScreenPositionY;

            if (selectionBoxWidth != 0 && selectionBoxHeight != 0 && !this.dragSelectionBox.gameObject.activeSelf)
            {
                this.dragSelectionBox.gameObject.SetActive(true);
            }

            this.dragSelectionBox.sizeDelta = new Vector2(Mathf.Abs(selectionBoxWidth), Mathf.Abs(selectionBoxHeight));
            this.dragSelectionBox.anchoredPosition = new Vector2(this.dragStartScreenPositionX + selectionBoxWidth / 2 - Screen.width / 2, this.dragStartScreenPositionY + selectionBoxHeight / 2 - Screen.height / 2);
        }

        bool leftMouseButtonUp = Input.GetMouseButtonUp(0);

        if (leftMouseButtonUp)
        {
            this.isDragging = false;
            this.dragSelectionBox.gameObject.SetActive(false);

            if (Input.mousePosition.x != this.dragStartScreenPositionX && Input.mousePosition.y != this.dragStartScreenPositionY)
            {
                this.dragSelectionMeshFilter.mesh.vertices = new Vector3[5]
                {
                    Camera.main.transform.position,
                    this.ComputeScreenPointAtFarPlane(this.dragStartScreenPositionX, this.dragStartScreenPositionY),
                    this.ComputeScreenPointAtFarPlane(Input.mousePosition.x, this.dragStartScreenPositionY),
                    this.ComputeScreenPointAtFarPlane(this.dragStartScreenPositionX, Input.mousePosition.y),
                    this.ComputeScreenPointAtFarPlane(Input.mousePosition.x, Input.mousePosition.y)
                };
                this.dragSelectionMeshFilter.mesh.RecalculateBounds();

                MeshCollider existingCollider = this.dragSelection.GetComponent<MeshCollider>();
                if (existingCollider)
                {
                    Destroy(existingCollider);
                }

                MeshCollider dragSelectionCollider = this.dragSelection.AddComponent<MeshCollider>();
                dragSelectionCollider.convex = true;
                dragSelectionCollider.isTrigger = true;

                this.ClearSelectedUnits();
            }
        }

        bool rightMouseButtonUp = Input.GetMouseButtonUp(1);

        if (leftMouseButtonUp || rightMouseButtonUp)
        {
            RaycastHit hit;

            // Pass QueryTriggerInteraction.Ignore to avoid hitting trigger colliders
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                GameObject hitObject = hit.collider.gameObject;
                
                if (leftMouseButtonUp)
                {
                    //if (hitObject.CompareTag("Friendly"))
                    //{
                    //    // clear current selection first
                    //    this.ClearSelectedUnits();

                    //    CatBehavior catBehavior = hitObject.GetComponent<CatBehavior>();
                    //    catBehavior.SetSelectedState(true);
                    //    this.selectedCatBehaviors.Add(catBehavior);
                    //}
                }
                else if (rightMouseButtonUp)
                {
                    if (hitObject.isStatic)
                    {
                        this.MoveUnitsToDestination(hit.point);
                    }
                    else if (hitObject.CompareTag("Enemy"))
                    {
                        foreach (GameObject selectedUnit in this.selectedUnits)
                        {
                            selectedUnit.GetComponent<CatBehavior>().SetAttackTarget(hitObject);
                        }
                    }
                }
            }
        }
    }

    public void SelectUnit(GameObject unit)
    {
        unit.GetComponent<CatBehavior>().SetSelectedState(true);
        this.selectedUnits.Add(unit);
    }

    private void ClearSelectedUnits()
    {
        foreach (GameObject selectedUnit in this.selectedUnits)
        {
            selectedUnit.GetComponent<CatBehavior>().SetSelectedState(false);
        }

        this.selectedUnits.Clear();
    }

    private Vector3 ComputeScreenPointAtFarPlane(float screenX, float screenY)
    {
        Vector3 cameraPosition = Camera.main.transform.position;
        return (Camera.main.ScreenToWorldPoint(new Vector3(screenX, screenY, Camera.main.nearClipPlane)) - cameraPosition) * 100 + cameraPosition;
    }

    private void MoveUnitsToDestination(Vector3 desiredDestination)
    {
        float coefficient = 1f;

        Debug.Log(this.selectedUnits.Count);

        // TODO: sort selected units by distance to desination and move the closest ones near the center
        if (this.selectedUnits.Count == 1)
        {
            this.selectedUnits[0].GetComponent<CatBehavior>().MoveTo(desiredDestination);
        }
        else if (this.selectedUnits.Count == 2)
        {
            this.selectedUnits[0].GetComponent<CatBehavior>().MoveTo(desiredDestination + DIRECTION_RIGHT * coefficient);
            this.selectedUnits[1].GetComponent<CatBehavior>().MoveTo(desiredDestination + DIRECTION_LEFT * coefficient);
        }
        else if (this.selectedUnits.Count == 3)
        {
            this.selectedUnits[0].GetComponent<CatBehavior>().MoveTo(desiredDestination + DIRECTION_UP * coefficient);
            this.selectedUnits[1].GetComponent<CatBehavior>().MoveTo(desiredDestination + (DIRECTION_LEFT * 0.866f + DIRECTION_DOWN * 0.5f).normalized * coefficient);
            this.selectedUnits[2].GetComponent<CatBehavior>().MoveTo(desiredDestination + (DIRECTION_RIGHT * 0.866f + DIRECTION_DOWN * 0.5f).normalized * coefficient);
        }
        else if (this.selectedUnits.Count >= 4)
        {
            Vector3 topLeftBasePosition = desiredDestination + (DIRECTION_UP + DIRECTION_LEFT).normalized * coefficient;
            Vector3 topRightBasePosition = desiredDestination + (DIRECTION_UP + DIRECTION_RIGHT).normalized * coefficient;
            Vector3 bottomLeftBasePosition = desiredDestination + (DIRECTION_DOWN + DIRECTION_LEFT).normalized * coefficient;
            Vector3 bottomRightBasePosition = desiredDestination + (DIRECTION_DOWN + DIRECTION_RIGHT).normalized * coefficient;

            this.selectedUnits[0].GetComponent<CatBehavior>().MoveTo(topLeftBasePosition);
            this.selectedUnits[1].GetComponent<CatBehavior>().MoveTo(topRightBasePosition);
            this.selectedUnits[2].GetComponent<CatBehavior>().MoveTo(bottomLeftBasePosition);
            this.selectedUnits[3].GetComponent<CatBehavior>().MoveTo(bottomRightBasePosition);

            int currentLayerNum = 1;
            int unitIndex = 3;

            while (true)
            {
                for (int i = 0; i < currentLayerNum * 2; i++)
                {
                    if (++unitIndex == this.selectedUnits.Count)
                    {
                        return;
                    }

                    this.selectedUnits[unitIndex].GetComponent<CatBehavior>().MoveTo(topLeftBasePosition + DIRECTION_UP * coefficient * 1.41f + DIRECTION_RIGHT * coefficient * 1.41f * i);
                }

                for (int i = 0; i < currentLayerNum * 2; i++)
                {
                    if (++unitIndex == this.selectedUnits.Count)
                    {
                        return;
                    }

                    this.selectedUnits[unitIndex].GetComponent<CatBehavior>().MoveTo(topRightBasePosition + DIRECTION_RIGHT * coefficient * 1.41f + DIRECTION_DOWN * coefficient * 1.41f * i);
                }

                for (int i = 0; i < currentLayerNum * 2; i++)
                {
                    if (++unitIndex == this.selectedUnits.Count)
                    {
                        return;
                    }

                    this.selectedUnits[unitIndex].GetComponent<CatBehavior>().MoveTo(topLeftBasePosition + DIRECTION_LEFT * coefficient * 1.41f + DIRECTION_DOWN * coefficient * 1.41f * i);
                }

                for (int i = 0; i < currentLayerNum * 2; i++)
                {
                    if (++unitIndex == this.selectedUnits.Count)
                    {
                        return;
                    }

                    this.selectedUnits[unitIndex].GetComponent<CatBehavior>().MoveTo(bottomLeftBasePosition + DIRECTION_DOWN * coefficient * 1.41f + DIRECTION_RIGHT * coefficient * 1.41f * i);
                }

                if (++unitIndex == this.selectedUnits.Count)
                {
                    return;
                }
                topLeftBasePosition = topLeftBasePosition + DIRECTION_UP * coefficient * 1.41f + DIRECTION_LEFT * coefficient * 1.41f;
                this.selectedUnits[unitIndex].GetComponent<CatBehavior>().MoveTo(topLeftBasePosition);

                if (++unitIndex == this.selectedUnits.Count)
                {
                    return;
                }
                topRightBasePosition = topRightBasePosition + DIRECTION_UP * coefficient * 1.41f + DIRECTION_RIGHT * coefficient * 1.41f;
                this.selectedUnits[unitIndex].GetComponent<CatBehavior>().MoveTo(topRightBasePosition);

                if (++unitIndex == this.selectedUnits.Count)
                {
                    return;
                }
                bottomLeftBasePosition = bottomLeftBasePosition + DIRECTION_DOWN * coefficient * 1.41f + DIRECTION_LEFT * coefficient * 1.41f;
                this.selectedUnits[unitIndex].GetComponent<CatBehavior>().MoveTo(bottomLeftBasePosition);

                if (++unitIndex == this.selectedUnits.Count)
                {
                    return;
                }
                bottomRightBasePosition = bottomRightBasePosition + DIRECTION_DOWN * coefficient * 1.41f + DIRECTION_RIGHT * coefficient * 1.41f;
                this.selectedUnits[unitIndex].GetComponent<CatBehavior>().MoveTo(bottomRightBasePosition);

                currentLayerNum++;
            }
        }
    }
}
