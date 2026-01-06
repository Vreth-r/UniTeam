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

    [YarnFunction("top_three_scores")]
    public static string TopThreeScores(
        float media,
        float jouranlism,
        float proCom,
        float perform,
        float crIndustries,
        float fashion,
        float imgArts,
        float gcm,
        float intDesign
    )
    {
        var scores = new Dicitonary<string, float>
        {
            { "Media", media },
            { "Journalism", jouranlism },
            { "ProCom", proCom },
            { "Performance", perform },
            { "Creative Industries", crIndustries },
            { "Fashion", fashion },
            { "Image Arts", imgArts },
            { "GCM", gcm },
            { "Interior Design", intDesign }
        };

        // Order descending and take top 3
        var topThree = scores
            .OrderByDescending(kv => kv.Value)
            .Take(3)
            .Select(kv => kv.Key);

        // Return as a comma-separated string (easy for Yarn)
        return string.Join(", ", topThree);
    }
}