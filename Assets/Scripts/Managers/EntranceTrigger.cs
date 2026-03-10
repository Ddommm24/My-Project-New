using UnityEngine;

public class EntranceTrigger : MonoBehaviour, ILoopResettable
{
    public FacilityEntrance entrance;
    bool triggered;

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;
        entrance.OnPlayerEntered();
    }

    public void ResetState()
    {
        triggered = false;
    }
}