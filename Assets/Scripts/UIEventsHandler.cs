using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEventsHandler : MonoBehaviour
{
    public GameObject building;

    private BuildingBehavior buildingBehavior;

    // Start is called before the first frame update
    void Start()
    {
        Button[] buttons = FindObjectsOfType<Button>();
        foreach (Button button in buttons) {
            button.onClick.AddListener(HandleButton1Click);
        }

        this.buildingBehavior = building.GetComponent<BuildingBehavior>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HandleButton1Click()
    {
        this.buildingBehavior.SpawnUnit();
    }
}
