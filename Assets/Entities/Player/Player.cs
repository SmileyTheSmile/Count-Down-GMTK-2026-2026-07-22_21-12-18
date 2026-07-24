using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Player : MonoBehaviour
{
    [SerializeField] private CollisionCheck _checkDown;
    [SerializeField] private CollisionCheck _checkUp;
    [SerializeField] private CollisionCheck _checkLeft;
    [SerializeField] private CollisionCheck _checkRight;

    [SerializeField] private Rigidbody2D _rigidbody;

    [SerializeField] private float _acceleration = 10f;
    [SerializeField] private float _maxSpeed = 10f;

    [SerializeField] private int _bloodNum = 5;

    private Vector2 _gravityDirection = Vector2.down;
    private float _currSpeed = 0;
    private bool _isGrounded = false;
    private bool _canMove = true;
    private bool _isKnockedBack = false;

    #region Unity Functions

    private void Update()
    {
        ProcessInput();
    }

    private void FixedUpdate()
    {
        ProcessMovement();
        Debug.Log($"Is Grounded: {_isGrounded}");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (Vector2.Dot(contact.normal, _gravityDirection) >= -0.5f)
                continue;

            switch (collision.gameObject.layer)
            {
                case int layerValue when layerValue == LayerMask.NameToLayer("Enemies"):
                    TouchedEnemy(collision);
                    break;
                case int layerValue when layerValue == LayerMask.NameToLayer("Ground"):
                    Debug.Log("Player hit the ground");
                    SoundManager.Instance.PlaySound("playerHitGround", transform);
                    Stop();
                    _isGrounded = true;
                    break;
                default:
                    Debug.Log("Player hit the unknown");
                    Stop();
                    break;
            }
        }
    }

    #endregion

    #region Custom Functions

    private void ChangeGravityDirection(Vector2 newDirection)
    {
        _gravityDirection = newDirection;
        _isGrounded = false;
        SoundManager.Instance.PlaySound("gravityChange", transform);
    }

    private void ProcessInput()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            ChangeGravityDirection(Vector2.up);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            ChangeGravityDirection(Vector2.down);
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChangeGravityDirection(Vector2.left);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChangeGravityDirection(Vector2.right);
        }
    }

    private void ProcessMovement()
    {
        if (_isGrounded) return;

        _currSpeed = Mathf.MoveTowards(_currSpeed, _maxSpeed, _acceleration * Time.fixedDeltaTime);
        _rigidbody.velocity = _currSpeed * _gravityDirection;
    }
    
    private void Hurt()
    {
        _bloodNum -= 1;
        SoundManager.Instance.PlaySound("playerHurt", transform);
        CinemachineShake.Instance.ShakeCamera(5f, 0.1f);
    }
    private void TouchedEnemy(Collision2D collision)
    {
        Debug.Log("Player hit the enemy");
        Stop();
        Hurt();
    }

    private void Stop()
    {
        _currSpeed = 0f;
        _rigidbody.velocity = Vector2.zero;
    }

    #endregion
}