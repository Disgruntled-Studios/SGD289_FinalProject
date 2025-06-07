using UnityEngine;

public class InSightScript : MonoBehaviour
{
    //put in coding so stays in place during boss fight scripting.
    //Or consider putting boss fight in another scene like mario does so easier to render.

    //Placeholder camera script
    
    // BJ Note: Player reference now comes from GameManager for transitioning
    
    private GameObject player;
    
    private Vector3 inSightOffset;

    private void Awake()
    {
        player = GameManager.Instance.Player;
    }
    
    void Start()
    {
        inSightOffset = transform.position - player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.transform.position + inSightOffset;
    }
}
