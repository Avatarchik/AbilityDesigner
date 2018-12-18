using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Matki.ActorSystem
{
    public class AmunationCooldownDisplay : MonoBehaviour
    {

        public AbilityDesigner.Ability m_Ability;
        public AbilityDesigner.AbilityInstanceManager m_Manager;

        private AbilityDesigner.AmunationCooldown m_AmunationCooldown;

        public Text m_Display;
        public RectTransform m_RectTransform;
        public Actor m_Target;
        
        void Start()
        {
            for (int r = 0; r < m_Ability.castRules.Length; r++)
            {
                AbilityDesigner.AmunationCooldown cooldown = (AbilityDesigner.AmunationCooldown)m_Ability.castRules[r];
                if (cooldown != null)
                {
                    m_AmunationCooldown = cooldown;
                }
            }
            if (m_AmunationCooldown == null)
            {
                Destroy(gameObject);
            }
        }

        // Update is called once per frame
        void Update()
        {
            float userFloat = m_Manager.GetCooldown(m_Target.GetInstanceID() + "");
            int amount = m_AmunationCooldown.GetAmunation(userFloat);
            m_Display.text = "";
            for (int a = 0; a < m_AmunationCooldown.cooldowns.Length; a++)
            {
                if (a < amount)
                {
                    m_Display.text += "<b>" + m_AmunationCooldown.cooldowns[a] + "</b>  ";
                }
                else
                {
                    m_Display.text += m_AmunationCooldown.cooldowns[a] + "  ";
                }
            }
            Vector3 scale = m_RectTransform.localScale;
            scale.x = m_AmunationCooldown.GetProgress(userFloat);
            m_RectTransform.localScale = scale;
        }
    }
}