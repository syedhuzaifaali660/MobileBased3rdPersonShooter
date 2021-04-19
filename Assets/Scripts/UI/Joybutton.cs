using UnityEngine.EventSystems;
using UnityEngine;

public class Joybutton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    [HideInInspector]
    public bool Pressed;

    public void OnPointerDown(PointerEventData eventData)
    {
        Pressed = true;
        Debug.Log(Pressed);
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Pressed = false;
       
    }

}
