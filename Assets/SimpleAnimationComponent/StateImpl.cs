using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateImpl : State
{
    private IState m_StateHandle;
    private SimpleAnimation m_Component;

    bool State.enabled
    {
        get { return m_StateHandle.enabled; }
        set
        {
            m_StateHandle.enabled = value;
            if (value)
            {
                m_Component.Kick();
            }
        }
    }

    bool State.isValid
    {
        get { return m_StateHandle.IsValid(); }
    }

    float State.time
    {
        get { return m_StateHandle.time; }
        set {
            m_StateHandle.time = value;
            m_Component.Kick(); 
        }
    }

    float State.normalizedTime
    {
        get { return m_StateHandle.normalizedTime; }
        set {
            m_StateHandle.normalizedTime = value;
            m_Component.Kick();
        }
    }

    float State.speed
    {
        get { return m_StateHandle.speed; }
        set {
            m_StateHandle.speed = value;
            m_Component.Kick();
        }
    }

    string State.name
    {
        get { return m_StateHandle.name; }
        set { m_StateHandle.name = value; }
    }

    float State.weight
    {
        get { return m_StateHandle.weight; }
        set {
            m_StateHandle.weight = value;
            m_Component.Kick();
        }
    }

    float State.length
    {
        get { return m_StateHandle.length; }
    }

    AnimationClip State.clip
    {
        get { return m_StateHandle.clip; }
    }

    WrapMode State.wrapMode
    {
        get { return m_StateHandle.wrapMode; }
        set { Debug.LogError("Not Implemented"); }
    }

    public StateImpl(IState handle, SimpleAnimation component)
    {
        m_StateHandle = handle;
        m_Component = component;
    }
}
