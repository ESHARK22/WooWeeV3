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
    [SerializeField] bool isLeft;
    private float horizMove;
    private float vertMove;
    
    public AudioClip dialogueSound;

    public Animator animator; 
    public Transform Transform;

    private DialogueAreaTrigger currentDialogueArea;
    

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
        animator.SetInteger("SpeedX", (int)speedX);
        animator.SetInteger("SpeedY", (int)speedY);
        Vector2 localScale = gameObject.transform.localScale;
        if (speedX < 0f)
        {
            isLeft = true;
        }
        else if (speedX > 0f)
        {
            isLeft = false;
        }
        if (isLeft)
        {
            localScale.x = -1f;
        }
        else
        {
            localScale.x = 1f;
        }
        Transform.localScale = localScale;
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        Debug.Log("INTERACT EVENT FIRED!");

        if (!context.performed)
            return;
        AudioSource.PlayClipAtPoint(dialogueSound, Transform.position);
        Debug.Log("Interact PERFORMED");

        if (currentDialogueArea != null)
        {
            Debug.Log("STARTING DIALOGUE");
            currentDialogueArea.StartDialogue();
        }
    }

    public void SetDialogueArea(DialogueAreaTrigger area)
    {
        currentDialogueArea = area;
    }

    public void ClearDialogueArea(DialogueAreaTrigger area)
    {
        if (currentDialogueArea == area)
        {
            currentDialogueArea = null;
        }
    }




}