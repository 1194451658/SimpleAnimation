using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(Animator))]
public partial class SimpleAnimation: MonoBehaviour, IAnimationClipSource
{
    // 默认状态名称
    const string kDefaultStateName = "Default";

    protected PlayableGraph m_Graph;
    protected PlayableHandle m_LayerMixer;
    protected PlayableHandle m_TransitionMixer;
    protected Animator m_Animator;
    protected bool m_Initialized;
    protected bool m_IsPlaying;

    protected SimpleAnimationPlayable m_Playable;

    [SerializeField]
    protected bool m_PlayAutomatically = true;

    [SerializeField]
    protected bool m_AnimatePhysics = false;

    [SerializeField]
    protected AnimatorCullingMode m_CullingMode = AnimatorCullingMode.CullUpdateTransforms;

    [SerializeField]
    protected WrapMode m_WrapMode;

    // 默认Cilp
    [SerializeField]
    protected AnimationClip m_Clip;

    [SerializeField]
    private EditorState[] m_States;

    public SimpleAnimationPlayable Playable
    {
        get { return m_Playable; }
    }

    // 初始化
    protected virtual void Awake()
    {
        Initialize();
    }

    protected virtual void OnEnable()
    {
        Initialize();
        m_Graph.Play();
        if (m_PlayAutomatically)
        {
            Stop();
            Play();
        }
    }

    protected virtual void OnDisable()
    {
        if (m_Initialized)
        {
            Stop();
            m_Graph.Stop();
        }
    }

    private void Reset()
    {
        if (m_Graph.IsValid())
            m_Graph.Destroy();
        
        m_Initialized = false;
    }


    // 初始化
    private void Initialize()
    {
        if (m_Initialized)
            return;

        // 设置Animator
        m_Animator = GetComponent<Animator>();
        m_Animator.updateMode = m_AnimatePhysics ? AnimatorUpdateMode.AnimatePhysics : AnimatorUpdateMode.Normal;
        m_Animator.cullingMode = m_CullingMode;

        // 创建PlayableGraph
        m_Graph = PlayableGraph.Create();
        m_Graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

        // 创建ScriptPlayable<SimpleAnimationPlayable>
        SimpleAnimationPlayable template = new SimpleAnimationPlayable();
        var playable = ScriptPlayable<SimpleAnimationPlayable>.Create(m_Graph, template, 1);
        m_Playable = playable.GetBehaviour();
        m_Playable.onDone += OnPlayableDone;

        // 如果没有EditorState
        // 设置一个 
        if (m_States == null)
        {
            m_States = new EditorState[1];
            m_States[0] = new EditorState();
            m_States[0].defaultState = true;
            m_States[0].name = "Default";
        }


        // 根据编辑器上设置
        // 在SimpleAnimationPlayable上
        // 添加Clip
        if (m_States != null)
        {
            foreach (var state in m_States)
            {
                if (state.clip)
                {
                    m_Playable.AddClip(state.clip, state.name);
                }
            }
        }

        // 创建默认的播放状态Default
        EnsureDefaultStateExists();

        // Q: ??
        AnimationPlayableUtilities.Play(m_Animator, m_Playable.playable, m_Graph);

        Play();
        Kick();
        m_Initialized = true;
    }

    // 保证有一个默认的状态
    // Default
    private void EnsureDefaultStateExists()
    {
        if ( m_Playable != null &&
            m_Clip != null &&
            m_Playable.GetState(kDefaultStateName) == null )
        {
            m_Playable.AddClip(m_Clip, kDefaultStateName);
            Kick();
        }
    }


    // 开始播放
    public void Kick()
    {
        if (!m_IsPlaying)
        {
            m_Graph.Play();
            m_IsPlaying = true;
        }
    }

    // 销毁Graph
    protected void OnDestroy()
    {
        if (m_Graph.IsValid())
        {
            m_Graph.Destroy();
        }
    }

    private void OnPlayableDone()
    {
        m_Graph.Stop();
        m_IsPlaying = false;
    }

    // 遍历Playable中状态
    // 重新构建在Inspector中
    private void RebuildStates()
    {
        // 遍历Playable中的状态
        var playableStates = GetStates();
        var list = new List<EditorState>();
        foreach (var state in playableStates)
        {
            var newState = new EditorState();
            newState.clip = state.clip;
            newState.name = state.name;
            list.Add(newState);
        }
        m_States = list.ToArray();
    }

    EditorState CreateDefaultEditorState()
    {
        var defaultState = new EditorState();
        defaultState.name = "Default";
        defaultState.clip = m_Clip;
        defaultState.defaultState = true;

        return defaultState;
    }

    // 检查是否是旧动画Clip
    static void LegacyClipCheck(AnimationClip clip)
    {
        if (clip && clip.legacy)
        {
            throw new ArgumentException(string.Format("Legacy clip {0} cannot be used in this component. Set .legacy property to false before using this clip", clip));
        }
    }
    
    void InvalidLegacyClipError(string clipName, string stateName)
    {
        Debug.LogErrorFormat(this.gameObject,"Animation clip {0} in state {1} is Legacy. Set clip.legacy to false, or reimport as Generic to use it with SimpleAnimationComponent", clipName, stateName);
    }

    private void OnValidate()
    {
        //Don't mess with runtime data
        if (Application.isPlaying)
            return;

        if (m_Clip && m_Clip.legacy)
        {
            Debug.LogErrorFormat(this.gameObject,"Animation clip {0} is Legacy. Set clip.legacy to false, or reimport as Generic to use it with SimpleAnimationComponent", m_Clip.name);
            m_Clip = null;
        }

        //Ensure at least one state exists
        if (m_States == null || m_States.Length == 0)
        {
            m_States = new EditorState[1];   
        }

        //Create default state if it's null
        if (m_States[0] == null)
        {
            m_States[0] = CreateDefaultEditorState();
        }

        //If first state is not the default state, create a new default state at index 0 and push back the rest
        if (m_States[0].defaultState == false || m_States[0].name != "Default")
        {
            var oldArray = m_States;
            m_States = new EditorState[oldArray.Length + 1];
            m_States[0] = CreateDefaultEditorState();
            oldArray.CopyTo(m_States, 1);
        }

        //If default clip changed, update the default state
        if (m_States[0].clip != m_Clip)
            m_States[0].clip = m_Clip;


        //Make sure only one state is default
        for (int i = 1; i < m_States.Length; i++)
        {
            if (m_States[i] == null)
            {
                m_States[i] = new EditorState();
            }
            m_States[i].defaultState = false;
        }

        //Ensure state names are unique
        int stateCount = m_States.Length;
        string[] names = new string[stateCount];

        for (int i = 0; i < stateCount; i++)
        {
            EditorState state = m_States[i];
            if (state.name == "" && state.clip)
            {
                state.name = state.clip.name;
            }

#if UNITY_EDITOR
            state.name = ObjectNames.GetUniqueName(names, state.name);
#endif
            names[i] = state.name;

            if (state.clip && state.clip.legacy)
            {
                InvalidLegacyClipError(state.clip.name, state.name);
                state.clip = null;
            }
        }

        m_Animator = GetComponent<Animator>();
        m_Animator.updateMode = m_AnimatePhysics ? AnimatorUpdateMode.AnimatePhysics : AnimatorUpdateMode.Normal;
        m_Animator.cullingMode = m_CullingMode;
    }

    public void GetAnimationClips(List<AnimationClip> results)
    {
        foreach (var state in m_States)
        {
            if (state.clip != null)
                results.Add(state.clip);
        }
    }
}
