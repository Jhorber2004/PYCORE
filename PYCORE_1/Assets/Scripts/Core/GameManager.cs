using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instancia;

    [Header("Datos del estudiante")]
    public string nombreEstudiante = "Estudiante";
    public int capitulActual = 1;

    [Header("Puntaje")]
    public int puntajeTotal = 0;
    public int puntajeCapitulo1 = 0;
    public int puntajeCapitulo2 = 0;
    public int puntajeCapitulo3 = 0;

    [Header("Progreso")]
    public bool capitulo1Completado = false;
    public bool capitulo2Completado = false;
    public bool capitulo3Completado = false;

    // NPCs visitados
    private List<string> npcsVisitados = new List<string>();

    // Cadena completa de la historia, confirmada con los mensajes "mensajeBloqueado"
    // de cada NPC en la escena. Indice 0 es un sentinela vacio (no se usa).
    private static readonly string[] ordenNPCs = {
        "",
        "prof_byte",     // 1 - Cap 1
        "npc_carlos",    // 2
        "npc_rosa",      // 3
        "npc_miguel",    // 4
        "ing. Ada",      // 5 - Cap 2
        "npc_maria",     // 6
        "npc_jorge",     // 7
        "npc_miguel2",   // 8
        "dr. Turing",    // 9 - Cap 3
        "npc_luis",      // 10
        "npc_secretaria",// 11
        "npc_miguel3"    // 12
    };

    void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject);

            bool nombreNuevoIngresado = PlayerPrefs.HasKey("NombreEstudiante");
            if (nombreNuevoIngresado)
                nombreEstudiante = PlayerPrefs.GetString("NombreEstudiante");

            CargarProgreso(nombreNuevoIngresado);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AgregarPuntos(int puntos)
    {
        puntajeTotal += puntos;

        if (capitulActual == 1) puntajeCapitulo1 += puntos;
        else if (capitulActual == 2) puntajeCapitulo2 += puntos;
        else if (capitulActual == 3) puntajeCapitulo3 += puntos;

        Debug.Log("Puntaje total: " + puntajeTotal);
    }

    // BUG CORREGIDO: se agrego Trim() para neutralizar los npcId con espacios
    // sobrantes en el Inspector (ej: 'ing. Ada ', 'dr. Turing ') sin depender
    // de que se corrijan manualmente en la escena.
    public void RegistrarVisitaNPC(string npcId)
    {
        if (string.IsNullOrEmpty(npcId)) return;
        npcId = npcId.Trim();

        if (!npcsVisitados.Contains(npcId))
        {
            npcsVisitados.Add(npcId);
            AgregarPuntos(10);
            Debug.Log("NPC visitado: " + npcId + " +10 puntos");
        }
    }

    public void CompletarMision(int puntos)
    {
        AgregarPuntos(puntos);
        Debug.Log("Misión completada! +" + puntos + " puntos");
    }

    public void CompletarCapitulo(int capitulo)
    {
        if (capitulo == 1) capitulo1Completado = true;
        else if (capitulo == 2) capitulo2Completado = true;
        else if (capitulo == 3) capitulo3Completado = true;

        capitulActual = capitulo + 1;
        GuardarProgreso();
        
        if (ReporteManager.instancia != null)
            ReporteManager.instancia.EnviarReporte();   
    }

    public void GuardarProgreso()
    {
        DatosProgreso datos = new DatosProgreso();
        datos.nombreEstudiante = nombreEstudiante;
        datos.capitulActual = capitulActual;
        datos.puntajeTotal = puntajeTotal;
        datos.puntajeCapitulo1 = puntajeCapitulo1;
        datos.puntajeCapitulo2 = puntajeCapitulo2;
        datos.puntajeCapitulo3 = puntajeCapitulo3;
        datos.capitulo1Completado = capitulo1Completado;
        datos.capitulo2Completado = capitulo2Completado;
        datos.capitulo3Completado = capitulo3Completado;
        datos.npcsVisitados = npcsVisitados;

        string json = JsonUtility.ToJson(datos, true);
        PlayerPrefs.SetString("ProgresoJuego", json);
        PlayerPrefs.Save();
        Debug.Log("Progreso guardado: " + json);
    }

    public void CargarProgreso(bool mantenerNombreActual = false)
    {
        if (PlayerPrefs.HasKey("ProgresoJuego"))
        {
            string json = PlayerPrefs.GetString("ProgresoJuego");
            DatosProgreso datos = JsonUtility.FromJson<DatosProgreso>(json);

            if (!mantenerNombreActual)
                nombreEstudiante = datos.nombreEstudiante;

            capitulActual = datos.capitulActual > 0 ? datos.capitulActual : 1;
            puntajeTotal = datos.puntajeTotal;
            puntajeCapitulo1 = datos.puntajeCapitulo1;
            puntajeCapitulo2 = datos.puntajeCapitulo2;
            puntajeCapitulo3 = datos.puntajeCapitulo3;
            capitulo1Completado = datos.capitulo1Completado;
            capitulo2Completado = datos.capitulo2Completado;
            capitulo3Completado = datos.capitulo3Completado;
            npcsVisitados = datos.npcsVisitados ?? new List<string>();
        }
    }

    public string ObtenerResumen()
    {
        return "Estudiante: " + nombreEstudiante +
               "\nPuntaje Total: " + puntajeTotal + "/300" +
               "\nCapítulo 1: " + puntajeCapitulo1 + "/100" +
               "\nCapítulo 2: " + puntajeCapitulo2 + "/100" +
               "\nCapítulo 3: " + puntajeCapitulo3 + "/100";
    }

    public int ObtenerOrdenNPC(string npcId)
    {
        if (string.IsNullOrEmpty(npcId)) return 0;
        npcId = npcId.Trim();

        for (int i = 1; i < ordenNPCs.Length; i++)
            if (ordenNPCs[i] == npcId) return i;

        return 0;
    }

    // BUG CORREGIDO: antes solo cubria 4 NPCs del Capitulo 1. Ahora cubre
    // toda la cadena narrativa de los 3 capitulos, confirmada con los
    // mensajes "mensajeBloqueado" de cada NPC en la escena.
    public bool PuedeHablarCon(string npcId)
    {
        int orden = ObtenerOrdenNPC(npcId);
        if (orden <= 1) return true;

        string npcAnterior = ordenNPCs[orden - 1];
        return npcsVisitados.Contains(npcAnterior);
    }

    // NUEVO: gate especifico para los evaluadores, que antes no validaban
    // ningun orden. Requiere haber hablado con el "Tecnico Miguel" de ese
    // capitulo (el ultimo paso antes del examen en cada capitulo).
    public bool PuedeRendirEvaluacion(int capitulo)
    {
        switch (capitulo)
        {
            case 1: return npcsVisitados.Contains("npc_miguel");
            case 2: return npcsVisitados.Contains("npc_miguel2");
            case 3: return npcsVisitados.Contains("npc_miguel3");
            default: return true;
        }
    }
}

[System.Serializable]
public class DatosProgreso
{
    public string nombreEstudiante;
    public int capitulActual;
    public int puntajeTotal;
    public int puntajeCapitulo1;
    public int puntajeCapitulo2;
    public int puntajeCapitulo3;
    public bool capitulo1Completado;
    public bool capitulo2Completado;
    public bool capitulo3Completado;
    public List<string> npcsVisitados;
}