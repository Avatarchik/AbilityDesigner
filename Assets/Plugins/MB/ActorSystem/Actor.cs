using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MB.ActorSystem
{
    public class Actor : MonoBehaviour, AbilityDesigner.IAbilityUser
    {
        #region Public Members

        public ActorTemplate m_Template;

        #endregion

        #region Private Members

        // private List<StatusEffectContainer> m_StatusEffects = new List<StatusEffectContainer>();

        #endregion

        #region Events

        public event HitDeleagte OnHit;
        public event DamageDealtDeleagte OnDamageDealt;
        public event DeathDeleagte OnDeath;

        public delegate void HitDeleagte(Actor originator, Actor target);
        public delegate void DamageDealtDeleagte(Actor originator, Actor target, float damage, float health);
        public delegate void DeathDeleagte(Actor originator, Actor target);

        #endregion

        #region Getter Setter

        private float m_Health;
        public float health
        {
            get
            {
                return m_Health;
            }
            private set
            {
                m_Health = Mathf.Clamp(value, 0f, maxHealth);
            }
        }

        public float healthPercentage
        {
            get
            {
                return health / maxHealth;
            }
        }

        public float maxHealth
        {
            get
            {
                return m_Template.m_Health;
            }
        }
        
        public virtual Vector3 environmentVelocity
        {
            get
            {
                return Vector3.zero;
            }
            protected set
            {

            }
        }

        public virtual Vector3 intendedVelocity
        {
            get
            {
                return Vector3.zero;
            }
            protected set
            {

            }
        }

        public virtual float intendedStrength
        {
            get
            {
                return 1f;
            }
            protected set
            {

            }
        }

        public virtual Vector3 velocity
        {
            get
            {
                return intendedVelocity + environmentVelocity;
            }
        }

        #endregion

        #region MonoBehaviour

        private void Awake()
        {
            OnAwake();
        }

        private void Start()
        {
            OnStart();
            if (m_Template != null)
            {
                m_Health = m_Template.m_Health;
            }
            InitiateComponents();
        }

        private void Update()
        {
            OnUpdate();   
        }

        private void LateUpdate()
        {
            OnLateUpdate();
        }

        private void FixedUpdate()
        {
            OnFixedUpdate();
        }

        #endregion

        #region Protected Calls

        protected virtual void InitiateComponents() { }
        protected virtual void OnAwake() { }
        protected virtual void OnStart() { }
        protected virtual void OnUpdate() { }
        protected virtual void OnLateUpdate() { }
        protected virtual void OnFixedUpdate() { }

        #endregion

        #region Methods

        public void DealDamage(Actor originator, float damage)
        {
            OnHit?.Invoke(originator, this);
            if (!m_Template.m_Vulnerable)
            {
                return;
            }
            health -= damage;
            OnDamageDealt?.Invoke(originator, this, damage, health);
            if (health <= 0f)
            {
                OnDeath?.Invoke(originator, this);
            }
        }

        public virtual void AddEnvironmentalForce(Vector3 force)
        {
            environmentVelocity += force;
        }

        public virtual void AddIntendedForce(Vector3 force)
        {
            intendedVelocity += force;
        }

        public virtual void SetIntendedStrength(float strength)
        {
            intendedStrength = strength;
        }
        /*
        public void AddStatusEffect(StatusEffectContainer effect)
        {
            for (int e = 0; e < m_StatusEffects.Count; e++)
            {
                if (m_StatusEffects[e].m_StatusEffect == effect.m_StatusEffect)
                {
                    m_StatusEffects[e] = effect;
                    effect.Initiate(this);
                    return;
                }
            }
            m_StatusEffects.Add(effect);
            effect.Initiate(this);
        }

        public void RemoveStatusEffect(StatusEffectContainer effect)
        {
            m_StatusEffects.Remove(effect);
        }*/

        public virtual Vector3 GetCenter()
        {
            MeshRenderer[] renderer = GetComponentsInChildren<MeshRenderer>();
            float height = -float.MaxValue;
            for (int r = 0; r < renderer.Length; r++)
            {
                float top = renderer[r].bounds.center.y + renderer[r].bounds.extents.y;
                if (height < top)
                {
                    height = top;
                }
            }
            height -= transform.position.y;
            Vector3 result = new Vector3(transform.position.x, transform.position.y + height * 0.5f, transform.position.z);
            return result;
        }

        #endregion
    }
}