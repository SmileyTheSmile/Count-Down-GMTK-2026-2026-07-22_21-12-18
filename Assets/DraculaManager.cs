using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraculaManager : MonoBehaviour
{
    public static DraculaManager Instance { get; private set; }
    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] Sprite _defaultFace;
    [SerializeField] Sprite _eatingFace;
    [SerializeField] Sprite _hurtFace;
    [SerializeField] bool _isHurt = false;
    [SerializeField] bool _isEating = false;

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

        _spriteRenderer.sprite = _defaultFace;
    }

    public void SetDefault()
    {
        _spriteRenderer.sprite = _defaultFace;
    }

    public void SetEating()
    {
        _spriteRenderer.sprite = _eatingFace;
        _isEating = true;
    }

    public void SetHurt()
    {
        _spriteRenderer.sprite = _hurtFace;
        _isHurt = true; 
    }

    void Update()
    {
        if (_isHurt || _isEating)
        {
            timer += Time.deltaTime;

            if (timer >= _hurtInterval)
            {
                _isHurt = false;
                SetDefault();
                timer = 0f;
            }
        }
    }
}
