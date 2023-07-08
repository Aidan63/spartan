using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BipedMotor : MonoBehaviour
{
    private const float DEADZONE = 0.25f;

    private const int PIXEL_PER_UNIT = 32;

    private const float PIXEL_UNIT = 1f / PIXEL_PER_UNIT;

    //

    private readonly Collider2D[] colliders = new Collider2D[1];

    private int layer;

    private float normalisedInput;

    private bool queuedJump;

    //

    public BoxCollider2D aabb;

    public Vector2 velocity;

    public Vector2 maxVelocity;

    public float acceleration;

    public float deceleration;

    public float jumpHeight;

    public float gravity;

    public void Input(float input)
    {
        normalisedInput = input;
    }

    public void Jump()
    {
        if (!PositionFree(aabb.transform.position - new Vector3(0, PIXEL_UNIT)))
        {
            queuedJump = true;
        }
    }

    private void Start()
    {
        Application.targetFrameRate = 60;

        layer           = LayerMask.GetMask("Ground");
        normalisedInput = 0;
        queuedJump      = false;
    }

    private void Update()
    {
        // Gravity

        if (PositionFree(aabb.transform.position - new Vector3(0, PIXEL_UNIT)))
        {
            velocity.y += gravity;
        }
        else
        {
            velocity.y = 0;
        }

        // Acceleration

        if (Mathf.Abs(normalisedInput) < DEADZONE)
        {
            if (velocity.x > 0)
            {
                velocity.x = Mathf.Max(velocity.x - deceleration, 0);
            }
            if (velocity.x < 0)
            {
                velocity.x = Mathf.Min(velocity.x + deceleration, 0);
            }
        }
        else
        {
            if (normalisedInput > 0)
            {
                velocity.x += acceleration * Mathf.Abs(normalisedInput);
            }
            else
            {
                velocity.x -= acceleration * Mathf.Abs(normalisedInput);
            }
        }

        // Jump

        if (queuedJump && !PositionFree(aabb.transform.position - new Vector3(0, PIXEL_UNIT)))
        {
            velocity.y = jumpHeight;

            queuedJump = false;
        }

        // Movement

        velocity.x = Mathf.Min(maxVelocity.x, Mathf.Max(velocity.x, -maxVelocity.x));
        velocity.y = Mathf.Min(maxVelocity.y, Mathf.Max(velocity.y, -maxVelocity.y));

        foreach (var _ in Enumerable.Range(0, Mathf.FloorToInt(Mathf.Abs(velocity.x))))
        {
            var increment = Mathf.Sign(velocity.x) * PIXEL_UNIT;

            if (PositionFree(aabb.transform.position + new Vector3(increment, 0)))
            {
                transform.position += new Vector3(increment, 0);

                if (!PositionFree(aabb.transform.position + new Vector3(increment, -PIXEL_UNIT * 2)) && PositionFree(aabb.transform.position + new Vector3(increment, -PIXEL_UNIT)))
                {
                    transform.position += new Vector3(0, -PIXEL_UNIT);
                }
            }
            else
            {
                if (PositionFree(aabb.transform.position + new Vector3(increment, PIXEL_UNIT)))
                {
                    transform.position += new Vector3(increment, PIXEL_UNIT);
                }
                else
                {
                    velocity.x = 0;

                    break;
                }
            }
        }

        foreach (var _ in Enumerable.Range(0, Mathf.FloorToInt(Mathf.Abs(velocity.y))))
        {
            var increment = Mathf.Sign(velocity.y) * PIXEL_UNIT;

            if (PositionFree(aabb.transform.position - new Vector3(0, increment)))
            {
                transform.position -= new Vector3(0, increment);
            }
            else
            {
                velocity.y = 0;

                break;
            }
        }
    }

    private bool PositionFree(Vector2 point)
    {
        return Physics2D.OverlapBoxNonAlloc(point, aabb.size, 0, colliders, layer) == 0;
    }
}
