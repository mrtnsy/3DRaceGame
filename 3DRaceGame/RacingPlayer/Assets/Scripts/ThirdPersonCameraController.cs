using UnityEngine;
using Photon.Pun; // Necessário para verificar se é a câmera local

public class ThirdPersonCameraController : MonoBehaviour
{
    [Header("Target & Offset")]
    public Transform target; // O carro que a câmera vai seguir
    public Vector3 offset = new Vector3(0f, 2f, -5f); // Posição relativa ao alvo (ex: 2m acima, 5m atrás)
    public float smoothSpeed = 0.125f; // Quão suave a câmera segue o alvo

    [Header("Rotation")]
    public float rotationSpeed = 5f; // Velocidade de rotação da câmera (manual)
    public float mouseSensitivity = 100f; // Sensibilidade para rotação da câmera com o mouse
    private float currentYaw = 0f; // Rotação horizontal atual da câmera
    private float currentPitch = 0f; // Rotação vertical atual da câmera

    // A câmera também pode ter um PhotonView se você quiser sincronizar algo específico dela
    // Mas geralmente não é necessário para a câmera do jogador, pois ela só é local.
    // private PhotonView photonViewCached;

    void Awake()
    {
        // photonViewCached = GetComponent<PhotonView>(); // Descomentar se precisar de PhotonView na câmera
    }

    void Start()
    {
        // Se esta câmera não é para o jogador local, ou se ela está no pool de objetos
        // e não deveria estar ativa no início, desative-a.
        // Para este cenário, a câmera será instanciada SOMENTE para o jogador local,
        // então não precisa de uma checagem IsMine aqui.

        // Opcional: Esconder e travar o cursor do mouse
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate() // Use LateUpdate para garantir que o carro já se moveu
    {
        if (target == null)
        {
            // Tenta encontrar o carro do jogador local se ele ainda não foi atribuído
            // Esta é uma maneira de encontrar o carro local dinamicamente.
            // Requer que o carro local tenha uma Tag, por exemplo, "LocalCar".
            GameObject localCar = GameObject.FindWithTag("LocalCar");
            if (localCar != null)
            {
                target = localCar.transform;
            }
            else
            {
                Debug.LogWarning("ThirdPersonCameraController: Target (LocalCar) not found!");
                return;
            }
        }

        // ---------- Lógica de Rotação da Câmera (Mouse) ----------
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        currentYaw += mouseX; // Rotação horizontal acumulada
        currentPitch -= mouseY; // Rotação vertical acumulada (inverte o eixo Y do mouse)
        currentPitch = Mathf.Clamp(currentPitch, -60f, 80f); // Limita o ângulo de rotação vertical

        Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);

        // ---------- Lógica de Posicionamento da Câmera ----------
        // Calcula a posição desejada da câmera
        // A posição é baseada no alvo, mas rotacionada pela rotação da câmera para aplicar o offset corretamente
        Vector3 desiredPosition = target.position + rotation * offset;

        // Suaviza o movimento da câmera para a posição desejada
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // Faz a câmera olhar para o alvo (carro)
        transform.LookAt(target.position + Vector3.up * 1f); // Ajuste 1f se quiser olhar um pouco acima do centro do carro

        // Opcional: Se o carro gira, você pode querer que a câmera também "siga" a rotação do carro um pouco
        // Ou o carro tem sua própria rotação independente e a câmera orbita.
        // Para "seguindo o carro":
        // transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, Time.deltaTime * rotationSpeed);
        // Mas isso dependerá de como você quer a rotação do carro e da câmera.
        // Com a rotação do mouse controlando a câmera, o LookAt é o suficiente para TPS.
    }
}