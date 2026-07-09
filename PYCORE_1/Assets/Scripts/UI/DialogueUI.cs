using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI instancia;

    [Header("UI Elements")]
    public GameObject panelDialogo;
    public TextMeshProUGUI textoNombre;
    public TextMeshProUGUI textoDialogo;
    public Button botonCerrar;
    public Image imagenNPC;
    public GameObject fondoOscuro;

    [Header("Opciones")]
    public Button opcion1;
    public Button opcion2;
    public Button opcion3;
    public TextMeshProUGUI textoOpcion1;
    public TextMeshProUGUI textoOpcion2;
    public TextMeshProUGUI textoOpcion3;

    private NPCController npcActual;
    private string systemPromptActual;
    private string npcIdActual;

    void Awake()
    {
        instancia = this;
        panelDialogo.SetActive(false);
        if (fondoOscuro != null)
            fondoOscuro.SetActive(false);
    }

    public void MostrarDialogo(string nombre, NPCController npc, string systemPrompt = "", string npcId = "", Sprite imagenPersonaje = null)
    {
        npcActual = npc;
        systemPromptActual = systemPrompt;
        npcIdActual = npcId;
        textoNombre.text = nombre;
        textoDialogo.text = "Hola, soy " + nombre + ". En que te puedo ayudar?";
        panelDialogo.SetActive(true);

        if (fondoOscuro != null)
            fondoOscuro.SetActive(true);

        if (imagenNPC != null)
        {
            if (imagenPersonaje != null)
            {
                imagenNPC.sprite = imagenPersonaje;
                imagenNPC.gameObject.SetActive(true);
            }
            else
            {
                imagenNPC.gameObject.SetActive(false);
            }
        }

        MostrarOpciones();
    }

    void MostrarOpciones()
    {
        opcion1.gameObject.SetActive(true);
        opcion2.gameObject.SetActive(true);
        opcion3.gameObject.SetActive(true);
    }

    public void SeleccionarOpcion(int opcion)
    {
        string mensaje = "";
        if (opcion == 1) mensaje = textoOpcion1.text;
        if (opcion == 2) mensaje = textoOpcion2.text;
        if (opcion == 3) mensaje = textoOpcion3.text;

        opcion1.gameObject.SetActive(false);
        opcion2.gameObject.SetActive(false);
        opcion3.gameObject.SetActive(false);

        textoDialogo.text = "...";

        APIManager.instancia.EnviarMensaje(npcIdActual, systemPromptActual, mensaje, (respuesta) =>
        {
            textoDialogo.text = respuesta;
            MostrarOpciones();
        });
    }

    public void CerrarDialogo()
    {
        panelDialogo.SetActive(false);

        if (fondoOscuro != null)
            fondoOscuro.SetActive(false);

        if (npcActual != null)
            npcActual.CerrarDialogo();
    }
}