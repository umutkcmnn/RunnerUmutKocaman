using UnityEngine;

public class Road : MonoBehaviour
{
    GameObject player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        
    }

    // Update is called once per frame
    void Update()
    {
        if ((player.transform.position.z - this.transform.position.z) > 25) 
        {
            Destroy(this.gameObject);
        
        }
    }
}
