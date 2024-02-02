using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    /* Hello welcome to my enemy code thing. This will control the enemys movement as well as communicate to the arms when their is an attack or how they should increase speed ect.  */
    public float minMoveDelay = 1f;
    public float maxMoveDelay = 3f;
    public float minMoveDistance = 0.5f;
    public float maxMoveDistance = 1f;
    public float lerpFactor = 0.1f;
    public int movesBeforeFixedPosition = 5;

    public Vector3 leftSidePosition = new Vector3(-2f, 0f, 0f);
    public Vector3 centerPosition = new Vector3(0f, 1f, 0f);
    public Vector3 rightSidePosition = new Vector3(2f, 0f, 0f);


    public float minAttackDelay = 5f;
    public float maxAttackDelay = 10f;

    // Health stage parameters
    public bool increaseMovementWithStage = true; // Increase or decrease movement speed, distance, and delay with health stage
    public int healthStage = 1; // 1, 2, or 3
    public float changePercentage = 0.7f; // Percentage change as the health stage increases

    public int maxAttack = 1;

    private Vector3 targetPosition;
    private bool isMoving = false;
    private int moveCounter = 0;

    private int attackCounter = 0;

    private RightArm rightArm;

    void Start()
    {
        rightArm = GetComponentInChildren<RightArm>();
        StartCoroutine(MoveCoroutine());
        StartCoroutine(AttackWithDelay());

        attackCounter = 0;
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
        while (true)
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
