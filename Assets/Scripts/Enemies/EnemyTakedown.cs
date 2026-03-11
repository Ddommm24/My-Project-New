using UnityEngine;

public class EnemyTakedown : MonoBehaviour, IInteractable, ILoopResettable
{
    Collider exposedCol;

    enum TakedownState
    {
        CanRemoveBack,
        WaitingToDisable,
        CanDisable,
        Disabled
    }

    TakedownState CurrentState
    {
        get
        {
            if (disabled)
                return TakedownState.Disabled;

            if (!backRemoved)
                return TakedownState.CanRemoveBack;

            if (Time.time < disableUnlockTime)
                return TakedownState.WaitingToDisable;

            return TakedownState.CanDisable;
        }
    }

    public int Priority => CurrentState == TakedownState.Disabled ? -1 : 100;

    [Header("Back Panel")]
    public GameObject backPanel;
    public GameObject exposedBackPanel;

    [Header("Interaction")]
    public float interactRange = 1.2f;
    public float behindDotThreshold = 0.6f;

    [Header("ID")]
    public int idDigit;

    bool backRemoved;
    bool disabled;
    float disableUnlockTime;

    public bool IsDisabled => disabled;

    Transform player;
    EnemyAI enemyAI;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
            player = p.transform;

        enemyAI = GetComponentInParent<EnemyAI>();
        exposedBackPanel.SetActive(false);

        Inspect inspect = exposedBackPanel.GetComponent<Inspect>();
        if (inspect != null)
        {
            inspect.requiresUnlock = true;
            inspect.unlocked = false;
            inspect.enabled = false;
        }

        exposedCol = exposedBackPanel.GetComponent<Collider>();
        if (exposedCol != null)
            exposedCol.enabled = false;
    }

    public bool CanInteract()
    {
        if (CurrentState == TakedownState.Disabled || CurrentState == TakedownState.WaitingToDisable)
            return false;

        if (player == null)
            return false;

        if (PlayerInventory.Instance == null || !PlayerInventory.Instance.HasScrewdriver)
            return false;

        Collider col = backRemoved
            ? exposedCol
            : backPanel.GetComponent<Collider>();

        if (col == null || !col.enabled)
            return false;

        float dist = Vector3.Distance(player.position, col.ClosestPoint(player.position));
        if (dist > interactRange)
            return false;

        Vector3 toPlayer = (player.position - enemyAI.transform.position).normalized;
        float dot = Vector3.Dot(enemyAI.transform.forward, toPlayer);

        if (dot > -behindDotThreshold)
            return false;

        return true;
    }

    public string GetPromptText()
    {
        return CurrentState switch
        {
            TakedownState.CanRemoveBack => "Press E to Remove Back Panel",
            TakedownState.WaitingToDisable => "Disable available in 1 second",
            TakedownState.CanDisable => "Press E to Disable Enemy",
            _ => ""
        };
    }

    public void Interact()
    {
        switch (CurrentState)
        {
            case TakedownState.CanRemoveBack:
                RemoveBack();
                break;

            case TakedownState.CanDisable:
                DisableEnemy();
                break;
        }
    }

    void RemoveBack()
    {
        backRemoved = true;
        disableUnlockTime = Time.time + 1f;

        Collider backCol = backPanel.GetComponent<Collider>();
        if (backCol != null)
            backCol.enabled = false;

        backPanel.SetActive(false);
        exposedBackPanel.SetActive(true);

        if (exposedCol != null)
            exposedCol.enabled = true;

        enemyAI.ForceChase(player.position);
        PlayerMovement.Instance.Stun(1f);
    }

    void DisableEnemy()
    {
        disabled = true;

        Collider myCol = GetComponent<Collider>();
        if (myCol != null)
            myCol.enabled = false;

        EnemyAttack attack = GetComponentInParent<EnemyAttack>();
        if (attack != null)
        {
            attack.disabled = true;
            attack.enabled = false;
        }

        enemyAI.PauseChase(true);
        enemyAI.DisableAI();

        Inspect inspect = exposedBackPanel.GetComponent<Inspect>();
        if (inspect != null)
        {
            inspect.unlocked = true;
            inspect.enabled = true;
        }
    }

    public void ResetState()
    {
        backRemoved = false;
        disabled = false;
        disableUnlockTime = 0f;
        enabled = true;

        if (backPanel != null)
        {
            backPanel.SetActive(true);

            Collider backCol = backPanel.GetComponent<Collider>();
            if (backCol != null)
                backCol.enabled = true;
        }

        if (exposedBackPanel != null)
            exposedBackPanel.SetActive(false);

        if (exposedCol != null)
            exposedCol.enabled = false;

        Collider myCol = GetComponent<Collider>();
        if (myCol != null)
            myCol.enabled = true;

        Inspect inspect = exposedBackPanel.GetComponent<Inspect>();
        if (inspect != null)
        {
            inspect.requiresUnlock = true;
            inspect.unlocked = false;
            inspect.enabled = false;
        }

        EnemyAttack attack = GetComponentInParent<EnemyAttack>();
        if (attack != null)
        {
            attack.disabled = false;
            attack.enabled = true;
        }

        if (enemyAI != null)
        {
            enemyAI.enabled = true;
            enemyAI.PauseChase(false);
        }
    }
}