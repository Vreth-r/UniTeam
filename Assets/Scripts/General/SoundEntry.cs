using UnityEngine;

[System.Serializable]
public class SoundEntry
{
    public string soundId;
    public SoundCategory category;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
    [Range(0f, 1f)] public float pitch = 1f;
    public bool loop = false;
}

public enum SoundCategory
{
    Music,
    UI,
    Gameplay
}