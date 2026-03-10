using UnityEngine;
using System.Collections;

public class OpenDrawer : MonoBehaviour, IInteractable, ILoopResettable
{
    public Vector3 openOffset = new Vector3(0, 0, 0.35f);
    public float moveSpeed = 4f;

    private Vector3 closedPos;
    private Vector3 openPos;
    private bool isOpen;
    private bool isMoving;

    public int Priority => 10;

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
        return isOpen ? "Press E to Close" : "Press E to Open";
    }

    public void Interact()
    {
        if (isMoving) return;

        isOpen = !isOpen;
        StartCoroutine(MoveDrawer());
    }

    IEnumerator MoveDrawer()
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
