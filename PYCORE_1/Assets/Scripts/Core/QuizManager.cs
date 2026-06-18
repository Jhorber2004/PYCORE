using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class QuizManager : MonoBehaviour
{
    public static QuizManager instancia;

    [Header("UI Elements")]
    public GameObject panelQuiz;
    public TextMeshProUGUI textoPregunta;
    public TextMeshProUGUI textoNumero;
    public TextMeshProUGUI textoResultado;
    public Button[] botonesRespuesta;
    public Button botonSiguiente;
    public Button botonCerrar;

    private List<Pregunta> preguntas = new List<Pregunta>();
    private int preguntaActual = 0;
    private int respuestasCorrectas = 0;
    private int capitulo = 1;

    void Awake()
    {
        instancia = this;
        panelQuiz.SetActive(false);
    }

    public void IniciarQuiz(int numeroCapitulo)
    {
        capitulo = numeroCapitulo;
        preguntaActual = 0;
        respuestasCorrectas = 0;
        preguntas = ObtenerPreguntasCapitulo(capitulo);
        preguntas = MezclarPreguntas(preguntas);
        panelQuiz.SetActive(true);
        botonSiguiente.gameObject.SetActive(false);
        botonCerrar.gameObject.SetActive(false);
        textoResultado.gameObject.SetActive(false);
        MostrarPregunta();
    }

    void MostrarPregunta()
    {
        if (preguntaActual >= preguntas.Count)
        {
            MostrarResultadoFinal();
            return;
        }

        Pregunta p = preguntas[preguntaActual];
        textoNumero.text = "Pregunta " + (preguntaActual + 1) + " de " + preguntas.Count;
        textoPregunta.text = p.enunciado;

        for (int i = 0; i < botonesRespuesta.Length; i++)
        {
            botonesRespuesta[i].gameObject.SetActive(true);
            botonesRespuesta[i].GetComponentInChildren<TextMeshProUGUI>().text = p.opciones[i];
            botonesRespuesta[i].image.color = Color.white;
            botonesRespuesta[i].interactable = true;
        }

        botonSiguiente.gameObject.SetActive(false);
        textoResultado.gameObject.SetActive(false);
    }

    public void ResponderPregunta(int indice)
    {
        Pregunta p = preguntas[preguntaActual];

        for (int i = 0; i < botonesRespuesta.Length; i++)
            botonesRespuesta[i].interactable = false;

        if (indice == p.respuestaCorrecta)
        {
            botonesRespuesta[indice].image.color = new Color(0.2f, 0.8f, 0.2f);
            textoResultado.text = "Correcto. " + p.retroalimentacion;
            textoResultado.color = new Color(0.2f, 0.8f, 0.2f);
            respuestasCorrectas++;
        }
        else
        {
            botonesRespuesta[indice].image.color = new Color(0.8f, 0.2f, 0.2f);
            botonesRespuesta[p.respuestaCorrecta].image.color = new Color(0.2f, 0.8f, 0.2f);
            textoResultado.text = "Incorrecto. " + p.retroalimentacion;
            textoResultado.color = new Color(0.8f, 0.2f, 0.2f);
        }

        textoResultado.gameObject.SetActive(true);
        botonSiguiente.gameObject.SetActive(true);
    }

    public void SiguientePregunta()
    {
        preguntaActual++;
        MostrarPregunta();
    }

    void MostrarResultadoFinal()
    {
        int puntaje = (respuestasCorrectas * 40) / preguntas.Count;
        GameManager.instancia.AgregarPuntos(puntaje);
        GameManager.instancia.GuardarProgreso();

        for (int i = 0; i < botonesRespuesta.Length; i++)
            botonesRespuesta[i].gameObject.SetActive(false);

        botonSiguiente.gameObject.SetActive(false);
        textoNumero.text = "Resultados del Capitulo " + capitulo;

        string nivel = "";
        float porcentaje = (float)respuestasCorrectas / preguntas.Count * 100;

        if (porcentaje >= 90)
            nivel = "Excelente - Dominaste el tema";
        else if (porcentaje >= 70)
            nivel = "Bueno - Vas por buen camino";
        else if (porcentaje >= 50)
            nivel = "Regular - Repasa los conceptos";
        else
            nivel = "Necesitas reforzar - Habla con los NPCs";

        textoPregunta.text = nivel +
            "\n\nRespondiste correctamente " + respuestasCorrectas +
            " de " + preguntas.Count + " preguntas." +
            "\n\nPuntos obtenidos: +" + puntaje;

        textoResultado.gameObject.SetActive(false);
        botonCerrar.gameObject.SetActive(true);

        if (respuestasCorrectas == preguntas.Count)
            GameManager.instancia.AgregarPuntos(20);
    }

    public void CerrarQuiz()
    {
        panelQuiz.SetActive(false);
        GameManager.instancia.CompletarCapitulo(capitulo);
    }

    List<Pregunta> MezclarPreguntas(List<Pregunta> lista)
    {
        for (int i = lista.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            Pregunta temp = lista[i];
            lista[i] = lista[j];
            lista[j] = temp;
        }
        return lista;
    }

    List<Pregunta> ObtenerPreguntasCapitulo(int cap)
    {
        List<Pregunta> lista = new List<Pregunta>();

        if (cap == 1)
        {
            lista.Add(new Pregunta(
                "Que es un algoritmo?",
                new string[] {
                    "Un programa de computadora listo para ejecutarse",
                    "Una secuencia finita de pasos ordenados para resolver un problema",
                    "Un lenguaje de programacion avanzado",
                    "Un tipo de inteligencia artificial"
                }, 1,
                "Un algoritmo es una secuencia finita y ordenada de instrucciones para resolver un problema especifico."
            ));

            lista.Add(new Pregunta(
                "Cuales son los 4 pilares del pensamiento computacional?",
                new string[] {
                    "Programar, depurar, ejecutar y compilar",
                    "Leer, escribir, calcular y memorizar",
                    "Descomposicion, reconocimiento de patrones, abstraccion y algoritmos",
                    "Hardware, software, redes y bases de datos"
                }, 2,
                "Los 4 pilares son: descomposicion, reconocimiento de patrones, abstraccion y diseno de algoritmos."
            ));

            lista.Add(new Pregunta(
                "Que significa descomposicion en el pensamiento computacional?",
                new string[] {
                    "Borrar partes de un programa",
                    "Dividir un problema complejo en partes mas pequenas y manejables",
                    "Ordenar datos de mayor a menor",
                    "Eliminar errores del codigo"
                }, 1,
                "La descomposicion consiste en dividir un problema grande en subproblemas mas faciles de resolver."
            ));

            lista.Add(new Pregunta(
                "Alex necesita entregar un proyecto de 20 paginas en una semana. Cual es el primer paso con pensamiento computacional?",
                new string[] {
                    "Escribir todo el proyecto el ultimo dia",
                    "Descomponer el proyecto en secciones y asignar dias para cada una",
                    "Buscar un companero que lo haga",
                    "Esperar inspiracion"
                }, 1,
                "El pensamiento computacional nos dice que debemos descomponer la tarea en partes manejables y planificar."
            ));

            lista.Add(new Pregunta(
                "Cual de estas opciones representa mejor un algoritmo de la vida universitaria?",
                new string[] {
                    "Pensar en ir a clases",
                    "Pasos para inscribirse en materias: ingresar al sistema, seleccionar materias, verificar horarios, confirmar",
                    "El nombre de la carrera",
                    "El numero de creditos"
                }, 1,
                "Los pasos para inscribirse siguen un orden logico y definido, eso es un algoritmo."
            ));

            lista.Add(new Pregunta(
                "Que es el reconocimiento de patrones en el pensamiento computacional?",
                new string[] {
                    "Memorizar formulas matematicas",
                    "Identificar similitudes o tendencias repetidas en problemas",
                    "Dibujar figuras geometricas",
                    "Escribir codigo repetitivo"
                }, 1,
                "El reconocimiento de patrones permite identificar elementos comunes para reutilizar soluciones."
            ));

            lista.Add(new Pregunta(
                "Alex nota que cada lunes tiene que entregar tareas de tres materias diferentes. Que pilar del pensamiento computacional esta aplicando al darse cuenta?",
                new string[] {
                    "Abstraccion",
                    "Algoritmo",
                    "Reconocimiento de patrones",
                    "Descomposicion"
                }, 2,
                "Al identificar que algo se repite cada semana, Alex esta reconociendo un patron en su rutina universitaria."
            ));

            lista.Add(new Pregunta(
                "Que es la abstraccion en el pensamiento computacional?",
                new string[] {
                    "Ignorar todo lo que no entendemos",
                    "Enfocarse solo en la informacion relevante e ignorar detalles innecesarios",
                    "Crear imagenes abstractas en el computador",
                    "Programar sin comentarios"
                }, 1,
                "La abstraccion nos ayuda a simplificar problemas enfocandose en lo esencial."
            ));

            lista.Add(new Pregunta(
                "Un estudiante quiere saber cuanto tiempo le queda antes del examen final. Que debe hacer primero aplicando pensamiento computacional?",
                new string[] {
                    "Entrar en panico",
                    "Identificar la fecha del examen, la fecha actual y calcular la diferencia",
                    "Preguntarle a un amigo",
                    "Revisar redes sociales"
                }, 1,
                "Identificar los datos relevantes y procesarlos es la base del pensamiento computacional."
            ));

            lista.Add(new Pregunta(
                "Cual es la diferencia entre un algoritmo y un programa de computadora?",
                new string[] {
                    "No hay diferencia, son lo mismo",
                    "Un algoritmo es la solucion logica en pasos y un programa es su implementacion en codigo",
                    "Un programa es mas inteligente que un algoritmo",
                    "Un algoritmo solo funciona en matematicas"
                }, 1,
                "El algoritmo es el plan logico para resolver un problema. El programa es ese plan escrito en un lenguaje de programacion."
            ));
        }

        return lista;
    }
}

[System.Serializable]
public class Pregunta
{
    public string enunciado;
    public string[] opciones;
    public int respuestaCorrecta;
    public string retroalimentacion;

    public Pregunta(string enunciado, string[] opciones, int respuestaCorrecta, string retroalimentacion)
    {
        this.enunciado = enunciado;
        this.opciones = opciones;
        this.respuestaCorrecta = respuestaCorrecta;
        this.retroalimentacion = retroalimentacion;
    }
}