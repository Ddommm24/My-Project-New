using UnityEngine;

public class FacilityClock : MonoBehaviour, ILoopResettable
{
    public AudioSource chimeSource;

    bool chimedThisLoop;

    void Update()
    {
        if (chimedThisLoop) return;

        if (TimeLoopManager.Instance.GetElapsedTime() >= TimeLoopManager.SHIFT_CHANGE_TIME)
        {
            if (TimeLoopManager.Instance.GetElapsedTime() > 5)
            {
                Debug.Log("chime chimed");
                chimedThisLoop = true;

                if (chimeSource != null && !chimeSource.isPlaying)
                    chimeSource.Play();
            }
        }
        
    }

    public string GetClockText()
    {
        float elapsed = TimeLoopManager.Instance.GetElapsedTime();
        float displayMins = 0;
        float displaySecs = 0;
        float displayHours;
        if (elapsed < 76)
        {
            displayHours = 11;
            if (elapsed < 16)
            {
                displayMins = 58;
                displaySecs = 44 + elapsed;
            }
            else
            {
                displayMins = 59;
                displaySecs = -16 + elapsed;
            }
        } 
        else
        {
            displayHours = 00;
            if (elapsed < 136)
            {
                displayMins = 00;
                displaySecs = -76 + elapsed;
            }
            if (elapsed < 196)
            {
                displayMins = 01;
                displaySecs = -136 + elapsed;
            }
            if (elapsed < 256)
            {
                displayMins = 02;
                displaySecs = -196 + elapsed;
            }
            if (elapsed < 316)
            {
                displayMins = 03;
                displaySecs = -256 + elapsed;
            }
        }

        int hours = Mathf.FloorToInt(displayHours);
        int mins = Mathf.FloorToInt(displayMins);
        int secs = Mathf.RoundToInt(displaySecs);

        if (secs >= 60)
        {
            secs = 0;
            mins++;

            if (mins >= 60)
            {
                mins = 0;
                hours = (hours + 1) % 24;
            }
        }

        return $"Facility Time:\n{hours:00}:{mins:00}:{secs:00}";
    }

    public void ResetState()
    {
        chimedThisLoop = false;
    }
}