
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private CollisionCheck _checkDown;
    [SerializeField] private CollisionCheck _checkUp;
    [SerializeField] private CollisionCheck _checkLeft;
    [SerializeField] private CollisionCheck _checkRight;

    [SerializeField] private Rigidbody2D _rigidbody;

    [SerializeField] private float _acceleration = 10f;
    [SerializeField] private float _maxSpeed = 10f;

    private Vector2 _gravityDirection = Vector2.down;
    private float _currSpeed = 0;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            _gravityDirection = Vector2.up;
            _rigidbody.velocity = new Vector2(0f, _rigidbody.velocity.y);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            _gravityDirection = Vector2.down;
            _rigidbody.velocity = new Vector2(0f, _rigidbody.velocity.y);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            _gravityDirection = Vector2.left;
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.y, 0f);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            _gravityDirection = Vector2.right;
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.y, 0f);
        }

        _currSpeed = Mathf.MoveTowards(_currSpeed, _maxSpeed, _acceleration * Time.fixedDeltaTime);
        _rigidbody.velocity = _currSpeed * _gravityDirection;
    }
}