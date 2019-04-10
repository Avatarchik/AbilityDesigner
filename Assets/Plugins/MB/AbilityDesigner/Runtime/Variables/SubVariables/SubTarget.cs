using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MB.AbilityDesigner
{
    public enum Target { Originator, Target };

    [System.Serializable]
    public class SubTarget : SubVariable<Target>
    {

    }
}