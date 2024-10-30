using Core.Portal.Scripts;
using UnityEngine;

public class ChangeToCube : MonoBehaviour, TransitionListener
{
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private Mesh newMesh;
    private bool firstTime = true;
    public void OnPortalEnter(Transition transitioning)
    {
        // throw new System.NotImplementedException();
    }

    public void OnPortalTransitioning(Transition transitioning)
    {
        // throw new System.NotImplementedException();
    }

    public void OnPortalExit(Transition transitioning)
    {
        // throw new System.NotImplementedException();
    }

    public void OnPortalCrossed(Transition transitioning)
    {
        if (firstTime)
        {
            meshFilter.mesh = newMesh;
            firstTime = false;
        }
    }
}
