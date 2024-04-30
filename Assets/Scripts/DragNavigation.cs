using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using TouchPhase = UnityEngine.TouchPhase;


public class DragNavigation : MonoBehaviour
{
    
    
    Views currentView;
    
    public enum Views
    {
        MainView,
        MissionView
    }
    
    Vector2 dragStartPosition;
    Vector2 dragEndPosition;
    Vector2 deltaPosition;
    
    public Vector2 viewOffset;
    
    public float transitionSpeed = 0.1f; // Speed of the transition
    private Vector3 targetPosition; // Target position for the transition
    private bool isTransitioning = false; // Whether a transition is in progress
    
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
                    SwipeRight();
                else if(deltaPosition.x < -250)
                    SwipeLeft();
                if(deltaPosition.y > 250)
                    SwipeUp();
                else if(deltaPosition.y < -250)
                    SwipeDown();
            }
        }
        
        if (isTransitioning)
        {
            RectTransform rect = GameObject.Find("Views").GetComponent<RectTransform>();
            rect.position = Vector3.Lerp(rect.position, targetPosition, transitionSpeed);
            if (Vector3.Distance(rect.position, targetPosition) < 0.01f)
            {
                rect.position = targetPosition;
                isTransitioning = false;
            }
        }
    }

    #region Swipes

    public void SwipeLeft()
    {
        Debug.Log("Left");
        if(currentView == Views.MainView)
        {
            currentView = Views.MissionView;
            targetPosition = new Vector3(viewOffset.x - 1200, viewOffset.y, 0);
            isTransitioning = true;
        }
    }
    
    public void SwipeRight()
    {
        Debug.Log("Right");
        if(currentView == Views.MissionView)
        {
            currentView = Views.MainView;
            targetPosition = new Vector3(viewOffset.x, viewOffset.y, 0);
            isTransitioning = true;
        }
        
    }
    
    public void SwipeUp()
    {
        Debug.Log("Up");
    }
    
    public void SwipeDown()
    {
        Debug.Log("Down");
    }

    #endregion
    
    public void ViewTransition()
    {
        
    }
    
}
