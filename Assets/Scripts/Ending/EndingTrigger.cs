using UnityEngine;

public class EndingTrigger : MonoBehaviour
{
    // For tracking when the player is at the point where the cutscene starts from
    bool triggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (triggered)
            return;

        if (other.CompareTag("Player"))
        {
            triggered = true;
            TimeLoopManager.Instance.StopLoopTimer();
            EndingCutsceneManager.Instance.StartCutscene();
        }
    }
}