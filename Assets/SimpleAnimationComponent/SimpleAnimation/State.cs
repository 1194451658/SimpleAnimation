using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 动作状态
// Q: 同SimpleAnimationPlayable_States.cs中的IState
public interface State
{
    bool enabled { get; set; }
    bool isValid { get; }
    float time { get; set; }
    float normalizedTime { get; set; }
    float speed { get; set; }
    string name { get; set; }
    float weight { get; set; }
    float length { get; }
    AnimationClip clip { get; }
    WrapMode wrapMode { get; set; }

}
