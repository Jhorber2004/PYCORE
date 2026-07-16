using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    public static FadeManager instancia;

    [Header("UI")]
    [Tooltip("CanvasGroup de una Image negra que cubre toda la pantalla")]
    public CanvasGroup panelNegro;
    [Tooltip("Image (hija del panel negro) donde se muestra la imagen de transicion")]
    public Image imagenTransicion;

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
        if (imagenTransicion != null)
            imagenTransicion.gameObject.SetActive(false);
    }

    // Fade normal, sin imagen (lo que ya usabas)
    public void TransicionCapitulo(System.Action enElPuntoNegro = null)
    {
        StartCoroutine(CoTransicion(enElPuntoNegro));
    }

    // Fade + imagen sostenida "duracionImagen" segundos antes de volver
    public void TransicionCapituloConImagen(Sprite sprite, float duracionImagen, System.Action enElPuntoNegro = null)
    {
        StartCoroutine(CoTransicionConImagen(sprite, duracionImagen, enElPuntoNegro));
    }

    IEnumerator CoTransicion(System.Action enElPuntoNegro)
    {
        yield return StartCoroutine(Fade(0f, 1f));
        enElPuntoNegro?.Invoke();
        yield return new WaitForSeconds(pausaEnNegro);
        yield return StartCoroutine(Fade(1f, 0f));
    }

    IEnumerator CoTransicionConImagen(Sprite sprite, float duracionImagen, System.Action enElPuntoNegro)
    {
        yield return StartCoroutine(Fade(0f, 1f));
        enElPuntoNegro?.Invoke();

        if (imagenTransicion != null && sprite != null)
        {
            imagenTransicion.sprite = sprite;
            imagenTransicion.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(duracionImagen);

        if (imagenTransicion != null)
            imagenTransicion.gameObject.SetActive(false);

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