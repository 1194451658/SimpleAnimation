using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 可以枚举SimpleAnimation.SimpleAnimationPlayable的m_States
public class StateEnumerable : IEnumerable<State>
{
    private SimpleAnimation m_Owner;

    public StateEnumerable(SimpleAnimation owner)
    {
        m_Owner = owner;
    }

    // 获取Enumerator
    public IEnumerator<State> GetEnumerator()
    {
        return new StateEnumerator(m_Owner);
    }

    // 获取Enumerator
    // 实现的接口!
    IEnumerator IEnumerable.GetEnumerator()
    {
        return new StateEnumerator(m_Owner);
    }

    class StateEnumerator : IEnumerator<State>
    {
        private SimpleAnimation m_Owner;

        // 再封装住IStateEnumerator
        private IEnumerator<IState> m_Impl;

        public StateEnumerator(SimpleAnimation owner)
        {
            m_Owner = owner;
            // 从Playble中返回状态
            // 返回的是IStateEnumerable
            // IStateEnumerator
            m_Impl = m_Owner.Playable.GetStates().GetEnumerator();
            Reset();
        }

        State GetCurrent()
        {
            // StateImpl:把IState变成State
            return new StateImpl(m_Impl.Current, m_Owner);
        }

        object IEnumerator.Current { get { return GetCurrent(); } }

        State IEnumerator<State>.Current { get { return GetCurrent(); } }

        public void Dispose() { }

        public bool MoveNext()
        {
            return m_Impl.MoveNext();
        }

        public void Reset()
        {
            m_Impl.Reset();
        }
    }
}
