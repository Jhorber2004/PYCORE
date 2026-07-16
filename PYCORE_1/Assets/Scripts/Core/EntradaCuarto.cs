using UnityEngine;

public class EntradaCuarto : MonoBehaviour
{
    [Header("Configuracion")]
    [Tooltip("Distancia a la que el jugador puede presionar E para entrar")]
    public float distanciaInteraccion = 2.5f;

    [Header("Referencias")]
    [Tooltip("Punto donde aparecera el jugador dentro del cuarto")]
    public Transform puntoDestino;
    [Tooltip("Objeto visual que indica 'Presiona E' (texto, icono, etc). Opcional.")]
    public GameObject senaletica;

    private Transform jugador;

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
        if (jugador == null) return;

        float distancia = Vector2.Distance(transform.position, jugador.position);
        bool enRango = distancia <= distanciaInteraccion;

        if (senaletica != null)
            senaletica.SetActive(enRango);

        if (enRango && Input.GetKeyDown(KeyCode.E))
        {
            Entrar();
        }
    }

    void Entrar()
    {
        if (puntoDestino == null)
        {
            Debug.LogWarning("EntradaCuarto: falta asignar 'puntoDestino' en " + gameObject.name);
            return;
        }

        // Sin fade, sin imagen: solo teletransporta al jugador dentro del cuarto.
        jugador.position = puntoDestino.position;

        if (senaletica != null)
            senaletica.SetActive(false);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, distanciaInteraccion);
    }
}