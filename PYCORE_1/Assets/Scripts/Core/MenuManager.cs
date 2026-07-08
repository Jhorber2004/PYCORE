using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public TMP_InputField inputNombre;
    public string escenaJuego = "SampleScene";

    public void Jugar()
    {
        string nombre = inputNombre.text.Trim();

        if (string.IsNullOrEmpty(nombre))
        {
            Debug.Log("Por favor ingresa tu nombre");
            return;
        }

        PlayerPrefs.SetString("NombreEstudiante", nombre);
        PlayerPrefs.Save();

        SceneManager.LoadScene(escenaJuego);
    }
}