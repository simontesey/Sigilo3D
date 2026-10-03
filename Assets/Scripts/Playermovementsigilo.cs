using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementSigilo : MonoBehaviour
{
    
    public float velocidadCaminando = 6f;
    public float velocidadAgachado = 2.5f; 
    public float velocidadRotacion = 120f; 

    
    public float fuerzaSalto = 8f;
    public float gravedad = -20f;

    
    public float alturaParado = 2f;
    public float alturaAgachado = 1f;
    public float velocidadTransicionAgacharse = 8f; 

    
    [Range(0f, 1f)] public float visibilidadParado = 1f;     
    [Range(0f, 1f)] public float visibilidadAgachado = 0.35f; 
    public bool estaAgachado { get; private set; }

    
    public string tagCajas = "Cajas";
    public LayerMask capaObstaculos = ~0; 
    public float alturaPuntoVisible = 1.2f; 

    
    public float VisibilidadActual => estaAgachado ? visibilidadAgachado : visibilidadParado;

    private CharacterController controller;
    private Vector3 velocidadVertical;
    private bool estaEnSuelo;
    private float alturaObjetivo;
    private Vector3 centroOriginal;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        
        alturaParado = controller.height;
        centroOriginal = controller.center;
        alturaObjetivo = alturaParado;
    }

    void Update()
    {
        var teclado = Keyboard.current;
        if (teclado == null) return; 

        estaEnSuelo = controller.isGrounded;

        if (estaEnSuelo && velocidadVertical.y < 0f)
            velocidadVertical.y = -2f;

        //Agacharse (C)
        ManejarAgacharse(teclado);

        //Rotacion (A / D) 
        float rotacionInput = 0f;
        if (teclado.aKey.isPressed) rotacionInput -= 1f;
        if (teclado.dKey.isPressed) rotacionInput += 1f;
        transform.Rotate(0f, rotacionInput * velocidadRotacion * Time.deltaTime, 0f);

        //Movimiento (W / S) 
        float movimientoInput = 0f;
        if (teclado.wKey.isPressed) movimientoInput += 1f;
        if (teclado.sKey.isPressed) movimientoInput -= 1f;

        float velocidadActual = estaAgachado ? velocidadAgachado : velocidadCaminando;
        Vector3 direccionMovimiento = transform.forward * movimientoInput;
        controller.Move(direccionMovimiento.normalized * velocidadActual * Time.deltaTime * Mathf.Abs(movimientoInput));

        //Salto 
        if (teclado.spaceKey.wasPressedThisFrame && estaEnSuelo && !estaAgachado)
        {
            velocidadVertical.y = fuerzaSalto;
        }

        //Gravedad
        velocidadVertical.y += gravedad * Time.deltaTime;
        controller.Move(velocidadVertical * Time.deltaTime);
    }

    void ManejarAgacharse(Keyboard teclado)
    {
        
        if (teclado.cKey.wasPressedThisFrame)
        {
            bool quiereLevantarse = estaAgachado;

            if (quiereLevantarse && HayObstaculoArriba())
            {
                
            }
            else
            {
                estaAgachado = !estaAgachado;
                alturaObjetivo = estaAgachado ? alturaAgachado : alturaParado;
            }
        }

        
        controller.height = Mathf.Lerp(controller.height, alturaObjetivo, velocidadTransicionAgacharse * Time.deltaTime);

        
        float diferenciaAltura = alturaParado - controller.height;
        controller.center = centroOriginal - new Vector3(0f, diferenciaAltura / 2f, 0f);
    }

    bool HayObstaculoArriba()
    {
        Vector3 origen = transform.position + controller.center;
        float distanciaChequeo = (alturaParado - controller.height) + 0.1f;
        return Physics.Raycast(origen, Vector3.up, distanciaChequeo);
    }

   
    public bool EsVisibleDesde(Vector3 posicionEnemigo)
    {
        Vector3 puntoDelJugador = transform.position + Vector3.up * alturaPuntoVisible;
        Vector3 direccion = puntoDelJugador - posicionEnemigo;
        float distancia = direccion.magnitude;

        if (Physics.Raycast(posicionEnemigo, direccion.normalized, out RaycastHit hit, distancia, capaObstaculos))
        {
            //Si lo primero que toca el rayo es una caja, el jugador esta tapado y no es visible
            if (hit.collider.CompareTag(tagCajas))
            {
                return false;
            }
        }

        return true;
    }
}