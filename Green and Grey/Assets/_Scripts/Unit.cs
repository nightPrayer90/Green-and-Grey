using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private Rigidbody unitRB;
    public float moveSpeed = 1f;
    private GridController gridController;

    // Start is called before the first frame update
    void Start()
    {
        unitRB = GetComponent<Rigidbody>();
        gridController = GameObject.Find("GridController").GetComponent<GridController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
            Cell cellBelow = gridController.curFlowField.GetCellFromWorldPos(transform.position);
            Vector3 moveDirection = new Vector3(cellBelow.bestDirection.Vector.x, 0, cellBelow.bestDirection.Vector.y);

            unitRB.velocity = moveDirection * moveSpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "stop")
        {
            Destroy(gameObject);
        }
    }
}
