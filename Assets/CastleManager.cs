using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleManager : MonoBehaviour
{
    public static CastleManager Instance { get; private set; }
    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] Sprite _stage1;
    [SerializeField] Sprite _stage2;
    [SerializeField] Sprite _stage3;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;

        _spriteRenderer.sprite = _stage1;
    }

    public void SetStage1()
    {
        _spriteRenderer.sprite = _stage1;
    }

    public void SetStage2()
    {
        _spriteRenderer.sprite = _stage2;
    }

    public void SetStage3()
    {
        _spriteRenderer.sprite = _stage3;
    }
}
