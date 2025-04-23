using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Rendering;

namespace TST
{
    public enum EInteractionType
    {
        Looting = 0,
        Interaction = 1,
        OpenDoor = 2,
        None,
    }

    [RequireComponent(typeof(EventHandler))]
    public class CharacterBase : MonoBehaviour, IDamage /*IDetect*/
    {
        void OnDrawGizmos()
        {
            Color transparentRed = new Color(1f, 0f, 0f, 0.3f);
            Gizmos.color = transparentRed;
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z),
                groundedRadius);
        }

        public bool IsActiveLeftHandIK { get => isActiveLeftHandIK; set => isActiveLeftHandIK = value; }
        private bool isActiveLeftHandIK = true;

        public bool IsActiveRightHandIK { get => isActiveRightHandIK; set => isActiveRightHandIK = value; }
        private bool isActiveRightHandIK = true;

        public bool IsActiveBodyIK { get => isActiveBodyIK; set => isActiveBodyIK = value; }
        private bool isActiveBodyIK = true;

        [SerializeField] private MultiAimConstraint multiAimConstraint_RightHand;
        [SerializeField] private MultiAimConstraint multiAimConstraint_Body;
        [SerializeField] private TwoBoneIKConstraint multiAimConstraint_LeftHand;

        #region Armed Status
        [Title("Armed Status", titleAlignment: TitleAlignments.Centered)]
        public bool IsArmed => currentWeapon != null;
        public bool IsArmedCompleted => isArmedCompleted;
        private bool isArmedCompleted = false;

        private bool isThrowMode = false;

        public bool IsSwitchingWeapon => isSwitchingWeapon;
        private bool isSwitchingWeapon = false;

        public BombProjectile currentThrowObject; 
        public BombProjectile throwObject;
        public Transform throwStartPoint;
        #endregion

        public Animator animator;
        public UnityEngine.CharacterController unityCharacterController;
        public CharacterController characterController;
        public Transform cameraPivot;
        public Transform fpsCameraPivot;

        #region Weapons & AimingPoint & Weapon Socket
        [Title("Weapons & AimingPoint", titleAlignment: TitleAlignments.Centered)]
        public WeaponBase primaryWeapon;
        public WeaponBase subWeapon;
        public WeaponBase grenadeWeapon;

        public WeaponBase currentWeapon;  // 코드상에서 자동으로 통제하는 변수
        public WeaponBase weaponToEquip;  // 코드상에서 자동으로 통제하는 변수

        public List<GameObject> fpsModeVisualObjects = new List<GameObject>();
        public Transform weaponSocket;
        public Transform subWeaponSocket;
        public Transform weaponHolder;
        public Transform aimingPoint;
        #endregion

        private bool isReloading = false;

        #region Rendering Volume
        [Title("Rendering Volume", titleAlignment: TitleAlignments.Centered)]
        public GameObject hitVolumeObject;
        public UnityEngine.Rendering.Volume hitVolume;
        #endregion
        public Drone drone;

        #region Rig & IK
        [Title("Rig & IK", titleAlignment: TitleAlignments.Centered)]
        public Rigidbody[] ragdollRigidbodies;
        public RigBuilder rigBuilder;
        public Rig aimingRig;
        public Rig lefthandRig;
        public GameObject multiParent;
        public Transform leftHandTarget;
        public Transform leftHandHint;
        //public Rig throwRig;

        public Vector3 offsetPosition;
        public Vector3 offsetRotation;
        public Vector3 subOffsetPosition;
        public Vector3 subOffsetRotation;
        #endregion

        #region Character Status
        [Title("Character Status", titleAlignment: TitleAlignments.Centered)]

        [SerializeField] private CharacterStat currentStat;
        [SerializeField] private CharacterStat maxStat;
        [field: SerializeField] private CharacterStatSetting CharacterStatConfig { get; set; }

        public float CurrentDamage { get => currentStat.damamge;
            set
            {
                currentStat.damamge = value;

                if (currentStat.damamge <= 0f)
                {
                    currentStat.damamge = 0f;
                }
                else if (currentStat.damamge >= maxStat.damamge)
                {
                    currentStat.damamge = maxStat.damamge;
                }
            }
        }

        public float CurrentDefence { get => currentStat.defense;
            set
            {
                currentStat.defense = value;

                if (currentStat.defense <= 0f)
                {
                    currentStat.defense = 0f;
                }
                else if (currentStat.defense >= maxStat.defense)
                {
                    currentStat.defense = maxStat.defense;
                }
            }
        }

        public float CurrentHp { get => currentStat.hp; 
            set
            {
                currentStat.hp = value;

                if (currentStat.hp <= 0f)
                {
                    currentStat.hp = 0f;
                    SetRagdollActive(true);
                }
                else if (currentStat.hp >= maxStat.hp)
                {
                    currentStat.hp = maxStat.hp;
                    SetRagdollActive(false);
                }
                else if (currentStat.hp > 0f)
                {
                    SetRagdollActive(false);
                }
            }
        }

        public float CurrentSpeed 
        {
            get
            {
                if (isWalk)
                    return currentStat.walkSpeed;
                else
                    return currentStat.runSpeed;
                
            }
            set
            {
                if (isWalk && currentStat.walkSpeed != value)
                {
                    if (value >= 0 && value <= maxStat.walkSpeed)
                    {
                        currentStat.walkSpeed = value;
                    }
                }
                else if (isWalk == false && currentStat.runSpeed != value)
                {
                    if (value >= 0 && value <= maxStat.runSpeed)
                    {
                        currentStat.runSpeed = value;
                    }
                }
            }
        }

        public float MaxHp { get => maxStat.hp;
            private set { }
        }

        public float rollSpeed = 4.0f;
        private float rollTime;
        public AnimationCurve rollSpeedCurve;
        #endregion

        #region Blend Member Variable
        [Title("Blend Member Variable", titleAlignment: TitleAlignments.Centered)]
        private float horizontal;
        private float vertical;
        private float speedBlend;
        private float idleBlend;
        private float armedBlend;
        private float crouchBlend;

        private float targetSpeed;
        private float targetHorizontal;
        private float targetVertical;

        private float aimingRigWeightBlend;
        private float lefthandRigWeightBlend;
        #endregion

        #region Property
        [Title("Property", titleAlignment: TitleAlignments.Centered)]
        public Vector3 AimingPosition
        {
            get => aimingPoint.position;
            set => aimingPoint.position = value;
        }

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
                    currentThrowObject = Instantiate(throwObject, handTransform);
                    currentThrowObject.SetStartTransform(throwStartPoint);
                    currentThrowObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                    currentThrowObject.gameObject.SetActive(true);
                }
                else
                {
                    Destroy(currentThrowObject);
                }
            }
        }

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
        #endregion

        #region Boolean & Crouch
        [Title("Boolean & Crouch", titleAlignment: TitleAlignments.Centered)]
        [field: SerializeField] private bool isSprint = true;
        private bool isAutoRunMode = false;
        private bool isWalk = false;
        private bool isRolling = false;
        private bool isZip = false;
        private bool isCrouch = false;
        [field: SerializeField] Vector3 crouchOffset;
        #endregion

        private float targetRotation = 0f;
        IngamePlayerDataDTO ingamePlayerData;

        #region Ammo
        [Title("Ammo", titleAlignment: TitleAlignments.Centered)]

        public List<AmmoBase> rifleAmmos;
        public List<AmmoBase> pistolAmmos;

        #region event
        public event Action onWeaponSwap;
        #endregion

        public AmmoBase SetRifleAmmo(AmmoBase ammo) // 이쪽 부분은 추후 인벤 개념 들어가면 구도를 좀 바꿔야함
        {
            for (int i = 0; i < rifleAmmos.Count; i++)
            {
                if (rifleAmmos[i] == ammo)
                    continue;

                if (rifleAmmos[i].CurrentBulletAmount > 0)
                    return rifleAmmos[i];
            }

            return null;
        }

        public AmmoBase SetPistolAmmo(AmmoBase ammo)
        {
            for (int i = 0; i < pistolAmmos.Count; i++)
            {
                if (pistolAmmos[i] == ammo)
                    continue;

                if (pistolAmmos[i].CurrentBulletAmount > 0)
                    return pistolAmmos[i];
            }

            return null;
        }

        private void InitAmmos()
        {
            for (int i = 0; i < rifleAmmos.Count; i++)
            {
                rifleAmmos[i].Initialize();
            }

            for (int i = 0; i < pistolAmmos.Count; i++)
            {
                pistolAmmos[i].Initialize();
            }
        }
        #endregion

        List<CharacterSkillBase> characterSkills = new List<CharacterSkillBase>();

        private void Awake()
        {
            rifleAmmos = new List<AmmoBase>();
            pistolAmmos = new List<AmmoBase>();

            AssetManager.Singleton.GetItemAmmoPrefab("APC Ammo", out GameObject rifleResult);
            AssetManager.Singleton.GetItemAmmoPrefab("Pistol Bullet", out GameObject pistolResult);


            GameObject rifleBullet = Instantiate(rifleResult, transform);
            GameObject pistolBullet = Instantiate(pistolResult);

            rifleAmmos.Add(rifleBullet.GetComponent<AmmoBase>());
            pistolAmmos.Add(pistolBullet.GetComponent<AmmoBase>());

            InitAmmos();
            primaryWeapon.SetPlayerAmmo_Event += SetRifleAmmo;
            primaryWeapon.InitializeWeapon(rifleAmmos);
            subWeapon.SetPlayerAmmo_Event += SetPistolAmmo;
            subWeapon.InitializeWeapon(pistolAmmos);

            // 추후에 이걸 클래스화로 나누자
            maxStat = CharacterStatConfig.CharacterStatData.Max;
            currentStat = CharacterStatConfig.CharacterStatData.Base;

            animator = GetComponent<Animator>();
            unityCharacterController = GetComponent<UnityEngine.CharacterController>();
            characterController = GetComponent<CharacterController>();
            ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
            eventHandler = GetComponent<EventHandler>();

            SetRagdollActive(false);


            //hitVolume = hitVolumeObject.GetComponent<Volume>();

            // AI 관련코드 이거 추후에 클래스 나누는 리팩토링 작업이 필요할듯함
            //aiSpawnPosition = gameObject.transform.position;
        }

        #region Ragdoll & IK
        public void SetRagdollActive(bool isActive)
        {
            foreach (var rb in ragdollRigidbodies)
            {
                rb.isKinematic = !isActive;
            }

            animator.enabled = !isActive;
            if (unityCharacterController)
                unityCharacterController.enabled = !isActive;
        }

        public void SetIKActive(bool isActive)
        {
            float value = isActive ? 1f : 0f;
            aimingRigWeightBlend = value;
            lefthandRigWeightBlend = value;
            //throwRig.weight = value;
        }
        #endregion
  
        private void Start()
        {
            aimingRig.weight = 0f;
            lefthandRig.weight = 0f;
            //throwRig.weight = 0f;
            rigBuilder.Build();
            SubscribeEventActions();
            // 데이터 관련
            //Initialize();
        }

        
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
            if (hitVolume != null)
            {
                //multiAimConstraint_Body.weight = isActiveBodyIK ? 1f : 0f;
                //multiAimConstraint_RightHand.weight = isActiveRightHandIK ? 1f : 0f;
                //multiAimConstraint_LeftHand.weight = isActiveLeftHandIK ? 1f : 0f;
                hitVolume.weight = effectVolumeBlend;
            }

            JumpAndGravity();
            FreeFall();
            CheckGround();
            CheckHitEffectVolume();

            CheckPlayerStatus();

            float targetIdleBlend = currentWeapon == null ? 0f : (float)currentWeapon.WeaponType;
            idleBlend = Mathf.Lerp(idleBlend, targetIdleBlend, Time.deltaTime * 10f);

            armedBlend = Mathf.Lerp(armedBlend, IsArmed ? 1f : 0f, Time.deltaTime * 10f);
            speedBlend = Mathf.Lerp(speedBlend, targetSpeed, Time.deltaTime * 10f);
            horizontal = Mathf.Lerp(horizontal, targetHorizontal, Time.deltaTime * 10f);
            vertical = Mathf.Lerp(vertical, targetVertical, Time.deltaTime * 10f);
            crouchBlend = Mathf.Lerp(crouchBlend, isCrouch ? 1f : 0f, Time.deltaTime * 10.0f);

            animator.SetFloat("Idle Blend", idleBlend);
            animator.SetFloat("Armed", armedBlend);
            animator.SetFloat("Speed", speedBlend);
            animator.SetFloat("Horizontal", horizontal);
            animator.SetFloat("Vertical", vertical);
            animator.SetFloat("Crouch", crouchBlend);

            if (isRolling)
                StartRoll();

            #region Legacy
            for (int i = 0; i < characterSkills.Count; i++)
            {

            }
            #endregion
        }

        private void CheckPlayerStatus()
        {
            if (currentWeapon == null)
                return;
            
            currentStat.currentBullet = currentWeapon.WeaponCurrentBulletAmount;
            currentStat.maxBullet = currentWeapon.MaxBulletAmount;
        }

        // ArmedComplete대신 함수로 하나 빼서 작업하자 // IsAimingRigFunctable 같은 거로 생성하자
        private void LateUpdate()
        {
            SetHandsIK();

            // 문 여닫이 IK 관련
            if (isDoorOpening || isReloading)
                SetIKActive(IKWeightValue);
        }

        private void SetHandsIK()
        {
            //aimingRigWeightBlend = CheckIKSuccess() ? 1f : 0f;
            aimingRigWeightBlend = Mathf.Lerp(aimingRigWeightBlend, CheckIKSuccess() ? 1f : 0f, Time.deltaTime * 30f);
            aimingRig.weight = aimingRigWeightBlend;

            lefthandRigWeightBlend = Mathf.Lerp(lefthandRigWeightBlend, CheckIKSuccess() ? 1f : 0f, Time.deltaTime * 10f);
            lefthandRig.weight = lefthandRigWeightBlend;

            //throwRig.weight = isThrowMode ? 1f : 0f;
        }

        private bool CheckIKSuccess()
        {
            if (isReloading || !isArmedCompleted || isRolling || isThrowMode || !isGrounded)
                return false;

            return true;
        }

        public void AIMove(bool isMove)
        {
            float result = isMove ? 1.0f : 0.0f;
            animator.SetFloat("Magnitude", result);
        }

        public void Move(Vector2 input, float yAxisAngle)
        {
            if (isRolling)
                return;

            if (isZip)
            {
                targetSpeed = isWalk ? 0.0f : 2.1f;
                animator.SetFloat("Magnitude", 0.0f);
                return;
            }

            if (CurrentHp <= 0)
                return;

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
                    movement = (transform.forward * input.y + transform.right * input.x) * CurrentSpeed * Time.deltaTime;
                }
                else
                {
                    targetVertical = 1f;
                    movement = transform.forward * CurrentSpeed * Time.deltaTime;
                }

                targetSpeed = isWalk ? 0.0f : 2.1f;
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
                SetIKActive(false);
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

            if (CurrentHp <= 0)
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
                    bool isFireSuccess = currentWeapon.Fire();
                    if (!isFireSuccess && currentWeapon.WeaponCurrentBulletAmount <= 0)
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
                    bool isFireSuccess = currentWeapon.Fire();
                    
                    if (!isFireSuccess && currentWeapon.WeaponCurrentBulletAmount <= 0)
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
            if (currentThrowObject == null)
                return;


            animator.SetTrigger("Throw Trigger");
            animator.SetBool("IsThrown", true);
            currentThrowObject.transform.SetParent(null);
            currentThrowObject.transform.position = throwStartPoint.position;

            currentThrowObject.Throw(throwStartPoint.position, aimingPoint.position);
            currentThrowObject = null;
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

        public void DroneSetting()
        {
            drone.IsShowing = !drone.IsShowing;
            drone.SetOwner(this.gameObject);
        }

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

            if (currentWeapon == null)
                return;

            if (currentWeapon.WeaponCurrentBulletAmount == currentWeapon.MaxBulletAmount)
                return;

            // 틱틱 거리는 사운드를 추가할지 말지 고민
            //if (currentWeapon.LoadedAmmo.Count <= 0)
            //    return;

            if (!isReloading && currentWeapon.WeaponCurrentBulletAmount != currentWeapon.clipSize)
            {
                isReloading = true;
                //multiParent.SetActive(true);
                //rigBuilder.Build();
                currentWeapon.StartUnloadSound();
                animator.SetTrigger("Reload Trigger");
            }
        }

        public void SetReloadComplete()
        {
            currentWeapon.Reload();
            isReloading = false;
            //multiParent.SetActive(false);
        }

        private void SetEquipmentIKPosAndRotation(WeaponType weaponType)
        {
            Vector3 rotation = Vector3.zero;
            switch (weaponType)
            {
                case WeaponType.Rifle:
                    offsetPosition = new Vector3(0.217f, -0.032f, 0.023f);
                    offsetRotation = new Vector3(0f, -90, -90);

                    leftHandTarget.localPosition = new Vector3(0.276f, -0.051f, 0.018f);
                    rotation = new Vector3(-57.174f, 246.766f, -74.789f);
                    leftHandTarget.localRotation = Quaternion.Euler(rotation);

                    leftHandHint.localPosition = new Vector3(-0.684f, -0.727f, 0.078f);
                    rotation = new Vector3(7.882f, 9.891f, 44.927f);
                    leftHandHint.localRotation = Quaternion.Euler(rotation);

                    break;
                case WeaponType.Pistol:
                    offsetPosition = new Vector3(0.184f, -0.042f, 0.067f);
                    offsetRotation = new Vector3(0f, -90, -90);

                    leftHandTarget.localPosition = new Vector3(0.04f, -0.0849f, -0.0417f);
                    rotation = new Vector3(21.75f, 162.175f, 5.948f);
                    leftHandTarget.localRotation = Quaternion.Euler(rotation);

                    leftHandHint.localPosition = new Vector3(-0.113f, -0.272f, -0.087f);
                    rotation = new Vector3(7.882f, 9.891f, 44.927f);
                    leftHandHint.localRotation = Quaternion.Euler(rotation);
                    break;
            }
        }

        public void SetWeaponAttachToHand(WeaponBase weapon)
        {
            weapon.transform.SetParent(weaponHolder, false);
            weapon.transform.localPosition = offsetPosition;
            weapon.transform.localRotation = Quaternion.Euler(offsetRotation);
        }

        public void SetWeaponAttachToHolster(WeaponBase weapon)
        {
            switch (weapon.WeaponType)
            {
                case WeaponType.Rifle:
                    weapon.transform.SetParent(weaponSocket, false);
                    break;
                case WeaponType.Pistol:
                    weapon.transform.SetParent(subWeaponSocket, false);
                    break;
            }
            weapon.transform.localPosition = Vector3.zero;
            weapon.transform.localRotation = Quaternion.identity;
        }

        public void ToggleEquipPrimaryWeapon()
        {
            if (currentWeapon != null && currentWeapon == primaryWeapon)
            {
                HolsterWeapon();
                return;
            }

            weaponToEquip = primaryWeapon;
            if (currentWeapon != null)
            {
                HolsterWeapon();
            }
            else
            {
                isSwitchingWeapon = true;
                onWeaponSwap?.Invoke();
                switch (primaryWeapon.WeaponType)
                {
                    case WeaponType.Rifle:
                        animator.SetTrigger("Equip Trigger Rifle");
                        animator.SetFloat("Armed Type", (float)primaryWeapon.WeaponType);
                        break;
                    case WeaponType.Pistol:
                        animator.SetTrigger("Equip Trigger Pistol");
                        animator.SetFloat("Armed Type", (float)primaryWeapon.WeaponType);
                        break;
                }
            }
        }

        public void ToggleEquipSecondaryWeapon()
        {
            if (currentWeapon != null && currentWeapon == subWeapon)
            {
                HolsterWeapon();
                return;
            }

            weaponToEquip = subWeapon;
            if (currentWeapon != null)
            {
                HolsterWeapon();
            }
            else
            {
                isSwitchingWeapon = true;
                onWeaponSwap?.Invoke();
                switch (subWeapon.WeaponType)
                {
                    case WeaponType.Rifle:
                        animator.SetTrigger("Equip Trigger Rifle");
                        animator.SetFloat("Armed Type", (float)subWeapon.WeaponType);
                        break;
                    case WeaponType.Pistol:
                        animator.SetTrigger("Equip Trigger Pistol");
                        animator.SetFloat("Armed Type", (float)subWeapon.WeaponType);
                        break;
                }
            }
        }

        private void HolsterWeapon()
        {
            if (isSwitchingWeapon)
                return;

            if (currentWeapon != null)
            {
                onWeaponSwap?.Invoke();
                isSwitchingWeapon = true;
                switch (currentWeapon.WeaponType)
                {
                    case WeaponType.Rifle:
                        animator.SetTrigger("Holster Trigger Rifle");
                        break;
                    case WeaponType.Pistol:
                        animator.SetTrigger("Holster Trigger Pistol");
                        break;
                }
            }
        }

        /// <summary> Animator의 해당 모션의 Animation Event 를 통해서 호출되는 함수 </summary>
        private void OnEquip()
        {
            if (weaponToEquip != null)
            {
                currentWeapon = weaponToEquip;
                weaponToEquip = null;

                SetEquipmentIKPosAndRotation(currentWeapon.WeaponType);
                SetWeaponAttachToHand(currentWeapon);
            }
        }

        /// <summary> Animator의 해당 모션의 Animation Event 를 통해서 호출되는 함수 </summary>
        private void OnHolster()
        {
            if (currentWeapon != null)
            {
                isArmedCompleted = false;
                SetWeaponAttachToHolster(currentWeapon);
                currentWeapon = null;
            }

            if (weaponToEquip != null)
            {
                switch (weaponToEquip.WeaponType)
                {
                    case WeaponType.Rifle:
                        ToggleEquipPrimaryWeapon();
                        break;
                    case WeaponType.Pistol:
                        ToggleEquipSecondaryWeapon();
                        break;
                }
            }
        }

        public void ReloadStart()
        {
            currentWeapon.StartLoadSound();
        }

        public void EquipStart()
        {

        }

        public void HolsterStart()
        {
            isArmedCompleted = false;
        }

        /// <summary> Animator - StateMachineBehaviour 를 통해서 호출 됨 </summary>
        public void EquipFinished()
        {
            isArmedCompleted = true;
            isSwitchingWeapon = false;
        }

        /// <summary> Animator - StateMachineBehaviour 를 통해서 호출 됨 </summary>
        public void HolsterFinished()
        {
            isSwitchingWeapon = false;
        }

        public void JumpStart()
        {
            SetIKActive(false);
        }

        public void JumpFinished()
        {
            SetIKActive(true);
        }

        public void RollingFinished(int flag)
        {
            SetIKActive(true);
            isRolling = false;
            rollTime = 0.0f;
        }

        #region IKWeight
        [Title("IKWeight", titleAlignment: TitleAlignments.Centered)]
        private bool IKWeightValue = false;
        private bool isDoorOpening = false;
        public void SetIKWeight(int flag)
        {
            if (IsArmed)
            {
                IKWeightValue = flag > 0;
                isDoorOpening = flag < 1;
            }
        }
        #endregion

        #region Jump
        [Title("JumpStatus", titleAlignment: TitleAlignments.Centered)]
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


        #endregion

        [Title("EventHandler", titleAlignment: TitleAlignments.Centered)]
        public EventHandler eventHandler;

        public void ApplyDamage(float damage, GameObject attacker) // Interface
        {
            CheckIsHit(true);

            eventHandler.OnDamaged(damage, attacker);
        }

        // 히트 이벤트
        #region Hit Event 
        [Title("HitEvent", titleAlignment: TitleAlignments.Centered)]
        public float restorationTime = 3.0f;    // 3초 지나면 복구되는 시간
        private float hitTime = 0.0f;
        private bool isHit = false;
        public float effectVolumeBlend;

        private void CaculateDamage()
        {
            
        }

        private void SubscribeEventActions()
        {
            eventHandler.OnDamagedAction += CaculateDamage;
        }

        // hit가 됐으면 쿨 확인
        private bool CheckHitEffectVolume()
        {
            // 다시 되돌아 갈 수 있는지 확인
            if (CheckHitTime() == false) // false일 시 계속해서 히트 volume 커져있어야함
            {
                effectVolumeBlend = Mathf.Lerp(effectVolumeBlend, 0.5f, Time.deltaTime * 10.0f);
                return true;
            }
            else
            {
                effectVolumeBlend = Mathf.Lerp(effectVolumeBlend, 0f, Time.deltaTime * 10.0f);
                return false;
            }
        }

        // 원상태로 돌아가는 시간 측정 // 하지만 다시 맞는다는 판정을 어떻게 처리해야할까
        private bool CheckHitTime()
        {
            // 계속해서 시간을 더해줌
            hitTime += Time.deltaTime;

            if (restorationTime <= hitTime)
            {
                isHit = false;
                return true;
            }

            if (isHit == false)
                return true;

            return false;
        }

        private void CheckIsHit(bool isHitted)
        {
            // 너가 만약 첫 피격 판정을 당했다 히트 시간을 초기화 시킴
            if (isHit == false && isHitted == true)
            {
                hitTime = 0.0f;
            }

            // 만약 맞았는데 또 맞았네?
            if (isHit == true && isHitted == true)
            {
                hitTime = 0.0f;
            }

            isHit = isHitted;
        }
        #endregion

        public void ExecuteSkill(int index)
        {
            // 주의사항. 쿨타임 여부 확인.. 상태 라던가.. 체크 


            characterSkills[index].OnExecute(this);
        }

        public void RegisterSkill(int index, CharacterSkill_SlingShot characterSkill_SlingShot)
        {
            if (characterSkills.Count <=0 )
            {
                characterSkills.Add(characterSkill_SlingShot);
            }
            else
            {
                characterSkills[index] = characterSkill_SlingShot;
            }
        }
    }
}
