using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Matki.AbilityDesigner.Phases
{
    public class StopParticleSystem : ActionPhase
    {
        [Header("Test 1")]
        [SerializeField]
        private float m_Value;
        [SerializeField]
        private string m_Value1;
        [SerializeField]
        private int m_Value2;
        [Header("Test 2")]
        public SharedInt m_IntValue;
        public SharedInt m_IntValue2;
        public SharedInt m_IntValue3;
        public SharedInt m_IntValue4;
    }
}