using UnityEngine;
using TMPro;

public class DialogueTrigger : MonoBehaviour, ILoopResettable
{
    [Header("Dialogue Settings")]
    [TextArea]
    public string dialogueText;

    public float duration = 3f;
    public Color textColor = Color.white;

    [Tooltip("If true, dialogue will appear every loop. If false, it will only appear once per game.")]
    public bool repeatEveryLoop = true;

    TMP_Text dialogueUI;

    bool hasTriggeredThisLoop = false;
    bool hasTriggeredEver = false;

    void Start()
    {
        if (TutorialManager.Instance != null)
        {
            dialogueUI = TutorialManager.Instance.tutorialText;
        }
    }

    // Handles what text is displayed when player enters which area
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (!repeatEveryLoop && hasTriggeredEver)
            return;

        if (hasTriggeredThisLoop)
            return;

        ShowDialogue();

        hasTriggeredThisLoop = true;
        hasTriggeredEver = true;
    }

    void ShowDialogue()
    {
        if (dialogueUI == null)
            return;

        StopAllCoroutines();
        StartCoroutine(DialogueRoutine());
    }

    System.Collections.IEnumerator DialogueRoutine()
    {
        dialogueUI.color = textColor;
        dialogueUI.text = dialogueText;
        dialogueUI.gameObject.SetActive(true);

        yield return new WaitForSeconds(duration);

        dialogueUI.gameObject.SetActive(false);
    }

    public void ResetState()
    {
        hasTriggeredThisLoop = false;

        if (repeatEveryLoop)
            hasTriggeredEver = false;
    }
}