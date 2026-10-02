using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instancia { get; private set; }


    [SerializeField] private GameObject pantallaVictoria;
    [SerializeField] private GameObject pantallaDerrota;

    private bool juegoTerminado = false;

    private void Awake()
    {
        Instancia = this;
    }

    private void Update()
    {
        if (juegoTerminado && Input.GetKeyDown(KeyCode.R))
        {
            ReiniciarNivel();
        }
    }

    public void PerderJuego()
    {
        if (juegoTerminado) return;

        juegoTerminado = true;
        Time.timeScale = 0f;

        if (pantallaDerrota != null)
            pantallaDerrota.SetActive(true);

        Debug.Log("PERDISTE - Te atrapó el enemigo. Presioná R para reiniciar.");
    }

    public void GanarJuego()
    {
        if (juegoTerminado) return;

        juegoTerminado = true;
        Time.timeScale = 0f;

        if (pantallaVictoria != null)
            pantallaVictoria.SetActive(true);

        Debug.Log("GANASTE - Rescataste a Zelda. Presioná R para reiniciar.");
    }

    private void ReiniciarNivel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}