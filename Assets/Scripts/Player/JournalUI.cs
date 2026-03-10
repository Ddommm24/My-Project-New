using UnityEngine;
using TMPro;

public class JournalUI : MonoBehaviour
{
    public GameObject panel;
    public TMP_Text historyText;
    public TMP_InputField inputField;
    public PlayerMovement playerMovement;

    bool open = false;

    void Start()
    {
        panel.SetActive(false);

    }

    void Update()
    {
        if (!open && Input.GetKeyDown(KeyCode.J) && Time.timeScale > 0f && !UIState.IsUIOpen)
        {
            Open();
        }
        else if (open && Input.GetKeyDown(KeyCode.Escape))
        {
            Close();
        }

        if (open && Input.GetKeyDown(KeyCode.Return))
        {
            SaveEntry();
        }
    }

    void Open()
    {
        open = true;
        panel.SetActive(true);
        UIState.IsUIOpen = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (playerMovement != null)
            playerMovement.enabled = false;

        Refresh();
        inputField.ActivateInputField();
    }

    void Close()
    {
        open = false;
        panel.SetActive(false);
        UIState.IsUIOpen = false;
        UIState.EscapeConsumed = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerMovement != null)
            playerMovement.enabled = true;
    }

    void SaveEntry()
    {
        string text = inputField.text;

        if (string.IsNullOrWhiteSpace(text))
            return;

        if (PlayerJournal.Instance != null)
        {
            PlayerJournal.Instance.AddEntry(text.Trim());
        }

        inputField.text = "";
        Refresh();
        inputField.ActivateInputField();
    }

    void Refresh()
    {
        if (PlayerJournal.Instance == null)
            return;

        historyText.text = "";

        foreach (string entry in PlayerJournal.Instance.GetEntries())
        {
            historyText.text += "- " + entry + "\n\n";
        }

        if (historyText.text == "")
            historyText.text = "Journal Empty";
    }
}