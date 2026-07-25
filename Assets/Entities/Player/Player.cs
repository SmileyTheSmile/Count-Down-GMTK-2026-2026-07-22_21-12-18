using UnityEditor;
using UnityEngine;
using static Cinemachine.CinemachineBlendDefinition;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Player : MonoBehaviour
{
    public PlayerSpawn SpawnPoint { get; set; }

    [SerializeField] private BoxCollider2D _collider;

    [SerializeField] private PlayerMovement _movement;

    [SerializeField] private int _bloodNum = 5;

    private bool _isGrounded = false;
    private bool _canMove = true;

    private Vector2 _groundCheckBoxSize;
    private Vector2 _groundCheckOrigin;
    private Vector2 _groundCheckBoxSizeVertical;
    private Vector2 _groundCheckBoxSizeHorizontal;
    private float _groundCheckDistance;
    private float _groundCheckDistanceHorizontal;
    private float _groundCheckDistanceVertical;

    private void Awake()
    {
        _groundCheckBoxSizeVertical = new Vector2(_collider.bounds.size.x, _collider.bounds.size.y * 0.1f);
        _groundCheckBoxSizeHorizontal = new Vector2(_collider.bounds.size.y * 0.1f, _collider.bounds.size.y);

        _groundCheckOrigin = (Vector2)_collider.transform.position + _collider.offset;

        _groundCheckDistanceHorizontal = _collider.bounds.size.x / 2;
        _groundCheckDistanceVertical = _collider.bounds.size.y / 2;

        _groundCheckDistance = _groundCheckDistanceVertical;
        _groundCheckBoxSize = _groundCheckBoxSizeVertical;
    }

    private void Update()
    {
        Debug.Log($"_groundCheckOrigin: {_groundCheckOrigin}, _collider.transform.position: {_collider.transform.position}");
        if (!_canMove) return;

        ProcessInput();
    }

    private void FixedUpdate()
    {
        _groundCheckOrigin = (Vector2)_collider.transform.position + _collider.offset;

        if (!_isGrounded)
        {
            _movement.FallingState();
            CheckHit();
            return;
        }
    }

    private void CheckHit()
    {
        Debug.Log($"IsGrounded: {_isGrounded}, CanMove: {_canMove}");
        int detectionMask = LayerMask.GetMask("Ground", "Enemies");

        RaycastHit2D hit = Physics2D.BoxCast(
            _groundCheckOrigin,
            _groundCheckBoxSize,
            0f,
            _movement.GravityDirection,
            _groundCheckDistance,
            detectionMask
        );

        if (hit.collider == null)
        {
            _isGrounded = false;
            _canMove = false;
            return;
        }

        int hitLayer = hit.collider.gameObject.layer;

        bool oldIsGrounded = _isGrounded;
        _isGrounded = hit.collider != null;

        if (_isGrounded && !oldIsGrounded)
        { 
            if (hitLayer == LayerMask.NameToLayer("Ground"))
                HitGround();
            else if (hitLayer == LayerMask.NameToLayer("Enemies"))
                HitEnemy(hit.transform);
            else if (hitLayer == LayerMask.NameToLayer("Peasants"))
                HitPeasant(hit.transform);
        }
        else if (_isGrounded && oldIsGrounded)
        {
            _canMove = true;
        }
    }

    private void ChangeGravityDirection(Vector2 newDirection)
    {
        if (newDirection.x == 0 && _movement.GravityDirection.x != 0)
        {
            _groundCheckBoxSize = _groundCheckBoxSizeVertical;
            _groundCheckDistance = _groundCheckDistanceVertical;
        }
        else if (newDirection.y == 0 && _movement.GravityDirection.y != 0)
        {
            _groundCheckBoxSize = _groundCheckBoxSizeHorizontal; ;
            _groundCheckDistance = _groundCheckDistanceHorizontal;
        }

        _movement.GravityDirection = newDirection;
        CheckHit();
    }

    private void ProcessInput()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            ChangeGravityDirection(Vector2.up);
            SoundManager.Instance.PlaySound("gravityChange", transform);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            ChangeGravityDirection(Vector2.down);
            SoundManager.Instance.PlaySound("gravityChange", transform);
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChangeGravityDirection(Vector2.left);
            SoundManager.Instance.PlaySound("gravityChange", transform);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChangeGravityDirection(Vector2.right);
            SoundManager.Instance.PlaySound("gravityChange", transform);
        }
    }
    
    private void Hurt()
    {
        _bloodNum -= 1;
        if (_bloodNum <= 0)
        {
            Death();
            return;
        }

        ChangeGravityDirection( Vector2.down);
        _canMove = true;
        SpawnPoint.Respawn();
        SoundManager.Instance.PlaySound("playerHurt", transform);
        CinemachineShake.Instance.ShakeCamera(5f, 0.1f);
    }
    
    private void HitEnemy(Transform transform)
    {
        _movement.Stop();
        Hurt();
    }

    private void HitGround()
    {
        _movement.Stop();
        SoundManager.Instance.PlaySound("playerHitGround", transform);
        CinemachineShake.Instance.ShakeCamera(1f, 0.05f);
        _canMove = true;
    }

    private void HitPeasant(Transform transform)
    {
        _movement.Stop();
        Hurt();
    }

    private void Death()
    {
        Debug.Log("Player died");
        SoundManager.Instance.PlaySound("playerDeath1", transform);
        SoundManager.Instance.PlaySound("playerDeath2", transform);
        CinemachineShake.Instance.ShakeCamera(10f, 0.2f);
        SpawnPoint.Respawn();
        _canMove = true;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = _isGrounded ? Color.green : Color.red;

        // Calculate the end position of the box based on direction and distance
        Vector3 endPosition = _groundCheckOrigin + (_movement.GravityDirection * _groundCheckDistance);

        // Draw a wire cube to represent the BoxCast area
        Gizmos.DrawWireCube(endPosition, _groundCheckBoxSize);

        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.red;
        style.fontSize = 30;
        style.fontStyle = FontStyle.Bold;

        Gizmos.DrawWireSphere((Vector2)_collider.transform.position + _collider.offset, 0.1f);

        Handles.Label(transform.position + Vector3.up, $"Blood: {_bloodNum}", style);
    }
#endif
}