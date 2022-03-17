using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
//using CinemachineCore;

/*
    0 2 5
    ZDamping initial value = 4
    after Login = 1.5
*/

public class VcamController : MonoBehaviour
{
    public CinemachineVirtualCamera currentCamera;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var dolly = currentCamera.GetCinemachineComponent<CinemachineTrackedDolly>();

        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            dolly.m_PathPosition += 1;
            //dolly.m_ZDamping = 1;
            //currentCamera.GetCinemachineComponent<Cine
        }
        
        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            dolly.m_PathPosition -= 1;
        }

    }

    public void Example()
    {
        print("WOW");
    }
}
