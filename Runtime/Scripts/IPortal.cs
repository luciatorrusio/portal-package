using System;
using UnityEngine;
public interface IPortal
{

    void OnPortalEnter(GameObject portal);

    void OnPortalTransitioning(GameObject portal);

    void OnPortalExit(GameObject portal);

}
