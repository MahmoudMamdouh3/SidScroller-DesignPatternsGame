using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    #region Constants
    private const float jumpDelayDuration = 0.5f;
    #endregion

    public float moveSpeed = 5.0f;
    public float jumpForce = 5.0f;
    public HealthStatus health;

    [SerializeField] private GameObject spearPrefab;

    private Rigidbody2D _rigidBody;
    private GameManager _gameMgr;
    private InputSystem_Actions _inputSystem;
    private Vector2 _moveDir;
    private bool _forward = true;
    private bool _groundCollision = false;
    private bool _jumpPress;
    private float _jumpDelay = 0;
    private bool _attackPress;
    private float _attackPowerTime;
    private bool _berserkPress;
    private float _moveSpeedFactor = 1.0f;
    private float _jumpForceFactor = 1.0f;

    void Awake()
    {
        LoadComponents();
        LoadInputSystem();
    }

    void Start()
    {
        _gameMgr = GameManager.Instance;
        _gameMgr.ListenBerserk(OnBerserk_Start, OnBerserk_End);
    }

    void OnDestroy()
    {
        _gameMgr.UnlistenBerserk(OnBerserk_Start, OnBerserk_End);
    }

    void OnEnable()
    {
        if (_inputSystem != null)
            _inputSystem.Enable();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnDisable()
    {
        if (_inputSystem != null)
            _inputSystem.Disable();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void FixedUpdate()
    {
        float deltaTime = Time.fixedDeltaTime;

        HandleMovement(deltaTime);
        HandleJump(deltaTime);
        HandleAttack(deltaTime);
        HandleBerserk(deltaTime);
    }

    private void LoadComponents()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _jumpDelay = jumpDelayDuration;

        health.Death += OnDeath;
    }

    private void LoadInputSystem()
    {
        _inputSystem = new InputSystem_Actions();

        _inputSystem.Player.Move.performed += OnMove_Complete;
        _inputSystem.Player.Move.canceled += OnMove_Cancel;
        _inputSystem.Player.Jump.performed += (context) => _jumpPress = true;
        _inputSystem.Player.Jump.canceled += (context) => _jumpPress = false;
        _inputSystem.Player.Attack.performed += OnAttackPress_Start;
        _inputSystem.Player.Attack.canceled += OnAttackPress_End;
        _inputSystem.Player.Berserk.performed += (context) => _berserkPress = true;
        _inputSystem.Player.Berserk.canceled += (context) => _berserkPress = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            _groundCollision = true;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            _groundCollision = false;
            _jumpDelay = jumpDelayDuration;

            Debug.Log("Ground exit: " + _jumpDelay);
        }
    }

    private void OnMove_Complete(InputAction.CallbackContext context)
    {
        _moveDir = context.ReadValue<Vector2>();
    }

    private void OnMove_Cancel(InputAction.CallbackContext context)
    {
        _moveDir = Vector2.zero;
    }

    private void OnAttackPress_Start(InputAction.CallbackContext context)
    {
        _attackPowerTime = 0;
        _attackPress = true;
    }

    private void OnAttackPress_End(InputAction.CallbackContext context)
    {
        _attackPress = false;

        float attackPower = _attackPowerTime;
        if (attackPower > 1.5f)
            attackPower = 1.5f;

        Attack_Spear(attackPower);
    }

    public void OnBerserk_Start(float berserkDuration, float damageIncrease, float moveSpeedIncrease, float jumpForceIncrease)
    {
        _moveSpeedFactor += moveSpeedIncrease;
        _jumpForceFactor += jumpForceIncrease;

        GetComponent<SpriteRenderer>().color = Color.red;
    }

    public void OnBerserk_End(float berserkDuration, float damageIncrease, float moveSpeedIncrease, float jumpForceIncrease)
    {
        _moveSpeedFactor -= moveSpeedIncrease;
        _jumpForceFactor -= jumpForceIncrease;

        GetComponent<SpriteRenderer>().color = Color.white;
    }

    private void OnDeath()
    {
        Debug.Log("Player Died!");
    }

    public void OnMonsterKill()
    {
        _gameMgr.TryIncreaseGuage();
    }

    private void HandleMovement(float deltaTime)
    {
        _rigidBody.linearVelocityX = _moveDir.x * moveSpeed * _moveSpeedFactor;

        if (_moveDir == Vector2.zero)
            return;
        
        _forward = _moveDir.x > 0;
        _rigidBody.SetRotation(_forward ? 1 : -1);
    }

    private void HandleJump(float deltaTime)
    {
        _jumpDelay += deltaTime;
        
        if (!_jumpPress
            || _jumpDelay < jumpDelayDuration)
            return;
        
        if (!Jump())
            return;

        _jumpDelay = 0;
    }

    private bool Jump()
    {
        if (!_groundCollision)
            return false;

        _rigidBody.AddForceY(jumpForce * _jumpForceFactor, ForceMode2D.Impulse);
        return true;
    }

    private void HandleAttack(float deltaTime)
    {
        if (!_attackPress)
            return;
        
        _attackPowerTime += deltaTime;
    }

    private void Attack_Spear(float attackPower)
    {
        Vector3 spawnPosition;
        Vector3 spawnRotation;

        if (_forward)
        {
            spawnPosition = transform.position + Spear.spawnPaddingForward;
            spawnRotation = Spear.spawnRotationForward;
        }
        else
        {
            spawnPosition = transform.position + Spear.spawnPaddingBackward;
            spawnRotation = Spear.spawnRotationBackward;
        }

        GameObject spearObject = Instantiate(spearPrefab, spawnPosition, Quaternion.Euler(spawnRotation));
        if (spearObject == null)
            return;
        
        Spear spear = spearObject.GetComponent<Spear>();
        if (spear == null)
            return;

        attackPower *= _moveSpeedFactor;
        
        // Throw power
        spear.powerForce.x += attackPower;
        spear.powerForce.y += attackPower * 0.2f;

        // Throw direction
        spear.powerForce.x *= _forward ? 1 : -1;

        spear.Attack();
    }

    private void HandleBerserk(float deltaTime)
    {
        if (!_berserkPress)
            return;
        
        _gameMgr.ActivateBerserk();
    }
}
