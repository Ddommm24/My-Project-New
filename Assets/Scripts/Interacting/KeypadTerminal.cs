using UnityEngine;
using TMPro;

public class KeypadTerminal : MonoBehaviour
{
    public BigDoor door;
    public TMP_Text terminalText;

    [Header("Interaction")]
    public float interactDistance = 3f;

    Camera cam;

    bool enteringCode;
    string currentInput = "";
    int[] enteredNumbers = new int[4];
    int currentIndex;

    void Start()
    {
        cam = Camera.main;
        terminalText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!enteringCode)
        {
            CheckForLook();
        }
        else
        {
            HandleCodeInput();
        }
    }

    // See if looking
    void CheckForLook()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            if (hit.collider.gameObject == gameObject)
            {
                terminalText.gameObject.SetActive(true);
                terminalText.text = "Press E to use terminal";

                if (Input.GetKeyDown(KeyCode.E))
                {
                    EnterTerminal();
                }

                return;
            }
        }

        terminalText.gameObject.SetActive(false);
    }

    void EnterTerminal()
    {
        enteringCode = true;
        currentIndex = 0;
        currentInput = "";

        terminalText.text = "ENTER NUMBER 1:";
    }

    void HandleCodeInput()
    {
        for (int i = 0; i <= 9; i++)
        {
            if (Input.GetKeyDown(i.ToString()))
            {
                if (currentInput.Length < 3)
                {
                    currentInput += i;
                    terminalText.text = currentInput;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Backspace) && currentInput.Length > 0)
        {
            currentInput = currentInput.Substring(0, currentInput.Length - 1);
            terminalText.text = currentInput;
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            ConfirmNumber();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitTerminal();
        }
    }

    void ConfirmNumber()
    {
        if (!int.TryParse(currentInput, out int value) || value < 1 || value > 100)
        {
            terminalText.text = "INVALID NUMBER (1–100)";
            currentInput = "";
            return;
        }

        enteredNumbers[currentIndex] = value;
        currentIndex++;
        currentInput = "";

        if (currentIndex >= 4)
        {
            SubmitCode();
        }
        else
        {
            terminalText.text = $"ENTER NUMBER {currentIndex + 1}:";
        }
    }

    void SubmitCode()
    {
        enteringCode = false;

        if (DoorCodeManager.Instance.CheckCode(enteredNumbers))
        {
            terminalText.text = "ACCESS GRANTED";
            door.TryOpen(enteredNumbers);
        }
        else
        {
            terminalText.text = "ACCESS DENIED";
            Invoke(nameof(ExitTerminal), 1.5f);
        }
    }

    void ExitTerminal()
    {
        enteringCode = false;
        terminalText.gameObject.SetActive(false);
    }
}
