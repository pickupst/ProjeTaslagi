using UnityEngine;

namespace _CodeBase.DemoFramework
{
    
    //Demo yolun oluşması için şart koşulan fonksiyonların bulunduğu interface
    
    public interface IDemo
    {

        void Awake();
        void Start();
        void Update();
        void OnGUI();
        void OnApplicationQuit();

    }
}