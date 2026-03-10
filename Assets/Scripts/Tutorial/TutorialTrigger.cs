using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    public string triggerId;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        TutorialManager.Instance.NotifyTrigger(triggerId);
        gameObject.SetActive(false);
    }
}
