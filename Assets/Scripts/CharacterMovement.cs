using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f; // Adjust the speed as needed
    [SerializeField] float rotationSpeed = 500f; // Adjust the speed as needed

    private float gravity = -9.81f;
    [SerializeField] private float gravityMultiplier = 3.0f;
    public float velocityY;
    public float jumpForce = 8.0f;



    Quaternion targetRotation;


    [SerializeField] float groundCheckRadius = 0.2f;
    [SerializeField] Vector3 groundCheckOffset;
    [SerializeField] LayerMask groundLayer;

    bool isGrounded;

    public Transform cam;

    public CameraController cameraController;
    [SerializeField] Animator animator;

    CharacterController characterController;
    Player player;

    [Header("Animator Parameters")]
    [Space]
    public string movementAmount = "moveAmount";
    public string moveX = "MoveX";
    public string moveZ = "MoveZ";
    public string specialAttack2 = "isAttack02";
    public string notspecialAttack2 = "isNotAttack02";
    public string specialAttack3 = "isAttack03";
    public string notspecialAttack3 = "isNotAttack03";


    [Header("Layer Index For Animation")]
    private int baseLayerIndex;
    private int combatLayerIndex;
    


    private PlayerController playerController;

    public bool inCombat = false;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerController = GetComponent<PlayerController>();
        baseLayerIndex = animator.GetLayerIndex("Base Layer");
        combatLayerIndex = animator.GetLayerIndex("Combat Layer");
        player = GetComponent<Player>(); 

    }

    private void Update()
    {


        // Get input axes (WASD or arrow keys)
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        //velocityY += gravity * gravityMultiplier * Time.deltaTime;

        //CLamp the value between 0 and 1 for the animation blend
        float moveAmount = Mathf.Clamp01(Mathf.Abs( moveVertical) + Mathf.Abs( moveHorizontal));

        // Calculate the movement vector
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        movement.Normalize(); // Normalize to ensure diagonal movement isn't faster

        //movement.y = velocityY;

        //var moveDirection = cameraController.PlanarRotation * movement;
        var moveDirection = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0) * movement;
        // Calculate the velocity based on the input and moveSpeed
        Vector3 velocity = moveDirection * moveSpeed;

        /*
        // Apply gravity
        velocityY += gravity * gravityMultiplier * Time.deltaTime;

        // Apply the vertical velocity to the object's movement
        velocity.y = velocityY;

       */



        GroundCheck();

        if (isGrounded && velocityY < 0.0f)
        {
            velocityY = -1f;

        }
        else
        {
            // Apply gravity
            velocityY += gravity * gravityMultiplier * Time.deltaTime;
        }

        // Apply the vertical velocity to the object's movement
        velocity.y = velocityY;

        if (inCombat == false)
        {
            player.StartHealthRegeneration(true);
            animator.SetLayerWeight(combatLayerIndex, 0);
            if (moveAmount > 0)
            {
                // Translate the character's position based on the input
                //transform.Translate(movement * moveSpeed * Time.deltaTime);
                //transform.position += moveDirection * moveSpeed * Time.deltaTime;

                characterController.Move(velocity * Time.deltaTime);

                targetRotation = Quaternion.LookRotation(moveDirection);

            }



            //transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            animator.SetFloat(movementAmount, moveAmount);
        }


        if (inCombat == true)
        {
            player.StopHealthRegeneration();
            animator.SetLayerWeight(combatLayerIndex, 1);
            if (moveAmount > 0)
            {
                // Translate the character's position based on the input
                //transform.Translate(movement * moveSpeed * Time.deltaTime);
                //transform.position += moveDirection * moveSpeed * Time.deltaTime;

                characterController.Move(velocity * Time.deltaTime);

                targetRotation = Quaternion.LookRotation(moveDirection);

            }



            //transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            animator.SetFloat(moveX, moveHorizontal);
            animator.SetFloat(moveZ, moveVertical);
        }


        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        


       


        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {

            animator.SetBool("IsJumping", true);
            velocityY = jumpForce;
            


        }
        else if (isGrounded)
        {
            animator.SetBool("IsJumping", false);
        }








    }


    void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius, groundLayer);
    }


    public void enteringCombat()
    {
        inCombat = true;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius);
    }

}
