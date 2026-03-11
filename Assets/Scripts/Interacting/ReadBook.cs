using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ReadBook : MonoBehaviour, IInteractable
{
    public static ReadBook ActiveBook { get; private set; }

    public int Priority => 10;

    public GameObject bookUI;
    public Image pageImage;
    public TMP_Text captionText;
    public TMP_Text pageNumberText;

    public int codeIndex = 3; // gives the 4th number

    public Sprite[] pages;
    [TextArea(3, 10)]
    public string[] captions;

    private int page;

    void Start()
    {
        bookUI.SetActive(false);
    }

    public bool CanInteract()
    {
        return ActiveBook == null;
    }

    public string GetPromptText()
    {
        if (ReadBook.ActiveBook != null)
        {
            return "";
        }
        else
        {
            return "Press E to Read Book";
        }
  
    }

    public void Interact()
    {
        ActiveBook = this;
        page = 0;

        bookUI.SetActive(true);
        UpdatePage();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Turning the page with arrow keys
    void Update()
    {
        if (ActiveBook != this) return;

        if (Input.GetKeyDown(KeyCode.RightArrow)) NextPage();
        if (Input.GetKeyDown(KeyCode.LeftArrow)) PrevPage();

        if (Input.GetKeyDown(KeyCode.Escape))
            CloseBook();
    }

    void NextPage()
    {
        if (page < pages.Length - 1)
        {
            page++;
            UpdatePage();
        }
    }

    void PrevPage()
    {
        if (page > 0)
        {
            page--;
            UpdatePage();
        }
    }

    // Makes sure correct thing is displayed depending on the page
    void UpdatePage()
    {
        pageImage.sprite = pages[page];

        if (page == 4 && DoorCodeManager.Instance != null)
        {
            int number = DoorCodeManager.Instance.GetNumber(0);
            captionText.text =
                $"1990 - Our Holiday at Hotel {number}";
        }
        else
        {
            captionText.text = captions[page];
        }

        pageNumberText.text = $"{page + 1} / {pages.Length}";
    }


    void CloseBook()
    {
        UIState.EscapeConsumed = true; 

        ActiveBook = null;
        bookUI.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
