using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObject : MonoBehaviour
{
    [SerializeField] Transform spawnPoint;
    [SerializeField] GameObject spawnObject;
    public void Spawn()
    {
        Instantiate(spawnObject, spawnPoint.position, spawnPoint.rotation);
    }
    
}
