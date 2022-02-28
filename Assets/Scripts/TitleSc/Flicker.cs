using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Flicker : MonoBehaviour
{

    private float flickSpeed = 0.8f;

    public Image border;
    public Text text;

    // Update is called once per frame
    void Update()
    {
        border.color = updateColor(border.color);
        text.color = updateColor(text.color);
    }

    private bool flag = false; // false - go down, true - go up

    private Color updateColor(Color c)
    {
        if(c.a >= 1f)
            flag = false;
        else if (c.a <= 0f)
            flag = true;
        
        if(!flag)
        {
            c.a -= flickSpeed * Time.deltaTime;
        }
        else
        {
            c.a += flickSpeed * Time.deltaTime;
        }       
        return c;

    }
}
