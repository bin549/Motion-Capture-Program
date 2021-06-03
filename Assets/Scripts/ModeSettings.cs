using UnityEngine;

[DefaultExecutionOrder(32000)]
public class ModeSettings : MonoBehaviour
{
    [SerializeField] private VideoCapture videoCapture;

    private void Start()
    {
        if (PlayerPrefs.HasKey("video_url"))
        {
            string video_url = PlayerPrefs.GetString("video_url", "");
            Debug.Log(video_url);

            if (video_url != "")
            {
                videoCapture.videoPlayer.url = video_url;
            }

        }
        if (PlayerPrefs.HasKey("mode_option"))
        {
            int modeOption = PlayerPrefs.GetInt("mode_option");
            videoCapture.useWebCam = modeOption == 0 ? true : false;
        }
    }

    private void OnEnable()
    {
        string video_url = PlayerPrefs.GetString("video_url", "");
        videoCapture.videoPlayer.url = video_url;
        int modeOption = PlayerPrefs.GetInt("mode_option");
        videoCapture.useWebCam = modeOption == 0 ? true : false;
        Debug.Log(PlayerPrefs.GetString("video_url", ""));
        Debug.Log(PlayerPrefs.GetInt("mode_option"));
    }
}
