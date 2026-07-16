using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float velocidad = 3f;
    private Rigidbody2D rb;
    private Vector2 movimiento;
    private Animator animator;
    private string estadoAnimActual = "";

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

void Update()
{
    // Si algun panel de dialogo, dialogo simple o quiz esta abierto, no se mueve y libera el teclado
    bool interaccionAbierta =
        (DialogueUI.instancia != null && DialogueUI.instancia.panelDialogo.activeSelf) ||
        (DialogueSimpleUI.instancia != null && DialogueSimpleUI.instancia.panelSimple.activeSelf) ||
        (QuizManager.instancia != null && QuizManager.instancia.panelQuiz.activeSelf);

    if (interaccionAbierta)
    {
        movimiento = Vector2.zero;
        rb.linearVelocity = Vector2.zero;
        CambiarAnimacion("Alex_Idle");
        return;
    }

    movimiento.x = Input.GetAxisRaw("Horizontal");
    movimiento.y = Input.GetAxisRaw("Vertical");

    if (movimiento.x > 0)
        CambiarAnimacion("Alex_Walk_Right");
    else if (movimiento.x < 0)
        CambiarAnimacion("Alex_Walk_Left");
    else if (movimiento.y > 0)
        CambiarAnimacion("Alex_Walk_Up");
    else if (movimiento.y < 0)
        CambiarAnimacion("Walk_Down");
    else
        CambiarAnimacion("Alex_Idle");
}

    void CambiarAnimacion(string estado)
    {
        // Solo reinicia el Animator si el estado realmente cambio, evita el parpadeo de la animacion
        if (estadoAnimActual == estado) return;
        estadoAnimActual = estado;
        animator.Play(estado);
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movimiento.normalized * velocidad * Time.fixedDeltaTime);
    }
}