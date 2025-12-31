using System;
using UnityEngine;

public static class AudioEvents
{
    // Play event
    public static event Action<string> OnPlaySound;

    // stop event
    public static event Action<string> OnStopSound;

    public static void Play(string soundId)
    {
        OnPlaySound?.Invoke(soundId);
    }

    public static void Stop(string soundId)
    {
        OnStopSound?.Invoke(soundId);
    }
}