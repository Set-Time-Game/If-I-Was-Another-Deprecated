using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public GameObject settingsPannel;
    public List<GameObject> toDisable;
    public static SceneLoader Singletone { get; set; }

    private void Awake()
    {
        Singletone = this;

        if (PlayerPrefs.GetInt("JoystickScale") == 0)
            PlayerPrefs.SetInt("JoystickScale", 10);

        if (PlayerPrefs.GetInt("ButtonsScale") == 0)
            PlayerPrefs.SetInt("ButtonsScale", 10);
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape) && !Input.GetKeyDown(KeyCode.Return)) return;
        var curr = SceneManager.GetActiveScene().name;

        switch (curr)
        {
            case "Game":
                Settings();
                break;
            case "Main":
                try
                {
                    if (settingsPannel.activeSelf)
                        Settings();
                }
                catch
                {
                    // ignored
                }

                break;
        }
    }

    public void LoadScene(string scene = "Game")
    {
        if (SceneManager.GetActiveScene().name != scene)
            SceneManager.LoadScene(scene);
    }

    public void Settings()
    {
        try
        {
            Disabler();
            if (SceneManager.GetActiveScene().name == "Game")
            {
                var ts = Time.timeScale;
                ts = ts == 0 ? 1f : 0f;
                Time.timeScale = ts;
            }

            settingsPannel.SetActive(!settingsPannel.activeSelf);
            settingsPannel.GetComponentInChildren<VariableJoystick>().Save();
        }
        catch
        {
            // ignored
        }
    }

    private void Disabler()
    {
        if (toDisable.Count <= 0) return;
        foreach (var i in toDisable) i.SetActive(!i.activeSelf);
    }
}