using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static Cinemachine.CinemachineBlendDefinition;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Player : MonoBehaviour
{
    //public static Player Instance { get; private set; }
    public PlayerSpawn SpawnPoint { get; set; }

    [SerializeField] private BoxCollider2D _collider;

    [SerializeField] private int _peasantBloodValue = 3;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _acceleration = 150f;
    [SerializeField] private float _maxSpeed = 30f;
    [SerializeField] private float _groundCheckBoxHeight = 0.1f;
    [SerializeField] private float _groundCheckDistance;

    public Vector2 GravityDirection = Vector2.down;

    private float _currSpeed = 0;
    private GameObject _lastHitTarget;

    public Vector2 _snapTargetPosition;
    public bool _isSnapping = false;
    public bool _isEating = false;
    public bool _isHooked = false;
    public bool CanHook = true;
    private float _snapSpeed = 10;

    private bool _isGrounded = false;
    private bool _canMove = true;

    private Vector2 _groundCheckBoxSize;
    private Vector2 _groundCheckOrigin;
    private Vector2 _groundCheckBoxSizeVertical;
    private Vector2 _groundCheckBoxSizeHorizontal;
    private float _groundCheckDistanceHorizontal;
    private float _groundCheckDistanceVertical;

    private void Awake()
    {
        _groundCheckBoxSizeVertical = new Vector2(_collider.bounds.size.x, _collider.bounds.size.y * _groundCheckBoxHeight);
        _groundCheckBoxSizeHorizontal = new Vector2(_collider.bounds.size.y * _groundCheckBoxHeight, _collider.bounds.size.y);

        _groundCheckOrigin = _collider.bounds.center;

        _groundCheckDistanceHorizontal = _collider.bounds.extents.x;
        _groundCheckDistanceVertical = _collider.bounds.extents.y;

        _groundCheckDistance = _groundCheckDistanceVertical;
        _groundCheckBoxSize = _groundCheckBoxSizeVertical;

        _animator.Play("playerIdleAnimation");
    }

    private void Update()
    {
        CheckHealth();

        if (!_canMove) return;

        ProcessInput();
    }
    
    private void FixedUpdate()
    {
        _groundCheckOrigin = _collider.bounds.center;

        if (_isEating)
        {
            EatingState();
            return;
        }
        else if (_isSnapping)
        {
            SnappingState();
            return;
        }
        else if (_isHooked)
        {
            HookedState();
            return;
        }
        else if (!_isGrounded)
        {
            FallingState();
            CheckHit();
            return;
        }
    }

    private void CheckHealth()
    {
        if (HealthManager.Instance.Health <= 0)
        {
            Death();
        }
    }

    private void CheckHit()
    {
        int detectionMask = LayerMask.GetMask("Ground", "Enemies", "Hazards", "Peasants", "Tools");

        RaycastHit2D hit = Physics2D.BoxCast(
            _groundCheckOrigin,
            _groundCheckBoxSize,
            0f,
            GravityDirection,
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
        bool hitSomething = hit.collider != null;

        if (hitSomething && !oldIsGrounded)
        { 
            if (hitLayer == LayerMask.NameToLayer("Ground"))
                HitGround();
            else if (hitLayer == LayerMask.NameToLayer("Enemies"))
                HitEnemy(hit.transform);
            else if (hitLayer == LayerMask.NameToLayer("Peasants"))
                HitPeasant(hit);
            else if (hitLayer == LayerMask.NameToLayer("Hazards"))
                HitHazard(hit.transform);
            else if (hitLayer == LayerMask.NameToLayer("Tools"))
                HitHook(hit);
        }
        else if (_isGrounded && oldIsGrounded)
        {
            _canMove = true;
        }
    }

    private void ChangeGravityDirection(Vector2 newDirection)
    {
        if (newDirection.x == 0 && GravityDirection.x != 0)
        {
            _groundCheckBoxSize = _groundCheckBoxSizeVertical;
            _groundCheckDistance = _groundCheckDistanceVertical;
        }
        else if (newDirection.y == 0 && GravityDirection.y != 0)
        {
            _groundCheckBoxSize = _groundCheckBoxSizeHorizontal;
            _groundCheckDistance = _groundCheckDistanceHorizontal;
        }

        if (_isHooked)
        {
            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
            CanHook = false;
            _isHooked = false;
            _canMove = true;
        }

        GravityDirection = newDirection;
        CheckHit();
    }

    private void ProcessInput()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            _animator.Play("playerFlyTopAnimation");
            ChangeGravityDirection(Vector2.up);
            SoundManager.Instance.PlaySound("gravityChange", transform);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            _animator.Play("playerFlyDownAnimation");
            ChangeGravityDirection(Vector2.down);
            SoundManager.Instance.PlaySound("gravityChange", transform);
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _animator.Play("playerFlyLeftAnimation");
            ChangeGravityDirection(Vector2.left);
            SoundManager.Instance.PlaySound("gravityChange", transform);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            _animator.Play("playerFlyRightAnimation");
            ChangeGravityDirection(Vector2.right);
            SoundManager.Instance.PlaySound("gravityChange", transform);
        }
    }
    
    private void Hurt()
    {
        HealthManager.Instance.Hurt(1);
        if (HealthManager.Instance.Health <= 0)
        {
            Death();
            return;
        }
        
        SpawnPoint.Respawn();
        ChangeGravityDirection( Vector2.down);
        _canMove = true;
        SoundManager.Instance.PlaySound("playerHurt", transform);
        CinemachineShake.Instance.ShakeCamera(5f, 0.1f);
    }
    
    private void HitEnemy(Transform transform)
    {
        Stop();
        _isGrounded = true;
        DraculaManager.Instance.SetHurt();
        Hurt();
    }
    
    private void HitHazard(Transform transform)
    {
        Stop();
        _isGrounded = true;
        DraculaManager.Instance.SetHurt();
        Hurt();
    }

    private void HitGround()
    {
        Stop();
        _isGrounded = true;
        SoundManager.Instance.PlaySound("playerHitGround", transform);
        CinemachineShake.Instance.ShakeCamera(1f, 0.05f);
        if (GravityDirection == Vector2.up)
            _animator.Play("playerIdleTopAnimation");
        else if (GravityDirection == Vector2.down)
            _animator.Play("playerIdleAnimation");
        else if (GravityDirection == Vector2.left)
            _animator.Play("playerIdleLeftAnimation");
        else if (GravityDirection == Vector2.right)
            _animator.Play("playerIdleRightAnimation");

        _canMove = true;
    }

    private void HitPeasant(RaycastHit2D peasant)
    {
        Stop();
        _lastHitTarget = peasant.collider.gameObject;
        HealthManager.Instance.Heal(_peasantBloodValue);
        SoundManager.Instance.PlaySound("playerHealthUp", transform);
        SoundManager.Instance.PlaySound("peasantHit", transform);

        float targetBottomY = peasant.collider.bounds.center.y - peasant.collider.bounds.extents.y;
        float playerBottomOffset = transform.position.y - (_collider.bounds.center.y - _collider.bounds.extents.y);
        float finalY = targetBottomY + playerBottomOffset;
        Vector2 target = new Vector2(peasant.transform.position.x, finalY);

        DraculaManager.Instance.SetEating();

        StartEating(target);
    }

    private void HitHook(RaycastHit2D hook)
    {
        if (!CanHook || _isHooked) return; 
        Stop();

        Vector2 target = hook.collider.bounds.center;

        StartSnapping(target);
    }

    private void Death()
    {
        SoundManager.Instance.PlaySound("playerDeath1", transform);
        SoundManager.Instance.PlaySound("playerDeath2", transform);
        CinemachineShake.Instance.ShakeCamera(10f, 0.2f);
        SpawnPoint.Respawn();
        HealthManager.Instance.Reset();
        GameManager.Instance.Restart();
        _canMove = true;
    }
    
    public void Stop()
    {
        _currSpeed = 0f;
        _rigidbody.velocity = Vector2.zero;
    }

    public void FallingState()
    {
        _currSpeed = Mathf.MoveTowards(_currSpeed, _maxSpeed, _acceleration * Time.fixedDeltaTime);
        _rigidbody.velocity = _currSpeed * GravityDirection;
    }

    public void StartSnapping(Vector2 newPosition)
    {
        _snapTargetPosition = newPosition;
        _rigidbody.bodyType = RigidbodyType2D.Kinematic;
        _isSnapping = true;
    }
    
    public void StartEating(Vector2 newPosition)
    {
        _snapTargetPosition = newPosition;
        _rigidbody.bodyType = RigidbodyType2D.Kinematic;
        _isEating = true;
    }

    public void EatingState()
    {
        transform.position = Vector2.MoveTowards(transform.position, _snapTargetPosition, _snapSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, _snapTargetPosition) < 0.01f)
        {
            transform.position = _snapTargetPosition;
            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
            _isEating = false;
            _canMove = true;
            Destroy(_lastHitTarget);
            ChangeGravityDirection(Vector2.down);
        }
    }
    
    public void SnappingState()
    {
        transform.position = Vector2.MoveTowards(transform.position, _snapTargetPosition, _snapSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, _snapTargetPosition) < 0.01f)
        {
            transform.position = _snapTargetPosition;
            _isSnapping = false;
            _isGrounded = false;
            _isHooked = true;
            _canMove = true;
        }
    }

    public void HookedState()
    {
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = _isGrounded ? Color.green : Color.red;

        // Calculate the end position of the box based on direction and distance
        Vector3 endPosition = _groundCheckOrigin + (GravityDirection * _groundCheckDistance);

        // Draw a wire cube to represent the BoxCast area
        Gizmos.DrawWireCube(endPosition, _groundCheckBoxSize);

        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.red;
        style.fontSize = 30;
        style.fontStyle = FontStyle.Bold;

        Gizmos.DrawWireSphere((Vector2)_collider.transform.position + _collider.offset, 0.1f);

        //Handles.Label(transform.position + Vector3.up, $"Blood: {HealthManager.Instance.Health}", style);
    }
#endif
}