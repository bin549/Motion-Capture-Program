using SFB;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

[DefaultExecutionOrder(32001)]
public class SceneUI : MonoBehaviour
{
    [SerializeField] private Button btnVideoSelect;
    [SerializeField] private MotionDataRecorder motionDataRecorder;
    [SerializeField] private BVHRecorder bvhRecorder;
    [SerializeField] private GameObject UIComponents;
    public bool isUIActive = true;
    public bool isRecording = false;
    [SerializeField] private BackButton backButton;
    [SerializeField] private Button recordButton;
    [SerializeField] private Animator recordButtonAnimator;
    [SerializeField] private Toggle isBVHRecorder;
    [SerializeField] private Text message;
    public Avatar avatar;
    [SerializeField] private Animator avatarAnimator;
    string msg = "";
    [SerializeField] private AppSettings appSettings;

    private void Awake()
    {
        appSettings = GameObject.FindObjectOfType<AppSettings>();
        motionDataRecorder = GameObject.FindObjectOfType<MotionDataRecorder>();
        backButton = GameObject.FindObjectOfType<BackButton>();
        UIComponents = GameObject.Find("UIComponents");
        recordButton = GameObject.Find("recordButton").GetComponent<Button>();
        btnVideoSelect = GameObject.Find("btnVideoSelect").GetComponent<Button>();
        isBVHRecorder = GameObject.Find("isBVHRecorder").GetComponent<Toggle>();
    }

    private void Start()
    {
        appSettings = GameObject.FindObjectOfType<AppSettings>();
        avatar = appSettings.GetAvatar();
        avatarAnimator = appSettings.GetAvatarAnimator();
        recordButtonAnimator = recordButton.gameObject.GetComponent<Animator>();
        message = GameObject.Find("Message").GetComponent<Text>();
        UIComponents.SetActive(false);
        avatarAnimator = avatar.gameObject.GetComponent<Animator>();

        btnVideoSelect.onClick.AddListener(() =>
      {
          backButton.VideoSelect();
      });

        recordButton.onClick.AddListener(() =>
      {
          RecordMotion();
      });

    }

    private void Update()
    {
        ShowUI();
        ChangeRecordExportMode();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RecordMotion();
        }
    }

    public void ShowUI()
    {
        if (Input.GetKeyDown(KeyCode.Home))
        {
            isUIActive = !isUIActive;
            UIComponents.SetActive(isUIActive);
        }
    }

    public void ChangeRecordExportMode()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            isBVHRecorder.isOn = !isBVHRecorder.isOn;
            Debug.Log(isBVHRecorder.isOn);
        }
    }


    public void RecordMotion()
    {
        msg = "";
        if (!isBVHRecorder.isOn)
        {
            if (!motionDataRecorder.recording)
            {
                recordButtonAnimator.SetBool("isRecording", true);
                motionDataRecorder.RecordStart();
            }
            else
            {
                try
                {
                    motionDataRecorder.RecordEnd();
                    recordButtonAnimator.SetBool("isRecording", false);
                    msg = "Record Anim Success!";
                }
                catch (System.Exception e)
                {
                    msg = "Record Anim Fail！!";
                    Debug.LogError("Fail！" + e.Message + e.StackTrace);
                }
            }
        }
        else
        {
            if (!isRecording)
            {
                recordButtonAnimator.SetBool("isRecording", true);
                bvhRecorder = gameObject.AddComponent<BVHRecorder>();
                isRecording = true;
                Debug.Log(isBVHRecorder.isOn);
                bvhRecorder.targetAvatar = avatarAnimator;
                bvhRecorder.scripted = true;
                bvhRecorder.blender = true;
                bvhRecorder.enforceHumanoidBones = true;
                bvhRecorder.getBones();
                bvhRecorder.rootBone = avatar.jointPoints[PositionIndex.Hip.Int()].transform;
                bvhRecorder.buildSkeleton();
                bvhRecorder.genHierarchy();
                bvhRecorder.capturing = true;
                bvhRecorder.frameRate = 60f;
                bvhRecorder.catchUp = true;
            }
            else
            {
                try
                {
                    isRecording = false;
                    recordButtonAnimator.SetBool("isRecording", false);
                    bvhRecorder.capturing = false;
                    var path = appSettings.GetSavedFolderPath();
                    if (path.Length != 0)
                    {
                        FileInfo fi = new FileInfo(path);
                        bvhRecorder.directory = fi.DirectoryName;
                        bvhRecorder.filename = string.Format("RecordMotion_{0}{1:yyyy_MM_dd_HH_mm_ss}.bvh", "motion", DateTime.Now);
                        bvhRecorder.saveBVH();
                    }
                    bvhRecorder.clearCapture();
                    bvhRecorder = null;
                    msg = "Record BVH Success!";
                }
                catch (System.Exception e)
                {
                    msg = "Record BVH Fail！!";
                    Debug.LogError("Fail！" + e.Message + e.StackTrace);
                }
            }
        }
        message.gameObject.SetActive(true);
        message.text = msg;
        Invoke("MessageHide", 0.5f);
    }

    public void MessageHide()
    {

        message.gameObject.SetActive(false);
    }
}
