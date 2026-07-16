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
    private NPCEvaluador evaluadorActual;

    void Awake()
    {
        instancia = this;
        panelQuiz.SetActive(false);
    }

    public bool IniciarQuiz(int numeroCapitulo, NPCEvaluador evaluador = null)
    {
        capitulo = numeroCapitulo;
        evaluadorActual = evaluador;
        preguntaActual = 0;
        respuestasCorrectas = 0;
        preguntas = ObtenerPreguntasCapitulo(capitulo);

        if (preguntas.Count == 0)
        {
            Debug.LogWarning("QuizManager: no hay preguntas cargadas para el capitulo " + capitulo + ". Revisa ObtenerPreguntasCapitulo().");
            return false;
        }

        preguntas = MezclarPreguntas(preguntas);
        panelQuiz.SetActive(true);
        botonSiguiente.gameObject.SetActive(false);
        botonCerrar.gameObject.SetActive(false);
        textoResultado.gameObject.SetActive(false);
        MostrarPregunta();
        return true;
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
        if (preguntas.Count == 0) return;

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

        if (capitulo == 3)
        {
            // Ultimo capitulo: mostramos la pantalla final en vez de actualizar la mision
            if (PantallaFinalManager.instancia != null)
                PantallaFinalManager.instancia.Mostrar();
        }
        else
        {
            if (MisionManager.instancia != null)
                MisionManager.instancia.ActualizarMisionPorCapitulo(GameManager.instancia.capitulActual);
        }

        if (evaluadorActual != null)
            evaluadorActual.CerrarEvaluacion();
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
        else if (cap == 2)
        {
            // Capitulo 2 - Ing. Ada: DESCOMPOSICION (dividir problemas grandes en subtareas)
            lista.Add(new Pregunta(
                "Que es la descomposicion como tecnica de pensamiento computacional?",
                new string[] {
                    "Eliminar partes de un problema hasta que sea facil",
                    "Dividir un problema grande y complejo en partes mas pequenas y manejables",
                    "Resolver el problema mas dificil primero sin planificar",
                    "Copiar la solucion de otro problema parecido"
                }, 1,
                "Descomponer significa partir un problema grande en subproblemas mas pequenos, cada uno mas facil de resolver por separado."
            ));

            lista.Add(new Pregunta(
                "Alex debe reparar la fuente de poder de su PC, pero la tarea se ve enorme. Que deberia hacer primero segun la descomposicion?",
                new string[] {
                    "Intentar arreglarlo todo de una sola vez sin plan",
                    "Dividir la reparacion en pasos: revisar cables, revisar la fuente, revisar la placa, probar encendido",
                    "Comprar una PC nueva",
                    "Pedirle a alguien mas que lo resuelva sin entender el problema"
                }, 1,
                "Dividir la reparacion en subtareas claras hace que un problema grande se sienta manejable y ordenado."
            ));

            lista.Add(new Pregunta(
                "Cual es el principal beneficio de descomponer un problema grande?",
                new string[] {
                    "Hace que el problema desaparezca automaticamente",
                    "Permite trabajar cada parte por separado, con mas claridad y menos carga mental",
                    "Aumenta la dificultad del problema",
                    "Evita tener que entender el problema original"
                }, 1,
                "Al trabajar cada subtarea por separado, el problema se vuelve mas claro, ordenado y facil de resolver."
            ));

            lista.Add(new Pregunta(
                "Un estudiante tiene que preparar 3 examenes en la misma semana. Como aplicaria la descomposicion?",
                new string[] {
                    "Estudiar todo junto la noche anterior a cada examen",
                    "Dividir el estudio de cada materia en temas y asignar un dia especifico a cada tema",
                    "Estudiar solo la materia que mas le gusta",
                    "No estudiar y confiar en la suerte"
                }, 1,
                "Separar cada materia en temas y repartirlos en dias distintos es aplicar descomposicion a un problema academico."
            ));

            lista.Add(new Pregunta(
                "Que diferencia hay entre descomponer un problema y simplemente dividir tareas al azar?",
                new string[] {
                    "No hay ninguna diferencia",
                    "La descomposicion busca que cada subtarea sea logica, manejable y tenga sentido dentro del problema completo",
                    "Descomponer siempre significa hacer mas tareas, no menos",
                    "Dividir al azar es mas eficiente"
                }, 1,
                "La descomposicion no es dividir al azar: cada subtarea debe tener sentido y aportar a resolver el problema original."
            ));

            lista.Add(new Pregunta(
                "Alex quiere organizar un proyecto grupal de la universidad. Cual de estas opciones es un buen ejemplo de descomposicion?",
                new string[] {
                    "Asignar a cada integrante una parte especifica del proyecto: investigacion, redaccion, diseno y presentacion",
                    "Que una sola persona haga todo el trabajo",
                    "Empezar el proyecto sin repartir tareas",
                    "Discutir sobre quien tiene mas experiencia sin asignar responsabilidades"
                }, 0,
                "Repartir el proyecto en partes claras (investigacion, redaccion, diseno, presentacion) es descomponerlo en subtareas manejables."
            ));

            lista.Add(new Pregunta(
                "Por que la descomposicion es especialmente util cuando un problema parece abrumador?",
                new string[] {
                    "Porque hace que el problema sea mas dificil de entender",
                    "Porque reduce la sensacion de sobrecarga al enfocarse en una parte pequena a la vez",
                    "Porque permite ignorar partes importantes del problema",
                    "Porque elimina la necesidad de un plan"
                }, 1,
                "Enfocarse en una subtarea a la vez reduce la sensacion de estar abrumado frente a un problema grande."
            ));

            lista.Add(new Pregunta(
                "Cual de estos NO es un paso tipico al aplicar descomposicion?",
                new string[] {
                    "Identificar el problema principal",
                    "Dividirlo en partes mas pequenas y logicas",
                    "Resolver cada parte por separado",
                    "Ignorar el orden en que se resuelven las partes"
                }, 3,
                "El orden si importa: algunas subtareas dependen de que otras se resuelvan primero para que el problema completo funcione."
            ));

            lista.Add(new Pregunta(
                "En el contexto de reparar la PC de Alex, cual seria una subtarea razonable dentro de la descomposicion del problema?",
                new string[] {
                    "Verificar si el cable de poder esta bien conectado",
                    "Rendir el examen final sin arreglar la PC",
                    "Cambiar de carrera universitaria",
                    "Ignorar el problema y usar la PC de otra persona para siempre"
                }, 0,
                "Verificar el cable de poder es una subtarea concreta y pequena dentro del problema mas grande de reparar la PC."
            ));

            lista.Add(new Pregunta(
                "Que relacion tiene la descomposicion con los algoritmos vistos en el capitulo anterior?",
                new string[] {
                    "No tienen ninguna relacion",
                    "Descomponer un problema en pasos ordenados es, en esencia, el inicio de construir un algoritmo",
                    "La descomposicion reemplaza por completo a los algoritmos",
                    "Los algoritmos solo sirven para programas, no para problemas de la vida real"
                }, 1,
                "Al descomponer un problema en subtareas ordenadas, ya estamos dando los primeros pasos para construir un algoritmo que lo resuelva."
            ));
        }
        else if (cap == 3)
        {
            // Capitulo 3 - Dr. Turing: RECONOCIMIENTO DE PATRONES Y ABSTRACCION
            lista.Add(new Pregunta(
                "Que es reconocer un patron en el pensamiento computacional?",
                new string[] {
                    "Memorizar todos los datos de un problema sin analizarlos",
                    "Identificar similitudes, repeticiones o tendencias entre distintos problemas o datos",
                    "Dibujar figuras geometricas repetidas",
                    "Ignorar cualquier dato que se repita"
                }, 1,
                "Reconocer patrones significa identificar similitudes o repeticiones que ayudan a entender y resolver un problema."
            ));

            lista.Add(new Pregunta(
                "El sistema de examenes de la universidad falla siempre que hay mas de 100 estudiantes conectados. Que estan observando al notar esto?",
                new string[] {
                    "Un algoritmo",
                    "Un patron en el comportamiento del sistema",
                    "Una descomposicion del problema",
                    "Una abstraccion del sistema"
                }, 1,
                "Notar que el error ocurre bajo una condicion especifica y repetida es identificar un patron."
            ));

            lista.Add(new Pregunta(
                "Que es la abstraccion en el pensamiento computacional?",
                new string[] {
                    "Agregar toda la informacion posible sin filtrar nada",
                    "Quedarse solo con la informacion relevante e ignorar los detalles que no importan para resolver el problema",
                    "Hacer un problema mas complicado a proposito",
                    "Copiar un problema identico para resolverlo dos veces"
                }, 1,
                "La abstraccion consiste en enfocarse solo en lo esencial de un problema, dejando de lado el detalle innecesario o el ruido."
            ));

            lista.Add(new Pregunta(
                "Por que reconocer patrones ayuda a resolver problemas mas rapido?",
                new string[] {
                    "Porque permite reutilizar soluciones que ya funcionaron en casos similares",
                    "Porque hace que el problema sea mas dificil de entender",
                    "Porque evita tener que pensar en la solucion",
                    "Porque los patrones no tienen relacion con la solucion del problema"
                }, 0,
                "Si un patron ya se resolvio antes de forma similar, se puede reutilizar ese enfoque en vez de empezar desde cero."
            ));

            lista.Add(new Pregunta(
                "Alex nota que el sistema de examenes falla justo despues de que un estudiante intenta reenviar una respuesta muy rapido. Que deberia hacer despues de notar este patron?",
                new string[] {
                    "Ignorarlo porque no tiene importancia",
                    "Usar abstraccion para enfocarse solo en esa condicion especifica y buscar una solucion para ese caso",
                    "Reiniciar toda la universidad",
                    "Cambiar de computadora sin investigar mas"
                }, 1,
                "Una vez identificado el patron, la abstraccion ayuda a enfocarse solo en la condicion relevante (el reenvio rapido) e ignorar el resto del ruido."
            ));

            lista.Add(new Pregunta(
                "Cual de estos es un buen ejemplo de abstraccion en la vida universitaria?",
                new string[] {
                    "Memorizar el nombre de todos los edificios del campus para resolver un examen de programacion",
                    "Al estudiar para un examen de programacion, enfocarse solo en los temas que entraran y dejar de lado informacion irrelevante",
                    "Anotar absolutamente todo lo que dice el profesor sin filtrar nada",
                    "Estudiar todas las materias de la carrera al mismo tiempo sin prioridad"
                }, 1,
                "La abstraccion es filtrar y quedarse solo con la informacion relevante para el objetivo, dejando fuera lo que no aporta."
            ));

            lista.Add(new Pregunta(
                "Que diferencia principal hay entre reconocimiento de patrones y abstraccion?",
                new string[] {
                    "Son exactamente lo mismo",
                    "El reconocimiento de patrones identifica repeticiones o similitudes; la abstraccion filtra la informacion relevante ignorando el detalle innecesario",
                    "La abstraccion solo se usa en matematicas",
                    "El reconocimiento de patrones no tiene ninguna utilidad practica"
                }, 1,
                "Son complementarios: primero se detectan similitudes (patrones) y luego se simplifica el problema quedandose con lo esencial (abstraccion)."
            ));

            lista.Add(new Pregunta(
                "Si el error del sistema de examenes ocurre siempre a la misma hora del dia, que tipo de patron esta observando Alex?",
                new string[] {
                    "Un patron temporal, relacionado con el momento en que ocurre el error",
                    "Un patron de color",
                    "Un patron de descomposicion",
                    "No es un patron, es una coincidencia sin ninguna logica"
                }, 0,
                "Un error que se repite en un momento especifico del dia es un patron temporal, util para encontrar su causa."
            ));

            lista.Add(new Pregunta(
                "Por que Dr. Turing insiste en que Alex razone con preguntas en vez de darle la respuesta directa?",
                new string[] {
                    "Porque no conoce la respuesta",
                    "Porque el objetivo es que Alex desarrolle su propia capacidad de reconocer patrones y abstraer informacion, no solo memorizar una solucion",
                    "Porque las preguntas son mas dificiles que las respuestas",
                    "Porque no le interesa que Alex apruebe el examen"
                }, 1,
                "El pensamiento computacional busca desarrollar la habilidad de razonar, no solo memorizar respuestas puntuales."
            ));

            lista.Add(new Pregunta(
                "Cual de las siguientes opciones resume mejor los 4 pilares del pensamiento computacional vistos en los tres capitulos?",
                new string[] {
                    "Programar, compilar, ejecutar y depurar",
                    "Descomposicion (dividir el problema), reconocimiento de patrones (identificar similitudes), abstraccion (quedarse con lo esencial) y diseno de algoritmos (definir los pasos de la solucion)",
                    "Memorizar, copiar, repetir y entregar",
                    "Diagnosticar, reparar, reiniciar y esperar"
                }, 1,
                "Los cuatro pilares trabajados a lo largo del juego son descomposicion, reconocimiento de patrones, abstraccion y diseno de algoritmos."
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