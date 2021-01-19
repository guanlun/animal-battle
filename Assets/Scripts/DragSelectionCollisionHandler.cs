using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragSelectionCollisionHandler : MonoBehaviour
{
    public InteractionController interactionController;

    // Start is called before the first frame update
    void Start()
    {
        this.interactionController = GameObject.Find("InteractionController").GetComponent<InteractionController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Friendly"))
        {
            interactionController.SelectUnit(other.gameObject);
        }
    }
}
