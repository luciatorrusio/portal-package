using Core.Portal.Utils;
using UnityEngine;

namespace Core.Portal.Scripts
{
    public interface CustomClone
    {
        GameObject CreateClone(GameObject original, Transform portalIn,Transform portalOut);
        PortalUtils.CloneMode GetMode();
    }
}
    