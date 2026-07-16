using UnityEngine;
using TMPro;
using UnityEngine.UI;

// Cualquier NPC que pueda mostrar un mensaje en DialogueSimpleUI (bloqueado, pista, etc.)
// debe implementar esto para que el panel sepa a quien avisarle cuando se cierra.
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

    void Awake()
    {
        instancia = this;
        panelSimple.SetActive(false);
    }

    public void MostrarDialogo(string nombre, string texto, IDialogable npc, Sprite imagenPersonaje = null, Sprite fondoDialogo = null)
    {
        npcActual = npc;
        textoNombre.text = nombre;
        textoDialogo.text = texto;
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
    }

    public void CerrarDialogo()
    {
        panelSimple.SetActive(false);
        if (npcActual != null)
            npcActual.CerrarDialogo();
    }
}