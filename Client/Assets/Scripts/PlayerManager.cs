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
    public SpriteRenderer model;

    public GameObject HealthPrefab;
    public Image BarBorder;
    public Image BarFilled;

    public float currentValue;

    // public float currentHealth;

    void Start()
    {
        BarBorder = Instantiate(HealthPrefab, FindObjectOfType<Canvas>().transform).GetComponent<Image>();
        BarFilled = new List<Image>(BarBorder.GetComponentsInChildren<Image>()).Find(img => img != BarBorder);
    }

    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;
        health = maxHealth;
    }

    void Update()
    {
        HandleHealthBar();
    }

    public void SetHealth(float _health)
    {
        health = _health;

        if (health <= 0f)
        {
            Die();
        }
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