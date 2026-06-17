using UnityEngine;
using TMPro;

public class PuntajeUI : MonoBehaviour
{
    public TextMeshProUGUI textoPuntaje;

    void Update()
    {
        if (GameManager.instancia != null)
            textoPuntaje.text = "⭐ " + GameManager.instancia.puntajeTotal + " / 300\nCap. " + GameManager.instancia.capitulActual;
    }
}