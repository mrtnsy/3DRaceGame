using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
//mensagem de entrada e saida do player, este script é responsavel por mostrar a mensagem de entrada e saida do player na tela. e também caso ele perca o jogo
public class Message : MonoBehaviourPunCallbacks
{
    [Header("Componentes de Menssagens")]
    public TMP_Text textMsg;
    public float timeForHide = 10f;

    [Space]
    [Header("Menssagens na Tela")]
    public string msgEnter = "Entrou na Sala";
    public string msgGameOuver = "Saiu da Sala";

    private void Awake()
    {
        textMsg.gameObject.SetActive(false);        
    }

    private void Start()
    {
        textMsg.text = (PhotonNetwork.NickName);
    }

    void MostrarMsg(string msg)
    {
        textMsg.text = msg;
        textMsg.gameObject.SetActive(true);

        StartCoroutine(WaitForHide(timeForHide));
    }

    IEnumerator WaitForHide(float waitForTime)
    {
        yield return new WaitForSeconds(waitForTime);
        textMsg.gameObject.SetActive(false);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        MostrarMsg(newPlayer.NickName + msgEnter); // "Nome do Player Entrou na sala".
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        MostrarMsg(otherPlayer.NickName + msgGameOuver); // "nome Player Saiu da Sala".
    }
}