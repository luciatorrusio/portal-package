
using Core.Portal.Scripts;
using UnityEditor;
[CustomEditor(typeof(Portal))]
public class PortalEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Portal portal = (Portal)target;
        portal.SetPortalMesh();
        portal.UpdateDefaultMaterial();
        
    }
}
