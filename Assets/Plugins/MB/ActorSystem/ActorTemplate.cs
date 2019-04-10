using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MB.ActorSystem
{
    [CreateAssetMenu()]
    public class ActorTemplate : ScriptableObject
    {
        [Header("Basic")]
        public string m_Title;
        public string m_Description;
        public Texture2D m_Icon;

        [Header("Behaviour")]
        public Team m_Team;

        [Header("Health")]
        public bool m_Vulnerable = true;
        public float m_Health;
        public float m_HealingRate;
    }
}