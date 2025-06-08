using UnityEngine;

public class CameraScript : MonoBehaviour
{
    //put in coding so stays in place during boss fight scripting.
    //Or consider putting boss fight in another scene like mario does so easier to render.

    //Placeholder camera script

    [SerializeField]
    private GameObject player;
    private Vector3 camOffset;
    
    void Start()
    {
        player = GameManager.Instance.Player;
        camOffset = transform.position - player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.transform.position + camOffset;
    }
}
