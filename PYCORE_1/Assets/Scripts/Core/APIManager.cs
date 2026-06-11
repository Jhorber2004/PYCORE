using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class APIManager : MonoBehaviour
{
    public static APIManager instancia;

    private string apiKey = "";
    private string apiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash-lite:generateContent";

    void Awake()
    {
        instancia = this;
        TextAsset configFile = Resources.Load<TextAsset>("config");
        if (configFile != null)
            apiKey = configFile.text.Trim();
        else
            Debug.LogError("No se encontró config.txt en Resources");
    }

    public void EnviarMensaje(string systemPrompt, string mensaje, System.Action<string> callback)
    {
        StartCoroutine(LlamarAPI(systemPrompt, mensaje, callback));
    }

    IEnumerator LlamarAPI(string systemPrompt, string mensaje, System.Action<string> callback)
    {
        string mensajeLimpio = mensaje.Replace("\"", "'").Replace("\n", " ");
        string promptLimpio = systemPrompt.Replace("\"", "'").Replace("\n", " ");

        string jsonBody = "{\"contents\":[{\"parts\":[{\"text\":\"" + promptLimpio + "\\n\\nEstudiante: " + mensajeLimpio + "\"}]}]}";

        Debug.Log("Enviando a Gemini: " + jsonBody);

        UnityWebRequest request = new UnityWebRequest(apiUrl + "?key=" + apiKey, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        Debug.Log("Código de respuesta: " + request.responseCode);
        Debug.Log("Respuesta completa: " + request.downloadHandler.text);

        if (request.result == UnityWebRequest.Result.Success)
        {
            string respuesta = request.downloadHandler.text;
            string texto = ExtraerTexto(respuesta);
            callback(texto);
        }
        else
        {
            Debug.LogError("Error: " + request.error);
            Debug.LogError("Respuesta error: " + request.downloadHandler.text);
            callback("Lo siento, no puedo responder ahora mismo.");
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