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
        int capitulo = (GameManager.instancia != null) ? GameManager.instancia.capitulActual : 1;
        ActualizarMisionPorCapitulo(capitulo);
    }

    // BUG CORREGIDO: antes este switch solo vivia dentro de Start(), asi que
    // si el jugador pasaba el examen y avanzaba de capitulo SIN recargar la
    // escena, el panel de mision se quedaba con el texto viejo. Ahora es un
    // metodo publico que QuizManager tambien puede llamar al cerrar el examen.
    public void ActualizarMisionPorCapitulo(int capitulo)
    {
        switch (capitulo)
        {
            case 2:
                ActualizarMision(
                    "Capitulo 2 - La reparacion pendiente",
                    "Habla con Ing. Ada para planear como reparar la fuente de poder de tu PC"
                );
                break;
            case 3:
                ActualizarMision(
                    "Capitulo 3 - El error en el sistema de examenes",
                    "Habla con Dr. Turing para investigar el patron detras del error del sistema"
                );
                break;
            default:
                ActualizarMision(
                    "Capitulo 1 - El examen y la PC danada",
                    "Habla con Prof. Byte para comenzar a diagnosticar tu PC"
                );
                break;
        }
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
        Invoke(nameof(CambiarObjetivo), 2f);
        objetivoActual = nuevoObjetivo;
    }

    void CambiarObjetivo()
    {
        textoObjetivo.text = "Objetivo: " + objetivoActual;
    }
}