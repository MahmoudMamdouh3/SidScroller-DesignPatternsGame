using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Constants
    [HideInInspector] public const byte berserkGuageMax = 5;
    #endregion

    private float _damageFactor = 1.0f;

    [SerializeField] private Player _player;
    private Berserk _berserk;
    public byte berserkIncreaseLuckRate = 10;

    public static GameManager Instance;

    void Awake()
    {
        if (Instance != null)
            Destroy(Instance);
        Instance = this;

        LoadBerserk();
    }

    void OnDestroy()
    {
        UnloadBerserk();
    }

    void FixedUpdate()
    {
        _berserk.Update(Time.fixedDeltaTime);
    }

    private void LoadBerserk()
    {
        _berserk = new Berserk();
        _berserk.IncreaseGuage(berserkGuageMax);
        _berserk.increaseLuckRate = berserkIncreaseLuckRate;

        _berserk.BerserkStart += OnBerserk_Start;
        _berserk.BerserkEnd += OnBerserk_End;
    }

    private void UnloadBerserk()
    {
        _berserk.BerserkStart -= OnBerserk_Start;
        _berserk.BerserkEnd -= OnBerserk_End;
    }

    public bool TryIncreaseGuage()
    {
        return _berserk.TryIncreaseGuage();
    }

    public bool ActivateBerserk()
    {
        return _berserk.Activate();
    }

    public void ListenBerserk(Berserk.BerserkEvent startHandler, Berserk.BerserkEvent endHandler)
    {
        _berserk.BerserkStart += startHandler;
        _berserk.BerserkEnd += endHandler;
    }

    public void UnlistenBerserk(Berserk.BerserkEvent startHandler, Berserk.BerserkEvent endHandler)
    {
        _berserk.BerserkStart -= startHandler;
        _berserk.BerserkEnd -= endHandler;
    }

    private void 

    private void OnBerserk_Start(float berserkDuration, float damageIncrease, float moveSpeedIncrease, float jumpForceIncrease)
    {
        _damageFactor += damageIncrease;
    }

    private void OnBerserk_End(float berserkDuration, float damageIncrease, float moveSpeedIncrease, float jumpForceIncrease)
    {
        _damageFactor -= damageIncrease;
    }
}
