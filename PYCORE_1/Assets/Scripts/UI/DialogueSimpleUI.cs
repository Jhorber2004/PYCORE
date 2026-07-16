using UnityEngine;
using TMPro;
using UnityEngine.UI;

public interface IDialogable
{
    void CerrarDialogo();
}

public class DialogueSimpleUI : MonoBehaviour
{
    public static DialogueSimpleUI instancia;

    [Header("UI Elements")]
    public GameObject panelSimple;
    public TextMeshProUGUI textoNombre;
    public TextMeshProUGUI textoDialogo;
    public Button botonCerrar;
    public Image imagenNPC;
    public Image fondoImagen;

    private IDialogable npcActual;
    private string[] lineasActuales;
    private int indiceActual;

    // BUG CORREGIDO: guardamos en que frame se abrio el panel para no
    // procesar la misma pulsacion de E que lo abrio como si fuera para avanzar/cerrar.
    private int frameApertura = -1;

    void Awake()
    {
        instancia = this;
        panelSimple.SetActive(false);
    }

    void Update()
    {
        // Permite avanzar el dialogo con E ademas del boton
        if (panelSimple.activeSelf && Time.frameCount != frameApertura && Input.GetKeyDown(KeyCode.E))
        {
            Avanzar();
        }
    }

    // Version con varias lineas seguidas
    public void MostrarDialogo(string nombre, string[] lineas, IDialogable npc, Sprite imagenPersonaje = null, Sprite fondoDialogo = null)
    {
        npcActual = npc;
        lineasActuales = lineas;
        indiceActual = 0;
        frameApertura = Time.frameCount;

        textoNombre.text = nombre;
        panelSimple.SetActive(true);

        if (fondoImagen != null && fondoDialogo != null)
            fondoImagen.sprite = fondoDialogo;

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

        MostrarLineaActual();
    }

    // Version con una sola linea (ej: mensaje de bloqueado)
    public void MostrarDialogo(string nombre, string texto, IDialogable npc, Sprite imagenPersonaje = null, Sprite fondoDialogo = null)
    {
        MostrarDialogo(nombre, new string[] { texto }, npc, imagenPersonaje, fondoDialogo);
    }

    void MostrarLineaActual()
    {
        textoDialogo.text = lineasActuales[indiceActual];
    }

    // Llamado por el boton (OnClick ya conectado en el Inspector) o por la tecla E
    public void Avanzar()
    {
        indiceActual++;

        if (indiceActual < lineasActuales.Length)
        {
            MostrarLineaActual();
        }
        else
        {
            CerrarDialogo();
        }
    }

    public void CerrarDialogo()
    {
        panelSimple.SetActive(false);
        if (npcActual != null)
            npcActual.CerrarDialogo();
    }
}