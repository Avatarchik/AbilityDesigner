using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Matki.ActorSystem
{
    [CreateAssetMenu()]
    public class Team : ScriptableObject
    {
        [Header("Basic")]
        public string m_Title;
        public string m_Description;

        [Header("Affection")]
        public Team[] m_Friends;
        public Team[] m_Enemies;

        public bool IsEnemy(Actor target)
        {
            for (int e = 0; e < m_Enemies.Length; e++)
            {
                if (target.m_Template.m_Team == m_Enemies[e])
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsFriend(Actor target)
        {
            for (int f = 0; f < m_Friends.Length; f++)
            {
                if (target.m_Template.m_Team == m_Friends[f])
                {
                    return true;
                }
            }
            return false;
        }
    }
}