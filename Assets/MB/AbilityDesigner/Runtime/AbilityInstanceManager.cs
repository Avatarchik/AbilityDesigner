﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Matki.AbilityDesigner
{
    public class AbilityInstanceManager : MonoBehaviour
    {
        public Ability ability { get; internal set; }

        public Vector3 poolPostion { get; internal set; }

        private AbilityInstance[] m_AbilityInstances = new AbilityInstance[0];
        private List<AbilityInstance> m_OccupiedInstances = new List<AbilityInstance>();

        private Dictionary<string, int> m_Holdings = new Dictionary<string, int>();
        private Dictionary<string, float> m_Cooldowns = new Dictionary<string, float>();

        private void Awake()
        {
            ability.instanceManager = this;
        }

        public bool IsCastLegitimate(string id)
        {
            int holdings = 0;
            if (m_Holdings.ContainsKey(id))
            {
                holdings = m_Holdings[id];
            }
            
            if (!m_Cooldowns.ContainsKey(id))
            {
                m_Cooldowns.Add(id, 0f);
            }
            
            for (int r = 0; r < ability.castRules.Length; r++)
            {
                if (!ability.castRules[r].IsCastLegitimate(m_OccupiedInstances.Count, holdings, m_Cooldowns[id]))
                {
                    return false;
                }
            }
            return true;
        }

        public AbilityInstance RequestInstance(string id)
        {
            if (m_OccupiedInstances.Count >= m_AbilityInstances.Length)
            {
                ExpandCache(ability.poolingChunkSize);
            }

            for (int i = 0; i < m_AbilityInstances.Length; i++)
            {
                if (!m_OccupiedInstances.Contains(m_AbilityInstances[i]))
                {
                    m_OccupiedInstances.Add(m_AbilityInstances[i]);

                    // Handle Holdings
                    if (m_Holdings.ContainsKey(id))
                    {
                        m_Holdings[id]++;
                    }
                    else
                    {
                        m_Holdings.Add(id, 1);
                    }
                    
                    if (!m_Cooldowns.ContainsKey(id))
                    {
                        m_Cooldowns.Add(id, -float.MaxValue);
                    }

                    for (int r = 0; r < ability.castRules.Length; r++)
                    {
                        float value = m_Cooldowns[id];
                        ability.castRules[r].ApplyCast(ref value);
                        m_Cooldowns[id] = value;
                    }

                    return m_AbilityInstances[i];
                }
            }

            return null;
        }

        public void ReturnInstance(string id, AbilityInstance instance)
        {
            m_OccupiedInstances.Remove(instance);

            // Handle Holdings
            if (m_Holdings.ContainsKey(id))
            {
                m_Holdings[id]--;
            }
        }

        private void ExpandCache(int size)
        {
            List<AbilityInstance> instances = new List<AbilityInstance>(m_AbilityInstances);
            for (int i = 0; i < size; i++)
            {
                GameObject instance = ability.CreateRuntimeInstance();
                instance.transform.SetParent(transform);
                instance.transform.position = poolPostion;
                instances.Add(instance.GetComponent<AbilityInstance>());
            }
            m_AbilityInstances = instances.ToArray();
        }
    }
}