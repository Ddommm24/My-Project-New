using UnityEngine;

public class ShiftChangeController : MonoBehaviour, ILoopResettable
{
    public TimedShiftDoor door;
    public EnemyShiftRoom shiftRoom;

    bool shiftTriggered;

    void Update()
    {
        if (shiftTriggered) return;

        if (TimeLoopManager.Instance.GetElapsedTime() >= TimeLoopManager.SHIFT_CHANGE_TIME)
        {
            shiftTriggered = true;

            Debug.Log("SHIFT CHANGE TRIGGERED");

            if (door != null)
                door.OpenForDuration(10f);

            if (shiftRoom != null)
                shiftRoom.DoShiftSwap();
        }
    }

    public void ResetState()
    {
        shiftTriggered = false;
    }
}
