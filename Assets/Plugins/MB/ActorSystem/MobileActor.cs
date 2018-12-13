using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Matki.ActorSystem
{
    [RequireComponent(typeof(CharacterController))]
    public class MobileActor : Actor
    {

        public float m_Mass = 1;

        public AnimationCurve m_SpeedOnTime = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));
        public float m_MaxSpeed;
        public float m_AccelerationDuration = 0.2f;
        public float m_DeccelerationDuration = 0.3f;

        public bool m_UseGravity = true;

        CharacterController m_Controller;

        #region Getter Setter

        private Vector3 m_EnvVelocity;
        private Vector3 m_EnvDecceleration;
        public override Vector3 environmentVelocity
        {
            get
            {
                return m_EnvVelocity;
            }

            protected set
            {
                m_EnvVelocity = value;
            }
        }

        private float m_IntendedStrength = 1f;
        public override float intendedStrength
        {
            get
            {
                return m_IntendedStrength;
            }

            protected set
            {
                m_IntendedStrength = value;
            }
        }

        private Vector3 m_IntVelocity;
        private float m_AccTime;
        private bool m_Accelerated;
        public override Vector3 intendedVelocity
        {
            get
            {
                return m_IntVelocity * m_SpeedOnTime.Evaluate(m_AccTime) * intendedStrength;
            }

            protected set
            {
                m_IntVelocity = value;
                if (m_IntVelocity.magnitude > m_MaxSpeed)
                {
                    m_IntVelocity = m_IntVelocity.normalized * m_MaxSpeed;
                }
                m_AccTime = Mathf.Min(m_AccTime + Mathf.InverseLerp(0f, m_AccelerationDuration, Time.deltaTime) * m_IntVelocity.magnitude, 1f);
                m_Accelerated = true;
            }
        }

        #endregion
        
        protected override void InitiateComponents()
        {
            base.InitiateComponents();
            m_Controller = GetComponent<CharacterController>();
        }

        private void Update()
        {
            m_Controller.Move(velocity * Time.deltaTime);
            float remainingTime = 1f / Mathf.Max(Mathf.Abs(m_Mass) * Physics.gravity.magnitude, float.Epsilon);
            environmentVelocity = Vector3.SmoothDamp(environmentVelocity, Vector3.zero, ref m_EnvDecceleration, remainingTime);
            if (m_UseGravity && !m_Controller.isGrounded)
            {
                AddEnvironmentalForce(Physics.gravity);
            }
        }

        private void LateUpdate()
        {
            if (!m_Accelerated)
            {
                m_AccTime = Mathf.Max(0f, m_AccTime - Mathf.InverseLerp(0f, m_DeccelerationDuration, Time.deltaTime));
            }
            m_Accelerated = false;
        }

    }
}