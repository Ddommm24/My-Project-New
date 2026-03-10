using UnityEngine;

public class EntryRouteManager : MonoBehaviour
{
    public static EntryRouteManager Instance;

    private bool hasEverEntered;
    private bool hasEverBroken;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public bool HasEverEntered() => hasEverEntered;
    public bool HasEverBroken() => hasEverBroken;

    public void MarkEntered()
    {
        hasEverEntered = true;
    }

    public void MarkBroken()
    {
        hasEverBroken = true;
    }
}