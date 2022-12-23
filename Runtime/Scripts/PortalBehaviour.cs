using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PortalBehaviour : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PortalOnTriggerEnter(other);
    }

    protected abstract void PortalOnTriggerEnter(Collider other);
}
