using UnityEngine;

/**<summary> Singleton class which is not MonoBehavior</summary> */
public class SingletonWithouMono<T> where T : new()
{
    protected static T instance;
    private bool initialized = false;

    public static T Instance
    {
        get
        {
            if (instance != null) return instance;
            instance = new T();
            (instance as SingletonWithouMono<T>).Initialize();
            return instance;
        }
    }

    protected virtual void Initialize()
    {
        if (initialized) return;
        initialized = true;
    }

    /**<summary> Call T.Instance.Preload() at the first script startup to pre init. </summary>*/
    public virtual void Preload() { }
}
public abstract class SingletonMono<T> : MonoBehaviour where T : Component
{
    protected static T instance;

    protected virtual void Awake()
    {
        if (instance == null) instance = this as T;
        else if (this != instance)
        {
            Debug.LogWarningFormat("[Singleton] Another instance of" + typeof(T).FullName + "is already exist ");
            DestroyImmediate(gameObject);
            return;
        }
    }
}
//Khởi tạo Instance = Tìm gameobject trên scene
//Nếu không tồn tại trên scene sẽ khởi tạo gameobject mới
public class Singleton<T> : SingletonMono<T> where T : Component
{
    public static T Instance
    {
        get
        {
            if (instance != null) return instance;
            instance = (T)FindObjectOfType(typeof(T));
            if (instance == null)
            {
                Debug.LogWarningFormat("[Singleton] Class {0} not found! Create empty instance", typeof(T));
                instance = new GameObject(typeof(T).Name).AddComponent<T>();
            }
            return instance;
        }
    }
}
//Singleton tồn tại ở tất cả các scene
public class SingletonDontDestroyOnLoad<T> : Singleton<T> where T : Component
{
    protected override void Awake()
    {
        DontDestroyOnLoad(gameObject);
        base.Awake();
    }
}