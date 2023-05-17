using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitioningPortalObject 
{
    public  Transform _original { get;  }
    public  Transform _clone { get;}
    public  Portal  _portalIn { get;  set;}
    public Portal _portalOut { get;  }

    public TransitioningPortalObject(Transform original, Transform clone, Portal portalIn, Portal portalOut)
    {
        _original = original;
        _clone = clone;
        _portalIn = portalIn;
        _portalOut = portalOut;
    }
}
