using UnityEngine;

public class EnemyChase : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 3f;
    public float stopDistance = 1.5f;   // distancia a la que se detiene del jugador
    public bool loseTargetOnExit = true; // si el player sale del trigger, deja de perseguir

    private Transform target;

    // El collider de este objeto (o de un hijo) debe tener "Is Trigger" activado
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            target = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (loseTargetOnExit && other.CompareTag("Player"))
        {
            target = null;
        }
    }

    private void Update()
    {
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);
        if (distance <= stopDistance) return;

        // Mirar al jugador (solo en el eje Y para que no se incline)
        Vector3 lookPos = new Vector3(target.position.x, transform.position.y, target.position.z);
        transform.LookAt(lookPos);

        // Moverse hacia el jugador
        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );
    }
}