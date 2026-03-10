using UnityEngine;

public class OpenGate : MonoBehaviour, IInteractable, ILoopResettable
{
    public Transform gate;
    public Vector3 openOffset = new Vector3(0, 3f, 0);
    public float openSpeed = 3f;

    [Header("Key Requirement")]
    public string requiredItemId = "GateKey";

    public int Priority => 10;

    Vector3 closedPos;
    Vector3 openPos;
    bool isOpen;

    void Start()
    {
        closedPos = gate.position;
        openPos = closedPos + openOffset;

        if (GamePhaseManager.Instance.CurrentPhase != GamePhase.Tutorial)
        {
            isOpen = true;
            gate.position = openPos;
        }
    }

    void Update()
    {
        Vector3 target = isOpen ? openPos : closedPos;
        gate.position = Vector3.MoveTowards(
            gate.position,
            target,
            openSpeed * Time.deltaTime
        );
    }

    public bool CanInteract()
    {
        if (isOpen) return false;
        return PlayerInventory.Instance.HasGateKey;
    }

    public string GetPromptText()
    {
        if (!PlayerInventory.Instance.HasGateKey)
            return "Requires a key";

        return "Press E to Open Gate";
    }

    public void Interact()
    {
        isOpen = true;

        GamePhaseManager.Instance.CompleteTutorial();

        TimeLoopManager.Instance.SetSpawnPointToCurrentPhase();
        
    }


    public void ResetState()
    {
        if (GamePhaseManager.Instance.CurrentPhase == GamePhase.MainGame)
        {
            isOpen = true;
            gate.position = openPos;
            return;
        }

        isOpen = false;
        gate.position = closedPos;
    }
}
