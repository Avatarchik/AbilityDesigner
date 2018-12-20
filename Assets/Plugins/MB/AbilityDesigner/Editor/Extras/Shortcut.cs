using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Matki.AbilityDesigner.Edit
{
    [System.Serializable]
    public class Shortcut
    {
        public enum ControlKey { None, Alt, Shift, Ctrl};
        public ControlKey m_ControlKey = ControlKey.None;

        public KeyCode m_Key;

        public bool m_Error = false;

        public bool Triggered(Event e)
        {
            if (e.type != EventType.KeyDown || m_Error)
            {
                return false;
            }

            if (e.alt && m_ControlKey != ControlKey.Alt)
            {
                return false;
            }
            if (e.shift && m_ControlKey != ControlKey.Shift)
            {
                return false;
            }
            if (e.control && m_ControlKey != ControlKey.Ctrl)
            {
                return false;
            }

            switch (m_ControlKey)
            {
                case ControlKey.None: return e.keyCode == m_Key;
                case ControlKey.Alt: return e.alt && e.keyCode == m_Key;
                case ControlKey.Shift: return e.shift && e.keyCode == m_Key;
                case ControlKey.Ctrl: return e.control && e.keyCode == m_Key;
            }
            return false;
        }

        public override string ToString()
        {
            return (m_ControlKey != ControlKey.None ? m_ControlKey.ToString() + " + " : "") + m_Key;
        }

        public bool Equals(Shortcut shortcut)
        {
            return (shortcut.m_ControlKey == m_ControlKey && shortcut.m_Key == m_Key);
        }

        public void SetError(bool error)
        {
            m_Error = error;
        }
    }
}