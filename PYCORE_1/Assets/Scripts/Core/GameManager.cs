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

    void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject);

            // El nombre ingresado en MenuManager.Jugar() se guarda aqui.
            // Se aplica ANTES de CargarProgreso() para que un progreso guardado
            // previamente no lo sobreescriba con un nombre viejo.
            if (PlayerPrefs.HasKey("NombreEstudiante"))
                nombreEstudiante = PlayerPrefs.GetString("NombreEstudiante");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Agregar puntos al capítulo actual
    public void AgregarPuntos(int puntos)
    {
        puntajeTotal += puntos;

        if (capitulActual == 1) puntajeCapitulo1 += puntos;
        else if (capitulActual == 2) puntajeCapitulo2 += puntos;
        else if (capitulActual == 3) puntajeCapitulo3 += puntos;

        Debug.Log("Puntaje total: " + puntajeTotal);
    }

    // Registrar visita a un NPC y dar puntos
    public void RegistrarVisitaNPC(string npcId)
    {
        if (!npcsVisitados.Contains(npcId))
        {
            npcsVisitados.Add(npcId);
            AgregarPuntos(10);
            Debug.Log("NPC visitado: " + npcId + " +10 puntos");
        }
    }

    // Completar misión del capítulo
    public void CompletarMision(int puntos)
    {
        AgregarPuntos(puntos);
        Debug.Log("Misión completada! +" + puntos + " puntos");
    }

    // Completar capítulo
    public void CompletarCapitulo(int capitulo)
    {
        if (capitulo == 1) capitulo1Completado = true;
        else if (capitulo == 2) capitulo2Completado = true;
        else if (capitulo == 3) capitulo3Completado = true;

        capitulActual = capitulo + 1;
        GuardarProgreso();
    }

    // Guardar progreso en JSON
    public void GuardarProgreso()
    {
        DatosProgreso datos = new DatosProgreso();
        datos.nombreEstudiante = nombreEstudiante;
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

    // Cargar progreso
    public void CargarProgreso()
    {
        if (PlayerPrefs.HasKey("ProgresoJuego"))
        {
            string json = PlayerPrefs.GetString("ProgresoJuego");
            DatosProgreso datos = JsonUtility.FromJson<DatosProgreso>(json);
            nombreEstudiante = datos.nombreEstudiante;
            puntajeTotal = datos.puntajeTotal;
            puntajeCapitulo1 = datos.puntajeCapitulo1;
            puntajeCapitulo2 = datos.puntajeCapitulo2;
            puntajeCapitulo3 = datos.puntajeCapitulo3;
            capitulo1Completado = datos.capitulo1Completado;
            capitulo2Completado = datos.capitulo2Completado;
            capitulo3Completado = datos.capitulo3Completado;
            npcsVisitados = datos.npcsVisitados;
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
    switch (npcId)
    {
        case "prof_byte": return 1;
        case "npc_carlos": return 2;
        case "npc_rosa": return 3;
        case "npc_miguel": return 4;
        default: return 0;
    }
}

    public bool PuedeHablarCon(string npcId)
    {
        int orden = ObtenerOrdenNPC(npcId);
        if (orden <= 1) return true;

        string[] ordenNPCs = { "", "prof_byte", "npc_carlos", "npc_rosa", "npc_miguel" };
        string npcAnterior = ordenNPCs[orden - 1];

        return npcsVisitados.Contains(npcAnterior);
    }
}

[System.Serializable]
public class DatosProgreso
{
    public string nombreEstudiante;
    public int puntajeTotal;
    public int puntajeCapitulo1;
    public int puntajeCapitulo2;
    public int puntajeCapitulo3;
    public bool capitulo1Completado;
    public bool capitulo2Completado;
    public bool capitulo3Completado;
    public List<string> npcsVisitados;
}