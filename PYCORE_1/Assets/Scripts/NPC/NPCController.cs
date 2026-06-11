using UnityEngine;
using TMPro;

public class NPCController : MonoBehaviour
{
    [Header("Configuración")]
    public float distanciaInteraccion = 3f;
    public string nombreNPC = "Prof. Byte";

    [Header("IA")]
    [TextArea(3, 6)]
    public string systemPrompt = "Eres Prof. Byte, un personaje del videojuego educativo PYCORE de la Universidad Central del Ecuador. Tu rol es guiar al estudiante Alex a entender qué es el pensamiento computacional. REGLAS: Nunca des la respuesta directa, siempre da pistas. Usa ejemplos universitarios. Máximo 3 oraciones por respuesta. Tono amigable y motivador.";

    [Header("Opciones de diálogo")]
    public string opcion1 = "¿Qué es el pensamiento computacional?";
    public string opcion2 = "¿Cómo se descompone un problema?";
    public string opcion3 = "Dame una pista para la misión";

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
        if (senaletica != null)
            senaletica.SetActive(false);

        DialogueUI.instancia.textoOpcion1.text = opcion1;
        DialogueUI.instancia.textoOpcion2.text = opcion2;
        DialogueUI.instancia.textoOpcion3.text = opcion3;
        DialogueUI.instancia.MostrarDialogo(nombreNPC, this, systemPrompt);
    }

    public void CerrarDialogo()
    {
        dialogoAbierto = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaInteraccion);
    }
}