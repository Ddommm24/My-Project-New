using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class JournalUI : MonoBehaviour
{
    public GameObject panel;
    public TMP_Text historyText;
    public TMP_InputField inputField;
    public PlayerMovement playerMovement;
    public ScrollRect scrollRect;

    bool open = false;

    // Works the same way as inventory
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
        // Press enter to save entry which stores the text written so it can be displayed
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
        // Write previous entries to the journal when opened
        if (PlayerJournal.Instance == null)
            return;

        historyText.text = "";

        foreach (string entry in PlayerJournal.Instance.GetEntries())
        {
            historyText.text += "- " + entry + "\n\n";
        }

        if (historyText.text == "")
            historyText.text = "Journal Empty";

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(
            historyText.rectTransform.parent as RectTransform
        );

        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 0f;
        }
    }
}