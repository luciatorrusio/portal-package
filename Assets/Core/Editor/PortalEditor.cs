using UnityEditor;

namespace Core.Editor
{
#if UNITY_EDITOR
    [CustomEditor(typeof(Portal.Scripts.Portal))]
    public class PortalEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Portal.Scripts.Portal portal = (Portal.Scripts.Portal)target;
            portal.SetPortalMesh();
            portal.UpdateDefaultMaterial();
        
        }
    }
#endif
}
