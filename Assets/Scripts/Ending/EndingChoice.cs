using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EndingChoice : MonoBehaviour
{
    public GameObject panel;

    [Header("Player")]
    public Transform player;
    public PlayerMovement playerMovement;

    [Header("Ending Walk Targets")]
    public Transform endingPointA;
    public Transform endingPointB;
    public float walkSpeed = 1.5f;

    [Header("Fade")]
    public CanvasGroup fadeGroup;
    public float fadeDuration = 2.5f;

    [Header("End Screen")]
    public GameObject endPanel;

    [Header("HUD")]
    public GameObject hudCanvas;

    Transform walkTarget;
    bool walking;

    void Start()
    {
        panel.SetActive(false);

        if (endPanel != null)
            endPanel.SetActive(false);

        if (fadeGroup != null)
            fadeGroup.alpha = 0f;
    }

    void Update()
    {
        // Walking means cutscene is ongoing and needs the player to be moved somewhere
        if (!walking)
            return;

        // Move to next point in the cutscene
        player.position = Vector3.MoveTowards(
            player.position,
            walkTarget.position,
            walkSpeed * Time.deltaTime
        );

        // Vaguely rotate to face a realistic direction
        Vector3 dir = (walkTarget.position - player.position).normalized;

        if (dir != Vector3.zero)
        {
            player.rotation = Quaternion.Slerp(
                player.rotation,
                Quaternion.LookRotation(dir),
                5f * Time.deltaTime
            );
        }

        // When close to endpoint start the fading
        if (Vector3.Distance(player.position, walkTarget.position) < 3f)
        {
            StartCoroutine(FadeSequence());
        }
    }

    public void ShowChoice()
    {
        panel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;
    }

    // Depending on which choice is shown
    public void ChooseEndingA()
    {
        StartEnding(endingPointA);
    }

    public void ChooseEndingB()
    {
        StartEnding(endingPointB);
    }

    void StartEnding(Transform target)
    {
        panel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Time.timeScale = 1f;

        playerMovement.enabled = false;

        if (hudCanvas != null)
            hudCanvas.SetActive(false);

        walkTarget = target;
        walking = true;
    }

    // Fade the screen to black at the end of the game
    IEnumerator FadeSequence()
    {
        Debug.Log("fading");

        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }

        fadeGroup.alpha = 1f;

        yield return new WaitForSeconds(3f);

        ShowEndScreen();
    }

    void ShowEndScreen()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (endPanel != null)
            endPanel.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}