using UnityEngine;

// Q: 同SimpleAnimation中的State?
public interface IState
{
    bool enabled { get; set; }

    float time { get; set; }

    float normalizedTime { get; set; }

    float speed { get; set; }

    string name { get; set; }

    float weight { get; set; }

    float length { get; }

    AnimationClip clip { get; }

    WrapMode wrapMode { get; }
    bool IsValid();
}