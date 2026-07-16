using UnityEngine;
using TMPro;

public class NPCSimple : MonoBehaviour, IDialogable
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
    public string mensajeBloqueado = "Todavia no puedo ayudarte. Habla primero con las otras personas.";

    [Header("Imagen y Fondo")]
    public Sprite imagenPersonaje;
    public Sprite fondoDialogo;

    [Header("Minijuego")]
    public bool activaMinijuego = false;
    public string archivoMinijuego = "";

    [Header("Senaletica")]
    public GameObject senaletica;

    private Transform jugador;
    private bool dialogoAbierto = false;

    // BUG CORREGIDO: antes no existia forma de distinguir un dialogo real
    // de un mensaje de bloqueo, y ambos activaban el minijuego.
    private bool mostrandoBloqueo = false;

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
                if (GameManager.instancia.PuedeHablarCon(npcId))
                    AbrirDialogo();
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
        mostrandoBloqueo = true;
        if (senaletica != null)
            senaletica.SetActive(false);

        DialogueSimpleUI.instancia.MostrarDialogo(nombreNPC, mensajeBloqueado, this, imagenPersonaje, fondoDialogo);
    }

    void AbrirDialogo()
    {
        dialogoAbierto = true;
        mostrandoBloqueo = false;
        if (senaletica != null)
            senaletica.SetActive(false);

        GameManager.instancia.RegistrarVisitaNPC(npcId);

        if (!string.IsNullOrEmpty(siguienteObjetivo))
            MisionManager.instancia.CompletarObjetivo(siguienteObjetivo);

        string[] dialogos = { dialogo1, dialogo2, dialogo3 };
        DialogueSimpleUI.instancia.MostrarDialogo(nombreNPC, dialogos, this, imagenPersonaje, fondoDialogo);
    }

    void LanzarMinijuego()
    {
        if (string.IsNullOrEmpty(archivoMinijuego)) return;
        MinijuegoManager.instancia.AbrirMinijuego(archivoMinijuego, this);
    }

    // Se llama automaticamente cuando MinijuegoManager recibe el puntaje del HTML.
    public void RecibirResultadoMinijuego(int puntaje)
    {
        Debug.Log(nombreNPC + " - minijuego terminado con " + puntaje + " puntos");
    }

    // Se llama automaticamente cuando DialogueSimpleUI cierra el panel
    public void CerrarDialogo()
    {
        // BUG CORREGIDO: ahora si diferenciamos dialogo real vs mensaje de bloqueo.
        bool eraDialogoCompleto = dialogoAbierto && !mostrandoBloqueo;
        dialogoAbierto = false;
        mostrandoBloqueo = false;

        if (eraDialogoCompleto && activaMinijuego)
            Invoke(nameof(LanzarMinijuego), 0.3f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, distanciaInteraccion);
    }
}