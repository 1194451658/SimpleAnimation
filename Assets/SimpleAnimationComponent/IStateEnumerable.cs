using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class IStateEnumerable : IEnumerable<IState>
{
    private SimpleAnimationPlayable m_Owner;

    public IStateEnumerable(SimpleAnimationPlayable owner)
    {
        m_Owner = owner;
    }

    public IEnumerator<IState> GetEnumerator()
    {
        return new IStateEnumerator(m_Owner);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return new IStateEnumerator(m_Owner);
    }

    class IStateEnumerator : IEnumerator<IState>
    {
        private int m_Index = -1;
        private int m_Version;
        private SimpleAnimationPlayable m_Owner;

        public IStateEnumerator(SimpleAnimationPlayable owner)
        {
            m_Owner = owner;
            m_Version = m_Owner.StatesVersion;
            Reset();
        }

        private bool IsValid()
        {
            return m_Owner != null && m_Version == m_Owner.StatesVersion;
        }

        IState GetCurrentHandle(int index)
        {
            if (!IsValid())
                throw new InvalidOperationException("The collection has been modified, this Enumerator is invalid");

            if (index < 0 || index >= m_Owner.States.Count)
                throw new InvalidOperationException("Enumerator is invalid");

            StateInfo state = m_Owner.States[index];
            if (state == null)
                throw new InvalidOperationException("Enumerator is invalid");

            return new StateHandle(m_Owner, state.index, state.playable);
        }

        object IEnumerator.Current
        {
            get { return GetCurrentHandle(m_Index); }
        }

        IState IEnumerator<IState>.Current
        {
            get { return GetCurrentHandle(m_Index); }
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            if (!IsValid())
                throw new InvalidOperationException("The collection has been modified, this Enumerator is invalid");

            do
            {
                m_Index++;
            } while (m_Index < m_Owner.States.Count && m_Owner.States[m_Index] == null);

            return m_Index < m_Owner.States.Count;
        }

        public void Reset()
        {
            if (!IsValid())
                throw new InvalidOperationException("The collection has been modified, this Enumerator is invalid");
            m_Index = -1;
        }
    }
}