using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class EndingCutsceneManager : MonoBehaviour
{
    public static EndingCutsceneManager Instance;
    public EndingChoice endingChoice;

    [Header("UI")]
    public TMP_Text textUI;

    [Header("Player")]
    public Transform player;
    public PlayerMovement playerMovement;
    public Transform walkTarget;
    public float walkSpeed = 1.5f;

    [Header("Steps")]
    public List<EndingStep> steps;

    int index = 0;
    float timer = 0f;
    bool playing = false;

    void Awake()
    {
        Instance = this;
    }

    public void StartCutscene()
    {
        // Only start cutscene once
        if (playing)
            return;

        playing = true;

        // Disable player control
        playerMovement.enabled = false;

        // Disable enemies
        EnemyAI[] enemies = FindObjectsOfType<EnemyAI>();
        foreach (EnemyAI enemy in enemies)
            enemy.DisableAI();

        // Show ending text
        textUI.gameObject.SetActive(true);

        StartStep();
    }

    // Ending Cutscene is made up of different steps of dialogue which are iterated through
    void Update()
    {
        if (!playing)
            return;

        timer += Time.deltaTime;

        // Move player
        player.position = Vector3.MoveTowards(
            player.position,
            walkTarget.position,
            walkSpeed * Time.deltaTime
        );

        if (timer >= steps[index].duration)
        {
            index++;

            if (index >= steps.Count)
            {
                EndCutscene();
                return;
            }

            StartStep();
        }
    }

    void StartStep()
    {
        timer = 0f;

        EndingStep step = steps[index];

        textUI.text = step.text;
        textUI.color = step.textColor;
    }

    void EndCutscene()
    {
        playing = false;

        textUI.gameObject.SetActive(false);

        endingChoice.ShowChoice();
    }
}

// A step contains text, duration and text colour which can be changed in the Unity editor
[System.Serializable]
public class EndingStep
{
    [TextArea]
    public string text;

    public float duration = 4f;

    public Color textColor = Color.white;
}