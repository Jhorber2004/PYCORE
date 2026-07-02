using UnityEngine;
using TMPro;

public class NPCSimple : MonoBehaviour
{
    [Header("Configuracion")]
    public float distanciaInteraccion = 3f;
    public string nombreNPC = "Estudiante";
    public string npcId = "npc_simple_1";

    [Header("Dialogos")]
    [TextArea(2, 4)]
    public string dialogo1 = "Pista 1 aqui";
    [TextArea(2, 4)]
    public string dialogo2 = "Pista 2 aqui";
    [TextArea(2, 4)]
    public string dialogo3 = "Pista 3 aqui";

    [Header("Mision")]
    public string siguienteObjetivo = "";

    [Header("Senaletica")]
    public GameObject senaletica;

    private Transform jugador;
    private bool dialogoAbierto = false;
    private int dialogoActual = 0;

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
                AbrirDialogo();
        }
        else
        {
            if (senaletica != null)
                senaletica.SetActive(false);
        }
    }

    void AbrirDialogo()
    {
        dialogoAbierto = true;
        if (senaletica != null)
            senaletica.SetActive(false);

        GameManager.instancia.RegistrarVisitaNPC(npcId);

        if (!string.IsNullOrEmpty(siguienteObjetivo))
            MisionManager.instancia.CompletarObjetivo(siguienteObjetivo);

        string[] dialogos = { dialogo1, dialogo2, dialogo3 };
        string texto = dialogos[dialogoActual % dialogos.Length];
        dialogoActual++;

        DialogueSimpleUI.instancia.MostrarDialogo(nombreNPC, texto, this);
    }

    public void CerrarDialogo()
    {
        dialogoAbierto = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, distanciaInteraccion);
    }
}