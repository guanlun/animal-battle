using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBehavior : MonoBehaviour
{
    public GameObject catPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnUnit()
    {
        GameObject spawnedUnit = Instantiate(catPrefab, this.transform.position, Quaternion.identity);
        CatBehavior spawnedUnitBehavior = spawnedUnit.GetComponent<CatBehavior>();

        spawnedUnitBehavior.MoveTo(this.transform.position + new Vector3(2, 0, 2));
    }
}
