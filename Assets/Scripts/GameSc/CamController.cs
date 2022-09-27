using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    public bool isMovable{ get; private set; }
    private bool isLerpingToPrev = false;

    [SerializeField] private Transform centralAxis;
    [SerializeField] private float camSpeed;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private List<Transform> playersTransform = new List<Transform>(); 
    [SerializeField] private Transform previousCamPos;
    private int playerPosIdx;

    private float mouseX;
    private float mouseY;

    private const int ROT_MIN = 0;
    private const float ROT_MAX = 35f;
    private const float lerpT = 0.07f;

    private void Awake() {
        isMovable = true;
    }

    /* Pin camera by player's mouse input function */
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

    public void SetCamToWinnerPos(int idx)
    {
        previousCamPos.position = mainCamera.transform.position;
        previousCamPos.rotation = mainCamera.transform.rotation;
        playerPosIdx = idx;
        isMovable = false;
    }
    
    public void SetCamToPrev()
    {
        isLerpingToPrev = true;
    }

    // Control pin cam here
    private void Update() {
        if(isMovable)
        {
            CamMove(); // Rotating cam
        }
    }

    // Control cam lerp when pot is finished with only one winner
    private void FixedUpdate() 
    {
        if(!isMovable && !isLerpingToPrev)
        {
            // Lerp
            Vector3 targetPos = playersTransform[playerPosIdx].position;
            Quaternion targetRot = playersTransform[playerPosIdx].rotation;

            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPos, lerpT);
            mainCamera.transform.rotation = Quaternion.Lerp(mainCamera.transform.rotation, targetRot, lerpT);
        }

        // Lerping to origin position
        if(isLerpingToPrev)
        {
            // Lerp
            Vector3 targetPos = previousCamPos.position;
            Quaternion targetRot = previousCamPos.rotation;

            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPos, lerpT);
            mainCamera.transform.rotation = Quaternion.Lerp(mainCamera.transform.rotation, targetRot, lerpT);

            // Reach target pos
            if(Vector3.Distance(mainCamera.transform.position, targetPos) < 0.01f)
            {
                // Lerping is over
                mainCamera.transform.position = targetPos;
                mainCamera.transform.rotation = targetRot;
                isMovable = true;
                isLerpingToPrev = false;
            }

            /* if(mainCamera.transform.position == targetPos && mainCamera.transform.rotation == targetRot)
            {
                // Lerping is over
                isMovable = true;
                isLerpingToPrev = false;
            } */
        }
        
    }
}
