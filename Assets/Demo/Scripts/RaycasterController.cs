using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Physics = Core.Portal.Scripts.Physics;

public class RaycasterController : MonoBehaviour
{
    
    private List<RaycastHit> hitInfo = new List<RaycastHit>();
    void Update()
    {
        var raycastClicked = Input.GetKey(KeyCode.R);
        if (raycastClicked)
        {
            if (Physics.PortalRaycast(transform.position, transform.forward, out hitInfo))
                print($"amount of rays: {hitInfo.Count}, {hitInfo[hitInfo.Count-1].collider.gameObject.name}");
            else
                print("didnt hit nything");
        }
        
    }
}
