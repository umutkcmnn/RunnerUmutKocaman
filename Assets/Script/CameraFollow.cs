using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform Player;

    float zOffset;

    private void Start()
    {
        zOffset = transform.position.z - Player.position.z;
    }
    private void LateUpdate()
    {   
    
        transform.position = new Vector3(transform.position.x, transform.position.y, Player.position.z + zOffset);
    }
}
