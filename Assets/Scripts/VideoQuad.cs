using UnityEngine.Video;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class VideoQuad : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    private RenderTexture videoTexture;
    public float sourceFps = 30f;
    private Animator animator;
    public SceneFader sceneFader;
    public Transform target;
    public Vector3 targetPos;
    public Vector3 originPos;
    public bool isForward, isBackward;
    public float smoothing = 2;

    private void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        animator = GetComponent<Animator>();
        sceneFader = GameObject.FindObjectOfType<SceneFader>();
        originPos = transform.position;
        targetPos = target.position;
        Destroy(target.gameObject);
    }

    private void Update()
    {
        if (isForward)
        {
            transform.position = Vector3.Lerp(originPos, targetPos, smoothing * Time.deltaTime);
            if (Vector3.Distance(originPos, targetPos) < 0.4f)
            {
                isForward = false;
            }
        }
        else if (isBackward)
        {
            transform.position = originPos;
            isBackward = false;
        }

    }

    public void SetupVideo(string path)
    {
        GetComponent<Renderer>().enabled = true;
        videoPlayer.url = path;
        VideoClip vclip = (VideoClip)Resources.Load(path);
        if (videoPlayer.clip != null)
        {
            if (videoPlayer.clip.width == 0 || videoPlayer.clip.height == 0)
            {
                videoTexture = new RenderTexture(1920, 1080, 24);
            }
            else
            {
                videoTexture = new RenderTexture((int)videoPlayer.clip.width, (int)videoPlayer.clip.height, 24);
            }
        }
        else
        {
            videoTexture = new RenderTexture(1920, 1080, 24);
        }
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = videoTexture;
        videoPlayer.Prepare();
        sourceFps = (float)videoPlayer.frameRate;
        this.gameObject.GetComponent<Renderer>().material.mainTexture = videoTexture;
        videoPlayer.Pause();
    }

    private void OnMouseEnter()
    {
        if (File.Exists(videoPlayer.url))
        {
            Selected();
        }
    }

    private void OnMouseExit()
    {
        UnSelected();
    }

    private void OnMouseDown()
    {
        if (videoPlayer.url != null)
        {
            Confirm();
        }
    }

    public void Selected()
    {
        isForward = true;
        isBackward = false;
        videoPlayer.Play();
        animator.SetBool("deLarge", false);
        animator.SetBool("enLarge", true);
    }

    public void UnSelected()
    {
        isForward = false;
        isBackward = true;
        videoPlayer.frame = 1280;
        videoPlayer.Pause();
        animator.SetBool("deLarge", true);
        animator.SetBool("enLarge", false);
    }

    public void Confirm()
    {
        PlayerPrefs.SetString("video_url", videoPlayer.url);
        sceneFader.FadeTo(Tags.SCENE);
    }
}
