using UnityEngine;
using TMPro;

public class NPCController : MonoBehaviour
{
    [Header("Configuración")]
    public float distanciaInteraccion = 3f;
    public string nombreNPC = "Prof. Byte";
    public string npcId = "prof_byte";

    [Header("Imagen")]
    public Sprite imagenPersonaje;

    [Header("IA")]
    [TextArea(5, 10)]
    public string systemPrompt = "Eres Prof. Byte, un profesor robot de la PCEI en la Universidad Central del Ecuador con mucha personalidad. Eres sarcástico pero genuinamente quieres que Alex aprenda. Te encanta hacer referencias a la cultura pop y comparar el pensamiento computacional con situaciones de la vida universitaria. MISIÓN: Guiar a Alex a entender algoritmos y secuencias. REGLAS: Nunca des la respuesta directa, siempre da pistas progresivas. Si el estudiante ya preguntó antes, da una pista más concreta. Máximo 3 oraciones por respuesta. Usa emojis ocasionalmente. Recuerda el historial de la conversación para no repetirte.";

    [Header("Opciones de diálogo")]
    public string opcion1 = "¿Qué es un algoritmo? 🤔";
    public string opcion2 = "¿Cómo organizo mi horario con pensamiento computacional?";
    public string opcion3 = "Estoy perdido, dame una pista 😅";

    [Header("Señalética")]
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
        GameManager.instancia.RegistrarVisitaNPC(npcId);
        MisionManager.instancia.CompletarObjetivo("Habla con Carlos el companero");
        if (senaletica != null)
            senaletica.SetActive(false);

        DialogueUI.instancia.textoOpcion1.text = opcion1;
        DialogueUI.instancia.textoOpcion2.text = opcion2;
        DialogueUI.instancia.textoOpcion3.text = opcion3;
        DialogueUI.instancia.MostrarDialogo(nombreNPC, this, systemPrompt, npcId, imagenPersonaje);
    }

    public void CerrarDialogo()
    {
        dialogoAbierto = false;
        // Limpiar historial al cerrar para empezar fresco la próxima vez
        // Comenta esta línea si quieres que recuerde entre sesiones
        // APIManager.instancia.LimpiarHistorial(npcId);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaInteraccion);
    }
}