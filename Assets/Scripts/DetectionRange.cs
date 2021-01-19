using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionRange : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        this.gameObject.GetComponentInParent<CatBehavior>().OnDetectionRangeTriggerEnter(other);
    }
}
