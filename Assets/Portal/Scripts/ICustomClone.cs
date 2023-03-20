using UnityEngine;
using Utils;

namespace Scripts
{
    public interface ICustomClone
    {
        GameObject GetClone();
        PortalUtils.CloneMode GetMode();
    }
}