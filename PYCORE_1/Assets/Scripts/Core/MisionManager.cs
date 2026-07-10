using UnityEngine;
using TMPro;

public class MisionManager : MonoBehaviour
{
    public static MisionManager instancia;

    [Header("UI")]
    public GameObject panelMision;
    public TextMeshProUGUI textoMision;
    public TextMeshProUGUI textoObjetivo;

    private string misionActual = "";
    private string objetivoActual = "";

    void Awake()
    {
        instancia = this;
    }

void Start()
{
    ActualizarMision(
        "Capitulo 1 - El examen y la PC danada",
        "Habla con Prof. Byte para comenzar a diagnosticar tu PC"
    );
}

    public void ActualizarMision(string mision, string objetivo)
    {
        misionActual = mision;
        objetivoActual = objetivo;
        textoMision.text = mision;
        textoObjetivo.text = "Objetivo: " + objetivo;
        panelMision.SetActive(true);
    }

    public void CompletarObjetivo(string nuevoObjetivo)
    {
        textoObjetivo.text = "Completado - " + objetivoActual;
        Invoke("CambiarObjetivo", 2f);
        objetivoActual = nuevoObjetivo;
    }

    void CambiarObjetivo()
    {
        textoObjetivo.text = "Objetivo: " + objetivoActual;
    }
}   