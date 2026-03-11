using UnityEngine;
using System.Collections;

public class RevealBed : MonoBehaviour, IInteractable, ILoopResettable
{
    public Vector3 openOffset = new Vector3(0, 0, 0.35f);
    public float moveSpeed = 4f;

    private Vector3 closedPos;
    private Vector3 openPos;
    private bool isOpen;
    private bool isMoving;

    public int Priority => 10;

    // Moves covers same way as opendoor works
    void Start()
    {
        closedPos = transform.localPosition;
        openPos = closedPos + openOffset;
    }

    public bool CanInteract()
    {
        return !isMoving;
    }

    public string GetPromptText()
    {
        return isOpen ? "Press E to Re-Cover" : "Press E to Reveal";
    }

    public void Interact()
    {
        if (isMoving) return;

        isOpen = !isOpen;
        StartCoroutine(MoveCovers());
    }

    IEnumerator MoveCovers()
    {
        isMoving = true;
        Vector3 target = isOpen ? openPos : closedPos;

        while (Vector3.Distance(transform.localPosition, target) > 0.001f)
        {
            transform.localPosition = Vector3.MoveTowards(
                transform.localPosition,
                target,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }

        transform.localPosition = target;
        isMoving = false;
    }

    public void ResetState()
    {
        StopAllCoroutines();
        isOpen = false;
        isMoving = false;
        transform.localPosition = closedPos;
    }
}
