using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();
    public static Dictionary<int, ProjectileManager> projectiles = new Dictionary<int, ProjectileManager>();
    public static Dictionary<int, EnemyManager> enemies = new Dictionary<int, EnemyManager>();
    public UIManager UImanager;

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
    public GameObject projectilePrefab;
    public GameObject enemyPrefab;

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
        int _characterSelection = Client.instance.characterSelection;

        int otherCharSelection = UIManager.instance.GameLobby[_id-1].charChoice;
        

        if (_id == Client.instance.myId)
        {
            if(_characterSelection == 0)
            {
                _localPrefab = bonesLocalPrefab;
                _player = Instantiate(_localPrefab, _position, _rotation);
            }
            else if (_characterSelection == 1)
            {
                _localPrefab = robotLocalPrefab;
                _player = Instantiate(_localPrefab, _position, _rotation);
            }
            else if (_characterSelection == 2)
            {
                _localPrefab = goopLocalPrefab;
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
            if (otherCharSelection == 0)
            {
                _foreignPrefab = bonesForeignPrefab;
                _player = Instantiate(_foreignPrefab, _position, _rotation);
            }
            else if (otherCharSelection == 1)
            {
                _foreignPrefab = robotForeignPrefab;
                _player = Instantiate(_foreignPrefab, _position, _rotation);
            }
            else if (otherCharSelection == 2)
            {
                _foreignPrefab = goopForeignPrefab;
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

    public void SpawnProjectile(int _id, Vector3 _position)
    {
        // Debug.Log("Projectile has spawned");
        GameObject _projectile = Instantiate(projectilePrefab, _position, Quaternion.identity);
        _projectile.GetComponent<ProjectileManager>().Initialize(_id);
        projectiles.Add(_id, _projectile.GetComponent<ProjectileManager>());
    }

    public void SpawnEnemy(int _id, Vector3 _position)
    {
        GameObject _enemy = Instantiate(enemyPrefab, _position, Quaternion.identity);
        _enemy.GetComponent<EnemyManager>().Initialize(_id);
        enemies.Add(_id, _enemy.GetComponent<EnemyManager>());
    }
}