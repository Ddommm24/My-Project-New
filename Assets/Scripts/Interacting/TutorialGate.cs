using UnityEngine;

public class TutorialGateTrigger : MonoBehaviour
{
    private bool completed;

    void OnTriggerEnter(Collider other)
    {
        if (completed) return;
        if (!other.CompareTag("Player")) return;

        completed = true;

        GamePhaseManager.Instance.CompleteTutorial();
    }
}
