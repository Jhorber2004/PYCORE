using UnityEngine;

public class SalidaCuarto : MonoBehaviour
{
    [Header("Configuracion")]
    [Tooltip("Distancia a la que el jugador puede presionar E para salir")]
    public float distanciaInteraccion = 2.5f;

    [Header("Referencias")]
    [Tooltip("Punto donde aparecera el jugador al salir del cuarto")]
    public Transform puntoDestino;
    [Tooltip("Imagen que aparece en pantalla negra al salir (ej: portada del siguiente capitulo)")]
    public Sprite imagenTransicion;
    [Tooltip("Cuantos segundos se queda la imagen en pantalla")]
    public float duracionImagen = 6f;
    [Tooltip("Objeto visual que indica 'Presiona E' (texto, icono, etc). Opcional.")]
    public GameObject senaletica;

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
            Salir();
        }
    }

    void Salir()
    {
        if (puntoDestino == null)
        {
            Debug.LogWarning("SalidaCuarto: falta asignar 'puntoDestino' en " + gameObject.name);
            return;
        }

        transicionEnCurso = true;
        if (senaletica != null)
            senaletica.SetActive(false);

        if (FadeManager.instancia != null)
        {
            FadeManager.instancia.TransicionCapituloConImagen(imagenTransicion, duracionImagen, () =>
            {
                jugador.position = puntoDestino.position;
            });

            float espera = duracionImagen + FadeManager.instancia.duracionFade * 2f;
            Invoke(nameof(TerminarTransicion), espera);
        }
        else
        {
            jugador.position = puntoDestino.position;
            transicionEnCurso = false;
        }
    }

    void TerminarTransicion()
    {
        transicionEnCurso = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaInteraccion);
    }
}