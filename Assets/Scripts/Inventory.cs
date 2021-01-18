using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    protected GameObject itemBar;
    protected GameObject inventoryWindow;
    protected List<InventoryItem> items = new List<InventoryItem>();
    protected Image[] itemImages;

    // Start is called before the first frame update
    void Start()
    {
        this.itemBar = GameObject.Find("ItemBar");
        this.inventoryWindow = GameObject.Find("InventoryWindow");
        this.inventoryWindow.SetActive(false);

        itemImages = itemBar.transform.GetComponentsInChildren<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            this.itemImages[0].sprite = null;
        }

        if (Input.GetKeyUp(KeyCode.I))
        {
            this.inventoryWindow.SetActive(!this.inventoryWindow.activeSelf);
        }
    }

    public void StoreItem(InventoryItem item)
    {
        this.items.Add(item);
        this.itemImages[0].sprite = item.inventorySprite;
    }
}
