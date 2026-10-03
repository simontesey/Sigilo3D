using UnityEngine;

public class EnemyChase : MonoBehaviour
{
   
    public float speed = 3f;
    public float stopDistance = 1.5f;   
    public bool loseTargetOnExit = true; 
    private Transform target;

    
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