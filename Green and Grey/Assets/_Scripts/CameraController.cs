using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Objects")]
    public GridController gridController;
    public Transform cameraTransform;

    [Header("Movement")]
    public float movementSpeed;
    public float rotationAmount;
    public float movementTime;
    public Vector3 zoomAmount;

    private Vector3 newPosition;
    private Quaternion newRotation;
    private Vector3 newZoom;

    private Vector3 dragStartPosition;
    private Vector3 dragCurrentPostion;
    private Vector3 rotateStartPosion;
    private Vector3 rotateCurrentPositon;

    [Header("Camera Limits")]
    public Vector2 zoomLimits;
    public Vector2 horizontalLimits;
    public Vector2 verticalLimits;
    private bool canNavigate = false;

    void Awake()
    {
        // get movmentlimits
        horizontalLimits = new Vector2(0, gridController.gridSize.x);
        verticalLimits = new Vector2(0, gridController.gridSize.y);
    }

    public void InitCamController()
    {
        // set Start Position - TODO - later start position = enemy goal
        Debug.Log(gridController.startPos);

        //transform.position = new Vector3(gridController.gridSize.x / 2, transform.position.y, gridController.gridSize.y / 2);
        //transform.position = new Vector3(gridController.startPos.x, transform.position.y, gridController.startPos.y);

        // set positions
        newPosition = new Vector3(gridController.startPos.x, transform.position.y, gridController.startPos.y);
        newRotation = transform.rotation;
        newZoom = cameraTransform.localPosition;

        canNavigate = true;
    }

    private void LateUpdate()
    {
        if (canNavigate == true)
        {
            HandleMouseInput();
            HandleMovmentInput();
            CalculateCameraTransforms();
        }
    }

    void HandleMouseInput()
    {
        // handle mouse scrolling
        if(Input.mouseScrollDelta.y > 0) // zoom in 
        {
            if (newZoom.y > zoomLimits.x)
                newZoom += Input.mouseScrollDelta.y * zoomAmount*50;
        }
        if (Input.mouseScrollDelta.y < 0) // zoom out
        {
            if (newZoom.y < zoomLimits.y)
                newZoom += Input.mouseScrollDelta.y * zoomAmount * 50;
        }


        // handle mouse drag movement
        if (Input.GetMouseButtonDown(1))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float entry;

            if(plane.Raycast(ray, out entry))
            {
                dragStartPosition = ray.GetPoint(entry); 
            }
        }
        if (Input.GetMouseButton(1))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float entry;

            if (plane.Raycast(ray, out entry))
            {
                dragCurrentPostion = ray.GetPoint(entry);
                newPosition = transform.position + dragStartPosition - dragCurrentPostion;
            }
        }

        // handle mouse rotation
        if (Input.GetMouseButtonDown(2))
        {
            rotateStartPosion = Input.mousePosition;
        }
        if (Input.GetMouseButton(2))
        {
            rotateCurrentPositon = Input.mousePosition;
            Vector3 difference = rotateStartPosion - rotateCurrentPositon;
            rotateStartPosion = rotateCurrentPositon;

            newRotation *= Quaternion.Euler(Vector3.up * (-difference.x / 5f));
        }

    }

    void HandleMovmentInput()
    {

        //ToDoo with ItputAXES
        // handle movment
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            //if (newPosition.)
            newPosition += (transform.forward * movementSpeed);
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            newPosition += (transform.forward * -movementSpeed);
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            newPosition += (transform.right * movementSpeed);
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            newPosition += (transform.right * -movementSpeed);
        }

        // handle rotation
        if (Input.GetKey(KeyCode.Q))
        {
            newRotation *= Quaternion.Euler(Vector3.up * -rotationAmount);
            //newPosition += (transform.right * movementSpeed);
        }
        if (Input.GetKey(KeyCode.E))
        {
            newRotation *= Quaternion.Euler(Vector3.up * rotationAmount);
            //newPosition += (transform.right * -movementSpeed);
        }

        // handle zoom
        if (Input.GetKey(KeyCode.R)) // zoom in
        {
            if (newZoom.y > zoomLimits.x)
                newZoom += zoomAmount;
        }
        if (Input.GetKey(KeyCode.F)) // zoom out 
        {
            if (newZoom.y < zoomLimits.y)
                newZoom -= zoomAmount;
        }

        
    }

    private void CalculateCameraTransforms()
    {
        // limit positions
        newPosition = new Vector3(Mathf.Min(Mathf.Max(newPosition.x, horizontalLimits.x), horizontalLimits.y), transform.position.y, Mathf.Min(Mathf.Max(newPosition.z, verticalLimits.x), verticalLimits.y));

        // set transforms
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * movementTime);
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newZoom, Time.deltaTime * movementTime);
    }
}
