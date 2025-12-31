using UnityEngine;
using Yarn.Unity;
using System.Collections;

public class CutsceneYarnCommands : MonoBehaviour
{
    // fires an event to the audio manager
    [YarnCommand("PlaySound")]
    public static void PlaySound(string soundId)
    {
        AudioEvents.Play(soundId);
    }
}