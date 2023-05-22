using UnityEngine;
using Utils;

namespace Scripts
{
    public interface CustomClone
    {
        GameObject CreateClone(GameObject original, Transform portalIn,Transform portalOut);
        PortalUtils.CloneMode GetMode();
    }
}