using UnityEngine;
using UnityEngine.SceneManagement;
public class Menu : MonoBehaviour
{
   public void play(){
      SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
   }

   public void quit(){
      Debug.Log("Tchau ZÉ!");
   }
}
