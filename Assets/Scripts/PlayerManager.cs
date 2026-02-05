using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour, IDataPersistence
{
    public Animator animator;
    public InputManager inputManager;
    public CameraManager cameraManager;
    public PlayerLocomotion playerLocomotion;
    public PlayerAttackAndWeaponManager playerAttackAndWeaponManager;
    public InventoryManager inventoryManager;



    public bool isInteracting;
    public bool isUsingRootMotion;
    public bool inMenu;
    [Header("Stats")]
    public HealthBar healthBar;
    public float currentHealth;
    public float maxHealth;
    public float currentStamina;
    public float maxStamina;
    public PostureBar staminaBar;
    public float recoverySpeed;
    public float recoveryTime;
    public float sprintStaminaConsumption;
    public float dodgeStaminaConsumption;
    public float attackStaminaConsumption;
    public float jumpStaminaConsumption;
    public float recoverySpentTime;
    public Collider hitboxCollider;
    public bool isBlocking;
    public AudioSource deflectSfx;
    public AudioSource blockSfx;
    public AudioSource attackSfx;
    public AudioSource hitSfx;
    public GameObject menu;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        inputManager = GetComponent<InputManager>();
        cameraManager = FindObjectOfType<CameraManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        playerAttackAndWeaponManager = GetComponent<PlayerAttackAndWeaponManager>();

    }

    private void Update()
    {
        inputManager.HandleAllInputs();
    }

    private void FixedUpdate()
    {
        playerLocomotion.HandleAllMovement();
    }
    private void LateUpdate()
    {
        cameraManager.HandleAllCameraMovement();

        isInteracting = animator.GetBool("isInteracting");
        isUsingRootMotion = animator.GetBool("isUsingRootMotion");
        animator.SetBool("isGrounded", playerLocomotion.isGrounded);
        if (animator.GetInteger("actionWanted") == 3 && !animator.GetCurrentAnimatorStateInfo(0).IsName("UnBlock"))
        {
            isBlocking = true;
        }
        else
        {
            isBlocking = false;
            animator.SetInteger(("actionWanted"), 0);
        }
        if (playerAttackAndWeaponManager.isLockedOn)
            animator.SetFloat(("isLockedOn"), 1);
        else 
            animator.SetFloat(("isLockedOn"), 0);

    }

    public void OpenMenu()
    {
        if (!inMenu)
        {
            menu.SetActive(true);
            inMenu = true;
        }
        else 
        {
            menu.SetActive(false);
            inMenu = false;
        }
    }
    public void SaveData(ref GameData data)
    {
        data.currentHealth = this.currentHealth;
    }
    public void LoadData(GameData data) 
    {
        this.currentHealth = data.currentHealth;
    }
}