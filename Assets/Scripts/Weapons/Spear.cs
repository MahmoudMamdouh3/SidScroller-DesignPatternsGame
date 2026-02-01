using System;
using UnityEngine;

public class Spear : MonoBehaviour
{
    #region Constants
    private const float despawnDelay = 2.0f;
    public static readonly Vector3 spawnPaddingForward = new Vector3(1.24591f, 0.676f, 0);
    public static readonly Vector3 spawnPaddingBackward = new Vector3(-1.25609f, 0.676f, 0);
    public static readonly Vector3 spawnRotationForward = new Vector3(0, 0, -75);
    public static readonly Vector3 spawnRotationBackward = new Vector3(0, 0, 75);
    #endregion

    [SerializeField] private WeaponType type;
    public float forcePowerX = 5.0f;
    public float forcePowerY = 5.0f;

    [HideInInspector] public Vector2 powerForce = new Vector2(1, 1);

    private Rigidbody2D _rigidBody;
    private bool _groundCollision;
    private float _despawnDelayTime = 0;

    void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        
    }

    void FixedUpdate()
    {
        Despawn();
    }
    

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            _groundCollision = true;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            _groundCollision = false;
    }

    private void Despawn()
    {
        if (!_groundCollision)
            return;

        _despawnDelayTime += Time.fixedDeltaTime;
        if (_despawnDelayTime < despawnDelay)
            return;

        _despawnDelayTime = 0;
        Destroy(gameObject);
    }

    public void Attack()
    {
        if (_rigidBody == null)
            return;
        
        _rigidBody.AddForce(new Vector2(forcePowerX * powerForce.x, forcePowerY * powerForce.y), ForceMode2D.Impulse);
    }
}
