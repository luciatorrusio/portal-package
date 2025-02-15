using System.Collections.Generic;
using UnityEngine;
using AlsetRGames.Portal.Support;
namespace AlsetRGames.Portal.Core
{
    public interface CustomClone
    {
        GameObject CreateClone(GameObject original, Transform portalIn,Transform portalOut, List<(Transform original, Transform clone)>  originalToClone);
        PortalUtils.CloneMode GetMode();
    }
}
    