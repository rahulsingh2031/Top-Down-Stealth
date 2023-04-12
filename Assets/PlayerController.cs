using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float TurnSpeed = 8f;
    Rigidbody rb;
    float smoothMagnitude;
    float smoothMoveVelocity;
    float smoothMoveTime = 0.1f;

    float angle;

    Vector3 velocity;
    Vector3 inputDirection;
    bool disabled = false;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        Guard.OnGuardSpottedPlayer += Guard_OnGuardSpottedPlayer_DisableMovement;
    }
    void Guard_OnGuardSpottedPlayer_DisableMovement()
    {
        disabled = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (!disabled)
        {
            inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        }


        float inputMagnitude = inputDirection.magnitude;
        smoothMagnitude = Mathf.SmoothDamp(smoothMagnitude, inputMagnitude, ref smoothMoveVelocity, smoothMoveTime);

        float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
        angle = Mathf.LerpAngle(angle, targetAngle, TurnSpeed * inputMagnitude * Time.deltaTime);

        //this is correct too velocity = inputDirection * smoothMagnitude * moveSpeed;
        velocity = transform.forward * smoothMagnitude * moveSpeed;
    }
    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + velocity * Time.deltaTime);
        rb.MoveRotation(Quaternion.Euler(Vector3.up * angle));
    }

    private void OnDestroy()
    {
        Guard.OnGuardSpottedPlayer -= Guard_OnGuardSpottedPlayer_DisableMovement;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Finish")
        {
            disabled = true;
        }
    }
}
