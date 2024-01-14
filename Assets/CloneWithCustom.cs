using System.Collections;
using System.Collections.Generic;
using Core.Portal.Scripts;
using Core.Portal.Utils;
using UnityEngine;

public class CloneWithCustom : MonoBehaviour, CustomClone
{
    public GameObject CreateClone(GameObject original, Transform portalIn, Transform portalOut,  List<KeyValuePair<Transform, Transform>>  originalToClone)
    {
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.parent = portalOut;
        originalToClone.Add( new KeyValuePair<Transform, Transform>(original.transform, cube.transform));
        return cube;
    }

    public PortalUtils.CloneMode GetMode()
    {
        return PortalUtils.CloneMode.CUSTOM;
    }
}
