using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;
using System;

public class StateHandle : IState
{
    public StateHandle(SimpleAnimationPlayable s, int index, Playable target)
    {
        m_Parent = s;
        m_Index = index;
        m_Target = target;
    }

    public bool IsValid()
    {
        return m_Parent.ValidateInput(m_Index, m_Target);
    }

    public bool enabled
    {
        get
        {
            if (!IsValid())
                throw new System.InvalidOperationException("This StateHandle is not valid");
            return m_Parent.States[m_Index].enabled;
        }

        set
        {
            if (!IsValid())
                throw new System.InvalidOperationException("This StateHandle is not valid");
            if (value)
                m_Parent.States.EnableState(m_Index);
            else
                m_Parent.States.DisableState(m_Index);
        }
    }

    public float time
    {
        get
        {
            if (!IsValid())
                throw new System.InvalidOperationException("This StateHandle is not valid");
            return m_Parent.States.GetStateTime(m_Index);
        }
        set
        {
            if (!IsValid())
                throw new System.InvalidOperationException("This StateHandle is not valid");
            m_Parent.States.SetStateTime(m_Index, value);
        }
    }

    public float normalizedTime
    {
        get
        {
            if (!IsValid())
                throw new System.InvalidOperationException("This StateHandle is not valid");

            float length = m_Parent.States.GetClipLength(m_Index);
            if (length == 0f)
                length = 1f;

            return m_Parent.States.GetStateTime(m_Index) / length;
        }
        set
        {
            if (!IsValid())
                throw new System.InvalidOperationException("This StateHandle is not valid");

            float length = m_Parent.States.GetClipLength(m_Index);
            if (length == 0f)
                length = 1f;

            m_Parent.States.SetStateTime(m_Index, value *= length);
        }
    }

    public float speed
    {
        get
        {
            if (!IsValid())
                throw new System.InvalidOperationException("This StateHandle is not valid");
            return m_Parent.States.GetStateSpeed(m_Index);
        }
        set
        {
            if (!IsValid())
                throw new System.InvalidOperationException("This StateHandle is not valid");
            m_Parent.States.SetStateSpeed(m_Index, value);
        }
    }

    public string name
    {
        get
        {
            if (!IsValid())
                throw new System.InvalidOperationException("This StateHandle is not valid");
            return m_Parent.States.GetStateName(m_Index);
        }
        set
        {
            if (!IsValid())
                throw new System.InvalidOperationException("This StateHandle is not valid");
            if (value == null)
                throw new System.ArgumentNullException("A null string is not a valid name");
            m_Parent.States.SetStateName(m_Index, value);
        }
    }

    public float weight
    {
        get
        {
            if (!IsValid())
                throw new System.InvalidOperationException("This StateHandle is not valid");
            return m_Parent.States[m_Index].weight;
        }
        set
        {
            if (!IsValid())
                throw new System.InvalidOperationException("This StateHandle is not valid");
            if (value < 0)
                throw new System.ArgumentException("Weights cannot be negative");

            m_Parent.States.SetInputWeight(m_Index, value);
        }
    }

    public float length
    {
        get
        {
            if (!IsValid())
                throw new System.InvalidOperationException("This StateHandle is not valid");
            return m_Parent.States.GetStateLength(m_Index);
        }
    }

    public AnimationClip clip
    {
        get
        {
            if (!IsValid())
                throw new System.InvalidOperationException("This StateHandle is not valid");
            return m_Parent.States.GetStateClip(m_Index);
        }
    }

    public WrapMode wrapMode
    {
        get
        {
            if (!IsValid())
                throw new System.InvalidOperationException("This StateHandle is not valid");
            return m_Parent.States.GetStateWrapMode(m_Index);
        }
    }

    public int index
    {
        get { return m_Index; }
    }

    private SimpleAnimationPlayable m_Parent;
    private int m_Index;
    private Playable m_Target;
}