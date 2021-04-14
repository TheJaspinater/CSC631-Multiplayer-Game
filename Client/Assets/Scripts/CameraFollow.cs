using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    private GameObject cam;
    void Start()
    {
        cam = GameObject.Find("Camera");
        Vector3 p = this.gameObject.transform.position;
        cam.transform.position = p;
    }

    void FixedUpdate()
    {
        Vector3 p = this.gameObject.transform.position;
        p.z = -20;
        cam.transform.position = p;

    }
}
