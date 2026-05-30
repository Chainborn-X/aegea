namespace Aegea;

public interface IInteractable
{
    string GetPrompt();
    void Interact(PlayerController player);
}
