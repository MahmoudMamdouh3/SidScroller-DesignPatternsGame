using System;
using UnityEngine;

[Serializable]
public class Berserk
{
    public delegate void BerserkEvent(float berserkDuration, float damageIncrease, float moveSpeedIncrease, float jumpForceIncrease);

    #region Constants
    public const float berserkDuration = 30.0f;
    public const float damageIncrease = 1.0f;
    public const float moveSpeedIncrease = 1.0f;
    public const float jumpForceIncrease = 0.5f;
    public byte increaseLuckRate { get; set; } = 10;
    #endregion

    private bool isActive = false;
    private float berserkTime;

    public byte guageMax = 5;
    public bool IsActive { get { return isActive; } }
    public byte Guage { get; set; }

    public event BerserkEvent BerserkStart;
    public event BerserkEvent BerserkEnd;

    public Berserk()
    {
        
    }

    public void Update(float deltaTime)
    {
        if (!isActive)
            return;
        
        berserkTime -= deltaTime;
        
        if (berserkTime <= 0)
            OnBerserkEnd();
    }

    public void IncreaseGuage(byte value)
    {
        Guage += value;

        if (Guage > guageMax)
            Guage = guageMax;
    }

    public bool TryIncreaseGuage()
    {
        if (isActive || Guage >= guageMax)
            return false;

        byte num = (byte)UnityEngine.Random.Range(1, 101);
        if (num > increaseLuckRate)
            return false;

        IncreaseGuage(1);
        return true;
    }

    public bool Activate()
    {
        if (isActive || Guage < guageMax)
            return false;

        OnBerserkStart();
        return true;
    }

    private void OnBerserkStart()
    {
        berserkTime = berserkDuration;
        isActive = true;

        BerserkStart?.Invoke(berserkDuration, damageIncrease, moveSpeedIncrease, jumpForceIncrease);
    }

    private void OnBerserkEnd()
    {
        Guage = 0;
        isActive = false;

        BerserkEnd?.Invoke(berserkDuration, damageIncrease, moveSpeedIncrease, jumpForceIncrease);
    }
}
