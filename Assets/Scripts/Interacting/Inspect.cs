using UnityEngine;
using TMPro;

public class Inspect : MonoBehaviour, IInteractable
{
    public TMP_Text descriptionText;
    public bool requiresUnlock = false;
    public bool unlocked = true;

    public int codeIndex = -1; // 0–3 are for the random code digits, -1 is normal inspect

    public int Priority => 10;
    
    public FacilityClock clockSource;

    [TextArea(5, 10)]
    public string description;


    void Start()
    {
        if (descriptionText != null)
            descriptionText.gameObject.SetActive(false);
    }

    public bool CanInteract()
    {
        if (requiresUnlock && !unlocked)
            return false;

        return !UIState.IsInspecting;
    }

    public string GetPromptText()
    {
        if (!UIState.IsInspecting) 
        {
            return "Press E to Inspect";
        }
        else
        {
            return "";
        }
        
    }

    public void Interact()
    {
        UIState.IsInspecting = true;
        descriptionText.gameObject.SetActive(true);

        if (codeIndex == 1 && DoorCodeManager.Instance != null)
        {
            int number = DoorCodeManager.Instance.GetNumber(codeIndex);
            descriptionText.text = description + number;
        }
        else if (codeIndex == 2 && DoorCodeManager.Instance != null)
        {
            int number = DoorCodeManager.Instance.GetNumber(codeIndex);
            descriptionText.text = description + number + " Hedgefield Drive";
        }
        else if (codeIndex == 3 && DoorCodeManager.Instance != null)
        {
            int number = DoorCodeManager.Instance.GetNumber(codeIndex);
            descriptionText.text = description + number;
        }
        else if (clockSource != null)
        {
            descriptionText.text = clockSource.GetClockText();
        }
        else
        {
            descriptionText.text = description;
        }
    }

    void Update()
    {
        if (!UIState.IsInspecting) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Close();
        }

    }

    void Close()
    {
        UIState.IsInspecting = false;
        descriptionText.gameObject.SetActive(false);
        UIState.LeftInspecting = true;

    }
}
