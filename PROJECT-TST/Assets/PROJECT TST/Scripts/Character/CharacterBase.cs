using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Windows;
//using static TST.LootAnimation;
using static UnityEngine.Rendering.DebugUI;



namespace TST
{
    public enum EInteractionType
    {
        Looting = 0,
        Interaction = 1,
        OpenDoor = 2,
        None,
    }

    public class CharacterBase : MonoBehaviour/*, IDamage, IDetect*/
    {
        void OnDrawGizmos()
        {
            Color transparentRed = new Color(1f, 0f, 0f, 0.3f);
            Gizmos.color = transparentRed;
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z),
                groundedRadius);
        }

        public Vector3 AimingPosition
        {
            get => aimingPoint.position;
            set => aimingPoint.position = value;
        }

        public bool IsArmed
        {
            get => isArmed;
            set
            {
                isArmed = value;
                SetEquipWeapon(isArmed);
            }
        }

        public bool IsArmedCompleted => isArmedCompleted;

        private bool isArmed = false;
        private bool isArmedCompleted = false;

        public bool IsThrowMode
        {
            get => isThrowMode;
            set
            {
                isThrowMode = value;
                animator.SetBool("IsThrowMode", isThrowMode);

                if (isThrowMode)
                {
                    Transform handTransform = animator.GetBoneTransform(HumanBodyBones.LeftHand);
                    CurrentThrowObject = Instantiate(throwObject, handTransform);
                    CurrentThrowObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                    CurrentThrowObject.gameObject.SetActive(true);
                }
                else
                {
                    Destroy(CurrentThrowObject);
                }
            }
        }
        private bool isThrowMode = false;
        public Rigidbody CurrentThrowObject { get; private set; }
        public Rigidbody throwObject;
        public Transform throwStartPoint;

        public Animator animator;
        public UnityEngine.CharacterController unityCharacterController;
        public CharacterController characterController;
        public Transform cameraPivot;
        public Rigidbody[] ragdollRigidbodies;

        public WeaponBase gunWeapon;
        public WeaponBase grenadeWeapon;
        public Transform weaponSocket;
        public Transform weaponHolder;
        public Transform aimingPoint;

        //public Drone drone;

        public RigBuilder rigBuilder;
        public Rig aimingRig;
        public Rig lefthandRig;
        //public Rig throwRig;

        public Vector3 offsetPosition;
        public Vector3 offsetRotation;

        [Title("Character Stat")]
        public CharacterStat characterStat;

        private float horizontal;
        private float vertical;
        private float speedBlend;
        private float armedBlend;
        private float crouchBlend;

        private float targetSpeed;
        private float targetHorizontal;
        private float targetVertical;

        private bool isReloading = false;

        private float aimingRigWeightBlend;
        private float lefthandRigWeightBlend;

        public float rollSpeed = 4.0f;
        private float rollTime;
        public AnimationCurve rollSpeedCurve;

        public bool IsSprint
        {
            get => isSprint;
            set
            {
                isSprint = value;
            }
        }

        public bool IsAutoRunMode
        {
            get => isAutoRunMode;
            set
            {
                if (value == false)
                    IsWalk = false;
                isAutoRunMode = value;
            }
        }

        public bool IsWalk
        {
            get => isWalk;
            set
            {
                // 자동 달리기 모드일 때만
                //if (IsAutoRunMode)
                    isWalk = value;
            }
        }

        public bool IsZip
        {
            get => isZip;
            set => isZip = value;
        }

        [field : SerializeField] private bool isSprint = true;
        private bool isAutoRunMode = false;
        private bool isWalk = false;
        private bool isRolling = false;
        private bool isZip = false;
        private bool isCrouch = false;
        [field: SerializeField] Vector3 crouchOffset;

        private float targetRotation = 0f;


        // FOR AI // 클래스 분할 필요할듯?
        // Action 
        //public event System.Action<GameObject> OnDamaged;
        //public event System.Action<GameObject> OnDetect;
        //public event System.Action<GameObject> OnCombatDetect;
        //public event System.Action<GameObject> OnIdle;

        public Vector3 aiSpawnPosition;
        //
        private void Awake()
        {
            animator = GetComponent<Animator>();
            unityCharacterController = GetComponent<UnityEngine.CharacterController>();
            characterController = GetComponent<CharacterController>();
            ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
            SetRagdollActive(false);

            // AI 관련코드 이거 추후에 클래스 나누는 리팩토링 작업이 필요할듯함
            aiSpawnPosition = gameObject.transform.position;
        }

        public void SetRagdollActive(bool isActive)
        {
            foreach (var rb in ragdollRigidbodies)
            {
                rb.isKinematic = !isActive;
            }
      
            animator.enabled = !isActive;
            unityCharacterController.enabled = !isActive;
        }

        
        public void SetIKActive(bool isActive)
        {
            float value = isActive ? 1f : 0f;
            aimingRigWeightBlend = value;
            lefthandRigWeightBlend = value;
            //throwRig.weight = value;
        }


        private void Start()
        {
            aimingRig.weight = 0f;
            lefthandRig.weight = 0f;
            //throwRig.weight = 0f;
            rigBuilder.Build();

            // 데이터 관련
            //Initialize();
        }

        IngamePlayerDataDTO ingamePlayerData;
        public void Initialize()
        {
            // TODO : 데이터 관련
            // 일단 데이터 받기부터
            var ingameData = UserDataModel.Singleton.IngamePlayerData;
            // TODO : 추후 이걸 spawn 하는 풀링 클래스 or Object 관리 클래스에서 데이터 세팅 하는 방식으로 변경 해야함
            ingamePlayerData = ingameData[1001];
            transform.position = ingamePlayerData.VecPosition;
            transform.rotation = ingamePlayerData.QuatRotation;
        }

        public void OnApplicationQuit()
        {
            // 일단 임시 잠거둠 AI 관련 위치로 인해 

            //// 흠 이거는 추후 프로퍼티로 값이 변환이 생긴다면 데이터를 전송하는 식으로 변경 해야할듯?
            //ingamePlayerData.VecPosition = transform.position;
            //ingamePlayerData.QuatRotation = transform.rotation;
            //// 플레이어 번호 ID
            //UserDataModel.Singleton.ChangeData<IngamePlayerDataDTO>(1001, ingamePlayerData);
        }


        private void Update()
        {
            JumpAndGravity();
            FreeFall();
            CheckGround();

            armedBlend = Mathf.Lerp(armedBlend, IsArmed ? 1f : 0f, Time.deltaTime * 10f);
            speedBlend = Mathf.Lerp(speedBlend, targetSpeed, Time.deltaTime * 10f);
            horizontal = Mathf.Lerp(horizontal, targetHorizontal, Time.deltaTime * 10f);
            vertical = Mathf.Lerp(vertical, targetVertical, Time.deltaTime * 10f);
            crouchBlend = Mathf.Lerp(crouchBlend, isCrouch ? 1f : 0f, Time.deltaTime * 10.0f);

            animator.SetFloat("Armed", armedBlend);
            animator.SetFloat("Speed", speedBlend);
            animator.SetFloat("Horizontal", horizontal);
            animator.SetFloat("Vertical", vertical);
            animator.SetFloat("Crouch", crouchBlend);

            if (isRolling)
                StartRoll();
        }

        private void LateUpdate()
        {
            aimingRigWeightBlend = Mathf.Lerp(aimingRigWeightBlend, (isArmedCompleted && !isRolling) ? 1f : 0f, Time.deltaTime * 10f);
            aimingRig.weight = aimingRigWeightBlend;

            lefthandRigWeightBlend = Mathf.Lerp(lefthandRigWeightBlend, isArmedCompleted && !isReloading && !isRolling ? 1f : 0f, Time.deltaTime * 10f);
            lefthandRig.weight = lefthandRigWeightBlend;

            //throwRig.weight = IsThrowMode ? 1f : 0f;

            // 문 여닫이 IK 관련
            if (isDoorOpening)
                SetIKActive(IKWeightValue);
        }

        // 이것도 virtual 키워드로 바꿔야할듯
        public void AIMove(bool isMove)
        {
            float result = isMove ? 1.0f : 0.0f;
            animator.SetFloat("Magnitude", result);
        }

        public void Move(Vector2 input, float yAxisAngle)
        {
            if (isZip)
            {
                targetSpeed = isWalk ? 0.0f : 2.1f;
                animator.SetFloat("Magnitude", 0.0f);
                return;
            }

            Vector3 movement = Vector3.zero;
            if (input.magnitude > 0f)   
            {
                if (!IsArmed)
                {
                    Vector3 inputDirection = new Vector3(input.x, 0, input.y);
                    targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + yAxisAngle;
                    transform.rotation = Quaternion.Euler(0f, targetRotation, 0f);
                }

                if (IsArmed)
                {
                    targetHorizontal = input.x;
                    targetVertical = input.y;
                    movement = (transform.forward * input.y + transform.right * input.x) * characterStat.moveSpeed * Time.deltaTime;
                }
                else
                {
                    targetVertical = 1f;
                    movement = transform.forward * characterStat.moveSpeed * Time.deltaTime;
                }

                targetSpeed = isWalk? 0.0f : 2.1f;
            }
            else
            {
                targetHorizontal = 0f;
                targetVertical = 0f;
            }

            //if (input.magnitude > 0f || IsAutoRunMode)
            //{
            //    // 자동달리기 켜져있으면 일단 스프린트 모드 On
            //    targetSpeed = IsSprint ? 1.0f : 0.0f;
            //    targetHorizontal = input.x;
            //    targetVertical = IsAutoRunMode ? 1.0f : input.y;

            //    Vector3 movement =  (transform.forward * targetVertical + transform.right * targetHorizontal) 
            //        * (IsSprint ? sprintSpeed : moveSpeed) * Time.deltaTime;
            //    unityCharacterController.Move(movement);
            //}
            //else
            //{
            //    IsSprint = true;
            //    targetSpeed = IsAutoRunMode ? targetSpeed : 0f;
            //    targetHorizontal = IsAutoRunMode ? targetHorizontal : 0f;
            //    targetVertical = IsAutoRunMode ? targetVertical : 0f;
            //}

            movement.y += verticalVelocity * Time.deltaTime;
            unityCharacterController.Move(movement);

            if (!isAutoRunMode)
                animator.SetFloat("Magnitude", input.magnitude);
            else
                animator.SetFloat("Magnitude", 1.0f);
        }


        private void StartRoll()
        {
            rollTime += Time.deltaTime;
            Vector3 movement = (transform.forward * 1.0f + transform.right * 0.0f)
                    * (rollSpeed * rollSpeedCurve.Evaluate(rollTime) * Time.deltaTime);
            unityCharacterController.Move(movement);
        }

        public void Roll()
        {
            if (BehaviorExceptionCheck())
                return;
            
            if (!isRolling)
            {
                animator.SetTrigger("Roll Trigger");
                isRolling = true;
            }
        }

        public void Crouch()
        {
            if (BehaviorExceptionCheck())
                return;

            // 카메라 위치를 조금 낮춥시다
            if (!isCrouch)
            {
                CameraSystem.Instance.SetCrouchOffSet(crouchOffset);
                animator.SetFloat("Crouch", 1.0f);
            }
            else
            {
                CameraSystem.Instance.SetCrouchOffSet(Vector3.zero);
                animator.SetFloat("Crouch", 0.0f);
            }

            isCrouch = !isCrouch;
        }

        private bool BehaviorExceptionCheck()
        {
            if (isLoot)
                return true;
            if (isZip)
                return true;

            return false;
        }

        public bool Rotate(Vector3 targetPoint)
        {
            // 타겟은 일단 에이밍 걸린 포인트이다
            if (isRolling)
                return false;

            // 내적 = 각 벡터의 길이 * cos세타
            
            if (IsArmed)
            {
                Vector3 target = targetPoint;
                target.y = transform.position.y;
                Vector3 pos = transform.position;
                Vector3 direction = (target - pos).normalized;

                Vector3 viewForward = Camera.main.transform.forward;
                viewForward.y = 0.0f;

                float dotResult = Vector3.Dot(viewForward, direction);
                // 내적값이 음수가 나오면 forward를 카메라 정면 방향으로 변경
                // targetPoint와 플레이어의 거리에 따라 예외처리가 필요할지..?
                if (dotResult < 0.9)
                {
                    transform.rotation = Quaternion.LookRotation(Vector3.Lerp(transform.forward, viewForward, Time.deltaTime * 10f));
                    return false;
                }

                transform.rotation = Quaternion.LookRotation(Vector3.Lerp(transform.forward, viewForward, Time.deltaTime * 10f));
            }

            return true;
        }

        public void AIShoot()
        {
            if (isLoot)
                return;
            if (isRolling)
                return;

            if (isThrowMode)
            {
                Throw();
            }
            else
            {
                if (IsArmed && isArmedCompleted)
                {
                    bool isFireSuccess = gunWeapon.Fire();
                    if (!isFireSuccess && gunWeapon.CurrentAmmo <= 0)
                    {
                        Reload();
                        return;
                    }
                }
            }
        }

        public void Shoot()
        {
            if (isLoot)
                return;
            if (isRolling)
                return;

            if (isThrowMode)
            {
                Throw();
            }
            else
            {
                if (IsArmed && isArmedCompleted)
                {
                    bool isFireSuccess = gunWeapon.Fire();
                    if (!isFireSuccess && gunWeapon.CurrentAmmo <= 0)
                    {
                        Reload();
                        characterController.PauseRecoil();
                        return;
                    }

                    if (isFireSuccess)
                        characterController.AddRecoil();
                }
            }
        }

        private void Throw()
        {
            if (isLoot)
                return;
            if (!isThrowMode)
                return;

            animator.SetTrigger("Throw Trigger");
            CurrentThrowObject.transform.SetParent(null);
            CurrentThrowObject.transform.position = throwStartPoint.position;
            CurrentThrowObject.isKinematic = false;
            CurrentThrowObject.AddForce(transform.forward * 50, ForceMode.Impulse);
        }

        public bool isLoot = false;
        public void SetLootType(float lootType)
        {
            animator.SetFloat("Loot Type", lootType);
        }

        public void SetLootisSucceed(bool isSucceed)
        {
            animator.SetBool("IsLootSucceed", isSucceed);
        }

        //public void SetLootInteractAnimation(ELootState state)
        //{
        //    isLoot = state != ELootState.None ? true : false;

        //    animator.SetFloat("Loot State", (float)state);
        //}

        public void SetInteractAnimation(EInteractionType interactType)
        {
            animator.SetFloat("Interaction Type", (float)interactType);

            animator.SetTrigger("Interaction Trigger");
        }

        //public void DroneSetting()
        //{
        //    drone.IsShowing = !drone.IsShowing;
        //    drone.SetOwner(this.gameObject);
        //}

        public void MeleeAttack()
        {

        }

        public void ShootFinished()
        {
            characterController.PauseRecoil();
        }

        public void LootFinished()
        {
            //SetLootType((float)ELootType.None);
            SetLootisSucceed(false);
        }

        public void Reload()
        {
            if (isLoot)
                return;

            if (!isReloading && gunWeapon.CurrentAmmo != gunWeapon.clipSize)
            {
                isReloading = true;
                animator.SetTrigger("Reload Trigger");
            }
        }

        public void SetReloadComplete()
        {
            gunWeapon.Reload();
            isReloading = false;
        }

        private void SetEquipWeapon(bool isArmed)
        {
            if (isArmed)
            {
                animator.SetTrigger("Equip Trigger");
            }
            else
            {
                animator.SetTrigger("Holster Trigger");
            }
        }

        public void SetEquipmentVisual(int activated)
        {
            if (activated == 1)
            {
                gunWeapon.transform.SetParent(weaponHolder);
                gunWeapon.transform.localPosition = offsetPosition;
                gunWeapon.transform.localRotation = Quaternion.Euler(offsetRotation);
            }
            else
            {
                gunWeapon.transform.SetParent(weaponSocket);
                gunWeapon.transform.localPosition = Vector3.zero;
                gunWeapon.transform.localRotation = Quaternion.identity;
            }
        }

        public void RollingFinished(int flag)
        {
            isRolling = false;
            rollTime = 0.0f;
        }

        public void SetArmedComplete(int flag)
        {
            isArmedCompleted = flag > 0;
        }

        bool IKWeightValue = false;
        bool isDoorOpening = false;
        public void SetIKWeight(int flag)
        {
            if (isArmed)
            {
                IKWeightValue = flag > 0;
                isDoorOpening = flag < 1;
            }
        }

        public float jumpHeight = 1.2f;          // JumpHeight : 점프력 최대 올라갈 수 있는 높이.
        public float gravity = -15.0f;           // Gravity : Rigidbody를 사용하지 않기 때문에, 별도 중력 값
        public float jumpTimeout = 0.3f;         // JumpTimeout : 점프 후 - 다시 점프 입력을 받을 수 있는 텀[:시간]
        public float fallTimeout = 0.15f;        // FallTimeout : 점프가 아닌, 절벽에서 떨어지는 경우, 떨어지는 중력을 적용받기까지의 텀[:시간]
        public float terminalVelocity = 53.0f;   // terminalVelocity : 최대 속도 For 점프하는 가속도에 영향을 준다.
        public float groundedOffset = -0.14f;
        public float groundedRadius = 0.28f;
        public LayerMask groundLayer;
        private float jumpTimeoutDelta;
        private float fallTimeoutDelta;
        private float verticalVelocity;
        private bool isJumping = false;
        private bool isGrounded = false;

        private int jumpCnt = 0;
        private int jumpMaxCnt = 2;
        public void Jump()
        {
            if ((!isJumping /*&& isGrounded*/))
            {
                if (jumpCnt >= jumpMaxCnt)
                    return;

                isJumping = true;
                //if (jumpCnt <= 0)
                    animator.SetTrigger("Jump Trigger");
                jumpCnt++;
            }
        }
        private void JumpAndGravity()
        {
            if (isGrounded)
            {
                if (verticalVelocity < 0f)
                {
                    verticalVelocity = -2f;
                }
                if (isJumping && jumpTimeoutDelta <= 0.0f) // 이쪽 관련 코드가 점프와 연관되어 있음 // 더블 점프도 이쪽에서 컨트롤 하면 문제 없을듯함
                {
                    jumpTimeoutDelta = jumpTimeout;
                    verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                    isJumping = false;
                }
                if (jumpTimeoutDelta >= 0f)
                {
                    jumpTimeoutDelta -= Time.deltaTime;
                }

                if (jumpCnt >= jumpMaxCnt)
                {
                    jumpCnt = 0;
                }
            }
            else
            {
                if (isJumping == true && jumpCnt <= jumpMaxCnt)
                {
                    verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                }

                jumpTimeoutDelta = jumpTimeout;
                jumpTimeoutDelta = jumpTimeout;
                isJumping = false;
            }
            if (verticalVelocity < terminalVelocity)
            {
                verticalVelocity += gravity * Time.deltaTime;
            }
        }
        private void FreeFall()
        {
            if (isGrounded)
            {
                fallTimeoutDelta = fallTimeout;
                animator.SetBool("IsFreeFall", false);
            }
            else
            {
                if (fallTimeoutDelta >= 0f)
                {
                    fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    if (false == animator.GetBool("IsFreeFall"))
                    {
                        animator.SetBool("IsFreeFall", true);
                    }
                }
            }
        }
        private void CheckGround()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z);
            isGrounded = Physics.CheckSphere(spherePosition, groundedRadius, groundLayer, QueryTriggerInteraction.Ignore);
            animator.SetBool("IsGrounded", isGrounded);
        }
    }
}
        //// 데미지를 입거나, Combat Range에 들어오면 전투
        //public void ApplyDamage(float damage, GameObject target)
        //{
        //    OnDamaged?.Invoke(target);
        //}

        //public void Detect(GameObject target)
        //{
        //    OnDetect?.Invoke(target);
        //}

        //public void UnDetect(GameObject target)
        //{
        //    OnIdle?.Invoke(target);
        //}

        //public void CombatDetect(GameObject target)
        //{
        //    OnCombatDetect?.Invoke(target);
        //}
