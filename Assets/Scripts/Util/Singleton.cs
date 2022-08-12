namespace Util
{
  public class Singleton<T> : UnityEngine.MonoBehaviour where T : UnityEngine.Component
  {
    private static T _instance;

    public static bool hasInstance { get => Singleton<T>._instance != null; }
    public static T instance
    {
      get
      {
        if (Singleton<T>._instance == null)
        {
          Singleton<T>._instance = UnityEngine.GameObject.FindObjectOfType<T>();
          if (Singleton<T>._instance == null)
          {
            UnityEngine.GameObject newInstance = new UnityEngine.GameObject();
            Singleton<T>._instance = newInstance.AddComponent<T>();

#if UNITY_EDITOR
            newInstance.name = Singleton<T>._instance.GetType().ToString();
#endif
          }
        }

        return Singleton<T>._instance;
      }
    }

    protected virtual void Awake() => Singleton<T>._instance = this as T;
  }
}
