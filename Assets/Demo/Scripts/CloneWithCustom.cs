using System.Collections.Generic;
using Core.Portal.Scripts;
using Core.Portal.Utils;
using UnityEngine;

public class CloneWithCustom : MonoBehaviour, CustomClone
{
    private bool firstTime = true;

    [SerializeField] private Material material;
    public GameObject CreateClone(GameObject original, Transform portalIn, Transform portalOut,  List<(Transform original, Transform clone)>  originalToClone)
    {
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.GetComponent<MeshRenderer>().material = material;
        originalToClone.Add( (original.transform, cube.transform));
        return cube;
    }

    public PortalUtils.CloneMode GetMode()
    {
        if (!firstTime) return PortalUtils.CloneMode.AUTOMATIC;
        firstTime = false;
        return PortalUtils.CloneMode.CUSTOM;

    }
}
