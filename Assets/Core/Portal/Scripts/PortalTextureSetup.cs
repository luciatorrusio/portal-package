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
    // [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(notBlocked))]
    [SerializeField] private Shader shader;
    // [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(notBlocked))]
    [SerializeField] private InPortal inPortal;

    private RenderTexture _targetTexture;
    [SerializeField] private Renderer renderer;
    
    // crea el material que tendra el plano y se lo pone al plano
    public void SetCameraMaterial()
    {
        _portalInMat = new Material(shader);
        renderer.material = _portalInMat;
        _targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        _portalInMat.mainTexture = _targetTexture;
    }

    public RenderTexture GetRenderTexture()
    {
        return _targetTexture;
    }
    void Start()
    {
       SetCameraMaterial();
    }

    public void SetDefaultMaterial()
    {
        renderer.material = _defaultMaterial;
    }
}
