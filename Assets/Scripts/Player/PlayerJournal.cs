using UnityEngine;
using System.Collections.Generic;

public class PlayerJournal : MonoBehaviour
{
    public static PlayerJournal Instance;

    List<string> entries = new List<string>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AddEntry(string entry)
    {
        entries.Add(entry);
        Debug.Log("Entry Added: " + entry);
    }

    public List<string> GetEntries()
    {
        return entries;
    }
}