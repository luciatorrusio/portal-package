using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move : MonoBehaviour, IPortal
{
    private float speed = 1;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.UpArrow))
            gameObject.transform.position+= Vector3.forward*speed * Time.deltaTime ;
        if(Input.GetKey(KeyCode.LeftArrow))
            gameObject.transform.position+= -Vector3.right*speed * Time.deltaTime ;
        if(Input.GetKey(KeyCode.DownArrow))
            gameObject.transform.position+= -Vector3.forward*speed * Time.deltaTime ;
        if(Input.GetKey(KeyCode.RightArrow))
            gameObject.transform.position+= Vector3.right*speed * Time.deltaTime ;
    }


    public void OnPortalEnter(TransitioningPortalObject transitioning)
    {
        print("onPortalEnter");
    }

    public void OnPortalTransitioning(TransitioningPortalObject transitioning)
    {
        print("OnPortalTransitioning");
    }

    public void OnPortalExit(TransitioningPortalObject transitioning)
    {
        print("OnPortalExit");
    }

    public void OnPortalCrossed(TransitioningPortalObject transitioning)
    {
        print("OnPortalCrossed");
    }
}
