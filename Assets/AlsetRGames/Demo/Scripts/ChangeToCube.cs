using AlsetRGames.Portal.Core;
using UnityEngine;

public class ChangeToCube : MonoBehaviour, TransitionListener
{
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private Mesh newMesh;
    private bool firstTime = true;
    public void OnPortalEnter(Transition transitioning)
    {
        print($"ENTERING PORTAL." +
              $"object: {transitioning._original.name}, " +
              $"with clone: {transitioning._clone.name}, " +
              $"is entering portal: {transitioning._portalIn.name}," +
              $"is coming out of portal: {transitioning._portalOut.name}");
    }

    public void OnPortalTransitioning(Transition transitioning)
    {
        print($"TRANSITIONING PORTAL" +
              $"object: {transitioning._original.name}, " +
              $"with clone: {transitioning._clone.name}, " +
              $"is entering portal: {transitioning._portalIn.name}," +
              $"is coming out of portal: {transitioning._portalOut.name}");
    }

    public void OnPortalExit(Transition transitioning)
    {
        print($"EXITING PORTAL" +
              $"object: {transitioning._original.name}, " +
              $"with clone: {transitioning._clone.name}, " +
              $"is entering portal: {transitioning._portalIn.name}, " +
              $"is coming out of portal: {transitioning._portalOut.name}");
    }

    public void OnPortalCrossed(Transition transitioning)
    {
        print($"CROSSED PORTAL" +
              $"object: {transitioning._original.name}, " +
              $"with clone: {transitioning._clone.name}, " +
              $"is entering portal: {transitioning._portalIn.name}, " +
              $"is coming out of portal: {transitioning._portalOut.name}");
        if (firstTime)
        {
            meshFilter.mesh = newMesh;
            firstTime = false;
        }
        
    }
}
