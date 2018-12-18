using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Matki.AbilityDesigner
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class PhaseIconAttribute : Attribute
    {
        private string m_Path;

        internal string path
        {
            get { return m_Path; }
        }

        public PhaseIconAttribute(string pathValue)
        {
            m_Path = pathValue;
        }
    }
}