using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

// Script de controle do jogador, adaptado para um jogo 3D.
[RequireComponent(typeof(Rigidbody))] // Ensure a Rigidbody component is present
public class PlayerController : MonoBehaviourPun
{
    public float playerSpeed = 5f;
    public float rotationSpeed = 10f; // Added for smooth 3D rotation
    private Rigidbody rigidB; // Changed to Rigidbody for 3D physics
    private PhotonView photonViewCached; // Cached PhotonView for performance
    public float carSpeed = 10f; // Velocidade do carro
    public float turnSpeed = 100f; // Velocidade de virada

    [Header("Health")]
    public float playerHealthMax = 100f;
    private float playerHealtCurrent;
    public Image playerHealthFill; // Assuming a UI Image for health bar

    [Header("Bullet Settings")]
    public GameObject bulletGoPrefab; // Renamed for clarity and to indicate it's a prefab
    public Transform spawBullet; // Transform for bullet spawn position/rotation

     [Header("Camera Settings")]
    public GameObject thirdPersonCameraPrefab; // Arraste seu ThirdPersonCameraPrefab para aqui
    private ThirdPersonCameraController myLocalCameraController;

    void Awake()
    {
        // Cache the PhotonView component
        photonViewCached = GetComponent<PhotonView>();
        rigidB = GetComponent<Rigidbody>(); // Get the 3D Rigidbody
    }

    void Start()
    {
        HealthManager(playerHealthMax); // Initialize health

        // SOMENTE o jogador que é o dono deste carro deve instanciar e controlar sua câmera
        if (photonViewCached.IsMine)
        {
            // Adicione uma tag para que a câmera possa encontrar este carro localmente
            gameObject.tag = "Player";

            // Instancia a câmera de terceira pessoa
            GameObject cameraGO = Instantiate(thirdPersonCameraPrefab, Vector3.zero, Quaternion.identity);
            myLocalCameraController = cameraGO.GetComponent<ThirdPersonCameraController>();

            if (myLocalCameraController != null)
            {
                myLocalCameraController.target = this.transform; // Define o carro como alvo da câmera
                Debug.Log("Câmera instanciada e configurada para o carro local.");
            }
            else
            {
                Debug.LogError("ThirdPersonCameraPrefab não tem o script ThirdPersonCameraController!");
            }

            // Opcional: Desabilitar o Renderer de carros de outros jogadores (se for primeira pessoa pura)
            // Ou apenas desabilitar a Camera (se for uma câmera interna ao prefab do jogador)
            // Para 3ª pessoa, todos os carros devem ser visíveis.
        }
        else
        {
            // Desabilitar controle e scripts de câmera para jogadores remotos
            // Isso evita que outros clientes tentem controlar sua câmera ou carro.
            // Exemplo: Desabilitar este script se não for o carro local
            // this.enabled = false; // Se você não quiser que outros clientes rodem o Update() deste script
            // Ou desabilitar componentes específicos se eles forem filhos
            // GetComponentInChildren<Camera>().enabled = false;
        }
    }

    void FixedUpdate() // Use FixedUpdate para física (movimento do carro)
    {
        if (photonViewCached.IsMine)
        {
            CarMove();
            // CarTurn() já pode ser controlada pela câmera, ou ser independente
            // Se o carro vira com as setas, implemente aqui:
            // CarTurn();
        }
    }

     void CarMove()
    {
        float verticalInput = Input.GetAxis("Vertical"); // W/S ou Seta Cima/Baixo
        float horizontalInput = Input.GetAxis("Horizontal"); // A/D ou Seta Esquerda/Direita

        // Movimento para frente/trás (relativo à frente do carro)
        Vector3 moveDirection = transform.forward * verticalInput * carSpeed * Time.fixedDeltaTime;
        rigidB.MovePosition(rigidB.position + moveDirection);

        // Rotação (virar o carro)
        float turnAmount = horizontalInput * turnSpeed * Time.fixedDeltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turnAmount, 0f);
        rigidB.MoveRotation(rigidB.rotation * turnRotation);
    }

    void Update()
    {
        if (photonViewCached.IsMine) // Use cached PhotonView
        {
            PlayerMove();
            PlayerTurn();
            Shooting();
        }
    }

    void PlayerMove()
    {
        // Get input for horizontal and vertical movement
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical"); // Changed 'y' to 'z' for forward/backward movement in 3D

        // Calculate movement direction based on input
        Vector3 moveDirection = transform.right * x + transform.forward * z;
        moveDirection.Normalize(); // Normalize to prevent faster diagonal movement

        // Apply movement using Rigidbody.velocity
        rigidB.linearVelocity = new Vector3(moveDirection.x * playerSpeed, rigidB.linearVelocity.y, moveDirection.z * playerSpeed);
    }

    void PlayerTurn()
    {
        // Mouse-based rotation for looking around in a 3D space
        // This is a simple implementation. For more complex camera/player control,
        // consider Cinemachine or dedicated character controllers.

        // Get mouse input for rotation
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y"); // For vertical camera look, if desired

        // Rotate the player horizontally (around Y-axis)
        transform.Rotate(Vector3.up, mouseX * rotationSpeed);

        // If you want to control vertical aim (e.g., a weapon, not the player's body)
        // you might rotate a child camera or weapon object up/down.
        // For a simple FPS-like control, you typically rotate the camera (or a camera parent) vertically.
    }

    void Shooting()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Instantiate the bullet across the network
            PhotonNetwork.Instantiate(bulletGoPrefab.name, spawBullet.position, spawBullet.rotation, 0);
        }
    }

    // PunRPC for synchronized damage across the network
    // [PunRPC]
    public void TakeDamage(float value)
    {
        // if (photonViewCached.IsMine) // Only the owner should apply damage locally after RPC
        // {
        //     photonViewCached.RPC("TakeDamageNetWork", RpcTarget.AllBuffered, value);
        // }

        photonViewCached.RPC("TakeDamageNetWork", RpcTarget.AllBuffered, value);
    }

    [PunRPC]
    void TakeDamageNetWork(float value)
    {
        Debug.Log("Recebendo dano.");
        HealthManager(value);

        if (playerHealtCurrent <= 0)
        {
            Debug.Log("***GAMEOVER****");
            // Optionally, instantiate a ragdoll or play a death animation
             if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(this.gameObject); // Destroi o player object across the network
        }
        }
    }

    void HealthManager(float value)
    {
        playerHealtCurrent += value;
        playerHealtCurrent = Mathf.Clamp(playerHealtCurrent, 0, playerHealthMax); // Clamp health between 0 and max

        if (playerHealthFill != null) // Ensure the UI element exists
        {
            playerHealthFill.fillAmount = playerHealtCurrent / playerHealthMax;
        }
        Debug.Log("Vida atual: " + playerHealtCurrent);
    }

    // Trigger for collision with barriers (3D Collider)
    private void OnTriggerEnter(Collider other) // Changed to Collider for 3D
    {
        if (other.CompareTag("Barriers"))
        {
            Debug.Log("Colisão com Barreira. Reaparecendo...");
            transform.position = new Vector3(0, 1, 0); // Reposition in 3D
             rigidB.linearVelocity = Vector3.zero; // Resetar velocidade
            rigidB.angularVelocity = Vector3.zero; // Resetar rotação
            // If you want to reset velocity, rigidB.velocity = Vector3.zero;
        }
    }
}