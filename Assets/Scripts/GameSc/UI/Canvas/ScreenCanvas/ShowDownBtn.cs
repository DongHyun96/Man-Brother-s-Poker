using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShowDownBtn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject pairCard;

    private const float BUTTON_SCALE = 1.2f;

    private Vector2 effectDist = new Vector2(6f, -6f);

    private Color outlineColor = new Color(1f, 0f, 237 / 255f, 1f);
    
    public void OnPointerEnter(PointerEventData data)
    {
        // Scale up
        transform.localScale = Vector3.one * BUTTON_SCALE;
        pairCard.transform.localScale = Vector3.one * BUTTON_SCALE;

        // Change Outline color and make it thicker
        this.GetComponent<Outline>().effectDistance = effectDist;
        this.GetComponent<Outline>().effectColor = outlineColor;
        pairCard.GetComponent<Outline>().effectDistance = effectDist;
        pairCard.GetComponent<Outline>().effectColor = outlineColor;
    }

    public void OnPointerExit(PointerEventData data)
    {
        // Scale down
        transform.localScale = Vector3.one;
        pairCard.transform.localScale = Vector3.one;

        // Change Outline color and make it thinner
        this.GetComponent<Outline>().effectDistance = new Vector2(2, -2);
        this.GetComponent<Outline>().effectColor = Color.black;
        pairCard.GetComponent<Outline>().effectDistance = new Vector2(2, -2);
        pairCard.GetComponent<Outline>().effectColor = Color.black;
    }



}
