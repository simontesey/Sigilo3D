using UnityEngine;
using TMPro; 

[RequireComponent(typeof(Collider))]
public class PuertaSalida : MonoBehaviour
{
    
    public string tagJugador = "Player";

    
    public GameObject panelVictoria; 
    public TextMeshProUGUI textoVictoria; // opcional: si el panel tiene un texto, se lo puede sobreescribir
    public string mensaje = "¡Escapaste!";
    public bool pausarJuego = true; 
    public bool mostrarCursor = true; 

    [Header("Opcional")]
    public GameObject efectoAlSalir; 

    private bool yaActivada = false;

    void Reset()
    {
        
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