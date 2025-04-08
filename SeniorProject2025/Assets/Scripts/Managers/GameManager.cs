using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool IsAiming { get; private set; }
    public bool IsLockedOnTarget { get; private set; }

    public static event Action OnStartAiming;
    public static event Action OnStopAiming;
    public static event Action OnShoot;

    public static event Action OnStartLockTarget;
    public static event Action OnStopLockTarget;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void SetAiming(bool value)
    {
        IsAiming = value;

        if (value)
        {
            OnStartAiming?.Invoke();
        }
        else
        { 
            OnStopAiming?.Invoke();
        }
    } 
    public void SetPlayerLock(bool value)
    {
        IsLockedOnTarget = value;

        if (value)
        {
            OnStartLockTarget?.Invoke();
        }
        else
        { 
            OnStopLockTarget?.Invoke();
        }
    }

    public void TriggerShoot()
    {
        OnShoot?.Invoke();
    }
}
