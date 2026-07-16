using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PantallaFinalManager : MonoBehaviour
{
    public static PantallaFinalManager instancia;

    [Header("UI")]
    public GameObject panelFinal;
    public TextMeshProUGUI textoTitulo;
    public TextMeshProUGUI textoResumen;
    public Button botonJugarDeNuevo;
    public Button botonSalir;

    [Header("Configuracion")]
    [TextArea(2, 4)]
    public string mensajeFelicitacion = "Felicidades, Alex! Aplicaste el pensamiento computacional para resolver cada reto y llegar hasta aqui.";

    void Awake()
    {
        instancia = this;
        if (panelFinal != null)
            panelFinal.SetActive(false);
    }

    void Start()
    {
        if (botonJugarDeNuevo != null)
            botonJugarDeNuevo.onClick.AddListener(JugarDeNuevo);
        if (botonSalir != null)
            botonSalir.onClick.AddListener(Salir);
    }

    public void Mostrar()
    {
        if (panelFinal == null || GameManager.instancia == null) return;

        if (textoTitulo != null)
            textoTitulo.text = mensajeFelicitacion;

        textoResumen.text = GameManager.instancia.ObtenerResumen();

        panelFinal.SetActive(true);
    }

    void JugarDeNuevo()
    {
        PlayerPrefs.DeleteKey("ProgresoJuego");
        SceneManager.LoadScene("MenuPrincipal");
    }

    void Salir()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}