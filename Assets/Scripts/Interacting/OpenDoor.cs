using UnityEngine;

public class OpenDoor : MonoBehaviour, IInteractable, ILoopResettable
{
    public Transform door;
    public Vector3 openOffset = new Vector3(0, 3f, 0);
    public float openSpeed = 3f;

    public int Priority => 10;

    Vector3 closedPos;
    Vector3 openPos;
    bool isOpen;

    void Start()
    {
        closedPos = door.position;
        openPos = closedPos + openOffset;
    }

    void Update()
    {
        Vector3 target = isOpen ? openPos : closedPos;
        door.position = Vector3.MoveTowards(
            door.position,
            target,
            openSpeed * Time.deltaTime
        );
    }

    public bool CanInteract()
    {
        return !isOpen;
    }

    public string GetPromptText()
    {
        return "Press E to Open Door";
    }

    public void Interact()
    {
        isOpen = true;
    }

    public void ResetState()
    {
        isOpen = false;
        door.position = closedPos;
    }
}
