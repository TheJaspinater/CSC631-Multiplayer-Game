using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerHandle
{
    public static void WelcomeReceived(int _fromClient, Packet _packet)
    {
        int _clientIdCheck = _packet.ReadInt();
        string _username = _packet.ReadString();
        int _charSelect = _packet.ReadInt();

        Debug.Log($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {_fromClient} playing as char {_charSelect}");
        if (_fromClient != _clientIdCheck)
        {
            Debug.Log($"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
        }

        Server.clients[_fromClient].SendIntoLobby(_fromClient,_username, _charSelect); //-MARCO CODE   
    }

    public static void StartGame(int _fromClient, Packet _packet)
    {
        string _username = _packet.ReadString();
        Debug.Log($"Sending player: {_username} with ID:{_fromClient}.");
        Server.clients[_fromClient].SendIntoGame(_username);
    }

    public static void PlayerWin(int _fromClient, Packet _packet)
    {
        int winnerID = _packet.ReadInt();
        Debug.Log($"Player ID:{winnerID} has won!");

        ServerSend.CompleteGameSession(winnerID);

    }

    public static void PlayerMovement(int _fromClient, Packet _packet)
    {
        bool[] _inputs = new bool[_packet.ReadInt()];
        for (int i = 0; i < _inputs.Length; i++)
        {
            _inputs[i] = _packet.ReadBool();
        }
        // Quaternion _rotation = _packet.ReadQuaternion();

        Server.clients[_fromClient].player.SetInput(_inputs);
    }

    public static void PlayerAttack(int _fromClient, Packet _packet)
    {
        bool[] _inputs = new bool[_packet.ReadInt()];
        for (int i = 0; i < _inputs.Length; i++)
        {
            _inputs[i] = _packet.ReadBool();
        }

        Server.clients[_fromClient].player.PlayerAttack(_inputs);
    }
}