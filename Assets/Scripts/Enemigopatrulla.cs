using UnityEngine;

public class EnemigoPatrulla : MonoBehaviour
{
    [Header("Puntos de patrulla")]
    public Transform[] puntosPatrulla; // arrastra aca los GameObjects (vacios) que marcan el recorrido, en orden

    [Header("Movimiento")]
    public float velocidad = 3f;
    public float velocidadRotacion = 5f;
    public float tiempoEsperaEnPunto = 2f; // cuanto se queda quieto en cada punto antes de seguir
    public bool hacerLoop = true; // al llegar al ultimo punto, vuelve al primero. Si es false, va y vuelve (ping-pong)

    private int indiceActual = 0;
    private int direccionPingPong = 1;
    private float tiempoEsperaRestante = 0f;

    void Update()
    {
        if (puntosPatrulla == null || puntosPatrulla.Length == 0) return;

        // Esperando en el punto actual antes de moverse al siguiente
        if (tiempoEsperaRestante > 0f)
        {
            tiempoEsperaRestante -= Time.deltaTime;
            return;
        }

        Transform destino = puntosPatrulla[indiceActual];
        if (destino == null) return;

        // Mover hacia el destino
        Vector3 direccionMovimiento = (destino.position - transform.position);
        direccionMovimiento.y = 0f; // ignora diferencias de altura para no inclinar al enemigo

        transform.position = Vector3.MoveTowards(transform.position, new Vector3(destino.position.x, transform.position.y, destino.position.z), velocidad * Time.deltaTime);

        // Rotar suavemente hacia la direccion de movimiento
        if (direccionMovimiento.sqrMagnitude > 0.01f)
        {
            Quaternion rotacionObjetivo = Quaternion.LookRotation(direccionMovimiento.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, velocidadRotacion * Time.deltaTime);
        }

        // Al llegar al punto, espera y avanza al siguiente
        float distancia = Vector3.Distance(new Vector3(transform.position.x, 0f, transform.position.z), new Vector3(destino.position.x, 0f, destino.position.z));
        if (distancia < 0.1f)
        {
            tiempoEsperaRestante = tiempoEsperaEnPunto;
            AvanzarAlSiguientePunto();
        }
    }

    void AvanzarAlSiguientePunto()
    {
        if (hacerLoop)
        {
            indiceActual = (indiceActual + 1) % puntosPatrulla.Length;
        }
        else
        {
            // Ping-pong: va del primero al ultimo y vuelve, en vez de saltar del ultimo al primero
            if (indiceActual + direccionPingPong >= puntosPatrulla.Length || indiceActual + direccionPingPong < 0)
            {
                direccionPingPong *= -1;
            }
            indiceActual += direccionPingPong;
        }
    }

    void OnDrawGizmosSelected()
    {
        // Dibuja el recorrido en la Scene view para que sea facil de ajustar
        if (puntosPatrulla == null || puntosPatrulla.Length < 2) return;

        Gizmos.color = Color.yellow;
        for (int i = 0; i < puntosPatrulla.Length; i++)
        {
            if (puntosPatrulla[i] == null) continue;
            Gizmos.DrawSphere(puntosPatrulla[i].position, 0.2f);

            Transform siguiente = hacerLoop
                ? puntosPatrulla[(i + 1) % puntosPatrulla.Length]
                : (i + 1 < puntosPatrulla.Length ? puntosPatrulla[i + 1] : null);

            if (siguiente != null)
            {
                Gizmos.DrawLine(puntosPatrulla[i].position, siguiente.position);
            }
        }
    }
}
