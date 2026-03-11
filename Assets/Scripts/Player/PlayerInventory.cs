using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour, ILoopResettable
{
    public static PlayerInventory Instance;

    private HashSet<string> items = new HashSet<string>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // Checks to see what items player has
    public bool HasScrewdriver => items.Contains("Screwdriver");
    public bool HasAxe => items.Contains("Axe");
    public bool HasGateKey => items.Contains("GateKey");

    public void AddItem(string id)
    {
        items.Add(id);
        Debug.Log($"Picked up {id}");
    }

    public void ResetState()
    {
        items.Clear();
        Debug.Log("Inventory reset");
    }
}
