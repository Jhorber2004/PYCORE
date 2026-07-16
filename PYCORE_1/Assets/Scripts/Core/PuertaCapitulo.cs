using UnityEngine;

public class PuertaCapitulo : MonoBehaviour
{
    [Header("Configuracion")]
    [Tooltip("Capitulo que debe estar completado para que esta puerta se abra (1, 2 o 3)")]
    public int capituloRequerido = 1;

    [Header("Referencias")]
    [Tooltip("Collider SOLIDO (Is Trigger DESACTIVADO) que bloquea el paso mientras la puerta esta cerrada")]
    public Collider2D colliderBloqueo;
    [Tooltip("Collider en modo Trigger (Is Trigger ACTIVADO), justo en el hueco de la puerta, para detectar cuando el jugador la cruza")]
    public Collider2D colliderCruce;
    public Animator animator;

    private bool abierta = false;
    private bool transicionDisparada = false;

    void Update()
    {
        if (abierta) return;
        if (GameManager.instancia == null) return;

        bool capituloListo =
            (capituloRequerido == 1 && GameManager.instancia.capitulo1Completado) ||
            (capituloRequerido == 2 && GameManager.instancia.capitulo2Completado) ||
            (capituloRequerido == 3 && GameManager.instancia.capitulo3Completado);

        if (capituloListo)
            AbrirPuerta();
    }

    void AbrirPuerta()
    {
        abierta = true;
        if (animator != null)
            animator.SetBool("abierta", true);
        if (colliderBloqueo != null)
            colliderBloqueo.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!abierta || transicionDisparada) return;
        if (!other.CompareTag("Player")) return;

        transicionDisparada = true;
        if (FadeManager.instancia != null)
            FadeManager.instancia.TransicionCapitulo();
    }
}