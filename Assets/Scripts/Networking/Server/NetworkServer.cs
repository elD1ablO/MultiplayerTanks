using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkServer : IDisposable
{
    private Dictionary<ulong, string> clientIdToAuth = new();
    private Dictionary<string, UserData> authIdToUserData = new();
    private NetworkManager _networkManager;
    public NetworkServer(NetworkManager networkManager)
    {
        _networkManager = networkManager;

        _networkManager.ConnectionApprovalCallback += ApprovalCheck;
        _networkManager.OnServerStarted += OnNetworkReady;
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        string payload = System.Text.Encoding.UTF8.GetString(request.Payload);
        UserData userData = JsonUtility.FromJson<UserData>(payload);

        clientIdToAuth[request.ClientNetworkId] = userData.userAuthID;
        authIdToUserData[userData.userAuthID] = userData;

        response.Approved = true;
        response.CreatePlayerObject = true;
    }

    private void OnNetworkReady()
    {
        _networkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void OnClientDisconnect(ulong clientID)
    {
        if(clientIdToAuth.TryGetValue(clientID, out string authId))
        {
            clientIdToAuth.Remove(clientID);
            authIdToUserData.Remove(authId);            
        }
    }

    public void Dispose()
    {
        if (_networkManager == null) return;

        _networkManager.OnClientDisconnectCallback -= OnClientDisconnect;
        _networkManager.ConnectionApprovalCallback -= ApprovalCheck;
        _networkManager.OnServerStarted -= OnNetworkReady;

        if (_networkManager.IsListening)
        {
            _networkManager.Shutdown();
        }
    }
}
