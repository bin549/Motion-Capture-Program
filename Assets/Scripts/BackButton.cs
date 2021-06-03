using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BackButton : MonoBehaviour
{
    public SceneFader sceneFader;
    public GameObject quitPanel;

    private void Awake()
    {
        sceneFader = GameObject.FindObjectOfType<SceneFader>();
        GetComponent<Button>().onClick.AddListener(() =>
      {
          SceneBack();
      });
        if (SceneManager.GetActiveScene().name == Tags.START_SCENE)
        {
            quitPanel = GameObject.Find("Quit");
            quitPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (quitPanel != null)
            {
                quitPanel.SetActive(true);
            }
            else
            {
                sceneFader.FadeTo(Tags.START_SCENE);
            }
        }
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            SceneBack();
        }
    }


    private void SceneBack()
    {
        bool isUseWebCam = PlayerPrefs.GetInt("mode_option") == 0 ? true : false;
        if (isUseWebCam || SceneManager.GetActiveScene().name == Tags.VIDEOSELECT_SCENE)
        {
            sceneFader.FadeTo(Tags.START_SCENE);
        }
        else
        {
            sceneFader.FadeTo(Tags.VIDEOSELECT_SCENE);
        }
    }

    public void VideoSelect()
    {
        sceneFader.FadeTo(Tags.VIDEOSELECT_SCENE);
    }
}
