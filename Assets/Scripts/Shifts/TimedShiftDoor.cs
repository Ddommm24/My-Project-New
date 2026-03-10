using UnityEngine;
using System.Collections;

public class TimedShiftDoor : MonoBehaviour
{
    public Transform door;
    public Vector3 openOffset = new Vector3(0, 10f, 0);
    public float openSpeed = 3f;

    Vector3 closedPos;
    Vector3 openPos;
    bool isOpen;
    Coroutine autoCloseRoutine;

    void Start()
    {
        closedPos = door.position;
        openPos = closedPos + openOffset;
    }

    void Update()
    {

        Vector3 pos = door.position;

        float targetY = isOpen
            ? closedPos.y + openOffset.y
            : closedPos.y;

        pos.y = Mathf.MoveTowards(
            pos.y,
            targetY,
            openSpeed * Time.deltaTime
        );

        door.position = pos;
    }

    public void OpenForDuration(float seconds)
    {
        if (autoCloseRoutine != null)
            StopCoroutine(autoCloseRoutine);

        isOpen = true;
        autoCloseRoutine = StartCoroutine(AutoClose(seconds));
    }

    IEnumerator AutoClose(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Debug.Log("closing");

        isOpen = false;
    }

    public void ResetState()
    {
        isOpen = false;
        door.position = closedPos;
    }
}
