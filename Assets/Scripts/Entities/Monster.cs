using UnityEngine;

public class Monster : MonoBehaviour
{
    #region Constants
    private float hitDelay = 2.0f;
    #endregion

    [SerializeField] private GameObject followTarget;
    public float damage = 20;
    public HealthStatus health;

    private GameManager _gameMgr;
    private bool _playerCollision = false;
    private float _hitDelayTime;

    void Start()
    {
        _gameMgr = GameManager.Instance;
    }

    void FixedUpdate()
    {
        Hit(Time.fixedDeltaTime);
    }

    void Hit(float deltaTime)
    {
        if (!_playerCollision)
            return;
        
        _hitDelayTime += deltaTime;
        if (_hitDelayTime < hitDelay)
            return;

        _hitDelayTime = 0;

        _gameMgr.UpdatePlayerHealth(damage);
    }
}
