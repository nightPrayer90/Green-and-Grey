using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementIndicator : MonoBehaviour
{
    // Start is called before the first frame update
    public BoxCollider indicatorCollider;
    public bool isCollision = false;

    private void OnEnable()
    {
        isCollision = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
            isCollision = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
            isCollision = false;
    }
}
