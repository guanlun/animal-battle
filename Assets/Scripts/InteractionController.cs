using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionController : MonoBehaviour
{
    public static Vector3 DIRECTION_UP = new Vector3(-1, 0, 1);
    public static Vector3 DIRECTION_LEFT = new Vector3(-1, 0, -1);
    public static Vector3 DIRECTION_DOWN = new Vector3(1, 0, -1);
    public static Vector3 DIRECTION_RIGHT = new Vector3(1, 0, 1);
    public static float CAMERA_MOVEMENT_SPEED = 0.1f;

    // properties for drag selection
    public RectTransform dragSelectionBox;
    private GameObject dragSelection;
    private MeshFilter dragSelectionMeshFilter;
    private bool isDragging = false;
    private float dragStartScreenPositionX;
    private float dragStartScreenPositionY;

    private List<CatBehavior> selectedCatBehaviors = new List<CatBehavior>();

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
            this.dragSelectionBox.gameObject.SetActive(true);
        }

        if (this.isDragging)
        {   
            float selectionBoxWidth = Input.mousePosition.x - this.dragStartScreenPositionX;
            float selectionBoxHeight = Input.mousePosition.y - this.dragStartScreenPositionY;

            this.dragSelectionBox.sizeDelta = new Vector2(Mathf.Abs(selectionBoxWidth), Mathf.Abs(selectionBoxHeight));
            this.dragSelectionBox.anchoredPosition = new Vector2(this.dragStartScreenPositionX + selectionBoxWidth / 2 - Screen.width / 2, this.dragStartScreenPositionY + selectionBoxHeight / 2 - Screen.height / 2);
        }

        bool leftMouseButtonUp = Input.GetMouseButtonUp(0);

        if (leftMouseButtonUp)
        {
            this.isDragging = false;
            this.dragSelectionBox.gameObject.SetActive(false);
            
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
                        foreach (CatBehavior selectedCatBehavior in this.selectedCatBehaviors)
                        {
                            selectedCatBehavior.MoveTo(hit.point);
                        }
                    }
                    else if (hitObject.CompareTag("Enemy"))
                    {
                        foreach (CatBehavior selectedCatBehavior in this.selectedCatBehaviors)
                        {
                            selectedCatBehavior.SetAttackTarget(hitObject);
                        }
                    }
                }
            }
        }
    }

    public void SelectUnit(CatBehavior catBehavior)
    {
        catBehavior.SetSelectedState(true);
        this.selectedCatBehaviors.Add(catBehavior);
    }

    private void ClearSelectedUnits()
    {
        foreach (CatBehavior selectedCatBehavior in this.selectedCatBehaviors)
        {
            selectedCatBehavior.SetSelectedState(false);
        }

        this.selectedCatBehaviors.Clear();
    }

    private Vector3 ComputeScreenPointAtFarPlane(float screenX, float screenY)
    {
        Vector3 cameraPosition = Camera.main.transform.position;
        return (Camera.main.ScreenToWorldPoint(new Vector3(screenX, screenY, Camera.main.nearClipPlane)) - cameraPosition) * 100 + cameraPosition;
    }
}
