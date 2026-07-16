using UnityEngine;
using TMPro;

public class NPCEvaluador : MonoBehaviour, IDialogable
{
    [Header("Configuracion")]
    public float distanciaInteraccion = 3f;
    public string nombreNPC = "Prof. Evaluador";
    public int capituloQuiz = 1;

    [Header("Imagen")]
    public Sprite imagenPersonaje;

    [Header("Senaletica")]
    public GameObject senaletica;

    [Tooltip("Mensaje si el jugador intenta rendir el examen fuera de orden.")]
    [TextArea(2, 3)]
    public string mensajeBloqueado = "Todavia no puedes rendir este examen. Completa los pasos anteriores primero.";

    private Transform jugador;
    private bool dialogoAbierto = false;

    void Start()
    {
        jugador = GameObject.FindWithTag("Player").transform;
        if (senaletica != null)
            senaletica.SetActive(false);
    }

    void Update()
    {
        if (jugador == null) return;

        float distancia = Vector2.Distance(transform.position, jugador.position);

        if (distancia <= distanciaInteraccion && !dialogoAbierto)
        {
            if (senaletica != null)
                senaletica.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                // BUG CORREGIDO: antes se abria el quiz sin validar ningun orden.
                if (GameManager.instancia.PuedeRendirEvaluacion(capituloQuiz))
                    AbrirEvaluacion();
                else
                    MostrarBloqueado();
            }
        }
        else
        {
            if (senaletica != null)
                senaletica.SetActive(false);
        }
    }

    void MostrarBloqueado()
    {
        dialogoAbierto = true;
        if (senaletica != null)
            senaletica.SetActive(false);

        DialogueSimpleUI.instancia.MostrarDialogo(nombreNPC, mensajeBloqueado, this, imagenPersonaje, null);
    }

    void AbrirEvaluacion()
    {
        dialogoAbierto = true;
        if (senaletica != null)
            senaletica.SetActive(false);

        bool quizAbierto = QuizManager.instancia.IniciarQuiz(capituloQuiz, this);

        if (!quizAbierto)
        {
            dialogoAbierto = false;
            return;
        }

        GameManager.instancia.AgregarPuntos(10);
    }

    // Llamado por DialogueSimpleUI cuando se cierra el mensaje de bloqueo
    public void CerrarDialogo()
    {
        dialogoAbierto = false;
    }

    // Llamado por QuizManager.CerrarQuiz() cuando se cierra el examen
    public void CerrarEvaluacion()
    {
        dialogoAbierto = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaInteraccion);
    }
}