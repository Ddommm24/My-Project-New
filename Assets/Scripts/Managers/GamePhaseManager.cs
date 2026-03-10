using UnityEngine;

public enum GamePhase
{
    Tutorial,
    MainGame
}

public class GamePhaseManager : MonoBehaviour
{
    public static GamePhaseManager Instance;

    public GamePhase CurrentPhase { get; private set; } = GamePhase.Tutorial;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void CompleteTutorial()
    {
        if (CurrentPhase == GamePhase.MainGame)
            return;

        Debug.Log("TUTORIAL COMPLETED");
        CurrentPhase = GamePhase.MainGame;
    }
}
