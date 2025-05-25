using Unity.VisualScripting;
using UnityEngine;

public class PlayerTankMovement : MonoBehaviour
{   
    

    #region GroundMovement Vars
    private Rigidbody rb;


    [Header("---GroundMovement Vars---")]
    [Tooltip("The speed at which the player walks.")]
    [SerializeField] float speed = 6f;
    [Tooltip("The speed at which the player turns.")]
    [SerializeField] float rotationSpeed = 6f;
    [Tooltip("How much drag is applied to the player while they are on the ground.")]
    [SerializeField] float groundDrag = 5f;
    [Tooltip("This is a reference to the empty GameObject that will keep track of the player's orientation.")]
    public Transform orientation;

    Vector2 horizontalInput;

    #endregion GroundMovement Vars

    #region Gravity Vars

    [Tooltip("The layer that holds ground objects.")]
    [SerializeField] LayerMask groundLayerMask;
    [Tooltip("How tall the player object is in unity's units of measurement.")]
    [SerializeField] private float playerHeight = 2;
    bool isGrounded;

    #endregion Gravity Vars
    private Vector3 rotationAmont;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //Each frame we check if the player is grounded or not.
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundLayerMask);
        Debug.DrawLine(transform.position, new Vector3(transform.position.x, (-1 * playerHeight * .5f + 0.2f), transform.position.z), Color.green);
        
        //If the player is grounded and has Velocity stored on the Y axis we reset their vertical velocity.X
        if (isGrounded)
        {
            rb.linearDamping = groundDrag;
        }
        else
        {
            rb.linearDamping = 0;
        }

        SpeedControl();
    }

    void FixedUpdate()
    {
        HandleInput();
    }

    void HandleInput()
    {
        if (horizontalInput.x < 0 || horizontalInput.x > 0)
        {
            Debug.Log("Input Detected " + horizontalInput.x);
            rotationAmont = new Vector3(0,horizontalInput.x,0);
        }
        else if (horizontalInput.x == 0)
        {
            rotationAmont = Vector3.zero;
        }

        if (horizontalInput.y != 0)
        {
            Debug.Log("Moving");
            rb.AddForce(orientation.forward * horizontalInput.y * speed * 10f, ForceMode.Force);
        }
        else
        {
            rb.angularVelocity = Vector3.zero;
        }

        transform.Rotate(rotationAmont * rotationSpeed);
        
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        //Limit the players current velocity if needed
        if (flatVel.magnitude > speed)
        {
            //Calculates what the player's max velocity should be and applies it.
            Vector3 limitedVel = flatVel.normalized * speed;
            rb.linearVelocity = new Vector3(limitedVel.x,rb.linearVelocity.y, limitedVel.z);
        }
    }

    /// <summary>
    /// Receives the mone input value to assign it to the horizontalInput var.
    /// </summary>
    /// <param name="_moveInput">This is the incoming input.</param>
    public void ReceiveMoveInput(Vector2 _moveInput)
    {
        horizontalInput = _moveInput;    
    }

}
