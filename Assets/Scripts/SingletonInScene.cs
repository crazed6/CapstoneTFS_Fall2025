using UnityEngine;

// This class is used to create a singleton that is destroyed when the scene changes.
[DefaultExecutionOrder(-1)]
public abstract class SingletonInScene<T> : MonoBehaviour where T : Component
{
    static T instance;
    public static T Instance
    {
        get
        {
            if (!instance)
                instance = FindAnyObjectByType<T>();

            if (!instance)
            {
                GameObject obj = new GameObject();
                obj.name = typeof(T).Name;
                instance = obj.AddComponent<T>();

                // Explicity do not call DontDestroyOnLoad(obj);
                // In this class, we want to destroy this singleton when the scene changes.
            }

            return instance;
        }
    }

    // Awake = Pre -start
    protected virtual void Awake()
    {
        if (!instance)
        {
            instance = this as T;
            return;
        }

        Destroy(gameObject);
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {

    }

    // Update is called once per frame
    protected virtual void Update()
    {

    }
}

