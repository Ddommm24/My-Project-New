using UnityEngine;

public class EndingTrigger : MonoBehaviour
{
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