using UnityEngine;

public class PlayerInputController : MonoBehaviour
{

    private PlayerTankMovement tankMovement;
    private GunFunctions gunFunc;

    private PlayerInput controls;
    private PlayerInput.PlayerActions in_Game;

    Vector2 horizontalInput;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (tankMovement == null)
        {
            tankMovement = GetComponent<PlayerTankMovement>();
        }
        if (gunFunc == null)
        {
            gunFunc = GetComponent<GunFunctions>();
        }
        controls = new PlayerInput();
        SetInputFuncs();
    }

    private void SetInputFuncs()
    {
        if (controls == null)
        {
            Debug.LogError("Controls not instanciated into a new PlayerInput!");
            return;
        }

        in_Game = controls.Player;

        //Set up actions here.
        in_Game.Move.performed += ctx => horizontalInput = ctx.ReadValue<Vector2>();

        //in_Game.Attack.performed += _ => gunFunc.Shoot;

        in_Game.Aim.performed += gunFunc.StartGunAim;
        in_Game.Aim.canceled += gunFunc.EndGunAim;

    }

    // Update is called once per frame
    void Update()
    {
        tankMovement.ReceiveMoveInput(horizontalInput);
    }

    #region Enable/Disable Functions
    private void OnEnable() 
    {
        controls.Enable();
    }
    private void OnDisable() 
    {
        controls.Disable();
    }
    #endregion Enable/Disable Functions

}
