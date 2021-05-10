using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public int id;
    public string username;
    public float health;
    public float maxHealth;
    public int kills;

    // private bool hasShot = false;

    public SpriteRenderer model;

    public GameObject HealthPrefab;
    public Image BarBorder;
    public Image BarFilled;

    private float currentValue;

    public GameObject Player;
    public GameObject Wisp;

    public static Dictionary<int, WillOWisp> wisps = new Dictionary<int, WillOWisp>();

    void Start()
    {
        BarBorder = Instantiate(HealthPrefab, FindObjectOfType<Canvas>().transform).GetComponent<Image>();
        BarFilled = new List<Image>(BarBorder.GetComponentsInChildren<Image>()).Find(img => img != BarBorder);
    }

    public void Initialize(int _id, string _username, GameObject _localPrefab, GameObject _foreignPrefab)
    {
        id = _id;
        username = _username;
        health = maxHealth;
        kills = 0;
    }

    void Update()
    {
        HandleHealthBar();
    }

    public int getId()
    {
        return id;
    }

    public void SetHealth(float _health)
    {
        health = _health;

        if (health <= 0f)
        {
            Die();
        }
    }
    
    public void SetKills(int _kills)
    {
        kills = _kills;
        SpawnWisp();
    }

    public void SpawnWisp()
    {
        GameObject skull;
        skull = Instantiate(Wisp, transform.position, Quaternion.identity);
        skull.GetComponent<WillOWisp>().Initialize(id, transform);
        wisps.Add(id, skull.GetComponent<WillOWisp>());
    }

    private float HealthMap(float currentHealth, float healthMin, float healthMax, float barMin, float barMax)
    {
        return (currentHealth - healthMin) * (barMax - barMin) / (healthMax - healthMin) + barMin;
    }

    private void HandleHealthBar()
    {
        currentValue = HealthMap(health, 0, maxHealth, 0, 1);

        BarFilled.fillAmount = Mathf.Lerp(BarFilled.fillAmount, currentValue, Time.deltaTime);
    }

    public void playerShot()
    {
        if (Player.GetComponent<PlayerAnimator>())
        {
            Player.GetComponent<PlayerAnimator>().isShooting(id, true);
        }
        else if (Player.GetComponent<ForeignAnimator>())
        {
            Player.GetComponent<ForeignAnimator>().isShooting(id, true);
        }
    }

    public void Die()
    {
        model.enabled = false;
    }

    public void Respawn()
    {
        model.enabled = true;
        SetHealth(maxHealth);
    }
}