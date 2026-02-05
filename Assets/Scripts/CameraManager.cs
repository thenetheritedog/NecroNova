using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    InputManager inputManager;
    public Transform targetTransform;
    PlayerManager player;
    public Transform cameraPivot;
    public Transform cameraTransform;
    public LayerMask collisionLayers;
    private float defaultPosition;
    private Vector3 cameraFollowVelocity = Vector3.zero;
    private Vector3 cameraVectorPosition;

    public float cameraFollowSpeed = 0.2f;
    public float cameraLookSpeed = 2;
    public float cameraPivotSpeed = 2;
    public float cameraCollisionsRad = 0.2f;
    public float cameraCollisionOffset = 0.2f;
    public float minCollisionOffset = 0.2f;
    

    public float lookAngle;
    public float pivotAngle;
    public float minPivotAngle = -35;
    public float maxPivotAngle = 35;
    public float lockOnPivotAngle;
    public float lockOnLookAngle;
    public float lockOnRadius = 20;
    public float minViewAngle = -50;
    public float maxViewAngle = 50;
    [SerializeField] private float unlockedCameraHeight = 1.5f;
    [SerializeField] private float lockedOnCameraHeight = 2f;
    [SerializeField] float lockOnFollowSpeed;
    private List<CharacterManager> avaliableTargets = new List<CharacterManager>();
    public CharacterManager nearestLockOnTarget;
    public CharacterManager leftLockOnTarget;
    public CharacterManager rightLockOnTarget;
    public bool unavalibleLockOnTarget;
    private Coroutine cameraLockOnHeightCoroutine;

    private void Awake()
    {
        inputManager = FindObjectOfType<InputManager>();
        player = FindObjectOfType<PlayerManager>();
        targetTransform = player.transform;
        cameraTransform = Camera.main.transform;
        defaultPosition = cameraTransform.localPosition.z;
    }

    public void HandleAllCameraMovement()
    {
        
        FollowTarget();
        RotateCamera();
        HandleCameraCollisions();
    }
    private void FollowTarget()
    {
        
            Vector3 targetPosition = Vector3.SmoothDamp
                (transform.position, targetTransform.position, ref cameraFollowVelocity, cameraFollowSpeed);
        
        transform.position = targetPosition;
    }
    private void RotateCamera()
    {
        if (player.playerAttackAndWeaponManager.isLockedOn)
        {
            Vector3 rotationDirection = player.playerAttackAndWeaponManager.currentTarget.lockOnTransform.position - transform.position;
            rotationDirection.Normalize();
            rotationDirection.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(rotationDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lockOnFollowSpeed);
            rotationDirection = player.playerAttackAndWeaponManager.currentTarget.lockOnTransform.position - cameraPivot.position;
            rotationDirection.Normalize();

            targetRotation = Quaternion.LookRotation(rotationDirection);
            cameraPivot.transform.rotation = Quaternion.Slerp(cameraPivot.rotation, targetRotation, lockOnFollowSpeed);

            lookAngle = transform.eulerAngles.y;
            pivotAngle = transform.eulerAngles.x;
        }
        else
        {
            Quaternion targetRotation;
            Vector3 rotation;

            lookAngle = lookAngle + (inputManager.cameraInputX * cameraLookSpeed);
            pivotAngle = pivotAngle - (inputManager.cameraInputY * cameraPivotSpeed);
            pivotAngle = Mathf.Clamp(pivotAngle, minPivotAngle, maxPivotAngle);
            rotation = Vector3.zero;
            rotation.y = lookAngle;
            targetRotation = Quaternion.Euler(rotation);
            transform.rotation = targetRotation;

            rotation = Vector3.zero;
            rotation.x = pivotAngle;

            targetRotation = Quaternion.Euler(rotation);
            cameraPivot.localRotation = targetRotation;
        }
    }
    private void HandleCameraCollisions()
    {
        float targetPosition = defaultPosition;
        
        
        RaycastHit hit;
        Vector3 direction = cameraTransform.position - cameraPivot.position;
        direction.Normalize();

        if (player.playerAttackAndWeaponManager.isLockedOn)
        {
            if (Vector3.Distance(player.transform.position, player.playerAttackAndWeaponManager.currentTarget.transform.position) - targetPosition < 10f)
            {
                targetPosition = -10f + Vector3.Distance(player.transform.position, player.playerAttackAndWeaponManager.currentTarget.transform.position);
            }
        }
        if (Physics.SphereCast
            (cameraPivot.transform.position, cameraCollisionsRad, direction, out hit, Mathf.Abs(targetPosition), collisionLayers))
        {
            float distance = Vector3.Distance(cameraPivot.position, hit.point);
            targetPosition =- (distance - cameraCollisionOffset);
        }
        if (Mathf.Abs(targetPosition) < minCollisionOffset)
        {
            targetPosition = targetPosition - minCollisionOffset;
        }
        if (Mathf.Abs(targetPosition) < minCollisionOffset)
        {
            targetPosition = targetPosition - minCollisionOffset;
        }



        cameraVectorPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, 0.2f);
        cameraTransform.localPosition = cameraVectorPosition;
    }
    public void HandleLocatingLockOnTargets()
    {
        float shortDistance = Mathf.Infinity;
        float shortDistanceOfLeftTarget = -Mathf.Infinity;
        float shortDistanceOfRightTarget = Mathf.Infinity;
        if (player.playerAttackAndWeaponManager.isLockedOn) { unavalibleLockOnTarget = true; }
        else { unavalibleLockOnTarget = false; }

        Collider[] colliders = Physics.OverlapSphere(player.transform.position, lockOnRadius, WorldUtilityManager.Instance.GetCharacterLayers());

        for (int i = 0; i < colliders.Length; i++)
        {
            CharacterManager lockOnTarget = colliders[i].GetComponent<CharacterManager>();

            if (lockOnTarget != null)
            {
                Vector3 lockOntTargetsDirection = lockOnTarget.transform.position - player.transform.position;
                float distanceFromTarget = Vector3.Distance(player.transform.position, lockOnTarget.transform.position);
                float viewableAngle = Vector3.Angle(lockOntTargetsDirection, transform.forward);

                if (lockOnTarget.isDead)
                    continue;
                if (lockOnTarget.transform.root == player.transform.root)
                    continue;

                    RaycastHit hit;
                    if (Physics.Linecast(player.playerAttackAndWeaponManager.lockOnTransform.position, lockOnTarget.lockOnTransform.position, out hit, WorldUtilityManager.Instance.GetEnviroLayers()))
                    {
                        continue;
                    }
                    else
                    {
                        
                        avaliableTargets.Add(lockOnTarget);
                        if (lockOnTarget == player.playerAttackAndWeaponManager.currentTarget)
                        {
                            unavalibleLockOnTarget = false;
                        }
                    }




            }
        }
        for (int j = 0; j < avaliableTargets.Count; j++)
        {
            if (avaliableTargets[j] != null)
            {
                float distanceFromTarget = Vector3.Distance(player.transform.position, avaliableTargets[j].transform.position);

                if (distanceFromTarget < shortDistance)
                {
                    shortDistance = distanceFromTarget;
                    nearestLockOnTarget = avaliableTargets[j];
                }
                if (player.playerAttackAndWeaponManager.isLockedOn)
                {
                    Vector3 relativeEnemyPositon = player.transform.InverseTransformPoint(avaliableTargets[j].transform.position);
                    var distanceFromLeftTarget = relativeEnemyPositon.x;
                    var distanceFromRightTarget = relativeEnemyPositon.x;

                    if (avaliableTargets[j] == player.playerAttackAndWeaponManager.currentTarget)
                        continue;


                    if (relativeEnemyPositon.x <= 0.00 && distanceFromLeftTarget > shortDistanceOfLeftTarget)
                    {
                        shortDistanceOfLeftTarget = distanceFromLeftTarget;
                        leftLockOnTarget = avaliableTargets[j];
                    }
                    else if (relativeEnemyPositon.x >= 0.00 && distanceFromRightTarget < shortDistanceOfRightTarget)
                    {
                        shortDistanceOfRightTarget = distanceFromRightTarget;
                        rightLockOnTarget = avaliableTargets[j];
                    }

                }
            }
            else
            {
                ClearLockOnTargets();
                player.playerAttackAndWeaponManager.isLockedOn = false;


            }
        }
        
    }
    public void ClearLockOnTargets()
    {
        nearestLockOnTarget = null;
        leftLockOnTarget = null;
        rightLockOnTarget = null;
        avaliableTargets.Clear();
    }

    public void SetLockCameraHeight()
    {
        if (cameraLockOnHeightCoroutine != null) 
        {
            StopCoroutine(cameraLockOnHeightCoroutine);
        }


        cameraLockOnHeightCoroutine = StartCoroutine(SetCameraHeight());
    }
    public IEnumerator WaitThenFindNewTarget()
    {
        while (player.isInteracting)
        {
            yield return null;
        }

        ClearLockOnTargets();
        HandleLocatingLockOnTargets();

        if (nearestLockOnTarget != null && !player.playerAttackAndWeaponManager.isLockedOn)
        {
            player.playerAttackAndWeaponManager.SetTarget(nearestLockOnTarget);
            player.playerAttackAndWeaponManager.isLockedOn = true;
        }

        yield return null;
    }

    private IEnumerator SetCameraHeight()
    {
        float duration = 1;
        float timer = 0;

        Vector3 velocity = Vector3.zero;
        Vector3 newLockedOnCameraHeight = new Vector3(cameraPivot.transform.localPosition.x, lockedOnCameraHeight);
        Vector3 newUnlockedCameraHeight = new Vector3(cameraPivot.transform.localPosition.x, unlockedCameraHeight);

        while (timer < duration)
        {
            timer += Time.deltaTime;

            if (player != null)
            {
                if (player.playerAttackAndWeaponManager.currentTarget != null)
                {
                    cameraPivot.transform.localPosition = 
                        Vector3.SmoothDamp(cameraPivot.transform.localPosition, newLockedOnCameraHeight, ref velocity, lockOnFollowSpeed);
                    cameraPivot.transform.localRotation =
                        Quaternion.Slerp(cameraPivot.transform.localRotation, Quaternion.Euler(0, 0, 0), lockOnFollowSpeed);
                }
                else
                {
                    cameraPivot.transform.localPosition = 
                        Vector3.SmoothDamp(cameraPivot.transform.localPosition, newLockedOnCameraHeight, ref velocity, lockOnFollowSpeed);
                }
            }
            yield return null;
        }

        if (player != null)
        {
            if (player.playerAttackAndWeaponManager.currentTarget != null)
            {
                cameraPivot.transform.localPosition = newLockedOnCameraHeight;
                cameraPivot.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                cameraPivot.transform.localPosition = newUnlockedCameraHeight;
            }

            yield return null;
        }


    }

}
