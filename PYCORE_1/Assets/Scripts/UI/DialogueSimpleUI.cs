using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueSimpleUI : MonoBehaviour
{
    public static DialogueSimpleUI instancia;

    [Header("UI Elements")]
    public GameObject panelSimple;
    public TextMeshProUGUI textoNombre;
    public TextMeshProUGUI textoDialogo;
    public Button botonCerrar;

    private NPCSimple npcActual;

    void Awake()
    {
        instancia = this;
        panelSimple.SetActive(false);
    }

    public void MostrarDialogo(string nombre, string texto, NPCSimple npc)
    {
        npcActual = npc;
        textoNombre.text = nombre;
        textoDialogo.text = texto;
        panelSimple.SetActive(true);
    }

    public void CerrarDialogo()
    {
        panelSimple.SetActive(false);
        if (npcActual != null)
            npcActual.CerrarDialogo();
    }
}