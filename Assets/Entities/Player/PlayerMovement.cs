using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private float _acceleration = 10f;
    [SerializeField] private float _maxSpeed = 10f;

    public Vector2 GravityDirection = Vector2.down;
    public bool IsKnockedBack = false;

    private float _currSpeed = 0;

    private float _knockbackForce = 100;
    private float _knockbackTimer = 0;
    private float _knockbackTotalTime = 5;

    public void Stop()
    {
        _currSpeed = 0f;
        _rigidbody.velocity = Vector2.zero;
    }

    public void KnockBack()
    {
        IsKnockedBack = true;
        _knockbackTimer = _knockbackTotalTime;
    }

    public void FallingState()
    {
        _currSpeed = Mathf.MoveTowards(_currSpeed, _maxSpeed, _acceleration * Time.fixedDeltaTime);
        _rigidbody.velocity = _currSpeed * GravityDirection;
    }

    public void EatingState()
    {
        if (_knockbackTimer <= 0)
        {
            IsKnockedBack = false;
            return;
        }
        _knockbackTimer -= Time.deltaTime;
        _rigidbody.velocity = -GravityDirection * _knockbackForce;
    }
}
