using UnityEngine;

public class FacilityEntrance : MonoBehaviour, ILoopResettable
{
    public GameObject blockade;

    void Start()
    {
        blockade.SetActive(false);
    }

    // When entered once, block off
    public void OnPlayerEntered()
    {
        EntryRouteManager.Instance.MarkEntered();
    }

    void ApplyState()
    {
        if (EntryRouteManager.Instance.HasEverEntered())
        {
            blockade.SetActive(true);
        }
        else
        {
            blockade.SetActive(false);
        }
    }

    // Make blockade disappear when broken
    public void HideForThisLoop()
    {
        blockade.SetActive(false);
    }

    public void ResetState()
    {
        ApplyState();
    }
}