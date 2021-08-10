using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    //Singelton
    public static CameraMovement instance;

    //Follow a specific Object
    public Transform followTransform;
    [SerializeField] Transform cameraTrn;

 
    [SerializeField] float normalSpeed;
    [SerializeField] float fastSpeed;
    [SerializeField] float movementTime;
    [SerializeField] float rotationAmount;
    [SerializeField] Vector3 zoomAmount;
    [SerializeField] float minZoomZ;
    [SerializeField] float maxZoomZ;
    [SerializeField] float minZoomY;
    [SerializeField] float maxZoomY;
    [SerializeField] Vector2 clampX;
    [SerializeField] Vector2 clampZ;

    float movementSpeed;

    Vector3 newPos;
    Quaternion newRot;
    Vector3 newZoom;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        newPos = transform.position;
        newRot = transform.rotation;
        newZoom = cameraTrn.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (followTransform != null)
        {
            transform.position = Vector3.Lerp(transform.position, followTransform.position, movementTime * Time.deltaTime);
            if (Input.GetKey(KeyCode.Escape) || !followTransform.GetComponent<UnitEngine>().unit.isSelected || SelectionManager.instance.selectedUnits.Count > 1)
                followTransform = null;
        }
        else
            CheckMovement();
        CheckForZoom();
        CheckRotation();
    }

    /// <summary>
    /// Camera Zoom In/Out 
    /// </summary>
    private void CheckForZoom()
    {
        if (Input.mouseScrollDelta.y != 0) 
        {
            newZoom += Input.mouseScrollDelta.y * zoomAmount;
        }
        newZoom.y = Mathf.Clamp(newZoom.y, minZoomY, maxZoomY);
        newZoom.z = Mathf.Clamp(newZoom.z, minZoomZ, maxZoomZ);
        cameraTrn.localPosition = Vector3.Lerp(cameraTrn.localPosition, newZoom, movementTime * Time.deltaTime);
    }
    /// <summary>
    /// Rotate Camera 
    /// </summary>
    private void CheckRotation()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            newRot *= Quaternion.Euler(Vector3.up * -rotationAmount);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            newRot *= Quaternion.Euler(Vector3.up * rotationAmount);
        }
        transform.rotation = Quaternion.Lerp(transform.rotation, newRot, movementTime * Time.deltaTime);
    }
    /// <summary>
    /// Move Camera By Command
    /// </summary>
    private void CheckMovement()
    {
        float xMousePos = Input.mousePosition.x;
        float yMousePos = Input.mousePosition.y;
        if (Input.GetKey(KeyCode.LeftShift))
            movementSpeed = fastSpeed;
        else
            movementSpeed = normalSpeed;
        if (Input.GetKey(KeyCode.A) || xMousePos > 0 && xMousePos < 15 || Input.GetKey(KeyCode.LeftArrow))
        {
            newPos += (transform.right * -movementSpeed);
        }
        if (Input.GetKey(KeyCode.D) || xMousePos < Screen.width && xMousePos > Screen.width - 15 || Input.GetKey(KeyCode.RightArrow))
        {
            newPos += (transform.right * movementSpeed);
        }
        if (Input.GetKey(KeyCode.W) || yMousePos < Screen.height && yMousePos > Screen.height - 15 || Input.GetKey(KeyCode.UpArrow))
        {
            newPos += (transform.forward * movementSpeed);
        }
        if (Input.GetKey(KeyCode.S) || yMousePos < 15 && yMousePos > 0 || Input.GetKey(KeyCode.DownArrow))
        {
            newPos += (transform.forward * -movementSpeed);
        }
        newPos.x = Mathf.Clamp(newPos.x, clampX.x, clampX.y);
        newPos.z = Mathf.Clamp(newPos.z, clampZ.x, clampZ.y);
        transform.position = Vector3.Lerp(transform.position, newPos, movementTime * Time.deltaTime);
    }

    /// <summary>
    /// Camera Follows the selected unit(Max 1 unit selected)
    /// </summary>
    public void FollowUnit()
    {
        if (SelectionManager.instance.selectedUnits.Count == 1)
            followTransform = SelectionManager.instance.selectedUnits[0].transform;
    }
}
