using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blueprint : MonoBehaviour
{
    [SerializeField] BuildingSettings building;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] int rotationAmount;

    Renderer renderer;

    Quaternion newRot;
    // Start is called before the first frame update
    void Start()
    {
        newRot = transform.rotation;
        renderer = GetComponent<Renderer>();
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10000f, groundLayer))
        {
            transform.position = hit.point;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10000f, groundLayer))
        {
            transform.position = hit.point;
        }
        if (CanSpawnBuilding())
        {
            if (Input.GetMouseButton(0))
            {
                UnitGUI.instance.isBluePrintEnabled = false;
                ResourceManager.instance.ReduceAmount(building.woodCost, building.stoneCost, transform.position);
                Instantiate(building.prefab, transform.position, transform.rotation);
                Destroy(gameObject);
            }
        }
        RotateObject();

    }

    private void RotateObject() 
    {
        if (Input.GetKey(KeyCode.R))
        {
            newRot *= Quaternion.Euler(Vector3.up * rotationAmount);
        }
        else if (Input.GetKey(KeyCode.F))
        {
            newRot *= Quaternion.Euler(Vector3.up * -rotationAmount);
        }
        transform.rotation = Quaternion.Lerp(transform.rotation, newRot, 5 * Time.deltaTime);
    }
    /// <summary>
    /// Check if the blueprint is place in a valid space
    /// </summary>
    /// <returns>true for ok</returns>
    private bool CanSpawnBuilding()
    {
        BoxCollider blueprintCollider = GetComponent<BoxCollider>();
        Vector3 worldCenter = blueprintCollider.transform.TransformPoint(blueprintCollider.center);
        Vector3 worldHalfExtents = blueprintCollider.size * 0.5f;
        Collider[] colliders = Physics.OverlapBox(worldCenter, worldHalfExtents, transform.rotation, ~groundLayer);
        if (colliders.Length > 1) 
        {
            renderer.material.color = new Color32(225, 108, 73, 255);//change color to red
            return false;
        }
        else
        {
            renderer.material.color = new Color32(60, 108, 73, 255);//change color to green
            return true;
        }
    }

    private void OnDrawGizmos()
    {
        BoxCollider blueprintCollider = GetComponent<BoxCollider>();
        Color prevColor = Gizmos.color;
        Matrix4x4 prevMatrix = Gizmos.matrix;

        Gizmos.color = Color.red;
        Gizmos.matrix = transform.localToWorldMatrix;

        Vector3 boxPosition = blueprintCollider.transform.TransformPoint(blueprintCollider.center);

        // convert from world position to local position 
        boxPosition = transform.InverseTransformPoint(boxPosition);

        Vector3 boxSize = blueprintCollider.size;
        Gizmos.DrawWireCube(boxPosition, boxSize);

        // restore previous Gizmos settings
        Gizmos.color = prevColor;
        Gizmos.matrix = prevMatrix;
    }
}
