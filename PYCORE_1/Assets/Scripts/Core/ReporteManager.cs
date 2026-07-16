using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ReporteManager : MonoBehaviour
{
    public static ReporteManager instancia;

    [Header("Configuracion Google Sheets")]
    [Tooltip("Pega aqui la URL de tu Apps Script (la que termina en /exec)")]
    public string urlReporte = "https://script.google.com/macros/s/AKfycbxtsh6cv6SkvJQosxz4BR27PQOjrBMiuEhTtf9VQeWqaGs5vxjYVPtmm2nH_IGdVrqx4Q/exec";

    void Awake()
    {
        instancia = this;
    }

    public void EnviarReporte()
    {
        if (GameManager.instancia == null) return;
        if (string.IsNullOrEmpty(urlReporte) || urlReporte.Contains("TU_ID_AQUI"))
        {
            Debug.LogWarning("ReporteManager: falta configurar la URL del Apps Script.");
            return;
        }

        StartCoroutine(CoEnviarReporte());
    }

    IEnumerator CoEnviarReporte()
    {
        GameManager gm = GameManager.instancia;

        WWWForm form = new WWWForm();
        form.AddField("nombre", gm.nombreEstudiante);
        form.AddField("capituloActual", gm.capitulActual);
        form.AddField("puntajeTotal", gm.puntajeTotal);
        form.AddField("puntajeCap1", gm.puntajeCapitulo1);
        form.AddField("puntajeCap2", gm.puntajeCapitulo2);
        form.AddField("puntajeCap3", gm.puntajeCapitulo3);
        form.AddField("cap1Completado", gm.capitulo1Completado.ToString());
        form.AddField("cap2Completado", gm.capitulo2Completado.ToString());
        form.AddField("cap3Completado", gm.capitulo3Completado.ToString());

        UnityWebRequest request = UnityWebRequest.Post(urlReporte, form);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
            Debug.Log("Reporte enviado correctamente: " + request.downloadHandler.text);
        else
            Debug.LogError("Error al enviar el reporte: " + request.error + " | " + request.downloadHandler.text);
    }
}