using UnityEngine;
using TMPro; // si usas Texto UI clasico (no TextMeshPro), cambia esto por: using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public class PuertaSalida : MonoBehaviour
{
    [Header("Configuracion")]
    public string tagJugador = "Player";

    [Header("Panel de victoria")]
    public GameObject panelVictoria; // arrastra aca el panel de UI que dice "Escapaste"
    public TextMeshProUGUI textoVictoria; // opcional: si el panel tiene un texto, se lo puede sobreescribir
    public string mensaje = "¡Escapaste!";
    public bool pausarJuego = true; // detiene el tiempo del juego mientras se muestra el panel
    public bool mostrarCursor = true; // muestra el mouse para poder tocar botones del panel (reintentar, salir, etc)

    [Header("Opcional")]
    public GameObject efectoAlSalir; // opcional: particulas/sonido al tocar la puerta

    private bool yaActivada = false;

    void Reset()
    {
        // Se asegura de que el collider sea un Trigger al agregar el script
        GetComponent<Collider>().isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (yaActivada) return;
        if (!other.CompareTag(tagJugador)) return;

        yaActivada = true;

        if (efectoAlSalir != null)
            Instantiate(efectoAlSalir, transform.position, Quaternion.identity);

        MostrarPanelVictoria();
    }

    void MostrarPanelVictoria()
    {
        if (textoVictoria != null)
            textoVictoria.text = mensaje;

        if (panelVictoria != null)
            panelVictoria.SetActive(true);
        else
            Debug.LogWarning("PuertaSalida: no asignaste un 'Panel Victoria' en el Inspector.");

        if (pausarJuego)
            Time.timeScale = 0f;

        if (mostrarCursor)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}