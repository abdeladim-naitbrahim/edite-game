using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class speedbutton : MonoBehaviour, IPointerDownHandler,IPointerUpHandler
{
    Button button;



    public  void OnPointerDown(PointerEventData eventData)
    {     // if(gameObject.CompareTag("butonspeed"))
        SnakeMovement.speedflag=true;
        Debug.Log("The mouse down was released");
        //show text
    }

    public  void OnPointerUp(PointerEventData eventData)
    {    //if(gameObject.CompareTag("butonspeed"))
         SnakeMovement.speedflag=false;
          Debug.Log("The mouse click was released");
        //hide text
    }
}