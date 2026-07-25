using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Player : MonoBehaviour
{
    [SerializeField] private CollisionCheck _checkDown;
    [SerializeField] private CollisionCheck _checkUp;
    [SerializeField] private CollisionCheck _checkLeft;
    [SerializeField] private CollisionCheck _checkRight;

    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private BoxCollider2D _collider;

    [SerializeField] private float _acceleration = 10f;
    [SerializeField] private float _maxSpeed = 10f;

    [SerializeField] private int _bloodNum = 5;

    private Vector2 _gravityDirection = Vector2.down;
    private float _currSpeed = 0;
    private bool _isGrounded = false;
    private bool _collidingWithGround = false;
    private bool _canMove = true;
    private bool _isKnockedBack = false;

    private Vector2 _groundCheckBoxSize;
    private float _groundCheckDistance = 0.5f;

    #region Unity Functions

    private void Awake()
    {
        _groundCheckBoxSize = new Vector2(_collider.bounds.size.x * 0.9f, _collider.bounds.size.y * 0.1f);
    }

    private void Update()
    {
        ProcessInput();
    }

    private void FixedUpdate()
    {
        CheckIfGrounded();
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
                    _collidingWithGround = true;
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

    private void CheckIfGrounded()
    {
        RaycastHit2D hit = Physics2D.BoxCast(
            transform.position,
            _groundCheckBoxSize,
            0f,
            _gravityDirection,
            _groundCheckDistance,
            LayerMask.GetMask("Ground")
        );

        bool oldIsGrounded = _isGrounded;

        // Returns true if the hit collider is not null
        _isGrounded = hit.collider != null && _collidingWithGround;

        if (_isGrounded && !oldIsGrounded)
            HitGround();
    }

    private void ChangeGravityDirection(Vector2 newDirection)
    {
        if (newDirection.x == 0 && _gravityDirection.x != 0 || newDirection.y == 0 && _gravityDirection.y != 0)
            _groundCheckBoxSize = new Vector2(_groundCheckBoxSize.y, _groundCheckBoxSize.x);

        _gravityDirection = newDirection;
        //_isGrounded = false;
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
        Stop();
        Hurt();
    }

    private void HitGround()
    {
        Stop();
        SoundManager.Instance.PlaySound("playerHitGround", transform);
        CinemachineShake.Instance.ShakeCamera(5f, 0.1f);
    }

    private void Stop()
    {
        _currSpeed = 0f;
        _rigidbody.velocity = Vector2.zero;
    }

    #endregion

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = _isGrounded ? Color.green : Color.red;

        // Calculate the end position of the box based on direction and distance
        Vector3 endPosition = transform.position + (Vector3)(_gravityDirection * _groundCheckDistance);

        // Draw a wire cube to represent the BoxCast area
        Gizmos.DrawWireCube(endPosition, _groundCheckBoxSize);
    }
#endif
}