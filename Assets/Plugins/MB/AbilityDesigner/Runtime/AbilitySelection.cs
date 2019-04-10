using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MB.AbilityDesigner
{
    public class AbilitySelection : MonoBehaviour
    {

        public event System.Action OnSelectionChanged;

        private int m_SelectedAbility;
        public int selectedAbility
        {
            get
            {
                return m_SelectedAbility;
            }
            set
            {
                if (value != m_SelectedAbility)
                {
                    OnSelectionChanged.Invoke();
                }

                m_SelectedAbility = Mathf.Clamp(value, 0, m_AbilitySet.m_Abilites.Length - 1);
            }
        }

        public AbilitySet m_AbilitySet;
    }
}