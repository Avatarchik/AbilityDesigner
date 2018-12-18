using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Matki.ActorSystem
{
    public class TestAbilityCast : MonoBehaviour
    {

        public AbilityDesigner.Ability m_Ability;
        public Actor m_Originator;
        public Actor m_Target;

        public bool m_Cast;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (m_Cast)
            {
                m_Cast = false;
                m_Ability.Cast(m_Originator, m_Target);
            }
        }
    }
}