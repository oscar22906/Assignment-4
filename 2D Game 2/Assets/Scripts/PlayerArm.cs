using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArm : MonoBehaviour
{
    public bool circleMove = true;
    public float moveRadius = 1f;
    public float baseMoveSpeed = 180f; // Degrees per second
    public float startAngle = 0f;
    public bool clockwise = true;

    // Jitter parameters
    public bool enableJitter = false;
    public float jitterAmount = 0.01f;

    public float damageMax = 10f;
    public float damageMin = 30f;

    // Parameters for health stage control
    private PlayerHealth playerHealth; // Reference to the EnemyController script
    public EnemyHealth enemyHealthScript;
    private Vector3 originalLocalPosition;
    private float currentAngle;

    // Start is called before the first frame update
    private void Start()
    {
        originalLocalPosition = transform.localPosition;
        currentAngle = startAngle;

        playerHealth = transform.parent.GetComponent<PlayerHealth>();


        StartCoroutine(MoveCoroutine());
    }

    public void AttackEnemy()
    {
        Debug.Log("AttackEnemy Called");
        enemyHealthScript.TakeDamage(Random.Range(damageMin, damageMax));
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
                x += Random.Range(-jitterAmount * playerHealth.healthStage, jitterAmount * playerHealth.healthStage);
                y += Random.Range(-jitterAmount * playerHealth.healthStage, jitterAmount * playerHealth.healthStage);
            }

            // Set the arm's position relative to the enemy
            transform.localPosition = originalLocalPosition + new Vector3(x, y, 0f);
        }
    }

    // Function to get the adjusted speed based on health stage and boolean
    private float GetAdjustedSpeed()
    {
        float adjustedSpeed = baseMoveSpeed;

        if (playerHealth != null)
        {
            // Use the health stage from the EnemyController script
            int healthStage = playerHealth.healthStage;

            // Increase or decrease speed based on health stage
            float percentageIncrease = (healthStage - 1) * playerHealth.changePercentage;

            if (playerHealth.increaseMovementWithStage)
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

