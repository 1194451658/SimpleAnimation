using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;
using System;

public partial class SimpleAnimationPlayable : PlayableBehaviour
{
    private int m_StatesVersion = 0;

    private void InvalidateStates()
    {
        m_StatesVersion++;
    }

    public int StatesVersion
    {
        get { return m_StatesVersion; }
    }


    private StateHandle StateInfoToHandle(StateInfo info)
    {
        return new StateHandle(this, info.index, info.playable);
    }


    private struct QueuedState
    {
        public QueuedState(StateHandle s, float t)
        {
            state = s;
            fadeTime = t;
        }

        public StateHandle state;
        public float fadeTime;
    }
}