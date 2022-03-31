using UnityEngine;

public class MonoBehaviourSingleton<T> : MonoBehaviour where T : Component
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                var instances = FindObjectsOfType(typeof(T)) as T[];
                if (instances.Length > 0)
                    _instance = instances[0];
                if (instances.Length > 1)
                {
                    Debug.LogError("There is more than one " + typeof(T).Name + " in the scene.");
                }
                if (_instance == null)
                {
                    GameObject newSingletonObject = new GameObject();
                    newSingletonObject.name = typeof(T).Name;
                    _instance = newSingletonObject.AddComponent<T>();
                }
                return _instance;
            }
            return _instance;
        }
    }
}