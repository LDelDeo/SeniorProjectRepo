using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool IsAiming { get; private set; }

    public static event Action OnStartAiming;
    public static event Action OnStopAiming;
    public static event Action OnShoot;

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

    public void TriggerShoot()
    {
        OnShoot?.Invoke();
    }
}
