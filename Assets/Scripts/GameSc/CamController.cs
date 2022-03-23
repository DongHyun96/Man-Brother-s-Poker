using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    public Transform centralAxis;
    public float camSpeed;

    private float mouseX;
    private float mouseY;

    private const int ROT_MIN = 0;
    private const float ROT_MAX = 35f;

    void CamMove()
    {
        if(Input.GetMouseButton(1))
        {
            mouseX += Input.GetAxis("Mouse X");
            mouseY += Input.GetAxis("Mouse Y") * -1;

            float rotX = Mathf.Clamp((centralAxis.rotation.x + mouseY) * camSpeed, ROT_MIN, ROT_MAX);
            float rotY = (centralAxis.rotation.y + mouseX)* camSpeed;

            // Rotate centralAxis
            centralAxis.rotation = Quaternion.Euler(new Vector3(rotX, rotY, 0));

  
        }
    }

    private void Update() {

        CamMove(); // Rotating cam
    }
}
