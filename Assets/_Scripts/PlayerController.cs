using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float startVelocity = 20f; //Başlangıç hızı
    public float minVelocity = 10f; //En düşük hızı

    private Rigidbody rb;

    private bool isOnGround = false;

    private float extraVelocityZ;
    private float extraVelocityY;

    private bool firstlyLine = false; //İlk düştüğü yükselti aşağıysa true yukarıysa false
    private int combo;

    private int score = 0;
    
    void Start()
    {

        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.forward * startVelocity;

    }
    private Vector3 localRotation;

    void FixedUpdate()
    {
        /*    Ters takla atmasının önüne geçiyoruz
         *
        */
        
        localRotation = UnityEditor.TransformUtils.GetInspectorRotation(transform);

        localRotation.x = Mathf.Clamp(localRotation.x, -60, 60);
        localRotation.y = 0;
        localRotation.z = 0;
        //Debug.Log(localRotation.x + " acım 2");

        transform.localRotation = Quaternion.Euler(localRotation);
        
        /*    Yokuş mu çıkıyor yokuş mu iniyor?
        *
        */
        if (isOnGround) //Eğer yerdeyse
        {
            if (localRotation.x > 0) //Yookuş iniyor
            {
                extraVelocityZ = Mathf.Abs(Mathf.Sin(localRotation.x));
                extraVelocityY = Mathf.Abs(Mathf.Cos(-localRotation.x) * 10);
                
                //Debug.Log(localRotation.x + " İNİYOR " + " extaZ:" + extraVelocityZ + " extraY:" + extraVelocityY);
                
                rb.velocity = new Vector3(0, rb.velocity.y - extraVelocityY, rb.velocity.z + extraVelocityZ);
                
                //Debug.Log(rb.velocity);
            }
            else if (localRotation.x < 0) //Yookuş çıkıyor
            {
                //Debug.Log(localRotation.x + " ÇIKIYOR ");
            }

        }
       
        
        /*    Sürekli hareketli tutmaya çalışıyoruz.
         *
        */
        
        if (rb.velocity.z < minVelocity) //Belli bir hızın altına asla düşmesin
        {
            rb.velocity = Vector3.forward * minVelocity;
        }
        
        /*    Ekrana dokunulduğunda öne doğru eğilmesini sağlıyoruz
        *
        */
        
        if (Input.anyKey)
        {
            //Debug.Log("BASTIN");
            rb.AddForceAtPosition(new Vector3(0,-500  * Time.deltaTime,0), new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.5f));
            
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y - Time.deltaTime * 10, rb.velocity.z);
        }
       
        
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Road"))
        {
            Debug.Log("----Karakter düştü---");
            
            isOnGround = true;
            
            if (localRotation.x > 0) //Yookuş iniyor
            {
                firstlyLine = true;
                combo++;
            }else if (localRotation.x < 0) //Yookuş çıkıyor
            {
                if (firstlyLine == false) //İlk çarptığı yer yokuş yukarıysa
                {
                    combo = 0;
                }
            }
        } else if (other.transform.CompareTag("JumpLine"))
        {
            minVelocity = -1;
            rb.velocity = Vector3.zero;
        }
    }
    
    private void OnCollisionExit(Collision other)
    {
        if (other.transform.CompareTag("Road"))
        {
            Debug.Log("----Karakter Yükseliyor---");
            
            isOnGround = false;
            firstlyLine = false;
            
            if (combo > 0)
            {
                Debug.Log("Kombo sayısı : " + combo);
            }
        }
    }
}
