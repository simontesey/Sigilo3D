using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemigoDeteccion : MonoBehaviour
{
    private enum EstadoEnemigo { Patrullando, Persiguiendo }

    
    public PlayerMovementSigilo jugador; 
    public EnemigoPatrulla scriptPatrulla; 

    
    public float distanciaMaximaVision = 15f;
    public float anguloVision = 90f;
    public Transform ojosEnemigo; 

    
    public float velocidadAumentoSospecha = 1f; 
    public float velocidadBajarSospecha = 0.5f;
    [Range(0f, 1f)] public float nivelSospecha = 0f; 

    
    public float velocidadPersecucion = 5f;
    public float velocidadRotacionPersecucion = 8f;
    public float distanciaAtrapar = 1.2f; 
    public float tiempoParaPerderJugador = 4f; 

   
    public bool mostrarConoVision = true;
    public Color colorPatrullando = new Color(1f, 1f, 0f, 0.25f); 
    public Color colorPersiguiendo = new Color(1f, 0f, 0f, 0.35f); 
    public int segmentosCono = 24;

    private EstadoEnemigo estado = EstadoEnemigo.Patrullando;
    private float tiempoSinVerJugador = 0f;
    private MeshRenderer rendererCono;
    private Material materialCono;

    void Start()
    {
        if (scriptPatrulla == null)
            scriptPatrulla = GetComponent<EnemigoPatrulla>();

        if (mostrarConoVision)
            GenerarConoVision();
    }

    void Update()
    {
        if (jugador == null) return;

        Vector3 posicionOjos = ojosEnemigo != null ? ojosEnemigo.position : transform.position;
        bool puedeVerAlJugador = PuedeVerAlJugador(posicionOjos);

        
        if (puedeVerAlJugador)
            nivelSospecha += velocidadAumentoSospecha * jugador.VisibilidadActual * Time.deltaTime;
        else
            nivelSospecha -= velocidadBajarSospecha * Time.deltaTime;

        nivelSospecha = Mathf.Clamp01(nivelSospecha);

        
        if (estado == EstadoEnemigo.Patrullando && nivelSospecha >= 1f)
        {
            EntrarEnPersecucion();
        }

        if (estado == EstadoEnemigo.Persiguiendo)
        {
            PerseguirAlJugador(puedeVerAlJugador);
        }

        ActualizarColorCono();
    }

    bool PuedeVerAlJugador(Vector3 posicionOjos)
    {
        Vector3 haciaJugador = jugador.transform.position - posicionOjos;

        if (haciaJugador.magnitude > distanciaMaximaVision) return false;

        float anguloHaciaJugador = Vector3.Angle(transform.forward, haciaJugador);
        if (anguloHaciaJugador > anguloVision * 0.5f) return false;

        return jugador.EsVisibleDesde(posicionOjos);
    }

    void EntrarEnPersecucion()
    {
        estado = EstadoEnemigo.Persiguiendo;
        tiempoSinVerJugador = 0f;

        if (scriptPatrulla != null)
            scriptPatrulla.enabled = false; 
    }

    void VolverAPatrullar()
    {
        estado = EstadoEnemigo.Patrullando;
        nivelSospecha = 0f;
        tiempoSinVerJugador = 0f;

        if (scriptPatrulla != null)
            scriptPatrulla.enabled = true;
    }

    void PerseguirAlJugador(bool puedeVerAlJugador)
    {
        Vector3 destino = new Vector3(jugador.transform.position.x, transform.position.y, jugador.transform.position.z);

        transform.position = Vector3.MoveTowards(transform.position, destino, velocidadPersecucion * Time.deltaTime);

        Vector3 direccion = destino - transform.position;
        if (direccion.sqrMagnitude > 0.01f)
        {
            Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, velocidadRotacionPersecucion * Time.deltaTime);
        }

        float distanciaAlJugador = Vector3.Distance(
            new Vector3(transform.position.x, 0f, transform.position.z),
            new Vector3(jugador.transform.position.x, 0f, jugador.transform.position.z));

        if (distanciaAlJugador <= distanciaAtrapar)
        {
            ReiniciarNivel();
            return;
        }

        if (puedeVerAlJugador)
        {
            tiempoSinVerJugador = 0f;
        }
        else
        {
            tiempoSinVerJugador += Time.deltaTime;
            if (tiempoSinVerJugador >= tiempoParaPerderJugador)
            {
                VolverAPatrullar();
            }
        }
    }

    void ReiniciarNivel()
    {
        Scene escenaActual = SceneManager.GetActiveScene();
        SceneManager.LoadScene(escenaActual.buildIndex);
    }



    void GenerarConoVision()
    {
        GameObject objetoCono = new GameObject("ConoVision");
        objetoCono.transform.SetParent(transform, false);
        objetoCono.transform.localPosition = new Vector3(0f, 0.05f, 0f); 
        objetoCono.transform.localRotation = Quaternion.identity;

        MeshFilter filtro = objetoCono.AddComponent<MeshFilter>();
        rendererCono = objetoCono.AddComponent<MeshRenderer>();

        filtro.mesh = ConstruirMeshCono();

        Shader shader = Shader.Find("Sprites/Default");
        if (shader == null) shader = Shader.Find("Unlit/Color"); 

        materialCono = new Material(shader);
        materialCono.color = colorPatrullando;
        rendererCono.material = materialCono;
        rendererCono.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        rendererCono.receiveShadows = false;
    }

    Mesh ConstruirMeshCono()
    {
        Mesh mesh = new Mesh();
        mesh.name = "MeshConoVision";

        int cantidadVertices = segmentosCono + 2;
        Vector3[] vertices = new Vector3[cantidadVertices];
        int[] triangulos = new int[segmentosCono * 3];

        vertices[0] = Vector3.zero; 

        float anguloInicial = -anguloVision * 0.5f;
        float pasoAngulo = anguloVision / segmentosCono;

        for (int i = 0; i <= segmentosCono; i++)
        {
            float anguloActual = anguloInicial + pasoAngulo * i;
            Vector3 direccion = Quaternion.Euler(0f, anguloActual, 0f) * Vector3.forward;
            vertices[i + 1] = direccion * distanciaMaximaVision;
        }

        for (int i = 0; i < segmentosCono; i++)
        {
            int indiceBase = i * 3;
            triangulos[indiceBase] = 0;
            triangulos[indiceBase + 1] = i + 1;
            triangulos[indiceBase + 2] = i + 2;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangulos;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }

    void ActualizarColorCono()
    {
        if (materialCono == null) return;
        materialCono.color = estado == EstadoEnemigo.Persiguiendo ? colorPersiguiendo : colorPatrullando;
    }
}
