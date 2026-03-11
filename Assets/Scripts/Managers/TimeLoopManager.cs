using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Linq;

public class TimeLoopManager : MonoBehaviour
{
    public static TimeLoopManager Instance;
    public const float SHIFT_CHANGE_TIME = 6f;
    private float elapsedTime;

    [Header("Loop Settings")]
    public float loopDuration = 300f; // 5 minutes
    private float timeRemaining;

    [Header("Player Reset")]
    public Transform player;
    private Vector3 playerStartPos;
    private Quaternion playerStartRot;

    [Header("UI")]
    public TMP_Text timeText;
    private bool showTime;

    [Header("Checkpoint Transforms")]
    public Transform tutorialStartPoint;
    public Transform mainGameStartPoint;

    [Header("Spawn Points")]
    public Transform tutorialSpawn;
    public Transform mainGameSpawn;

    bool loopStopped = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        timeRemaining = loopDuration;
        elapsedTime = 0f;

        SetSpawnPointToCurrentPhase();

    }

    void Update()
    {
        // Counts down the time in the loop and can be displayed with T
        if (!loopStopped)
        {
            float dt = Time.deltaTime;

            timeRemaining -= dt;
            elapsedTime += dt;

            if (timeRemaining <= 0f)
            {
                ResetLoop();
            }
        }

        if (showTime)
        {
            timeText.text = GetFormattedTime();
        }
        if (Keyboard.current != null && Keyboard.current.vKey.wasPressedThisFrame)
        {
            if (UIState.IsUIOpen)
                return;
            ResetLoop();
        }
    }

    public void ShowTime(bool show)
    {
        showTime = show;

        if (timeText == null)
        {
            Debug.LogError("TimeText not assigned!");
            return;
        }

        timeText.gameObject.SetActive(show);

        if (show)
            timeText.text = GetFormattedTime();
    }


    public void ResetLoop()
    {
        
        Debug.Log("TIME LOOP RESET");

        timeRemaining = loopDuration;

        Transform resetPoint =
            GamePhaseManager.Instance.CurrentPhase == GamePhase.Tutorial
            ? tutorialStartPoint
            : mainGameStartPoint;

        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc) cc.enabled = false;

        player.position = resetPoint.position;
        player.rotation = resetPoint.rotation;

        if (cc) cc.enabled = true;

        // Calls all resettable objects to reset their state
        MonoBehaviour[] allMono = FindObjectsOfType<MonoBehaviour>(true);
        foreach (var obj in allMono)
        {
            if (obj is ILoopResettable resettable)
                resettable.ResetState();
        }

        elapsedTime = 0f;
    }


    public void ResetLoopFromDeath()
    {
        // Useful for debugging
        Debug.Log("PLAYER DIED — RESETTING LOOP");
        ResetLoop();
    }


    public float GetTimeRemaining()
    {
        return timeRemaining;
    }

    public string GetFormattedTime()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        return $"{minutes:00}:{seconds:00}";
    }

    public float GetElapsedTime()
    {
        return elapsedTime;
    }

    public void SetSpawnPointToCurrentPhase()
    {
        // Spawn player in correct place depending on their game phase
        if (GamePhaseManager.Instance.CurrentPhase == GamePhase.Tutorial)
        {
            playerStartPos = tutorialSpawn.position;
            playerStartRot = tutorialSpawn.rotation;
        }
        else
        {
            playerStartPos = mainGameSpawn.position;
            playerStartRot = mainGameSpawn.rotation;
        }

        Debug.Log("Loop anchor updated to " + GamePhaseManager.Instance.CurrentPhase);
    }

    public void StopLoopTimer()
    {
        loopStopped = true;
    }
}
