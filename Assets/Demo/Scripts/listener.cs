using System.Collections;
using System.Collections.Generic;
using Core.Portal.Scripts;
using UnityEngine;

public class listener : MonoBehaviour, TransitionListener
{
    private int _counter = 0;
    public void OnPortalEnter(Transition transitioning)
    {
        print($"{_counter++}. Enter portal {transitioning._portalIn.name}");
    }

    public void OnPortalTransitioning(Transition transitioning)
    {
        // print($"{_counter++}.transition portal {transitioning._portalIn.name}");
    }

    public void OnPortalExit(Transition transitioning)
    {
        print($"{_counter++}.exit portal {transitioning._portalIn.name}");
    }

    public void OnPortalCrossed(Transition transitioning)
    {
        print($"{_counter++}.crossed portal, portal in: {transitioning._portalIn.name}. Portal out: {transitioning._portalOut.name}");
    }
}
