using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine;



public class Player : MonoBehaviour
{
    // movement, rotation vectors
    Vector3 movementInput = Vector3.zero;
    Vector3 rotationInput = Vector3.zero;
    Vector3 headRotationInput;
    
    // movement speed
    public float baseMoveSpeed = 5f;

    public float rotationSpeed = 60f;

    // stamina and running variables
    private float totalStamina = 100f;
    private float currentStamina;
    private float staminaRegen = 5f;

    private float sprintDrainRate = 10f;
    [SerializeField] private bool sprint;

    [Range(0f, 10f)] public float sprintfactor;

    // jumping stuff
    public float jumpForce;

    // health status
    bool isDead = false;
    public float totalHealth = 20;
    public float currentHealth;
    public float regenHealth = 2;
    // player UI
    public Scrollbar healthBar;
    public Scrollbar staminaBar;

    //interactions
    float interactionDistance = 20f;

    bool interact = false;

    //camera
    public GameObject playerCamera;

    // animator
    public GameObject playerAnimator;

    //mouse issue fixes
    [Header("Mouse/Look")]
    public bool lockMouse = false;
    float xRot = 0f;
    public float lookSens = 5f;
    public float xRotClamp = 60;
    // time stuff
    [SerializeField]
    private float regenDelay = 3;
    public float damagedTime = -1;

    //collected items stuff
    // first stage collectable
    internal static int firstCollectable = 0;
    //third stage collectable
    internal static int finalCollectable = 0;
    // second stage score (shoot in order) 
    internal static int mazeScore = 0;

    //layers
    public LayerMask firstCollectableMask;
    public LayerMask finalCollectableMask;

    // Audio clips

    //audio for when item is picked up
    public AudioClip picked;


    
    //default actions for player
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        currentHealth = totalHealth;
        currentStamina = totalStamina;
        sprint = false;
    }
    private void Start()
    {
        // mouse pointer stuff
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }

    //update frame
    private void Update()
    {
        
        if (lockMouse)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;

        }
        // health regen stuff 
         if (currentHealth < totalHealth && Time.time - damagedTime >= regenDelay)
        {
            currentHealth += regenHealth * Time.deltaTime;
            healthBar.size = currentHealth / totalHealth;
        }

        //check if collectable in front of player
        var ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 25, firstCollectableMask))
        {
            Destroy(hit.transform.gameObject);
            firstCollectable += 1;
            GetComponent<AudioSource>().PlayOneShot(picked, 1);

        }
        var ray2 = new Ray(transform.position, transform.forward);
        RaycastHit hit2;
        if (Physics.Raycast(ray, out hit2, 25, finalCollectableMask))
        {
            Destroy(hit.transform.gameObject);
            finalCollectable += 1;
            GetComponent<AudioSource>().PlayOneShot(picked, 1);

        }
        if (!isDead)
        {
            Rotation();
            Movement();
            Raycasting();

        }
        interact = false;
    }

    private void Rotation()
    {
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + rotationInput * rotationSpeed * Time.deltaTime);
        playerCamera.transform.rotation = Quaternion.Euler(playerCamera.transform.rotation.eulerAngles + headRotationInput * rotationSpeed * Time.deltaTime);
    }

    private void Movement()
    {
        // Create a new Vector3
        Vector3 movementVector = Vector3.zero;

        // Add the forward direction of the player multiplied by the user's up/down input.
        movementVector += transform.forward * movementInput.y;

        // Add the right direction of the player multiplied by the user's right/left input.
        movementVector += transform.right * movementInput.x;

        // Create a local variable to hold the base move speed so that the base speed doesn't get altered.
        float moveSpeed = baseMoveSpeed;

        if (sprint)
        {
            Debug.Log("check Sprint");

            if (currentStamina > 0)
            {
                Debug.Log("Sprinting");
                moveSpeed += sprintfactor;

                //check if player moving
                if (movementVector.sqrMagnitude > 0)
                {
                    currentStamina -= sprintDrainRate * Time.deltaTime;
                }
                else if(currentStamina < totalStamina)
                {
                    //regen
                    currentStamina += staminaRegen * Time.deltaTime;
                }
            }
        }
        else
        {
            if (currentStamina <= totalStamina)
            {
                //regen
                currentStamina += staminaRegen * Time.deltaTime;
            }
        }

        staminaBar.size = currentStamina / totalStamina;

        // Apply the movement vector multiplied by movement speed to the player's position.
        transform.position += movementVector * moveSpeed * Time.deltaTime;


    }
    // raycasting stufff here
    private void Raycasting()
    {
        Debug.DrawLine(playerCamera.transform.position, playerCamera.transform.position + (playerCamera.transform.forward * interactionDistance));
        RaycastHit hitInfo;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hitInfo, interactionDistance))
        {
            // Print the name of the object hit. For debugging purposes.
            Debug.Log(hitInfo.transform.name);
            if (hitInfo.transform.tag == "Collectable")
            {
                if (interact)
                {
                    hitInfo.transform.GetComponent<>().Collected();
                }
            }
        }
    }

    //kill player

    void KillPlayer()
    {
        isDead = true;
        playerAnimator.applyRootMotion = false;
        playerAnimator.SetBool("PlayerDead", isDead);
        GameManager.instance.ToggleRespawnMenu();
        lockMouse = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    //take damage part
    public void TakeDamae(float damage)
    {
        if (!isDead)
        {
            damagedTime = Time.time;
            currentHealth -= damage;
            healthBar.size = currentHealth / totalHealth;
            if(currentHealth <= 0)
            {
                KillPlayer();
            }
        }
    }
    public void ResetPlayer()
    {
        movementInput = Vector3.zero;
        rotationInput = Vector3.zero;
        headRotationInput = Vector3.zero;

        currentHealth = totalHealth;
        currentStamina = totalStamina;
        healthBar.size = 1;
        isDead = false;
        playerAnimator.applyRootMotion = true;
        playerAnimator.SetBool("PlayerDead", isDead);

        playerAnimator.gameObject.transform.eulerAngles = Vector3.zero;
    }

    #region unity stuff

    // tp player to end scene after completing dungeon
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "")
        {

        }
    }
    void OnLook(InputValue value)
    {
        if (!isDead)
        {
            rotationInput.y = value.Get<Vector2>().y;
            headRotationInput.x = -value.Get<Vector2>().x;

            transform.Rotate(Vector3.up, value.Get<Vector2>().x * lookSens * Time.smoothDeltaTime);


            xRot -= value.Get<Vector2>().y * lookSens * Time.smoothDeltaTime;
            xRot = Mathf.Clamp(xRot, -xRotClamp, xRotClamp);
            Vector3 target = transform.eulerAngles;
            target.x = xRot;
            playerCamera.transform.eulerAngles = target;

        }
    }
    void OnMove(InputValue value)
    {
        if (!isDead)
        {
            movementInput = value.Get<Vector2>();
        }
    }
    void OnFire()
    {
        interact = true;
    }
    void OnSprint()
    {
        Debug.Log("sprint");
        sprint = !sprint;
    }
    void OnPause()
    {
        GameManager.instance.TogglePause();
        lockMouse = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    void OnJump()
    {
        GetComponent<Rigidbody>().AddForce(transform.up * jumpForce, ForceMode.Impulse);
        
    }
}
#endregion