using UnityEngine;

public class ShiftDoorButton : MonoBehaviour, IInteractable
{
    public TimedShiftDoor door;
    public float manualOpenDuration = 10f;

    public int Priority => 10;

    public bool CanInteract()
    {
        return true;
    }

    public string GetPromptText()
    {
        return "Press E to Open Door";
    }

    public void Interact()
    {
        

        Debug.Log($"Button interacted, opening door on {door.gameObject.name}");
        door.OpenForDuration(manualOpenDuration);
    }
}
