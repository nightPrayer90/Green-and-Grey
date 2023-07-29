using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    public GameObject objectToSpawn;
    public float spawnInterval;
   

    private void Start()
    {
        InvokeRepeating("SpawnUnit",8f, spawnInterval);
    }

    private void SpawnUnit()
    {
        Instantiate(objectToSpawn, transform.position, transform.rotation);

    }
}
