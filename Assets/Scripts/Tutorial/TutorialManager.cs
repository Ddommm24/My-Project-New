using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    public TMP_Text tutorialText;
    public PlayerMovement playerMovement;
    public List<TutorialStepData> steps;

    int currentIndex = 0;
    float timer = 0f;

    bool stepActive = false;

    HashSet<string> enteredTriggers = new HashSet<string>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        tutorialText.gameObject.SetActive(false);
        TryStartStep();
    }

    void Update()
    {
        if (!stepActive || currentIndex >= steps.Count)
            return;

        timer += Time.deltaTime;
        var step = steps[currentIndex];

        bool minTimePassed = timer >= step.minimumTime;

        if (step.kind == TutorialStepKind.Timed)
        {
            if (minTimePassed)
                CompleteStep();
        }
        else if (step.kind == TutorialStepKind.KeyPress)
        {
            if (minTimePassed && Input.GetKeyDown(step.requiredKey))
                CompleteStep();
        }
    }

    void TryStartStep()
    {
        if (currentIndex >= steps.Count)
            return;

        var step = steps[currentIndex];

        // Trigger-gated step: wait until trigger entered
        if (!string.IsNullOrEmpty(step.startTriggerId) &&
            !enteredTriggers.Contains(step.startTriggerId))
        {
            tutorialText.gameObject.SetActive(false);
            stepActive = false;
            return;
        }

        // Start step
        timer = 0f;
        stepActive = true;

        tutorialText.text = step.text;
        tutorialText.gameObject.SetActive(true);

        // Only lock player for intro
        playerMovement.enabled = currentIndex != 0;
    }

    void CompleteStep()
    {
        tutorialText.gameObject.SetActive(false);
        stepActive = false;
        currentIndex++;

        TryStartStep();
    }

    public void NotifyTrigger(string triggerId)
    {
        enteredTriggers.Add(triggerId);
        TryStartStep();
    }
}

[System.Serializable]
public class TutorialStepData
{
    [TextArea]
    public string text;

    public float minimumTime = 2f;

    public TutorialStepKind kind;

    public KeyCode requiredKey;

    // For trigger-gated steps
    public string startTriggerId;
}

public enum TutorialStepKind
{
    Timed,
    KeyPress
}

