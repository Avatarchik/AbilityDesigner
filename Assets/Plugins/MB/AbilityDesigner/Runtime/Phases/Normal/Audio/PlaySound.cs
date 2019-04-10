using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MB.AbilityDesigner.Phases
{
    [PhaseCategory("Audio")]
    public class PlaySound : Phase
    {

        public AudioClip m_AudioClip;

        protected override void OnCast()
        {
        }

        protected override Result OnUpdate()
        {
            if (originator.gameObject == null)
            {
                return Result.Fail;
            }

            AudioSource.PlayClipAtPoint(m_AudioClip, transform.position);

            return Result.Success;
        }

        protected override void OnCache()
        {
        }
        protected override void OnReset()
        {
        }
    }
}