using UnityEngine;
using UnityEngine.EventSystems; 

public class EventSystemGlobal : MonoBehaviour
{
    public static EventSystemGlobal Instance;

    void Awake()
    {
      
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            
            Destroy(gameObject);
        }
    }
}