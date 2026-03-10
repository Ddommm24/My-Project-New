using UnityEngine;

public class FacilityEntrance : MonoBehaviour, ILoopResettable
{
    public GameObject blockade;

    void Start()
    {
        blockade.SetActive(false);
    }

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

    public void HideForThisLoop()
    {
        blockade.SetActive(false);
    }

    public void ResetState()
    {
        ApplyState();
    }
}