using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MB.AbilityDesigner
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class PhaseCategoryAttribute : Attribute
    {
        private string m_Path;

        internal string path
        {
            get { return m_Path; }
        }

        public PhaseCategoryAttribute(string pathValue)
        {
            m_Path = pathValue;
        }
    }
}