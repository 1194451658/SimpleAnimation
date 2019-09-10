using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;
using System;
using UnityEngine;

public class StateInfo
{
    // 状态名称
    private string m_StateName;

    // 要播放的动画Clip
    private AnimationClip m_Clip;

    // 循环模式
    private WrapMode m_WrapMode;

    // 当前Playable时间
    private float m_Time;

    private bool m_Fading;

    // FadeTo，到达目标weight时间
    float m_FadeSpeed;

    // 当前weight
    float m_Weight;

    // 目标weight
    private float m_TargetWeight;

    private int m_Index;

    private Playable m_Playable;

    private bool m_IsClone;

    private bool m_ReadyForCleanup;

    public StateHandle m_ParentState;

    private bool m_WeightDirty;

    private bool m_Enabled;

    // Enable()被调用了
    private bool m_EnabledDirty;

    private bool m_TimeIsUpToDate;

    public string stateName
    {
        get { return m_StateName; }
        set { m_StateName = value; }
    }

    public AnimationClip clip
    {
        get { return m_Clip; }
    }

    public WrapMode wrapMode
    {
        get { return m_WrapMode; }
    }

    public void Initialize(string name, AnimationClip clip, WrapMode wrapMode)
    {
        m_StateName = name;
        m_Clip = clip;
        m_WrapMode = wrapMode;
    }

    // 获取当前Playable时间
    public float GetTime()
    {
        if (m_TimeIsUpToDate)
            return m_Time;

        m_Time = (float) m_Playable.GetTime();
        m_TimeIsUpToDate = true;
        return m_Time;
    }

    // 重新设置Playable时间
    public void SetTime(float newTime)
    {
        m_Time = newTime;
        m_Playable.ResetTime(m_Time);
        m_Playable.SetDone(m_Time >= m_Playable.GetDuration());
    }

    public void Enable()
    {
        if (m_Enabled)
            return;

        m_EnabledDirty = true;
        m_Enabled = true;
    }

    public void Disable()
    {
        if (m_Enabled == false)
            return;

        m_EnabledDirty = true;
        m_Enabled = false;
    }

    // 暂停Playable
    public void Pause()
    {
        m_Playable.SetPlayState(PlayState.Paused);
    }

    // 播放Playable
    public void Play()
    {
        m_Playable.SetPlayState(PlayState.Playing);
    }

    public void Stop()
    {
        m_FadeSpeed = 0f;
        ForceWeight(0.0f);
        Disable();
        SetTime(0.0f);
        m_Playable.SetDone(false);
        if (isClone)
        {
            m_ReadyForCleanup = true;
        }
    }

    public void ForceWeight(float weight)
    {
        m_TargetWeight = weight;
        m_Fading = false;
        m_FadeSpeed = 0f;
        SetWeight(weight);
    }

    public void SetWeight(float weight)
    {
        m_Weight = weight;
        m_WeightDirty = true;
    }

    // 到达目标weight
    public void FadeTo(float weight, float speed)
    {
        m_Fading = Mathf.Abs(speed) > 0f;
        m_FadeSpeed = speed;
        m_TargetWeight = weight;
    }

    // 销毁playable
    public void DestroyPlayable()
    {
        if (m_Playable.IsValid())
        {
            m_Playable.GetGraph().DestroySubgraph(m_Playable);
        }
    }

    // Q: ??
    public void SetAsCloneOf(StateHandle handle)
    {
        m_ParentState = handle;
        m_IsClone = true;
    }

    public bool enabled
    {
        get { return m_Enabled; }
    }


    public int index
    {
        get { return m_Index; }
        set
        {
            Debug.Assert(m_Index == 0, "Should never reassign Index");
            m_Index = value;
        }
    }

    public bool fading
    {
        get { return m_Fading; }
    }

    public float targetWeight
    {
        get { return m_TargetWeight; }
    }

    public float weight
    {
        get { return m_Weight; }
    }

    public float fadeSpeed
    {
        get { return m_FadeSpeed; }
    }

    public float speed
    {
        get { return (float) m_Playable.GetSpeed(); }
        set { m_Playable.SetSpeed(value); }
    }

    public float playableDuration
    {
        get { return (float) m_Playable.GetDuration(); }
    }

    public void SetPlayable(Playable playable)
    {
        m_Playable = playable;
    }

    public bool isDone
    {
        get { return m_Playable.IsDone(); }
    }

    public Playable playable
    {
        get { return m_Playable; }
    }

    //Clone information
    public bool isClone
    {
        get { return m_IsClone; }
    }

    public bool isReadyForCleanup
    {
        get { return m_ReadyForCleanup; }
    }

    public StateHandle parentState
    {
        get { return m_ParentState; }
    }

    public bool enabledDirty
    {
        get { return m_EnabledDirty; }
    }

    public bool weightDirty
    {
        get { return m_WeightDirty; }
    }

    public void ResetDirtyFlags()
    {
        m_EnabledDirty = false;
        m_WeightDirty = false;
    }

    public void InvalidateTime()
    {
        m_TimeIsUpToDate = false;
    }
}