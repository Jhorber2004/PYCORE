using UnityEngine;
using System.Runtime.InteropServices;

public class MinijuegoManager : MonoBehaviour
{
    public static MinijuegoManager instancia;

    [DllImport("__Internal")]
    private static extern void AbrirMinijuegoHTML(string url);

    [DllImport("__Internal")]
    private static extern void CerrarMinijuegoHTML();

    void Awake()
    {
        instancia = this;
    }

    public void AbrirMinijuego(string archivoHTML)
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        AbrirMinijuegoHTML(archivoHTML);
        #else
        Debug.Log("El minijuego HTML solo funciona en build WebGL, no en el editor.");
        #endif
    }

    public void RecibirResultadoMinijuego(string puntajeStr)
    {
        int puntaje = int.Parse(puntajeStr);
        GameManager.instancia.AgregarPuntos(puntaje);
        Debug.Log("Minijuego completado con " + puntaje + " puntos");
    }

    public void CerrarMinijuego()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        CerrarMinijuegoHTML();
        #endif
    }
}