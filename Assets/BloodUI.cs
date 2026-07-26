using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class BloodUI : MonoBehaviour
{
    [SerializeField] private GameObject bloodMask;
    [SerializeField] private Transform _noHealthTarget;
    [SerializeField] private Transform _maxHealthTarget;
    [SerializeField] private TMP_Text text;

    private void Start()
    {
        UpdateHealth();
    }

    public void UpdateHealth()
    {
        text.text = $"{HealthManager.Instance.Health:D3}";

        float healthPercent = (float)HealthManager.Instance.Health / HealthManager.Instance.MaxHealth;

        healthPercent = Mathf.Clamp01(healthPercent);
        Vector2 newPosition = Vector2.Lerp(_noHealthTarget.position, _maxHealthTarget.position, healthPercent);

        bloodMask.transform.position = newPosition;
    }
}
