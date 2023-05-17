using System;
using UnityEngine;
public interface IPortal
{

    void OnPortalEnter(TransitioningPortalObject transitioning);

    void OnPortalTransitioning(TransitioningPortalObject transitioning);

    void OnPortalExit(TransitioningPortalObject transitioning);
    void OnPortalCrossed(TransitioningPortalObject transitioning);

}
