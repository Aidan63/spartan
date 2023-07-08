using PowerTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BipedAnimator : MonoBehaviour
{
    private const int TORSO_NODE_ID = 0;

    public BipedMotor motor;

    public Animator animator;

    public Camera gameCamera;

    public SpriteAnimNodes nodes;

    public Transform torso;

    public Transform head;

    public Transform arm;

    public Color[] palette = new Color[8];

    private void Update()
    {
        // Set animator properties.

        var mousePos           = Mouse.current.position.ReadValue();
        var screenPos          = gameCamera.WorldToScreenPoint(transform.position);
        var walkAnimMultiplier = Mathf.Abs(motor.velocity.x) / Mathf.Abs(motor.maxVelocity.x) * 1f;
        var mouseToTheRight    = mousePos.x > screenPos.x;

        if (mouseToTheRight)
        {
            transform.localScale =
                new Vector3(
                    1,
                    transform.localScale.y,
                    transform.localScale.z);

            animator.SetFloat("hSpeed", motor.velocity.x < 0 ? -walkAnimMultiplier : walkAnimMultiplier);
        }
        else
        {
            transform.localScale =
                new Vector3(
                    -1,
                    transform.localScale.y,
                    transform.localScale.z);

            animator.SetFloat("hSpeed", motor.velocity.x > 0 ? -walkAnimMultiplier : walkAnimMultiplier);
        }

        animator.SetInteger("vSpeed", Mathf.FloorToInt(motor.velocity.y));
        animator.SetBool("movement", Mathf.FloorToInt(Mathf.Abs(motor.velocity.x)) != 0);

        if (motor.velocity.y < 0)
        {
            animator.SetTrigger("jump");
        }

        // Set torso offset and rotation
        // I'm not really sure why this angle wrapping stuff works, need to look into it and properly understand it
        // almost certainly related to angle wrap around values.

        var angle = Mathf.Atan2(mousePos.y - screenPos.y, mousePos.x - screenPos.x) * Mathf.Rad2Deg;

        float cappedAngle;
        if (mouseToTheRight)
        {
            cappedAngle = Mathf.Clamp(angle, -20, 20);
        }
        else
        {
            if (angle > 0)
            {
                angle -= 180;
                cappedAngle = Mathf.Clamp(angle, -20, 20);
            }
            else
            {
                angle = (angle + 180) % 360;
                cappedAngle = Mathf.Clamp(angle, -20, 20);
            }
        }

        torso.SetPositionAndRotation(nodes.GetPosition(TORSO_NODE_ID), Quaternion.Euler(0, 0, cappedAngle));

        // Set head rotation

        head.rotation = Quaternion.Euler(0, 0, cappedAngle * 2);

        arm.rotation = Quaternion.Euler(0, 0, angle);
    }
}
