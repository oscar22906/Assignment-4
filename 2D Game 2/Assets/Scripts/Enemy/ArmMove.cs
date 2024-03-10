using System.Collections;
using UnityEngine;

public class ArmMove : MonoBehaviour
{
    public bool circleMove = true;
    public float moveRadius = 1f;
    public float baseMoveSpeed = 180f; // Degrees per second
    public float startAngle = 0f;
    public bool clockwise = true;

    // Jitter parameters
    public bool enableJitter = false;
    public float jitterAmount = 0.01f;

    // Parameters for health stage control
    private EnemyController enemyController; // Reference to the EnemyController script

    private Vector3 originalLocalPosition;
    private float currentAngle;

    // Start is called before the first frame update
    private void Start()
    {
        originalLocalPosition = transform.localPosition;
        currentAngle = startAngle;

        // Get the reference to the EnemyController script
        enemyController = GetComponentInParent<EnemyController>();


        StartCoroutine(MoveCoroutine());
    }

    


    private IEnumerator MoveCoroutine()
    {
        while (true)
        {
            yield return null; // Wait for the next frame

            // Calculate the next position within the specified radius
            float deltaAngle = GetAdjustedSpeed() * Time.deltaTime;
            currentAngle += (clockwise ? 1 : -1) * deltaAngle;

            // Keep the angle within a full circle
            if (currentAngle > 360f)
            {
                currentAngle -= 360f;
            }
            else if (currentAngle < 0f)
            {
                currentAngle += 360f;
            }

            float x = 0.0f;
            float y = 0.0f;

            if (circleMove == true)
            {
                // Calculate the new position based on the current angle
                x = Mathf.Cos(Mathf.Deg2Rad * currentAngle) * moveRadius;
                y = Mathf.Sin(Mathf.Deg2Rad * currentAngle) * moveRadius;
            }

            // Apply jitter if enabled
            if (enableJitter)
            {
                x += Random.Range(-jitterAmount * enemyController.healthStage, jitterAmount * enemyController.healthStage);
                y += Random.Range(-jitterAmount * enemyController.healthStage, jitterAmount * enemyController.healthStage);
            }

            // Set the arm's position relative to the enemy
            transform.localPosition = originalLocalPosition + new Vector3(x, y, 0f);
        }
    }

    // Function to get the adjusted speed based on health stage and boolean
    private float GetAdjustedSpeed()
    {
        float adjustedSpeed = baseMoveSpeed;

        if (enemyController != null)
        {
            // Use the health stage from the EnemyController script
            int healthStage = enemyController.healthStage;

            // Increase or decrease speed based on health stage
            float percentageIncrease = (healthStage - 1) * enemyController.changePercentage;

            if (enemyController.increaseMovementWithStage)
            {
                // Increase speed by a percentage based on health stage
                adjustedSpeed *= 1 + percentageIncrease;
            }
            else
            {
                // Decrease speed by a percentage based on health stage
                adjustedSpeed *= 1 - percentageIncrease;
            }
        }

        return adjustedSpeed;
    }
}
