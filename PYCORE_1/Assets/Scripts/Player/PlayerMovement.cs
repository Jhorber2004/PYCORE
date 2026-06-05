using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float velocidad = 3f;
    private Rigidbody2D rb;
    private Vector2 movimiento;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        movimiento.x = Input.GetAxisRaw("Horizontal");
        movimiento.y = Input.GetAxisRaw("Vertical");

        if (movimiento.x > 0)
            animator.Play("Alex_Walk_Right");
        else if (movimiento.x < 0)
            animator.Play("Alex_Walk_Left");
        else if (movimiento.y > 0)
            animator.Play("Alex_Walk_Up");
        else if (movimiento.y < 0)
            animator.Play("Walk_Down");
        else
            animator.Play("Alex_Idle");
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movimiento.normalized * velocidad * Time.fixedDeltaTime);
    }
}