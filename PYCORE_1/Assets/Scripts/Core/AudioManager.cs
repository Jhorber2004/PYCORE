using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instancia;

    [Header("Musica Ambiental")]
    [Tooltip("Clip de musica que suena en loop de fondo")]
    public AudioClip musicaAmbiental;
    [Range(0f, 1f)]
    public float volumenMusica = 0.5f;

    private AudioSource fuenteMusica;

    void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject);

            fuenteMusica = gameObject.AddComponent<AudioSource>();
            fuenteMusica.clip = musicaAmbiental;
            fuenteMusica.loop = true;
            fuenteMusica.volume = volumenMusica;
            fuenteMusica.playOnAwake = false;

            if (musicaAmbiental != null)
                fuenteMusica.Play();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CambiarMusica(AudioClip nuevoClip)
    {
        if (fuenteMusica.clip == nuevoClip) return;

        fuenteMusica.clip = nuevoClip;
        fuenteMusica.Play();
    }

    public void SetVolumen(float volumen)
    {
        volumenMusica = volumen;
        fuenteMusica.volume = volumen;
    }

    public void Pausar()
    {
        fuenteMusica.Pause();
    }

    public void Reanudar()
    {
        fuenteMusica.UnPause();
    }
}   