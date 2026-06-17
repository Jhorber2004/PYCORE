using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class APIManager : MonoBehaviour
{
    public static APIManager instancia;

    private string apiKey = "";
    private string apiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash-lite:generateContent";

    // Historial de conversación por NPC
    private Dictionary<string, List<string>> historialConversaciones = new Dictionary<string, List<string>>();

    void Awake()
    {
        instancia = this;
        TextAsset configFile = Resources.Load<TextAsset>("config");
        if (configFile != null)
            apiKey = configFile.text.Trim();
        else
            Debug.LogError("No se encontró config.txt en Resources");
    }

    public void EnviarMensaje(string npcId, string systemPrompt, string mensaje, System.Action<string> callback)
    {
        // Inicializar historial si no existe
        if (!historialConversaciones.ContainsKey(npcId))
            historialConversaciones[npcId] = new List<string>();

        // Agregar mensaje del estudiante al historial
        historialConversaciones[npcId].Add("Estudiante: " + mensaje);

        StartCoroutine(LlamarAPI(npcId, systemPrompt, callback));
    }

    public void LimpiarHistorial(string npcId)
    {
        if (historialConversaciones.ContainsKey(npcId))
            historialConversaciones[npcId].Clear();
    }

    IEnumerator LlamarAPI(string npcId, string systemPrompt, System.Action<string> callback)
    {
        // Construir historial como contexto
        string historial = "";
        if (historialConversaciones.ContainsKey(npcId))
        {
            foreach (string linea in historialConversaciones[npcId])
                historial += linea + "\\n";
        }

        string promptLimpio = systemPrompt.Replace("\"", "'").Replace("\n", " ");
        string historialLimpio = historial.Replace("\"", "'");

        string textoCompleto = promptLimpio + "\\n\\nHISTORIAL DE CONVERSACIÓN:\\n" + historialLimpio;

        string jsonBody = "{\"contents\":[{\"parts\":[{\"text\":\"" + textoCompleto + "\"}]}]}";

        UnityWebRequest request = new UnityWebRequest(apiUrl + "?key=" + apiKey, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string respuesta = request.downloadHandler.text;
            string texto = ExtraerTexto(respuesta);

            // Guardar respuesta del NPC en el historial
            if (historialConversaciones.ContainsKey(npcId))
                historialConversaciones[npcId].Add("NPC: " + texto);

            callback(texto);
        }
        else
        {
            Debug.LogError("Error: " + request.error);
            Debug.LogError("Respuesta error: " + request.downloadHandler.text);
            callback("Hmm... parece que mi cerebro digital tuvo un cortocircuito. ¡Intenta de nuevo!");
        }
    }

    string ExtraerTexto(string json)
    {
        int inicio = json.IndexOf("\"text\": \"") + 9;
        int fin = json.IndexOf("\"", inicio);
        if (inicio > 9 && fin > inicio)
            return json.Substring(inicio, fin - inicio);
        return "...";
    }
}