using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class EndAnimationController : MonoBehaviour
{
    // public GameCamera cutsceneCam;
    public Animator playerEndAnim;
    public GameObject playerModel;
    public string endSceneSong;
    public PlayableAsset endingPlayableAsset;
    public PlayableDirector cameraDirector;


    void Awake()
    {
        playerEndAnim.speed = 0;
        playerModel.SetActive(false);
        GetComponent<AudioListener>().enabled = false;
        cameraDirector.gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == GameManager.Instance.Player)
        {
            other.gameObject.SetActive(false);
            // CameraManager.Instance.TrySwitchToCamera(cutsceneCam.CameraID);
            Destroy(GameManager.Instance.Player);
            GetComponent<AudioListener>().enabled = true;
            StartEndAnim();
        }
    }

    private void StartEndAnim()
    {
        Destroy(CameraManager.Instance.gameObject);
        cameraDirector.gameObject.SetActive(true);
        Destroy(SelectionManager.Instance.gameObject);
        Destroy(GameManager.Instance.gameObject);
        Destroy(UIManager.Instance.gameObject);
        if (endSceneSong != string.Empty)
        {
            SoundManager.Instance.FadeInSFX(endSceneSong, 43f, 8f);
        }
        cameraDirector.Play();
        playerEndAnim.speed = 1;
        playerModel.SetActive(true);
    }

    public void EndCutscene()
    {
        Debug.Log("Ending Cutscene");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

}
