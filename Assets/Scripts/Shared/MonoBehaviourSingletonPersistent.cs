using UnityEngine;

public class MonoBehaviourSingletonPersistent<T> : MonoBehaviourSingleton<T> where T : Component
{
    public static new T Instance { get; private set; }

    public virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
}