using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BulletDestroy : MonoBehaviour
{
   public float lifeTime = 5f;
   void Start()
   {
      Destroy(gameObject, lifeTime);
   }
   void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Enemy")){ 
        GameManager.instance.AddScore();
        Destroy(collision.gameObject); 
        Destroy(gameObject); 
        }
        

    }
}
