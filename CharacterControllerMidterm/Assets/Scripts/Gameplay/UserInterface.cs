using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInterface : MonoBehaviour
{
    public static UserInterface userInterface;
    [SerializeField] private GameObject healthBarText;
    [SerializeField] private GameObject curItemDisplay;
    [SerializeField] private GameObject inventoryUI;

    private void Awake()
    {
        if(userInterface != null)
        {
            Destroy(userInterface);
        }
        userInterface = this;
        DontDestroyOnLoad(userInterface);
    }

    public void TogglePauseScreen(bool paused)
    {
        if (!paused)
            DisablePauseScreen();
        else
            EnablePauseScreen();
    }

    public void EnablePauseScreen()
    {
        healthBarText.SetActive(false);
        curItemDisplay.SetActive(false);
        inventoryUI.SetActive(true);
    }

    public void DisablePauseScreen()
    {
        healthBarText.SetActive(true);
        curItemDisplay.SetActive(true);
        inventoryUI.SetActive(false);
    }
}
