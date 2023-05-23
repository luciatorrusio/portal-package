using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class listener : MonoBehaviour, TransitionListener
{
    public void OnPortalEnter(Transition transitioning)
    {
        print("hrllo");
    }

    public void OnPortalTransitioning(Transition transitioning)
    {
        print("hello");
    }

    public void OnPortalExit(Transition transitioning)
    {
        print("hello");
    }

    public void OnPortalCrossed(Transition transitioning)
    {
        print("helo");
    }
}
