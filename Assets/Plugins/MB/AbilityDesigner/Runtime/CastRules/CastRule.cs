using UnityEngine;

namespace MB.AbilityDesigner
{
    public abstract class CastRule : ScriptableObject
    {
        public abstract bool IsCastLegitimate(int globalInstances, int userInstances, float userFloat);
        public abstract void ApplyCast(ref float userFloat);
    }
}