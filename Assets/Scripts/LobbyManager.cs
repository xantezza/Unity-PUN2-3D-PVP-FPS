using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button _startButton;
    [SerializeField] private Text _inLobby;

    public void Awake()
    {
        _startButton.interactable = false;
        PhotonNetwork.ConnectUsingSettings();
    }

    public void OnStartButtonPressed()
    {
        PhotonNetwork.JoinRandomOrCreateRoom();
    }

    public override void OnConnectedToMaster()
    {
        _startButton.interactable = true;
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(1);
    }

    private void Update()
    {
        _inLobby.text = PhotonNetwork.CountOfPlayers.ToString();
    }
}