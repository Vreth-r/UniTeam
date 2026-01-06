using UnityEngine;
using UnityEngine.Video;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;

public class IntroRunner : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        videoPlayer.url = "Assets/Videos/Pan Machine(1).webm";
        videoPlayer.loopPointReached += LoopText;
        InputSystem.onAnyButtonPress.Call(HandleClick);
        
    }

    // Update is called once per frame
    void LoopText(VideoPlayer vp)
    {
        if (videoPlayer.url == "Assets/Videos/Pan Machine(1).webm")
        {
            videoPlayer.url = "Assets/Videos/Text Loop Machine.webm";
            videoPlayer.isLooping = true;
        }

        if (videoPlayer.url == "Assets/Videos/Loading Machine(1).webm")
        {
            SceneManager.LoadSceneAsync("QuizLayout");
        }
    }

    void HandleClick(InputControl button)
    {
        if (videoPlayer.url == "Assets/Videos/Text Loop Machine.webm")
        {
            videoPlayer.url = "Assets/Videos/Loading Machine(1).webm";
            videoPlayer.isLooping = false;
        }
    }
}
