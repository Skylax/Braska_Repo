using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEngine.GridBrushBase;

public class PlayerController_2 : MonoBehaviour
{
    [Header("Movement stats")]
    [SerializeField] bool canMove;
    [SerializeField] float moveSpeed;
    [SerializeField] float accelerationSpeed;
    [SerializeField] float maxSpeed;
    [SerializeField] Vector2 moveInput; // Input from controller.

    [Header("Rotation stats")]
    [SerializeField] float playerAngle;
    [SerializeField] Vector3 targetRotation;
    [SerializeField] Vector3 meshTargetRotation;
    [SerializeField] float rotationSpeed;

    [Header("Actions stats")]
    public bool canBark;
    public bool canDig;

    [Header("Border stats")]
    [SerializeField] GameObject borderCheckA;
    [SerializeField] GameObject borderCheckB;
    [SerializeField] float borderCheckRadius;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] bool groundAhead;

    [Header("Object references")]
    private Rigidbody playerRb;
    [SerializeField] GameObject playerMesh;
    [SerializeField] GameObject areaBark;
    [SerializeField] GameObject areaDig;
    private GameObject worldAxsis;
    [SerializeField] GameObject orb;
    public ParticleSystem trackParticles;


   






    private void Awake()
    {
        playerRb = GetComponent<Rigidbody>();

        worldAxsis = GameObject.Find("PF_WorldAxsis");
        orb = GameObject.Find("PF_Orb");
    }

    private void Start()
    {
     
        if (trackParticles != null)
        {
            trackParticles.Stop();
        }

        areaBark.SetActive(false);
        areaDig.SetActive(false);

        canBark = true;
        canDig = true;
        canMove = true;

        SpawnTransform();

        if (SceneManager.GetActiveScene().name == "SCN_Level0" && ScenesManager.instance.collectedOrbs == -1)
        {
            ObjectManager.instance.hasOrb = true;
        }
        else
        {
            ObjectManager.instance.hasOrb = false;
        }
    }
    private void SpawnTransform() // Spawns the player where it should be.
    {
        transform.position = ScenesManager.instance.spawnPoint;

        transform.eulerAngles = new Vector3(
            transform.eulerAngles.x,
            ScenesManager.instance.spawnView,
            transform.eulerAngles.z
        );       
    }

    private void Update()
    {
        CheckUpdate();
    }

    private void CheckUpdate() // Updates all terrain checks.
    {
        groundAhead =
            Physics.CheckSphere(borderCheckA.transform.position, borderCheckRadius, groundLayer)
            && Physics.CheckSphere(borderCheckB.transform.position, borderCheckRadius, groundLayer);
    }

    private void FixedUpdate()
    {
        PlayerRotation();

        PlayerMove();
    }

    private void PlayerRotation()
    {
        // Defines where should the player rotate.
        targetRotation = new Vector3(
            transform.eulerAngles.x,
            playerAngle + worldAxsis.transform.eulerAngles.y, // Adds the camera rotation.
            transform.eulerAngles.z
        );

        // Rotates the player.
        if (moveInput != new Vector2(0, 0) && canMove)
        {
            transform.rotation = Quaternion.RotateTowards
            (
                transform.rotation,
                Quaternion.Euler(targetRotation),
                rotationSpeed * Time.deltaTime
            );
        }
    }

    private void PlayerMove()
    {
        if (moveInput != new Vector2(0, 0) && canMove && groundAhead) // Accelerates the player when it can and starts moving (and there's ground/slope).
        {
            // Prevents the player from going too fast.
            if (moveSpeed <= maxSpeed)
            {
                moveSpeed += accelerationSpeed;
            }
            else if (moveSpeed > maxSpeed)
            {
                moveSpeed = maxSpeed;
            }
        }
        else if (!groundAhead) // Stops and decelerates the player when there is not terrain ahead.
        {
            moveSpeed = 0;
            moveSpeed -= accelerationSpeed * 1.1f;
        }
        else if (moveSpeed > 0) // Decelerates the player when it stops moving.
        {
            moveSpeed -= accelerationSpeed * 2f;
        }
        else if (moveSpeed != 0) // Completely stops the player from moving while still.
        {
            moveSpeed = 0;

            playerRb.linearVelocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
        }
     
        transform.position += moveSpeed * Time.deltaTime * transform.forward; // Moves the player forward.
    }

    private void StartBark()
    {
        if (canBark && canDig)
        {
            int[] barkSFXIndices = new int[] { 7, 8, 1 };

            // Elegir uno aleatoriamente
            int randomIndex = Random.Range(0, barkSFXIndices.Length);
            int sfxIndex = barkSFXIndices[randomIndex];

            // Reproducir el SFX
            AudioManager.Instance.PlaySFX(sfxIndex);

            canBark = false;
            canDig = false;
            canMove = false;
            areaBark.SetActive(true);

            Invoke(nameof(FinishAction), 0.5f);
        }
    }

    private void StartDig()
    {
        if (canDig && canBark)
        {
           

            canBark = false;
            canDig = false;
            canMove = false;
            areaDig.SetActive(true);

            ObjectManager.instance.LocateOrb();

            Invoke(nameof(StartTrack), 0.1f);   
        }
    }

    private void StartTrack()
    {
        if (trackParticles != null && !ObjectManager.instance.hasOrb)
        {
            trackParticles.Play();
            AudioManager.Instance.PlaySFX(5);

        }

        Invoke(nameof(FinishAction), 0.9f);
    }

    private void FinishAction()
    {
        areaBark.SetActive(false);
        areaDig.SetActive(false);
        canMove = true;
        canBark = true;
        canDig = true;
    }

    #region Input Methods

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        if (!context.canceled)
        {
            playerAngle = Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg; // Transform the input vector 2 into a float .        
        }
    }

    public void OnBark(InputAction.CallbackContext context)
    {
        StartBark();
    }

    public void OnDig(InputAction.CallbackContext context)
    {
        StartDig();
    }

    #endregion
}
