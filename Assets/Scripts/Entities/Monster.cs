using UnityEngine;

public class Monster : MonoBehaviour
{
    #region Constants
    private float hitDelay = 2.0f;
    private float speedReachTime = 1.0f;
    #endregion

    [SerializeField] private GameObject followTarget;
    public int damage = 20;
    [SerializeField] private float moveSpeed = 4.0f;
    public HealthStatus health;

    private Rigidbody2D _rigidBody;
    private GameManager _gameMgr;
    private bool _playerCollision = false;
    private float _hitDelayTime;

    void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _gameMgr = GameManager.Instance;
    }

    void FixedUpdate()
    {
        float deltaTime = Time.fixedDeltaTime;

        Hit(deltaTime);
        Follow(deltaTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            _playerCollision = true;
            _hitDelayTime = hitDelay;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
            _playerCollision = false;
    }

    private void Hit(float deltaTime)
    {
        if (!_playerCollision)
            return;
        
        _hitDelayTime += deltaTime;
        if (_hitDelayTime < hitDelay)
            return;

        _hitDelayTime = 0;

        _gameMgr.UpdatePlayerHealth(damage);
    }

    private void Follow(float deltaTime)
    {
        if (followTarget == null || _rigidBody == null)
            return;
        
        bool forward = followTarget.transform.position.x >= transform.position.x;

        if (
            (forward && _rigidBody.linearVelocityX >= moveSpeed)
            || (!forward && _rigidBody.linearVelocityX <= -moveSpeed)
            )
            return;
        
        _rigidBody.linearVelocityX += (forward ? moveSpeed : -moveSpeed) * (deltaTime / speedReachTime);
    }
}
