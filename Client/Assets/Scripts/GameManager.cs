using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();

    public GameObject bonesLocalPrefab;
    public GameObject goopLocalPrefab;
    public GameObject robotLocalPrefab;
    public GameObject wraithLocalPrefab;
    public GameObject playerPrefab;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    public void SpawnPlayer(int _id, string _username, Vector3 _position, Quaternion _rotation)
    {
        GameObject _player;
        if (_id == Client.instance.myId)
        {
            if(players.Count == 0)
            {
                _player = Instantiate(bonesLocalPrefab, _position, _rotation);
            }
            else if (players.Count == 1)
            {
                _player = Instantiate(goopLocalPrefab, _position, _rotation);
            }
            else if (players.Count == 2)
            {
                _player = Instantiate(robotLocalPrefab, _position, _rotation);
            }
            else
            {
                _player = Instantiate(wraithLocalPrefab, _position, _rotation);
            }
        }
        else
        {
            _player = Instantiate(playerPrefab, _position, _rotation);
        }

        _player.GetComponent<PlayerManager>().id = _id;
        _player.GetComponent<PlayerManager>().username = _username;
        players.Add(_id, _player.GetComponent<PlayerManager>());
    }
}