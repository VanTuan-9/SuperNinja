using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonScript : MonoBehaviour, IPointerDownHandler, IPointerExitHandler, IPointerUpHandler
{
    public int typeButton = 1; //1: left, 2: right, 3: jump, 4: attack
    private bool isHoverButton = false;

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        this.PostEvent(EventID.isMove, typeButton);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        if(typeButton != 3) {
            this.PostEvent(EventID.isNotMove);
        }
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        if(typeButton != 3) {
            this.PostEvent(EventID.isNotMove);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
