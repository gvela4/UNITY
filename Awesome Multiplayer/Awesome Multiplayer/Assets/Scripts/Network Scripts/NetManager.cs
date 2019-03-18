using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetManager : NetworkManager
{
    private bool firtPlayerJoined;

    // only works if auto create is checked
    // to spawn players
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        GameObject playerObj = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);

        List<Transform> spawnPositions = NetworkManager.singleton.startPositions;

        if (!firtPlayerJoined)
        {
            firtPlayerJoined = true;
            playerObj.transform.position = spawnPositions[0].position;
        }
        else
        {
            playerObj.transform.position = spawnPositions[1].position;
        }
        NetworkServer.AddPlayerForConnection(conn, playerObj, playerControllerId);
    }

    void SetPortAndAddress()
    {
        NetworkManager.singleton.networkPort = 7777;
        NetworkManager.singleton.networkAddress = "localhost";
    }

    public void HostGame()
    {
        SetPortAndAddress();
        NetManager.singleton.StartHost();
    }

    public void Join()
    {
        SetPortAndAddress();
        NetManager.singleton.StartClient();
    }
}
