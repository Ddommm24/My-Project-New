using UnityEngine;

public class BreakableBlockade : MonoBehaviour, IInteractable
{
    public FacilityEntrance entrance;
    public int Priority => 10;

    public bool CanInteract()
    {
        // Handling loop logic
        if (!EntryRouteManager.Instance.HasEverEntered())
            return false;

        if (EntryRouteManager.Instance.HasEverBroken())
            return false;

        return PlayerInventory.Instance.HasAxe;
    }

    public string GetPromptText()
    {
        return "Press E to Break Blockade";
    }

    public void Interact()
    {
        EntryRouteManager.Instance.MarkBroken();
        entrance.HideForThisLoop();
    }
}