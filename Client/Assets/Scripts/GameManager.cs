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
    public GameObject bonesForeignPrefab;
    public GameObject goopForeignPrefab;
    public GameObject robotForeignPrefab;
    public GameObject wraithForeignPrefab;
    private GameObject _localPrefab;
    private GameObject _foreignPrefab;

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
                _localPrefab = bonesLocalPrefab;
                _player = Instantiate(_localPrefab, _position, _rotation);
            }
            else if (players.Count == 1)
            {
                _localPrefab = goopLocalPrefab;
                _player = Instantiate(_localPrefab, _position, _rotation);
            }
            else if (players.Count == 2)
            {
                _localPrefab = robotLocalPrefab;
                _player = Instantiate(_localPrefab, _position, _rotation);
            }
            else
            {
                _localPrefab = wraithLocalPrefab;
                _player = Instantiate(_localPrefab, _position, _rotation);
            }
        }
        else
        {
            if (players.Count == 0)
            {
                _foreignPrefab = bonesForeignPrefab;
                _player = Instantiate(bonesForeignPrefab, _position, _rotation);
            }
            else if (players.Count == 1)
            {
                _foreignPrefab = goopForeignPrefab;
                _player = Instantiate(_foreignPrefab, _position, _rotation);
            }
            else if (players.Count == 2)
            {
                _foreignPrefab = robotForeignPrefab;
                _player = Instantiate(_foreignPrefab, _position, _rotation);
            }
            else
            {
                _foreignPrefab = wraithForeignPrefab;
                _player = Instantiate(_foreignPrefab, _position, _rotation);
            }
        }

        _player.GetComponent<PlayerManager>().Initialize(_id, _username, _localPrefab, _foreignPrefab);
        players.Add(_id, _player.GetComponent<PlayerManager>());
    }
}