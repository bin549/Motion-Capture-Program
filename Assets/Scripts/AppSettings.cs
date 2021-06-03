using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class AppSettings : MonoBehaviour
{
    private string videosFolderPath;
    private string modelsFolderPath;
    private string savedFolderPath;
    private static AppSettings instance;
    public bool showSkeleton = false;
    public bool isBVHRecorder = false;
    public int isMaleModel = 1;
    public VideoPlayer videoPlayer;
    public Avatar avatar;
    [SerializeField] private Animator avatarAnimator;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        videosFolderPath = FolderUtils.CheckDirectory(Application.dataPath + @"/Videos");
        modelsFolderPath = FolderUtils.CheckDirectory(Application.dataPath + @"/Models");
        savedFolderPath = FolderUtils.CheckDirectory(Application.dataPath + @"/Resources");
        if (string.IsNullOrEmpty(savedFolderPath))
        {
            FolderUtils.SafeCreateDirectory(Application.dataPath + @"/Resources");
            savedFolderPath = FolderUtils.CheckDirectory(Application.dataPath + @"/Resources");
        }
    }

    public void SetVideoPlayer(VideoPlayer videoPlayer)
    {
        this.videoPlayer = videoPlayer;
    }

    public VideoPlayer GetVideoPlayer()
    {
        if (videoPlayer != null)
        {
            return videoPlayer;
        }
        return null;
    }

    public void SetAvatar(Avatar avatar)
    {
        this.avatar = avatar;
    }

    public Avatar GetAvatar()
    {
        if (avatar != null)
        {
            return avatar;
        }
        return null;
    }


    public Animator GetAvatarAnimator()
    {
        if (avatarAnimator != null)
        {
            return avatarAnimator;
        }
        return null;
    }

    public void SetAvatarAnimator(Animator avatarAnimator)
    {
        this.avatarAnimator = avatarAnimator;
    }

    public string GetVideosFolderPath()
    {
        return videosFolderPath;
    }

    public string[] GetModels()
    {
        string[] extensions = new string[]
      {
        ".fbx", ".obj"
      };
        return FolderUtils.GetFilterdFiles(modelsFolderPath, extensions);
    }

    public string[] GetVideos()
    {
        string[] extensions = new string[]
        {
            ".mp4", ".mov"
        };
        //var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);

        //SelectFolder();
        return FolderUtils.GetFilterdFiles(videosFolderPath, extensions);
    }

    public string GetModelsFolderPath()
    {
        return modelsFolderPath;
    }
    public string GetSavedFolderPath()
    {
        return savedFolderPath;
    }

    public void SetVideosFolderPath()
    {
        var path = FolderUtils.SelectFolder();
        if (!string.IsNullOrEmpty(path))
        {
            videosFolderPath = path;
        }
    }

    public void SetModelsFolderPath()
    {
        var path = FolderUtils.SelectFolder();
        if (!string.IsNullOrEmpty(path))
        {
            modelsFolderPath = path;
        }
    }
    public void SetSavedFolderPath()
    {
        var path = FolderUtils.SelectFolder();
        if (!string.IsNullOrEmpty(path))
        {
            savedFolderPath = path;
        }
    }
}
