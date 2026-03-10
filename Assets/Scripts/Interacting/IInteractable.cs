public interface IInteractable
{
    bool CanInteract();
    string GetPromptText();
    void Interact();

    int Priority { get; }
}
