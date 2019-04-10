using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MB.AbilityDesigner
{
    public class AbilityInstanceManager : MonoBehaviour
    {
        [SerializeField]
        private Ability m_Ability;
        public Ability ability { get { return m_Ability; } internal set { m_Ability = value; } }

        [SerializeField]
        private Vector3 m_PoolPosition;
        public Vector3 poolPostion { get { return m_PoolPosition; } internal set { m_PoolPosition = value; } }

        private AbilityInstance[] m_AbilityInstances = new AbilityInstance[0];
        private List<AbilityInstance> m_OccupiedInstances = new List<AbilityInstance>();

        private Dictionary<string, int> m_Holdings = new Dictionary<string, int>();
        private Dictionary<string, float> m_Cooldowns = new Dictionary<string, float>();

        private void Awake()
        {
            ability.instanceManager = this;
        }

        private void Start()
        {
            ExpandCache(ability.poolingChunkSize);
        }

        public float GetCooldown(string id)
        {
            if (!m_Cooldowns.ContainsKey(id))
            {
                return 0f;
            }
            return m_Cooldowns[id];
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

                    m_AbilityInstances[i].gameObject.SetActive(true);
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

            instance.transform.position = m_PoolPosition;
            instance.gameObject.SetActive(false);
        }

        private void ExpandCache(int size)
        {
            List<AbilityInstance> instances = new List<AbilityInstance>(m_AbilityInstances);
            for (int i = 0; i < size; i++)
            {
                GameObject instance = ability.CreateRuntimeInstance();
                instance.transform.SetParent(transform);
                instance.transform.position = poolPostion;
                AbilityInstance abilityInstace = instance.GetComponent<AbilityInstance>();
                abilityInstace.ability = m_Ability;
                instances.Add(abilityInstace);
                instance.SetActive(false);
            }
            m_AbilityInstances = instances.ToArray();
        }
    }
}