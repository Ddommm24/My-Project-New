using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject panel;

    bool paused;

    void Start()
    {
        panel.SetActive(false);
    }

    void Update()
    {
        if (UIState.EscapeConsumed)
        {
            UIState.EscapeConsumed = false;
            return;
        }
        if (UIState.LeftInspecting)
        {
            UIState.LeftInspecting = false;
            return;
        }
        if (ReadBook.ActiveBook != null)
            return;
            
        // Only allow pause if no other UI is open
        if (Input.GetKeyDown(KeyCode.Escape) && !UIState.IsUIOpen)
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (UIState.IsUIOpen || UIState.IsInspecting)
            return;

        paused = !paused;

        panel.SetActive(paused);

        if (paused)
        {
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void ResumeGame()
    {
        paused = false;
        panel.SetActive(false);

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ExitGame()
    {
        Application.Quit();

        // For testing inside editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}