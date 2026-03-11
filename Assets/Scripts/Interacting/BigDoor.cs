using UnityEngine;

public class BigDoor : MonoBehaviour
{
    bool isOpen;

    // Final door with the 4 digit code
    public void TryOpen(int[] input)
    {
        if (isOpen) return;

        if (DoorCodeManager.Instance.CheckCode(input))
        {
            isOpen = true;
            transform.position += Vector3.up * 5f;
            Debug.Log("BIG DOOR OPENED");
        }
        else
        {
            Debug.Log("Incorrect code");
        }
    }
}
