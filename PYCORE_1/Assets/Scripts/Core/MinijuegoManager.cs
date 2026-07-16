using UnityEngine;
using System.Runtime.InteropServices;

public class MinijuegoManager : MonoBehaviour
{
    public static MinijuegoManager instancia;

    [DllImport("__Internal")]
    private static extern void AbrirMinijuegoHTML(string url);

    [DllImport("__Internal")]
    private static extern void CerrarMinijuegoHTML();

    private NPCSimple npcActual;

    void Awake()
    {
        instancia = this;
    }

    public void AbrirMinijuego(string archivoHTML, NPCSimple npc = null)
    {
        npcActual = npc;

        if (string.IsNullOrEmpty(archivoHTML))
        {
            Debug.LogWarning("MinijuegoManager: archivoHTML vacio, no se puede abrir el minijuego.");
            return;
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        // BUG CORREGIDO: antes se pasaba "minijuegos/archivo.html" directo al iframe,
        // pero en el build WebGL los archivos de StreamingAssets quedan bajo la carpeta
        // "StreamingAssets/", asi que la ruta quedaba incompleta y el iframe apuntaba
        // a un archivo inexistente. Application.streamingAssetsPath arma la ruta correcta
        // tanto en WebGL (relativa al index.html del build) como en otras plataformas.
        string url = Application.streamingAssetsPath + "/" + archivoHTML.Trim();
        AbrirMinijuegoHTML(url);
        #else
        Debug.Log("El minijuego HTML solo funciona en build WebGL, no en el editor. Ruta que se usaria: "
            + Application.streamingAssetsPath + "/" + archivoHTML.Trim());
        #endif
    }

    public void RecibirResultadoMinijuego(string puntajeStr)
    {
        int puntaje;
        if (!int.TryParse(puntajeStr, out puntaje))
        {
            Debug.LogWarning("MinijuegoManager: puntaje invalido recibido del HTML: '" + puntajeStr + "'. Se usa 0.");
            puntaje = 0;
        }

        GameManager.instancia.AgregarPuntos(puntaje);
        Debug.Log("Minijuego completado con " + puntaje + " puntos");

        if (npcActual != null)
            npcActual.RecibirResultadoMinijuego(puntaje);
    }

    public void CerrarMinijuego()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        CerrarMinijuegoHTML();
        #endif
    }
}