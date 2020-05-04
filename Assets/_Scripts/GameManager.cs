using _CodeBase.DemoFramework;
using _CodeBase.Demos;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    //Demo yolun tüm fonksiyonlarını çalıştıracak script
    public static IDemo Demo { get; private set; }

    private void Awake()
    {
        Debug.Log("----Oyun Başladı----");
        
        Demo = new TerrainDemo();
        Demo.Awake();
    }

    void Start()
    {
        Demo.Start();
    }

    void Update()
    {
        Demo.Update();
    }

    private void OnGUI()
    {
        Demo.OnGUI();
    }

    private void OnApplicationQuit()
    {
        Demo.OnApplicationQuit();
    }
}























