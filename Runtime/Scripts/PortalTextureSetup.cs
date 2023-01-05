using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using Utils;

[RequireComponent(typeof(Renderer))] 
public class PortalTextureSetup : MonoBehaviour
{
    private  Camera cameraOut;
    private Material _portalInMat;
    [SerializeField] private Material _defaultMaterial;
    private bool notBlocked = false;
    [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(notBlocked))]
    [SerializeField] private Shader shader;
    [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(notBlocked))]
    [SerializeField] private InPortal inPortal;
    public void SetCameraMaterial()
    {
        var rend = GetComponent<Renderer> ();
        _portalInMat = new Material(shader);
        rend.material = _portalInMat;
        if(cameraOut.targetTexture != null)
        {
            cameraOut.targetTexture.Release();
        }

        cameraOut.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        _portalInMat.mainTexture = cameraOut.targetTexture;
 
    }

    public void SetCameraOut(Camera camera)
    {
        cameraOut = camera;
    }
    void Start()
    {
        var rend = GetComponent<Renderer> ();
        _portalInMat = new Material(shader);
        rend.material = _portalInMat;
        if(cameraOut.targetTexture != null)
        {
            cameraOut.targetTexture.Release();
        }

        cameraOut.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        _portalInMat.mainTexture = cameraOut.targetTexture;

    }

    public void SetDefaultMaterial()
    {
        var rend = GetComponent<Renderer> ();
        rend.material = _defaultMaterial;
    }

    private void OnBecameInvisible()
    {
        inPortal.NotVisible();
    }

    private void OnBecameVisible()
    {
        inPortal.Visible();
    }
}
