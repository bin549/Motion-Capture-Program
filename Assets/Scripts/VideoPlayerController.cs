using UnityEngine;
using UnityEngine.Video;

[DefaultExecutionOrder(2)]
public class VideoPlayerController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public AppSettings appSettings;

    public enum Status
    {
        VideoPlay,
        VideoPause,
    }
    private Status status = Status.VideoPlay;
    private void Awake()
    {
        appSettings = GameObject.FindObjectOfType<AppSettings>();
    }

    private void Start()
    {
        videoPlayer = appSettings.GetVideoPlayer();
    }

    private void Update()
    {
        ChangePlayProgress();
        if (Input.GetKeyDown(KeyCode.S))
        {
            ChangePlayStatus();
        }
    }

    public void ChangePlayProgress()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            videoPlayer.frame -= 180;

        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            videoPlayer.frame += 180;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            videoPlayer.frame = 180;
        }
    }

    public void ChangePlayStatus()
    {
        if (status == Status.VideoPlay)
        {
            videoPlayer.Pause();
            status = Status.VideoPause;
        }
        else if (status == Status.VideoPause)
        {
            videoPlayer.Play();
            status = Status.VideoPlay;
        }
    }

    public bool IsPlay()
    {
        return status == Status.VideoPlay;
    }

    public bool IsPause()
    {
        return status == Status.VideoPause;
    }
}
