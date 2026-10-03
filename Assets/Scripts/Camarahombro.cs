using UnityEngine;

public class CameraHombro : MonoBehaviour
{
    
    public Transform jugador; 

   
    public Vector3 offsetHombro = new Vector3(0.6f, 1.6f, -2.8f); 

    
    public float suavizadoPosicion = 10f; 
    public float suavizadoRotacion = 10f;

    void LateUpdate()
    {
        if (jugador == null) return;

        
        Vector3 posicionObjetivo = jugador.position + jugador.rotation * offsetHombro;

        transform.position = Vector3.Lerp(transform.position, posicionObjetivo, suavizadoPosicion * Time.deltaTime);

        
        transform.rotation = Quaternion.Slerp(transform.rotation, jugador.rotation, suavizadoRotacion * Time.deltaTime);
    }
}