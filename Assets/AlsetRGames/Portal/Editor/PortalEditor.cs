using UnityEditor;
using AlsetRGames.Portal.Core;

namespace Editor
{
#if UNITY_EDITOR
    [CustomEditor(typeof(AlsetRGames.Portal.Core.Portal))]
    public class PortalEditor : UnityEditor.Editor
    {
        // Updates the portal each time the Portal script is clicked on the editor
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            AlsetRGames.Portal.Core.Portal portal = (AlsetRGames.Portal.Core.Portal)target;
            portal.UpdateDefaultMaterial();
        
        }
    }
#endif
}
