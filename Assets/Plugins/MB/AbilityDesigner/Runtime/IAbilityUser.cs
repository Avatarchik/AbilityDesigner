using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MB.AbilityDesigner
{
    public interface IAbilityUser
    {
        GameObject gameObject { get; }
        int GetInstanceID();
        Vector3 GetCenter();
    }
}