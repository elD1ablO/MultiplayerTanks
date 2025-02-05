using System.Threading.Tasks;
using UnityEngine;

public class HostSingleton : MonoBehaviour
{
    private static HostSingleton instance;
    
    private HostGameManager gameManager;
    public static HostSingleton Instance 
    { 
        get 
        {
            if (instance != null) { return instance; }

            instance = FindAnyObjectByType<HostSingleton>();
            if (instance == null)
            {
                Debug.LogError("NO HostSingleton in the scene!");
                return null;
            }
            return instance;
        } 
    }
    
    void Start()
    {
        DontDestroyOnLoad(gameObject);        
    }

    public void CreateHost()
    {
        gameManager = new HostGameManager();
    }


}
