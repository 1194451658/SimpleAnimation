using UnityEngine;
using UnityEngine.Playables;

public static class CustomPlayableExtensions
{
    // Q: 调用2次SetTime??
    public static void ResetTime(this Playable playable, float time)
    {
        playable.SetTime(time);
        playable.SetTime(time);
    }
}
