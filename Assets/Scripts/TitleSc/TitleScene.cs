using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleScene : MonoBehaviour
{
    public Image mask;

    public GameObject titleObj;
    public GameObject wordObj;

    private void Start() {
        // Initialize and connect websockets
        MainMsgHandler.GetInstance();
    }
    
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //StartCoroutine("FadeRoutine");
            //SceneManager.LoadScene(1);
            LoadingSceneController.LoadScene("EnteringScene");
        }
    }

    
    IEnumerator FadeRoutine()
    {
        Color startColor = mask.color;

        // Fade out
        for(int i = 0; i<100; i++)
        {
            startColor.a = startColor.a + 0.01f;
            mask.color = startColor;
            yield return new WaitForSeconds(0.005f);
        }

        // Go to next scene
        SceneManager.LoadScene(1);
    }
 
    
}
