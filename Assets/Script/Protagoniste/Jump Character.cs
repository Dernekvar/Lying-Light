using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class JumpCharacter : MonoBehaviour
{
    public float jumpHeight = 5f;
    private Rigidbody2D rb;
    private bool isGrounded;
    public LayerMask groundLayer;
    public InputActionAsset _asset;
    private InputAction jumpAction;
    public InputActionAsset inputActions;

    [Header("Floating Fall Settings")]
    public float normalGravity = 1f;
    public float floatGravity = 0.3f;
    public float fallSpeedLimit = 3f;

    void OnEnable()
    {
        if (inputActions == null)
        {
            Debug.LogError(" inputActions est NULL ! Vérifie son assignation dans l'Inspector.");
            return;
        }

        jumpAction = inputActions.FindAction("Player/Jump", true);

        Debug.Log(" Vérification Jump : " +
                  "\nJump Action: " + (jumpAction != null ? "OK" : "Manquant"));

        jumpAction.started += HandleJump;
        jumpAction.Enable();
    }

    void OnDisable()
    {
        jumpAction.started -= HandleJump;
        _asset.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Appliquer effet de flottement pendant la chute
        if (rb.velocity.y < 0)
        {
            rb.gravityScale = floatGravity;

            // Limite la vitesse de chute
            if (rb.velocity.y < -fallSpeedLimit)
            {
                rb.velocity = new Vector2(rb.velocity.x, -fallSpeedLimit);
            }
        }
        else
        {
            rb.gravityScale = normalGravity;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            isGrounded = false;
        }
    }

    void HandleJump(InputAction.CallbackContext ctx)
    {
        if (isGrounded && ctx.ReadValue<float>() > 0)
        {
            Jump();
        }
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpHeight * 1.5f); // Augmenter juste l'impulsion verticale
    }
}
