using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandRing : MonoBehaviour
{
    [SerializeField] float destroyDelay;
    GameObject[] previusCommand;
    // Update is called once per frame
    private void Start()
    {
        Destroy(gameObject, destroyDelay);
        previusCommand = GameObject.FindGameObjectsWithTag("TargetCommand");
        if (previusCommand.Length > 1)
            Destroy(previusCommand[0]);

    }
    void Update()
    {
        transform.Rotate(Vector3.forward);
    }
}
