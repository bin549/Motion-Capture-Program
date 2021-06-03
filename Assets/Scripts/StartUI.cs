using UnityEngine;
using UnityEngine.UI;

public class StartUI : MonoBehaviour
{
    public AppSettingsPanel appSettingsPanel;
    [SerializeField] private Button showPanelButton;
    public bool isPanelActive = false;

    private void Awake()
    {
        appSettingsPanel = GameObject.FindObjectOfType<AppSettingsPanel>();
        showPanelButton = GameObject.Find("ShowPanelButton").GetComponent<Button>();
        showPanelButton.onClick.AddListener(() =>
        {
            ShowPanel();
        });
    }

    private void Start()
    {
        appSettingsPanel.gameObject.SetActive(false);
        showPanelButton.gameObject.SetActive(false);
        Invoke("ShowButton", 0.5f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (appSettingsPanel == null)
            {
                return;
            }

            if (!isPanelActive)
            {
                ShowPanel();
            }
            else
            {
                appSettingsPanel.Fade();
            }
            isPanelActive = !isPanelActive;
        }
    }

    public void ShowPanel()
    {
        appSettingsPanel.gameObject.SetActive(true);
    }

    public void ShowButton()
    {
        showPanelButton.gameObject.SetActive(true);
    }
}
