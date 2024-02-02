using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb; // CHANGE --- No need for public, we're already getting the reference on line 24.

    public float moveSpeed; //Player Movement Speed
    Vector2 movement;

    bool isRight = false; //Player Sprite Flip
    public Transform weapon;
    public GameObject lookPoint;

[SerializeField] float startDashTime = 0.3f; // CHANGE --- Better starting number.
    [SerializeField] float dashSpeed = 15f; // CHANGE --- Better starting number.

    float currentDashTime;

    bool canDash = true;
    bool canMove = true; // CHANGE --- Need to disable movement when dashing.

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
    {
        //Plyaer Movement Input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (canDash && Input.GetKeyDown(KeyCode.Space))
        {
            if (Input.GetKeyDown(KeyCode.W) && Input.GetKeyDown(KeyCode.D))
            {
                StartCoroutine(Dash(new Vector2(1f, 1f)));
            }

            else if (Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.D))
            {
                StartCoroutine(Dash(new Vector2(1f, -1f)));
            }

            else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.W))
            {
                StartCoroutine(Dash(new Vector2(-1f, 1f)));
            }

            else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.S))
            {
                StartCoroutine(Dash(new Vector2(-1f, -1f)));
            }

            else if (Input.GetKey(KeyCode.W))
            {
                StartCoroutine(Dash(Vector2.up));
            }

            else if (Input.GetKey(KeyCode.A))
            {
                StartCoroutine(Dash(Vector2.left));
            }

            else if (Input.GetKey(KeyCode.S))
            {
                StartCoroutine(Dash(Vector2.down));
            }

            else if (Input.GetKey(KeyCode.D))
            {
                StartCoroutine(Dash(Vector2.right));
            }
        }
    }

    void FixedUpdate()
    {
        if (canMove == true) // CHANGE --- Need to disable movement when dashing.
        {
            movement.Normalize();
            rb.velocity = new Vector2(movement.x * moveSpeed, movement.y * moveSpeed); // CHANGE --- No need to multiply by Time.deltaTime / Physics are already frame rate independent.
        }
    }

    void Flip() //Player Sprite Flip Function
    {
        isRight = !isRight;
        this.transform.Rotate(0f, 180f, 0f);
        weapon.transform.Rotate(0f, 180f, 0);
    }

    IEnumerator Dash(Vector2 direction)
    {
        canDash = false;
        canMove = false; // CHANGE --- Need to disable movement when dashing.
        currentDashTime = startDashTime; // Reset the dash timer.

        Vector3 pos = lookPoint.transform.position;
        Vector3 dir = (lookPoint.transform.position - this.transform.position).normalized;
        Debug.DrawLine(pos, pos + dir * 10, Color.red, Mathf.Infinity);

        while (currentDashTime > 0f)
        {
            currentDashTime -= Time.deltaTime; // Lower the dash timer each frame.

            rb.velocity = dir * 10 * dashSpeed; // Dash in the direction that was held down.
                                                 // No need to multiply by Time.DeltaTime here, physics are already consistent across different FPS.

            yield return null; // Returns out of the coroutine this frame so we don't hit an infinite loop.
        }

        rb.velocity = new Vector2(0f, 0f); // Stop dashing.

        canDash = true;
        canMove = true; // CHANGE --- Need to enable movement after dashing.
    }
}