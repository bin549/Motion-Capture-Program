using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ModeSelect : MonoBehaviour
{
    [SerializeField] public ModeImage[] images;
    [SerializeField] public ModeImage current_image;
    public int currentModeOption;

    private void Awake()
    {
        images = GetComponentsInChildren<ModeImage>(false);
    }

    private void Start()
    {
        for (int i = 0; i < images.Length; i++)
        {
            images[i].Init(this);
        }
        current_image = images[currentModeOption];
        current_image.Selected();
    }

    public void ClearChoose()
    {
        NextImage();
    }

    private void Update()
    {
        ChooseMode();
    }

    private void ChooseMode()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            PeviewImage();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            NextImage();
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (current_image != null)
            {
                current_image.Confirm();
            }
        }
    }

    private void PeviewImage()
    {
        current_image.UnSelected();
        currentModeOption--;
        if (currentModeOption == -1)
        {
            currentModeOption = images.Length - 1;
        }
        current_image = images[currentModeOption];
        current_image.Selected();
    }

    private void NextImage()
    {
        current_image.UnSelected();
        currentModeOption++;
        currentModeOption %= images.Length;
        current_image = images[currentModeOption];
        current_image.Selected();
    }

    public void SetCurrentModeOption(int modeOption)
    {
        if (currentModeOption != modeOption)
        {
            NextImage();
        }
    }
}
