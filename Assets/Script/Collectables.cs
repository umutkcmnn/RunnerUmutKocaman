using UnityEngine;
using DG.Tweening;

public class Collectables : MonoBehaviour
{
    public CollectablesEnum collectablesEnum;

    public int TobeAddedHealth;
    public int toBeAddedScore;
    public int toBeAddedSpeed;
    public GameObject Player;

    private void Start()
    {
        if (collectablesEnum == CollectablesEnum.Coin)
        {
            Player = GameObject.FindFirstObjectByType<PlayerController>().gameObject;// playerime eriþiyorum.


        }
    }
    private void Update()
    {
        if (collectablesEnum == CollectablesEnum.Coin && Player.GetComponent<PlayerController>().isMagnetActive)
        {
            if (Vector3.Distance(Player.transform.position, this.transform.position) < 8)
            {

                transform.DOMove(Player.transform.position + new Vector3(0, 1, 0), 0.35f);
            }

        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("obstacle"))
        {
            Destroy(other.gameObject);
        }
    }
}
