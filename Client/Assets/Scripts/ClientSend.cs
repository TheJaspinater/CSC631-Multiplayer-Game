using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    private static void SendTCPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.tcp.SendData(_packet);
    }

    private static void SendUDPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.udp.SendData(_packet);
    }

    #region Packets
    public static void WelcomeReceived()
    {
        using (Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            _packet.Write(Client.instance.myId);
            _packet.Write(UIManager.instance.usernameField.text);
            _packet.Write(Client.instance.characterSelection);

            SendTCPData(_packet);
        }
    }

    public static void StartGame()
    {

        /*will handle closing the menus
         */
        using (Packet _packet = new Packet((int)ClientPackets.startGame))
        {
            _packet.Write(UIManager.instance.usernameField.text);

            SendTCPData(_packet);
        }
    }

    public static void PlayerMovement(bool[] _inputs)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerMovement))
        {
            _packet.Write(_inputs.Length);
            foreach (bool _input in _inputs)
            {
                _packet.Write(_input);
            }
            _packet.Write(GameManager.players[Client.instance.myId].transform.rotation);

            SendUDPData(_packet);
        }
    }

    public static void PlayerAttack(bool[] _attInputs)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerAttack))
        {
            _packet.Write(_attInputs.Length);
            foreach (bool _inputs in _attInputs)
            {
                _packet.Write(_inputs);
            }
            _packet.Write(GameManager.players[Client.instance.myId].transform.rotation);

            SendUDPData(_packet);
        }
    }

    public static void PlayerWin(int _playerId)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerWin))
        {
            _packet.Write(_playerId);

            SendUDPData(_packet);
        }
    }
    #endregion
}