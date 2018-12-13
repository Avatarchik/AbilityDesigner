using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Matki.ActorSystem
{
    [RequireComponent(typeof(Actor))]
    public class DeathHandler : MonoBehaviour
    {

        public UnityEvent m_Event;

        void Start()
        {
            GetComponent<Actor>().OnDeath += HandleDeath;
            GetComponent<Actor>().OnDeath += HandleDeathEvents;
        }

        void HandleDeathEvents(Actor originator, Actor target)
        {
            m_Event.Invoke();
        }

        protected virtual void HandleDeath(Actor originator, Actor target)
        {

        }

        public void DestroyGameObject()
        {
            Destroy(gameObject);
        }
    }
}