using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

// Controller de conexão com o servidor. Não esqueça de colocar o ID do projeto no PhotonServerSettings.
public class NetWorkController : MonoBehaviourPunCallbacks
{
    [Header("UI GameObjects")]
    public GameObject loginGO;
    public GameObject partidasGO;
    public GameObject informactionGO;
    public GameObject menuBackground;
    public GameObject menuTitle;

    [Header("Player Settings")]
    public TMP_InputField playerNameInput;
    private string playerNameTemp; // Changed to private as it's internally managed
    public GameObject myPlayerPrefab; // Changed name to myPlayerPrefab to indicate it's a prefab

    [Header("Room Settings")]
    public TMP_InputField roomName;

    [Header("Information Display")]
    public TMP_Text Info;
    public TMP_Text TextInfo;


    void Start()
    {
        playerNameTemp = "Player" + Random.Range(1000, 10000);
        playerNameInput.text = playerNameTemp;

        roomName.text = "Room" + Random.Range(1000, 10000);

        // Set initial UI states
        loginGO.gameObject.SetActive(true);
        partidasGO.gameObject.SetActive(false);
        informactionGO.gameObject.SetActive(false);
    }

    public void BtLogin()
    {
        if (!string.IsNullOrEmpty(playerNameInput.text)) // Use string.IsNullOrEmpty for better check
        {
            PhotonNetwork.NickName = playerNameInput.text;
            Debug.Log("Usuário logado como: " + PhotonNetwork.NickName);
        }
        else
        {
            PhotonNetwork.NickName = playerNameTemp;
            Debug.Log("Usuário logado como: " + PhotonNetwork.NickName);
        }

        PhotonNetwork.ConnectUsingSettings();

        loginGO.gameObject.SetActive(false);
        // partidasGO will be activated OnConnectedToMaster
    }

    public void BtBuscarPartidaRapida()
    {
        PhotonNetwork.JoinLobby();
    }

    public void BtCriarSala()
    {
        string roomNameTemp = roomName.text;
        RoomOptions roomOptions = new RoomOptions() { MaxPlayers = 20 };
        PhotonNetwork.JoinOrCreateRoom(roomNameTemp, roomOptions, TypedLobby.Default);
    }

    // --- MonoBehaviourPunCallbacks Overrides ---

    public override void OnConnected()
    {
        Debug.Log("OnConnected: Conectado ao servidor.");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster: Conectado ao Master Server.");
        Debug.Log("Meu Servidor: " + PhotonNetwork.CloudRegion + "  /  Ping: " + PhotonNetwork.GetPing());
        partidasGO.gameObject.SetActive(true);
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("OnJoinedLobby: Entrou no lobby. Tentando entrar em uma sala aleatória...");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogWarning($"OnJoinRandomFailed: Código {returnCode}, Mensagem: {message}. Criando nova sala.");
        string roomTemp = "Room" + Random.Range(1000, 10000) + Random.Range(1000, 10000);
        PhotonNetwork.CreateRoom(roomTemp);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom: Entrou na sala " + PhotonNetwork.CurrentRoom.Name);
        Debug.Log("Jogadores na Sala: " + PhotonNetwork.CurrentRoom.PlayerCount);

        Info.text = $"Nome do Jogador: {PhotonNetwork.NickName}\nNome da Sala: {PhotonNetwork.CurrentRoom.Name}";
        informactionGO.SetActive(true);
        partidasGO.SetActive(false);
        menuBackground.SetActive(false);
        menuTitle.SetActive(false);

        // Instantiate the player prefab in 3D space
        // Using Vector3.zero for position and Quaternion.identity for rotation as a starting point.
        // You might want to define specific spawn points in your 3D scene.
        PhotonNetwork.Instantiate(myPlayerPrefab.name, Vector3.zero, Quaternion.identity, 0);
        InfoPlayer();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning("OnDisconnected: " + cause);
        // Optionally, return to the login screen or show a disconnection message
        loginGO.gameObject.SetActive(true);
        partidasGO.gameObject.SetActive(false);
        informactionGO.gameObject.SetActive(false);
    }

    private void InfoPlayer()
    {
        TextInfo.text = ""; // Clear previous info
        Debug.Log("--- Detalhes dos Jogadores na Sala ---");
        foreach (var player in PhotonNetwork.PlayerList)
        {
            Debug.Log($"Nome: {player.NickName}, Master Client: {player.IsMasterClient}, Ativo: {!player.IsInactive}, User ID: {player.UserId}");
            TextInfo.text += $"Jogador: {player.NickName} ({(player.IsMasterClient ? "Mestre" : "Cliente")})\n";
        }
        Debug.Log("-------------------------------------");
    }
}