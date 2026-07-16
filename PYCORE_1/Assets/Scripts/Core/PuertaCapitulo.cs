using UnityEngine;

public class PuertaCapitulo : MonoBehaviour
{
    [Header("Configuracion")]
    [Tooltip("Capitulo que debe estar completado para que esta puerta se pueda cruzar (1, 2 o 3)")]
    public int capituloRequerido = 1;
    [Tooltip("Distancia a la que el jugador puede presionar E para cruzar")]
    public float distanciaInteraccion = 2.5f;

    [Header("Modo Prueba")]
    [Tooltip("SOLO PARA PROBAR: si esta activo, la puerta se puede cruzar sin cumplir el requisito. Desactivar antes de entregar el proyecto.")]
    public bool modoPrueba = false;

    [Header("Imagen de Transicion")]
    [Tooltip("Imagen que aparece en pantalla negra al cruzar (ej: portada Capitulo 2)")]
    public Sprite imagenSiguienteCapitulo;
    [Tooltip("Cuantos segundos se queda la imagen en pantalla")]
    public float duracionImagen = 6f;

    [Header("Referencias")]
    [Tooltip("Punto donde aparecera el jugador al otro lado de la puerta")]
    public Transform puntoDestino;
    [Tooltip("Objeto visual que indica 'Presiona E' (texto, icono, etc). Opcional.")]
    public GameObject senaletica;
    [Tooltip("Mensaje si el capitulo aun no esta completado. Dejar vacio para no mostrar nada.")]
    public string mensajeBloqueado = "Todavia no puedes cruzar aqui.";

    private Transform jugador;
    private bool transicionEnCurso = false;

    void Start()
    {
        GameObject jugadorObj = GameObject.FindWithTag("Player");
        if (jugadorObj != null)
            jugador = jugadorObj.transform;

        if (senaletica != null)
            senaletica.SetActive(false);
    }

    void Update()
    {
        if (jugador == null || transicionEnCurso) return;

        float distancia = Vector2.Distance(transform.position, jugador.position);
        bool enRango = distancia <= distanciaInteraccion;

        if (senaletica != null)
            senaletica.SetActive(enRango);

        if (enRango && Input.GetKeyDown(KeyCode.E))
        {
            if (modoPrueba || CapituloListo())
                CruzarPuerta();
            else if (!string.IsNullOrEmpty(mensajeBloqueado))
                Debug.Log(mensajeBloqueado);
        }
    }

    bool CapituloListo()
    {
        if (GameManager.instancia == null) return false;

        return (capituloRequerido == 1 && GameManager.instancia.capitulo1Completado) ||
               (capituloRequerido == 2 && GameManager.instancia.capitulo2Completado) ||
               (capituloRequerido == 3 && GameManager.instancia.capitulo3Completado);
    }

    void CruzarPuerta()
    {
        if (puntoDestino == null)
        {
            Debug.LogWarning("PuertaCapitulo: falta asignar 'puntoDestino' en " + gameObject.name);
            return;
        }

        transicionEnCurso = true;
        if (senaletica != null)
            senaletica.SetActive(false);

        if (FadeManager.instancia != null)
        {
            FadeManager.instancia.TransicionCapituloConImagen(imagenSiguienteCapitulo, duracionImagen, () =>
            {
                jugador.position = puntoDestino.position;
            });
        }
        else
        {
            jugador.position = puntoDestino.position;
        }

        Invoke(nameof(TerminarTransicion), duracionImagen + (FadeManager.instancia != null ? FadeManager.instancia.duracionFade * 2f : 1.5f));
    }

    void TerminarTransicion()
    {
        transicionEnCurso = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaInteraccion);
    }
}