using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] public GameObject gameStartMenu;
    [SerializeField] public GameObject gameRestartMenu;
    [SerializeField] public TextMeshProUGUI endScore;
    [SerializeField] public TextMeshProUGUI mainScore;
    [SerializeField] public TextMeshProUGUI Health;



    private void Start()
    {
        gameStartMenu.SetActive(true);
        gameRestartMenu.SetActive(false);
    }

    private void Update()
    {
        mainScore.text= "Score : " + playerController.score;
       
        Health.text = "Health : " + playerController.Health;
        if (playerController.isDead) 
        {
            gameRestartMenu.SetActive(true);
            endScore.text = "Score : " + playerController.score;
        }
           
    }
    public void StartGame() 
    {
        playerController.isStart = true;// oyun baþladý
        playerController.myAnim.SetBool("Run", true);//animasyonu hareketlendirdik
        gameStartMenu.SetActive(false);

       // playerController.enabled = true;
    }

    public void RestartGame() 
    {
        // SceneManager.LoadScene(0);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
