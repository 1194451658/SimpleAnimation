using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(Animator))]
public partial class SimpleAnimation: MonoBehaviour
{
    // 缓存控制的Animator
    public Animator animator
    {
        get
        {
            if (m_Animator == null)
            {
                m_Animator = GetComponent<Animator>();
            }
            return m_Animator;
        }
    }

    // 封装Animator.updateMode
    // 在物理Loop中进行动画更新
    // When turned on, animations will be executed in the physics loop.
    // This is only useful in conjunction with kinematic rigidbodies.
    public bool animatePhysics
    {
        // 在SimpleAnimation_impl.cs中定义
        get { return m_AnimatePhysics; }
        set {
            m_AnimatePhysics = value;
            animator.updateMode = m_AnimatePhysics ? AnimatorUpdateMode.AnimatePhysics : AnimatorUpdateMode.Normal; 
        }
    }

    // 封装Animator.cullingMode
    public AnimatorCullingMode cullingMode
    {
        get { return animator.cullingMode; }
        set { m_CullingMode = value;  animator.cullingMode = m_CullingMode; }
    }

    public bool isPlaying { get { return m_Playable.IsPlaying(); } }

    public bool playAutomatically
    {
        get { return m_PlayAutomatically; }
        set { m_PlayAutomatically = value; }
    }

    public AnimationClip clip
    {
        get { return m_Clip; }
        set
        {
            LegacyClipCheck(value);
            m_Clip = value;
        }  
    }

    public WrapMode wrapMode
    {
        get { return m_WrapMode; }
        set { m_WrapMode = value; }
    }

    public void AddClip(AnimationClip clip, string newName)
    {
        // 检查是否是旧动画
        LegacyClipCheck(clip);
        AddState(clip, newName);
    }

    public void Blend(string stateName, float targetWeight, float fadeLength)
    {
        m_Animator.enabled = true;
        Kick();
        m_Playable.Blend(stateName, targetWeight,  fadeLength);
    }

    public void CrossFade(string stateName, float fadeLength)
    {
        m_Animator.enabled = true;
        Kick();
        m_Playable.Crossfade(stateName, fadeLength);
    }

    public void CrossFadeQueued(string stateName, float fadeLength, QueueMode queueMode)
    {
        m_Animator.enabled = true;
        Kick();
        m_Playable.CrossfadeQueued(stateName, fadeLength, queueMode);
    }

    public int GetClipCount()
    {
        return m_Playable.GetClipCount();
    }

    public bool IsPlaying(string stateName)
    {
        return m_Playable.IsPlaying(stateName);
    }

    public void Stop()
    {
        m_Playable.StopAll();
    }

    public void Stop(string stateName)
    {
        m_Playable.Stop(stateName);
    }

    public void Sample()
    {
        m_Graph.Evaluate();
    }

    public bool Play()
    {
        m_Animator.enabled = true;

        // 开始播放
        Kick();
        if (m_Clip != null && m_PlayAutomatically)
        {
            m_Playable.Play(kDefaultStateName);
        }
        return false;
    }

    public void AddState(AnimationClip clip, string name)
    {
        LegacyClipCheck(clip);
        Kick();
        if (m_Playable.AddClip(clip, name))
        {
            RebuildStates();
        }
        
    }

    public void RemoveState(string name)
    {
        if (m_Playable.RemoveClip(name))
        {
            RebuildStates();
        }
    }

    public bool Play(string stateName)
    {
        m_Animator.enabled = true;
        Kick();
        return m_Playable.Play(stateName);
    }

    public void PlayQueued(string stateName, QueueMode queueMode)
    {
        m_Animator.enabled = true;
        Kick();
        m_Playable.PlayQueued(stateName, queueMode);
    }

    public void RemoveClip(AnimationClip clip)
    {
        if (clip == null)
            throw new System.NullReferenceException("clip");

        if ( m_Playable.RemoveClip(clip) )
        {
            RebuildStates();
        }
       
    }

    public void Rewind()
    {
        Kick();
        m_Playable.Rewind();
    }

    public void Rewind(string stateName)
    {
        Kick();
        m_Playable.Rewind(stateName);
    }

    public State GetState(string stateName)
    {
        IState state = m_Playable.GetState(stateName);
        if (state == null)
            return null;

        return new StateImpl(state, this);
    }

    public IEnumerable<State> GetStates()
    {
        return new StateEnumerable(this);
    }

    public State this[string name]
    {
        get { return GetState(name); }
    }

}
