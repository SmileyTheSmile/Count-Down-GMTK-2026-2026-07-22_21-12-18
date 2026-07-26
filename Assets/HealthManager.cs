using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public static HealthManager Instance { get; private set; }
    public BloodUI BloodUI;
    public int Health = 20;
    public int DefaultBloodNum = 20;
    public int MaxHealth = 20;

    [SerializeField] private float _hurtInterval = 1.0f;

    private float timer = 0.0f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= _hurtInterval)
        {
            Hurt(1);
            timer = 0f;
        }
    }

    public void Hurt(int num)
    {
        Health -= num;
        BloodUI.UpdateHealth();
    }
    public void Heal(int num)
    {
        Health += num;
        BloodUI.UpdateHealth();
    }
    public void Reset()
    {
        Health = DefaultBloodNum;
        BloodUI.UpdateHealth();
    }
}
