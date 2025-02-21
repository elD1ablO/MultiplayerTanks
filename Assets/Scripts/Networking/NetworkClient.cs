using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkClient : IDisposable
{
    private const string MENUSCENENAME = "MainMenu";
    private NetworkManager _networkManager;
    public NetworkClient(NetworkManager networkManager)
    {
        _networkManager = networkManager;

        _networkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void OnClientDisconnect(ulong clientID)
    {
        if (clientID != 0 && clientID != _networkManager.LocalClientId) return;
        
        if(SceneManager.GetActiveScene().name != MENUSCENENAME)
        {
            SceneManager.LoadScene(MENUSCENENAME);
        }

        if (_networkManager.IsConnectedClient)
        {
            _networkManager.Shutdown();
        }
    }
    public void Dispose()
    {
        if(_networkManager != null) 
            _networkManager.OnClientDisconnectCallback -= OnClientDisconnect;
    }
}
