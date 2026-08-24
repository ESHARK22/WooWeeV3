using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    [Header("Player Component References")]
    [SerializeField] Rigidbody2D rb;

    [Header("Player Settings")]
    [SerializeField] float walkSpeed;
    
    [Header("Active Value")]
    [SerializeField] float speedX;
    [SerializeField] float speedY;
    private float horizMove;
    private float vertMove;
    

    void FixedUpdate()
    {
        Walking();
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizMove = context.ReadValue<Vector2>().x;
        vertMove = context.ReadValue<Vector2>().y;
    }

    private void Walking()
    {
        speedX = walkSpeed * horizMove;
        speedY = walkSpeed * vertMove;
        rb.linearVelocity = new Vector2(speedX, speedY);
    }




}