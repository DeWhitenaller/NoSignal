using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Button button;
    public Sprite img;
    public Sprite imgNew;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("HI");
        button.image.sprite = img;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("Bye");
        button.image.sprite = imgNew;
    }
}
