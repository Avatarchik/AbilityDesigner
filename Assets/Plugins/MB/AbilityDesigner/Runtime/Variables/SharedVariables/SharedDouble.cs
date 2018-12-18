using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Matki.AbilityDesigner
{
    [System.Serializable]
    public class SharedDouble : SharedVariable<double>
    {
        public static implicit operator SharedDouble(double value) { return new SharedDouble { m_Value = value }; }
    }
}