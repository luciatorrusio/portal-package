using System;
using UnityEngine;
public interface IPortal
{

    void OnPortalEnter(Portal portal);

    void OnPortalTransitioning(Portal portal);

    void OnPortalExit(Portal portal);

}
