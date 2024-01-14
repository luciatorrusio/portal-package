using System.Collections.Generic;
using Core.Portal.Utils;
using UnityEngine;

namespace Core.Portal.Scripts
{
    public interface CustomClone
    {
        GameObject CreateClone(GameObject original, Transform portalIn,Transform portalOut, List<KeyValuePair<Transform, Transform>>  originalToClone);
        PortalUtils.CloneMode GetMode();
    }
}
    