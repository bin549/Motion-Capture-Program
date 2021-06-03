using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ModeImage : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private string next_scene;
    [SerializeField] private int modeOption;
    public ModeSelect modeSelect;
    [SerializeField] private AppSettings appSettings;
    [SerializeField] private Text message;
    public SceneFader sceneFader;
    [SerializeField] private Button showPanelButton;

    public void Init(ModeSelect modeSelect)
    {
        this.modeSelect = modeSelect;
    }

    private void Awake()
    {
        message = GameObject.Find("Message").GetComponent<Text>();
        appSettings = GameObject.FindObjectOfType<AppSettings>();
        sceneFader = GameObject.FindObjectOfType<SceneFader>();
        showPanelButton = GameObject.Find("ShowPanelButton").GetComponent<Button>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        UnSelected();
    }

    public void Selected()
    {
        animator.enabled = true;
    }

    public void UnSelected()
    {
        if (animator != null)
        {
            animator.enabled = false;
        }
    }
    public void Confirm()
    {
        PlayerPrefs.SetInt("mode_option", modeOption);

        bool isUseVideo = PlayerPrefs.GetInt("mode_option") == 1 ? true : false;
        if (isUseVideo && string.IsNullOrEmpty(appSettings.GetVideosFolderPath()))
        {
            message.gameObject.SetActive(true);
            string msg = "set path first.";
            message.text = msg;
            Invoke("MessageHide", 1.0f);
            return;
        }
        showPanelButton.gameObject.SetActive(false);
        sceneFader.FadeTo(next_scene);
    }

    private void OnMouseEnter()
    {
        modeSelect.SetCurrentModeOption(modeOption);
        Selected();
    }

    private void OnMouseExit()
    {
        //  modeSelect.ClearChoose();
        //  UnSelected();
    }

    private void OnMouseDown()
    {
        Confirm();
    }


    public void MessageHide()
    {

        message.gameObject.SetActive(false);
    }
}
