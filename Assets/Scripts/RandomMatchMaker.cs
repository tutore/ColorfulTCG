using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using ExitGames.Client.Photon;

public class RandomMatchMaker : Photon.PunBehaviour {

    public PhotonLogLevel Loglevel = PhotonLogLevel.Full;
    public byte MaxPlayersPerRoom = 2;
    [Tooltip("The UI Panel to let the user enter name, connect and play")]
    public GameObject controlPanel;
    [Tooltip("The UI Label to inform the user that the connection is in progress")]
    public GameObject progressLabel;

    string _gameVersion = "0.2";
    bool isConnecting;
    private ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties;

    void Start()
    {
        //PhotonNetwork.ConnectUsingSettings(_gameVersion);
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
    }

    void Awake()
    {
        // 콘솔에 로그 찍기
        PhotonNetwork.logLevel = Loglevel;

        PhotonNetwork.autoJoinLobby = true;

        PhotonNetwork.automaticallySyncScene = true;

    }

    void Update()
    {

    }

    void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }
    
    // ConnectUsingSettings를 가진 함수를 만들어 onClick으로 호출할 수 있게 한다
    public void Connect()
    {
        isConnecting = true;
        progressLabel.SetActive(true);
        controlPanel.SetActive(false);

        if (PhotonNetwork.connected)
        {
            PhotonNetwork.JoinRandomRoom();
            isConnecting = false;
        }
        else
        {
            // 포톤 클라우드에 연결 -> 로비에 진입
            PhotonNetwork.ConnectUsingSettings(_gameVersion);
        }
    }

    // 로비에 들어가는 데 성공 시 호출됨
    public override void OnJoinedLobby()
    {
        Debug.Log("Matchmacker : OnJoinedLobby() was called by PUN");
        PhotonNetwork.JoinRandomRoom();
    }
    // 룸에 들어가는 데 성공 시 호출됨
    public override void OnJoinedRoom()
    {
        Debug.Log("Matchmacker : OnJoinedRoom() was called by PUN");
        Debug.Log("Now, this client is in a room");

        if (PhotonNetwork.room.PlayerCount == 2)
        {
            Debug.Log("Players were matched!");
            Debug.Log("We load the 'Room for 2' ");
            PhotonNetwork.LoadLevel("Room for 2");
        }
    }
    
    // 룸에 다른 플레이어가 들어왔을 때 호출
    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        Debug.Log("Matchmacker : OnPhotonPlayerConnected() was called by PUN");
        Debug.Log("We load the 'Room for 2' ");
        PhotonNetwork.LoadLevel("Room for 2");
    }
    
    // 랜덤 매칭에 들어가는 데 실패 시 호출됨
    void OnPhotonRandomJoinFailed()
    {
        Debug.Log("Matchmacker : OnPhotonRandomJoinFailed() was called by PUN");
        Debug.Log("There is no room, So we create a new room");
        PhotonNetwork.CreateRoom(null);
    }

    public override void OnDisconnectedFromPhoton()
    {
        Debug.Log("Matchmacker : OnDisconnectedFromPhoton() was called by PUN");
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
    }
    
}
