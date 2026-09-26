using UnityEngine;

public class CameraHombro : MonoBehaviour
{
    [Header("Objetivo")]
    public Transform jugador; // arrastra aca el Transform del personaje (el mismo que rota con A/D)

    [Header("Posicion sobre el hombro")]
    public Vector3 offsetHombro = new Vector3(0.6f, 1.6f, -2.8f); // x = izq/der, y = altura, z = atras (negativo)

    [Header("Suavizado")]
    public float suavizadoPosicion = 10f; // mas alto = camara mas rigida/rapida
    public float suavizadoRotacion = 10f;

    void LateUpdate()
    {
        if (jugador == null) return;

        // Calcula la posicion objetivo en el espacio local del jugador, asi el offset
        // (a la derecha, arriba y atras) se mantiene sin importar hacia donde este rotado.
        Vector3 posicionObjetivo = jugador.position + jugador.rotation * offsetHombro;

        transform.position = Vector3.Lerp(transform.position, posicionObjetivo, suavizadoPosicion * Time.deltaTime);

        // La camara rota junto con el jugador: cuando gira con A/D, la camara gira igual
        // (siempre mirando en la misma direccion en la que mira el personaje).
        transform.rotation = Quaternion.Slerp(transform.rotation, jugador.rotation, suavizadoRotacion * Time.deltaTime);
    }
}