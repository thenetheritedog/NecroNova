using UnityEngine;

public class PlayerLocomotion : MonoBehaviour {


    PlayerManager playerManager;
    AnimatorManager animatorManager;
    InputManager inputManager;
    CameraManager cameraManager;

    Vector3 moveDirection;
    Transform cameraObject;
    public Rigidbody playerRigidbody;
    
    

    [Header("Falling Idle")]
    public float inAirTimer;
    public float leapingVelocity;
    public float fallingVelocity;
    public float rayCastHeightOffset = 0.5f;
    public LayerMask groundLayer;

    [Header("Movement Flags")]
    public bool isGrounded;
    public bool isSprinting;
    

    [Header("Jump Speeds")]
    public float jumpHeight = 3;
    public float gravityIntensity = -15;

    [Header("Movement Speeds")]
    public float walkingSpeed = 1.5f;
    public float runningSpeed = 5;
    public float sprintingSpeed = 7;
    public float rotationSpeed = 15;

    
    private void Awake()
    {
        animatorManager = GetComponent<AnimatorManager>();
        playerManager = GetComponent<PlayerManager>();
        inputManager = GetComponent<InputManager>();
        playerRigidbody = GetComponent<Rigidbody>();
        cameraManager = FindFirstObjectByType<CameraManager>();
        cameraObject = Camera.main.transform;

    }
    public void HandleAllMovement()
    {
        
        HandleFallingAndLanding();
        if (playerManager.isInteracting || !isGrounded)
            return;

        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        if (playerManager.playerAttackAndWeaponManager.isLockedOn && !isSprinting)
        {
            moveDirection = cameraObject.forward * (inputManager.verticalInput);
            moveDirection += cameraObject.right * inputManager.horizontalInput;
            moveDirection.Normalize();
            moveDirection.y = 0;
        }
        else
        {
            moveDirection = transform.forward * Mathf.Abs(inputManager.verticalInput);
            moveDirection += transform.forward * Mathf.Abs(inputManager.horizontalInput);
            moveDirection.Normalize();
            moveDirection.y = 0;
        }

        if (isSprinting && playerManager.currentStamina > playerManager.sprintStaminaConsumption)
        {
            playerManager.currentStamina -= playerManager.sprintStaminaConsumption * Time.deltaTime;
            playerManager.recoverySpentTime = 0;
            playerManager.staminaBar.UpdatePostureBar(playerManager.maxStamina, playerManager.currentStamina);
            moveDirection = moveDirection * sprintingSpeed;
        }
        else
        {
            if (inputManager.moveAmount >= 0.5f)
            {
                moveDirection = moveDirection * runningSpeed;
            }
            else
            {
                moveDirection = moveDirection * walkingSpeed;
            }
            isSprinting = false;
        }

        

        Vector3 movementVelocity = moveDirection;
        playerRigidbody.linearVelocity = movementVelocity;
    }
    private void HandleRotation()
    {
        Vector3 targetDirection = Vector3.zero;

        if (playerManager.playerAttackAndWeaponManager.isLockedOn && !isSprinting) 
        {
            if (playerManager.playerAttackAndWeaponManager.currentTarget == null)
                return;
            Vector3 rotationDirection = playerManager.playerAttackAndWeaponManager.currentTarget.lockOnTransform.position - transform.position;
            rotationDirection.Normalize();
            rotationDirection.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(rotationDirection);
            transform.rotation = targetRotation;
        }
        else
        {
           

            targetDirection = cameraObject.forward * inputManager.verticalInput;
            targetDirection = targetDirection + cameraObject.right * inputManager.horizontalInput;
            targetDirection.Normalize();
            targetDirection.y = 0;

            if (targetDirection == Vector3.zero)
            {
                targetDirection = transform.forward;
            }
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            if (playerRigidbody.linearVelocity.magnitude > 0) 
            { 
                transform.rotation = targetRotation;
            }
            else
            {
                Quaternion playerRotation = Quaternion.Slerp
                (transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                transform.rotation = playerRotation;
            }
        }
    }

    private void HandleFallingAndLanding()
    {
        RaycastHit hit;
        Vector3 rayCastOrigin = transform.position;
        Vector3 targetPosition;
        rayCastOrigin.y = rayCastOrigin.y + rayCastHeightOffset;
        targetPosition = transform.position;

        if (!isGrounded)
        {
            if (!playerManager.isInteracting)
            {
                animatorManager.PlayTargetAnimation("Falling Idle", true);
            }

            animatorManager.animator.SetBool("isUsingRootMotion", false);
            inAirTimer -= Time.deltaTime * gravityIntensity;
            playerRigidbody.AddForce(-Vector3.up * fallingVelocity * inAirTimer);
        }
        if (Physics.SphereCast(rayCastOrigin, 0.2f, -Vector3.up, out hit, groundLayer) && inAirTimer >= 0)
        {
            if (!isGrounded && !playerManager.isInteracting)
            {
                animatorManager.PlayTargetAnimation("Land", true);
            }
            Vector3 rayCastHitPoint = hit.point;
            targetPosition.y = rayCastHitPoint.y;
            inAirTimer = 0;
            isGrounded = true;
            playerRigidbody.linearVelocity = new Vector3
                (playerRigidbody.linearVelocity.x, 0 , playerRigidbody.linearVelocity.z);
        }
        else
        {
            isGrounded = false;
        }

        if (isGrounded)
        {
            if (playerManager.isInteracting || inputManager.moveAmount > 0) 
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime / 0.01f);
                
            }
            else
            {
                transform.position = targetPosition;
            }
            if (playerManager.currentStamina != playerManager.maxStamina)
            {
                playerManager.recoverySpentTime += Time.deltaTime;
                if (playerManager.recoverySpentTime > playerManager.recoveryTime)
                {
                    playerManager.currentStamina += playerManager.recoverySpeed * Time.deltaTime;
                    playerManager.currentStamina = Mathf.Clamp(playerManager.currentStamina, 0, playerManager.maxStamina);
                    playerManager.staminaBar.UpdatePostureBar(playerManager.maxStamina, playerManager.currentStamina);
                }
            }
            else { playerManager.recoverySpentTime = 0; }
            
        }
    }

    public bool HandleJumping()
    {
        if (isGrounded && !playerManager.isInteracting && playerManager.currentStamina > playerManager.jumpStaminaConsumption)
        {
            playerManager.currentStamina -= playerManager.jumpStaminaConsumption;
            playerManager.recoverySpentTime = 0;
            playerManager.staminaBar.UpdatePostureBar(playerManager.maxStamina, playerManager.currentStamina);
            animatorManager.PlayTargetAnimation("Jump", true);

            float jumpingVelocity = -Mathf.Sqrt(-2 * gravityIntensity * jumpHeight);
            Vector3 playerVelocity = moveDirection;
            playerVelocity.y = 0;
            playerRigidbody.linearVelocity = playerVelocity;
            inAirTimer = jumpingVelocity;
            isGrounded = false;
            return false;
        }
        else
            return true;
    }

    public void HandleDodge()
    {
        if (playerManager.isInteracting || playerManager.currentStamina < playerManager.dodgeStaminaConsumption)
            return;
            Vector3 targetDirection;
            targetDirection = cameraObject.forward * inputManager.verticalInput;
            targetDirection = targetDirection + cameraObject.right * inputManager.horizontalInput;
            targetDirection.Normalize();
            targetDirection.y = 0;
            if (targetDirection == Vector3.zero)
            {
                targetDirection = transform.forward;
            }
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            Quaternion playerRotation = targetRotation;
            transform.rotation = playerRotation;
        playerManager.currentStamina -= playerManager.dodgeStaminaConsumption;
        playerManager.recoverySpentTime = 0;
        playerManager.staminaBar.UpdatePostureBar(playerManager.maxStamina, playerManager.currentStamina);
        animatorManager.PlayTargetAnimation("Dodge", true, true);
        
    }
   
}