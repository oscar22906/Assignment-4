using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Move Settings")]
    public bool doMove;
    public float minMoveDelay = 1f;
    public float maxMoveDelay = 3f;
    public float minMoveDistance = 0.5f;
    public float maxMoveDistance = 1f;
    public float lerpFactor = 0.1f;
    public int movesBeforeFixedPosition = 5;
    
    [Header("Position Resets")]
    public Vector3 leftSidePosition = new Vector3(-2f, 0f, 0f);
    public Vector3 centerPosition = new Vector3(0f, 1f, 0f);
    public Vector3 rightSidePosition = new Vector3(2f, 0f, 0f);

    [Header("Attack")]
    public bool doAttack;
    public float minAttackDamage = 5f;
    public float maxAttackDamage = 10f;
    public float minAttackDelay = 5f;
    public float maxAttackDelay = 10f;
    public int maxAttack = 1;

    [Header("Effect")]
    public bool simpleDamageEffect = true; // Whether the damage is sprite based or animated
    private EnemyEffects enemyEffect;


    [Header("Health Stage")]
    public bool increaseMovementWithStage = true; // Increase or decrease movement speed, distance, and delay with health stage
    public int healthStage = 1; // 1, 2, or 3
    public float changePercentage = 0.7f; // Percentage change as the health stage increases

    private Vector3 targetPosition;
    private bool isMoving = false;
    private int moveCounter = 0;

    private int attackCounter = 0;

    private RightArm rightArm;
    private EnemyHealth enemyHealth;
    private PlayerHealth playerHealth;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
        }
        enemyEffect = GetComponent<EnemyEffects>();
        rightArm = GetComponentInChildren<RightArm>();
        enemyHealth = GetComponent<EnemyHealth>();

        StartCoroutine(MoveCoroutine());
        if (doAttack)
        {
            StartCoroutine(AttackWithDelay());
        }

        attackCounter = 0;
    }
    
    void Update()
    {

    }

    public void DamageUpdate()
    {
        float healthPercentage = (float)enemyHealth.currentHealth / enemyHealth.maxHealth;
        
        if (!simpleDamageEffect)
        {
            enemyEffect.DamageUpdate(healthPercentage);
        }



        if (healthPercentage >= 1f)
        {
            healthStage = 1;
        }
        else if (healthPercentage >= 2f / 3f)
        {
            healthStage = 2;
        }
        else
        {
            healthStage = 3;
        }
    }

    public void DealDamage()
    {
        float damage = Random.Range(minAttackDamage, maxAttackDamage);
        playerHealth.TakeDamage(damage);
        Debug.Log("enemy dealt damage: " + damage);
    }





    void Attack()
    {
        StartCoroutine(AttackWithDelay());
    }

    IEnumerator AttackWithDelay()
    {
        float attackDelay = Random.Range(minAttackDelay, maxAttackDelay);
        yield return new WaitForSeconds(attackDelay);
        Debug.Log("Enemy attacked! With delay of " + attackDelay);

        // Call the punch animation function from the rightArm script
        rightArm.PerformPunchAnimation(maxAttack);

        // No delay for chained attacks
        for (int i = 1; i <= maxAttack; i++)
        {
            attackCounter++;
            Debug.Log("Attacked " + attackCounter + " times.");
        }

        // Reset the counter after the chained attacks
        attackCounter = 0;
        Attack();
    }


    IEnumerator MoveCoroutine()
    {
        while (doMove)
        {
            float currentMoveDelay = GetAdjustedMoveDelay();
            yield return new WaitForSeconds(currentMoveDelay);

            if (moveCounter >= movesBeforeFixedPosition)
            {
                // Move to a fixed position after a certain number of moves
                moveCounter = 0;
                SetRandomFixedPosition();
            }
            else
            {
                // Move to a random position within the specified range
                moveCounter++;
                targetPosition = transform.position + new Vector3(Random.Range(-GetAdjustedMoveDistance(), GetAdjustedMoveDistance()), Random.Range(-GetAdjustedMoveDistance(), GetAdjustedMoveDistance()), 0f);
                StartCoroutine(MoveTowardsTarget());
            }
        }
    }

    void SetRandomFixedPosition()
    {
        int randomIndex = Random.Range(0, 3);

        switch (randomIndex)
        {
            case 0:
                targetPosition = leftSidePosition;
                break;
            case 1:
                targetPosition = centerPosition;
                break;
            case 2:
                targetPosition = rightSidePosition;
                break;
        }

        StartCoroutine(MoveTowardsTarget());
    }

    IEnumerator MoveTowardsTarget()
    {
        if (!isMoving)
        {
            isMoving = true;

            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, lerpFactor);
                yield return null;
            }

            isMoving = false;
        }
    }

    // Function to get the adjusted move delay based on health stage and boolean
    private float GetAdjustedMoveDelay()
    {
        float adjustedMoveDelay = Random.Range(minMoveDelay, maxMoveDelay);

        if (increaseMovementWithStage)
        {
            // Decrease move delay by a percentage based on health stage
            float percentageDecrease = (healthStage - 1) * changePercentage; // Adjust the multiplier as needed
            adjustedMoveDelay *= 1 - percentageDecrease;
        }
        else
        {
            // Increase move delay by a percentage based on health stage
            float percentageIncrease = (3 - healthStage) * changePercentage; // Adjust the multiplier as needed
            adjustedMoveDelay *= 1 + percentageIncrease;
        }

        return adjustedMoveDelay;
    }

    // Function to get the adjusted move distance based on health stage and boolean
    private float GetAdjustedMoveDistance()
    {
        float adjustedMoveDistance = Random.Range(minMoveDistance, maxMoveDistance);

        if (increaseMovementWithStage)
        {
            // Increase move distance by a percentage based on health stage
            float percentageIncrease = (healthStage - 1) * changePercentage; // Adjust the multiplier as needed
            adjustedMoveDistance *= 1 + percentageIncrease;
        }
        else
        {
            // Decrease move distance by a percentage based on health stage
            float percentageDecrease = (3 - healthStage) * changePercentage; // Adjust the multiplier as needed
            adjustedMoveDistance *= 1 - percentageDecrease;
        }

        return adjustedMoveDistance;
    }
}
