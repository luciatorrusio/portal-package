using UnityEditor;

namespace Core.Editor
{
#if UNITY_EDITOR
    [CustomEditor(typeof(Portal.Scripts.Portal))]
    public class PortalEditor : UnityEditor.Editor
    {
        // Updates the portal each time the Portal script is clicked on the editor
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Portal.Scripts.Portal portal = (Portal.Scripts.Portal)target;
            portal.UpdateDefaultMaterial();
        
        }
    }
#endif
}
