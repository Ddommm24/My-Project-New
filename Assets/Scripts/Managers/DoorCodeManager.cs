using UnityEngine;

public class DoorCodeManager : MonoBehaviour
{
    public static DoorCodeManager Instance;

    // Four numbers, 1–100
    public int[] codeNumbers = new int[4];

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            GenerateCode();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void GenerateCode()
    {
        for (int i = 0; i < 4; i++)
        {
            codeNumbers[i] = Random.Range(1, 101);
        }

        Debug.Log(
            $"Door Code Generated: {codeNumbers[0]}, {codeNumbers[1]}, {codeNumbers[2]}, {codeNumbers[3]}"
        );
    }

    public int GetNumber(int index)
    {
        return codeNumbers[index];
    }

    public bool CheckCode(int[] input)
    {
        if (input.Length != 4) return false;

        for (int i = 0; i < 4; i++)
        {
            if (input[i] != codeNumbers[i])
                return false;
        }

        return true;
    }
}
