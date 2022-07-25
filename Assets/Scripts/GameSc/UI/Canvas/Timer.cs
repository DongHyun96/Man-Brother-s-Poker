using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public UnityEvent OnTimeOut;

    public Image fill;

    public bool IsTimerActive
    {
        set
        {
            if(value == true)
            {
                // Reset timer fill amount
                fill.fillAmount = 0f;

                // Set isSfxPlaying to false
                isSfxPlaying = false;
            }
            
            Animator animator = gameObject.GetComponent<Animator>();

            // Set Scalar animation
            if(gameObject.GetComponent<Animator>() != null)
            {
                if(animator.GetBool("isIn") != value)
                {
                    animator.SetBool("isIn", value);
                }
            }

            isTimerActive = value;
        }

        get => isTimerActive;
    }

    private bool isTimerActive = false;

    private const float t = 20f;

    [SerializeField] private Animator ActionTextAnim;
    
    private bool isSfxPlaying = false;

    private void FixedUpdate() 
    {
        if(isTimerActive)
        {
            if(fill.fillAmount < 1)
            {
                fill.fillAmount += Time.deltaTime * (1 / t); 

                if(fill.fillAmount >= 0.75f && !isSfxPlaying)
                {
                    // Play ticking sound
                    gameObject.GetComponent<AudioSource>().Play();
                    isSfxPlaying = true;
                }
            }
            else
            {
                OnTimeOut.Invoke();
                fill.fillAmount = 1f;
                IsTimerActive = false;
            }
        }
    }

    public void StopSfx()
    {
        gameObject.GetComponent<AudioSource>().Stop();
    }
}
