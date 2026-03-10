using UnityEngine;

public class PickupItem : MonoBehaviour, IInteractable, ILoopResettable
{
    public string itemId;
    public string promptText = "Pick up item";

    [Header("Permanent Disable After Break")]
    public bool disableAfterBlockadeBroken = false;

    [Header("Optional Chain Object")]
    public GameObject chainObject;

    public int Priority => 10;

    private bool pickedUp;

    void Start()
    {
        UpdatePermanentVisualState();
    }

    void UpdatePermanentVisualState()
    {
        if (disableAfterBlockadeBroken &&
            EntryRouteManager.Instance.HasEverBroken())
        {
            if (chainObject != null)
                chainObject.SetActive(true);
        }
        else
        {
            if (chainObject != null)
                chainObject.SetActive(false);
        }
    }

    public bool CanInteract()
    {
        if (pickedUp)
            return false;

        if (disableAfterBlockadeBroken &&
            EntryRouteManager.Instance.HasEverBroken())
            return false;

        return true;
    }

    public string GetPromptText()
    {
        if (!CanInteract())
            return "";

        return promptText;
    }

    public void Interact()
    {
        PlayerInventory.Instance.AddItem(itemId);
        pickedUp = true;
        gameObject.SetActive(false);
    }

    public void ResetState()
    {
        pickedUp = false;
        gameObject.SetActive(true);

        UpdatePermanentVisualState();
    }
}