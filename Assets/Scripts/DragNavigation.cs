using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TouchPhase = UnityEngine.TouchPhase;


public class DragNavigation : MonoBehaviour
{
    Vector2 dragStartPosition;
    Vector2 dragEndPosition;
    Vector2 deltaPosition;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                dragStartPosition = touch.position;

                
            }
            
            else if (touch.phase == TouchPhase.Moved)
            {
                dragEndPosition = touch.position;
                deltaPosition = dragEndPosition - dragStartPosition;
                if(deltaPosition.x > 250)
                    Debug.Log("Right");
                else if(deltaPosition.x < -250)
                    Debug.Log("Left");
                if(deltaPosition.y > 250)
                    Debug.Log("Up");
                else if(deltaPosition.y < -250)
                    Debug.Log("Down");
            }
            
            
        }
    }
}
