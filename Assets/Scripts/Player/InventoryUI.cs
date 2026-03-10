using UnityEngine;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public GameObject panel;
    public TMP_Text itemsText;

    bool open = false;


    void Update()
    {
        if (!open && Input.GetKeyDown(KeyCode.I) && !UIState.IsUIOpen)
        {
            Open();
        }
        else if (open && Input.GetKeyDown(KeyCode.Escape))
        {
            Close();
        }
    }

    void Open()
    {
        open = true;
        panel.SetActive(true);
        UIState.IsUIOpen = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("Inventory Open Called");

        Refresh();
    }

    void Close()
    {
        open = false;
        panel.SetActive(false);
        UIState.IsUIOpen = false;
        UIState.EscapeConsumed = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }


    void Refresh()
    {
        if (PlayerInventory.Instance == null)
            return;

        itemsText.text = "";

        if (PlayerInventory.Instance.HasScrewdriver)
            itemsText.text += "- Screwdriver\n";

        if (PlayerInventory.Instance.HasAxe)
            itemsText.text += "- Axe\n";

        if (itemsText.text == "")
            itemsText.text = "Inventory Empty";
    }
}
