using UnityEngine;
using TMPro;

public class NPCEvaluador : MonoBehaviour
{
    [Header("Configuracion")]
    public float distanciaInteraccion = 3f;
    public string nombreNPC = "Prof. Evaluador";
    public int capituloQuiz = 1;

    [Header("Senaletica")]
    public GameObject senaletica;

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
                AbrirEvaluacion();
        }
        else
        {
            if (senaletica != null)
                senaletica.SetActive(false);
        }
    }

    void AbrirEvaluacion()
    {
        dialogoAbierto = true;
        if (senaletica != null)
            senaletica.SetActive(false);

        GameManager.instancia.AgregarPuntos(10);
        QuizManager.instancia.IniciarQuiz(capituloQuiz, this);
    }

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