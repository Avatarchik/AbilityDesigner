using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MB.AbilityDesigner;
using MB.AbilityDesigner.Phases;

namespace MB.Abilities
{
    [PhaseCategory("Transform")]
    public class MoveToAnchor : Phase
    {

        public string m_AnchorPointName;

        private AbilityAnchorPoint m_AnchorPoint;

        protected override void OnCast()
        {
            List<AbilityAnchorPoint> points = new List<AbilityAnchorPoint>(originator.gameObject.GetComponentsInChildren<AbilityAnchorPoint>());
            m_AnchorPoint = points.Find(obj => obj.m_AnchorPointName == m_AnchorPointName);
        }

        protected override Result OnUpdate()
        {
            if (m_AnchorPoint == null)
            {
                return Result.Fail;
            }

            transform.position = m_AnchorPoint.transform.position;

            return Result.Success;
        }
    }
}