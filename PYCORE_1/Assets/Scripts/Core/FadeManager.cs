using UnityEngine;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    public static FadeManager instancia;

    [Header("UI")]
    [Tooltip("CanvasGroup de una Image negra que cubre toda la pantalla")]
    public CanvasGroup panelNegro;

    [Header("Configuracion")]
    public float duracionFade = 0.6f;
    public float pausaEnNegro = 0.15f;

    void Awake()
    {
        instancia = this;
        if (panelNegro != null)
        {
            panelNegro.alpha = 0f;
            panelNegro.blocksRaycasts = false;
        }
    }

    // Se pone en negro, ejecuta una accion opcional en el punto mas oscuro
    // (por ejemplo, mover camara o reproducir un sonido), y vuelve a mostrar la escena.
    public void TransicionCapitulo(System.Action enElPuntoNegro = null)
    {
        StartCoroutine(CoTransicion(enElPuntoNegro));
    }

    IEnumerator CoTransicion(System.Action enElPuntoNegro)
    {
        yield return StartCoroutine(Fade(0f, 1f));
        enElPuntoNegro?.Invoke();
        yield return new WaitForSeconds(pausaEnNegro);
        yield return StartCoroutine(Fade(1f, 0f));
    }

    IEnumerator Fade(float desde, float hasta)
    {
        if (panelNegro == null) yield break;

        panelNegro.blocksRaycasts = true;
        float tiempo = 0f;
        while (tiempo < duracionFade)
        {
            tiempo += Time.deltaTime;
            panelNegro.alpha = Mathf.Lerp(desde, hasta, tiempo / duracionFade);
            yield return null;
        }
        panelNegro.alpha = hasta;
        panelNegro.blocksRaycasts = (hasta > 0.5f);
    }
}