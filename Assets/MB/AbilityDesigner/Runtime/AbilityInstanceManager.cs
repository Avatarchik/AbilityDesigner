using System.Collections;
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
            if (m_OccupiedInstances.Count >= ability.maxCountGlobal)
            {
                return false;
            }

            if (m_Cooldowns.ContainsKey(id))
            {
                float offset = Time.timeSinceLevelLoad - m_Cooldowns[id];
                if (offset > ability.cooldowns[0])
                {
                    return false;
                }
            }

            if (m_Holdings.ContainsKey(id))
            {
                if (m_Holdings[id] >= ability.maxCountUser)
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

                    return m_AbilityInstances[i];
                }
            }

            return null;
        }

        public void CastCooldown(string id)
        {
            // Handle Cooldowns
            if (!m_Cooldowns.ContainsKey(id))
            {
                float offset = Time.timeSinceLevelLoad - m_Cooldowns[id];
                float newOffset;
                int currentCooldown = GetCooldownID(offset, out newOffset);
                m_Cooldowns[id] = Time.timeSinceLevelLoad - newOffset;
            }
            else
            {
                float newOffset;
                int currentCooldown = GetCooldownID(float.MaxValue, out newOffset);
                m_Cooldowns.Add(id, Time.timeSinceLevelLoad - newOffset);
            }
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

        private int GetCooldownID(float difference, out float offset)
        {
            float expectedCooldown = 0f;
            for (int c = 0; c < ability.cooldowns.Length; c++)
            {
                expectedCooldown += ability.cooldowns[c];
                if (expectedCooldown > difference)
                {
                    offset = expectedCooldown - ability.cooldowns[c];
                    return c - 1;
                }
            }
            offset = expectedCooldown;
            return ability.cooldowns.Length - 1;
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