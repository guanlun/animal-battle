using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class InventoryItem : MonoBehaviour
{
    protected Inventory inventory;
    public Sprite inventorySprite;

    // Start is called before the first frame update
    void Start()
    {
        this.inventory = FindObjectOfType<Inventory>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    abstract public void Use();

    public void TryPickUp()
    {
        this.inventory.StoreItem(this);
    }
}
