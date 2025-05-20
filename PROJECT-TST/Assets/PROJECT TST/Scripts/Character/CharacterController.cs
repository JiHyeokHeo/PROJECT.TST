using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public class CharacterController : MonoBehaviour
    {
        public static CharacterController Instance { get; private set;}

        public bool IsActiveControl_Movement { get; set; } = true;
        public bool IsActiveControl_Camera { get; set; } = true;
        public bool IsControlLocked { get; set; } = false;

        public CharacterBase linkedCharacter;
        public Transform cameraPivot;
        public Transform fpsCameraPivot;

        public LayerMask aimingLayer;
        public LineRenderer trajectoryRenderer;

        public float topClampLimit = 80;
        public float bottomClampLimit = -80;

        private float threshold = 0.01f;
        private float targetYaw;
        private float targetPitch;

        [SerializeField]
        private float recoilAmount = 10.0f;
        //private float recoilSpeed = 10.0f; 
        private float currentRecoil = 0.0f;

        private float recoilMaxThreshold = 20.0f;

        public event Action MainHudChangeEvent;
        private void Awake()
        {
            Instance = this;
            UIManager.Singleton.GetUI<IndicatorUI>(UIList.Indicator_UI);
            linkedCharacter = GetComponent<CharacterBase>();
            linkedCharacter.eventHandler.OnDamagedAction += OnLinkedCharacterDamaged;
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        private void OnLinkedCharacterDamaged(GameObject attacker)
        {
            if (linkedCharacter.CurrentHp <= 0)
            {
                GameManager.Instance.OnPlayerDead();
            }
        }

        private void Start()
        {
            MainHudUI mainHud = UIManager.Singleton.GetUI<MainHudUI>(UIList.MainHudUI);
            mainHud.SetLinkedCharacter(linkedCharacter);
            linkedCharacter.onWeaponSwap += mainHud.SetHud;
            linkedCharacter.eventHandler.OnPulseAction += mainHud.SetPulse;
            mainHud.SetHud();
            linkedCharacter.eventHandler.OnDamagedAction += (_) =>mainHud.SetHpTextImage();
            MainHudChangeEvent += mainHud.SetBulletTextImage;
            mainHud.SetBulletTextImage();
            mainHud.SetHpTextImage();

            //transform.position = UserDataModel.Singleton.IngamePlayerData.Values[0].Position;
            //transform.rotation = UserDataModel.Singleton.IngamePlayerData.PlayerRotation;
            InputSystem.Singleton.OnInput_Jump += OnExecuteJump;
            InputSystem.Singleton.OnInput_MainWeapon += OnExecuteMainWeaponSwap;
            InputSystem.Singleton.OnInput_SubWeapon += OnExecuteSubWeaponSwap;
            InputSystem.Singleton.OnInput_ToggleFpsRightButtonTransition += OnExcuteFpsZoomTransition;
            InputSystem.Singleton.OnInput_MaintainZoom += OnExecuteMaintainZoom;
            InputSystem.Singleton.OnInput_ReturnToTps += OnExecuteReturnToTps;
            InputSystem.Singleton.OnInput_InventoryToggle += OnExecuteInventoryUI;
            InputSystem.Singleton.OnInput_EquipmentToggle += OnExecutePlayerEquipmentUI;
            InputSystem.Singleton.OnInput_Roll += OnExecutePlayerRoll;
            InputSystem.Singleton.OnInput_Interact += OnExectuePlayerInteract;
            InputSystem.Singleton.OnInput_PlayerThirdViewRightLeftChange += OnExecuteThirdRightLeftViewChange;
            InputSystem.Singleton.OnInput_Shoot += OnExecuteShoot;
            InputSystem.Singleton.OnInput_ShootFinish += FinishShoot;
            InputSystem.Singleton.OnInput_Reload += OnExecuteReload;
            InputSystem.Singleton.OnInput_Crouch += OnExecuteCrouch;
            InputSystem.Singleton.OnInput_WorldMap += OnExecuteWorldMap;
            InputSystem.Singleton.OnInput_ShortCutItemUse += OnExecutePlayerShortCutItem;
           // += CommandExecuteSkill // input 연동

            InventoryUI inventoryUI = UIManager.Singleton.GetUI<InventoryUI>(UIList.InventoryUI);
            inventoryUI.SetLinkedCharacter(linkedCharacter);

            //PlayerEquipmentUI equipmentUI = UIManager.Singleton.GetUI<PlayerEquipmentUI>(UIList.PlayerEquipmentUI);


            #region 캐릭터 총기 관련
  
            #endregion

            #region Skill 
            //GameDataModel.Singleton.GetSkillData("SlingShot", out SkillData slingShotData);
            //linkedCharacter.RegisterSkill(0, new CharacterSkill_SlingShot(slingShotData));
            #endregion
        }

        void OnExecuteWorldMap()
        {
            CameraSystem.Instance.IsActiveWorldCamera = !CameraSystem.Instance.IsActiveWorldCamera;
            // World Map 화면과 관련된 UI를 Show / Hide 하기.
        }

        void OnExecuteJump()
        {
            linkedCharacter.Jump();
        }

        void OnExecutePlayerRoll()
        {
            linkedCharacter.Roll();
        }

        void OnExecuteShoot()
        {
            if (IsControlLocked == false)
            {
                linkedCharacter.Shoot();
                MainHudChangeEvent?.Invoke();
            }
        }

        void OnExecuteReload()
        {
            linkedCharacter.Reload();
        }

        void OnExecuteCrouch()
        {
            linkedCharacter.IsCrouch = !linkedCharacter.IsCrouch;
        }

        void FinishShoot()
        {
            linkedCharacter.ShootFinished();
        }

        void OnExectuePlayerInteract()
        {
            if (linkedCharacter.IsLooting)
                return;

            if (currentInteractables.Count > 0)
            {
                InteractType interactType = currentInteractables[0].InteractType;

                switch (interactType)
                {
                    case InteractType.Item:
                        linkedCharacter.IsLooting = !linkedCharacter.IsLooting;
                        PlayLootAnimation();
                        break;
                    case InteractType.NPC:
                        currentInteractables[0].Interact(linkedCharacter.gameObject);
                        break;
                    case InteractType.None:
                        currentInteractables[0].Interact(linkedCharacter.gameObject);
                        break;
                }
            }
        }

        void OnExecuteMainWeaponSwap()
        {
            if (IsControlLocked)
                return;
            
            // 1번 키를 눌럿을 때, 들어오는 이벤트
            // 1번 키를 눌렀을 때 => 1번 무기로 변경하는 명령만 CharacterBase 에게 전달
                linkedCharacter.ToggleEquipPrimaryWeapon();
            linkedCharacter.IsThrowMode = false;
            MainHudChangeEvent?.Invoke();
            ReturnToTPSModeCheck();
        }

        void OnExecuteSubWeaponSwap()
        {
            if (IsControlLocked)
                return;
            // 2번 키를 눌럿을 때, 들어오는 이벤트
            // 2번 키를 눌렀을 때 => 1번 무기로 변경하는 명령만 CharacterBase 에게 전달
            linkedCharacter.ToggleEquipSecondaryWeapon();
            linkedCharacter.IsThrowMode = false;
            MainHudChangeEvent?.Invoke();
            ReturnToTPSModeCheck();
        }

        void OnExcuteFpsZoomTransition()
        {
            if (IsControlLocked)
                return;

                if (linkedCharacter != null && linkedCharacter.currentWeapon == linkedCharacter.primaryWeapon)
            {
                CameraSystem.Instance.IsFpsMode = !CameraSystem.Instance.IsFpsMode;
            }

            if (CameraSystem.Instance.IsFpsMode)
            {
                foreach (var obj in linkedCharacter.fpsModeVisualObjects)
                {
                    obj.SetActive(false);
                }
            }
            else
            {
                foreach (var obj in linkedCharacter.fpsModeVisualObjects)
                {
                    obj.SetActive(true);
                }
            }
        }

        void OnExecuteThirdRightLeftViewChange()
        {
            if (IsControlLocked)
                return;
            CameraSystem.Instance.IsCameraSideOnRight = !CameraSystem.Instance.IsCameraSideOnRight;
        }

        void OnExecuteMaintainZoom()
        {
            if (IsControlLocked)
                return;
            CameraSystem.Instance.IsCameraZoom = true;
        }

        void OnExecuteReturnToTps()
        {
            if (IsControlLocked)
                return;
            CameraSystem.Instance.IsCameraZoom = false;
        }

        private void ReturnToTPSModeCheck()
        {
            if (IsControlLocked)
                return;
            if (CameraSystem.Instance.IsFpsMode == true)
                CameraSystem.Instance.IsFpsMode = !CameraSystem.Instance.IsFpsMode;
        }

        bool OnExecuteInventoryUI()
        {
            var inventoryUI = UIManager.Singleton.GetUI<InventoryUI>(UIList.InventoryUI);
            if (inventoryUI == null)
                return false;

            if (true == inventoryUI.gameObject.activeSelf)
            {
                UIManager.Hide<InventoryUI>(UIList.InventoryUI);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                return false;
            }
            else 
            {
                UIManager.Show<InventoryUI>(UIList.InventoryUI);
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
                return true;
            }
        }

        bool OnExecutePlayerEquipmentUI()
        {
            var playerEquipmentUI = UIManager.Singleton.GetUI<PlayerEquipmentUI>(UIList.PlayerEquipmentUI);
            if (playerEquipmentUI == null)
                return false;

            if (true == playerEquipmentUI.gameObject.activeSelf)
            {
                UIManager.Hide<PlayerEquipmentUI>(UIList.PlayerEquipmentUI);
                return false;
            }
            else
            {
                UIManager.Show<PlayerEquipmentUI>(UIList.PlayerEquipmentUI);
                return true;
            }
        }

        void OnExecutePlayerShortCutItem()
        {
            if (UIManager.Singleton.GetUI(UIList.ShortCutUI, out ShortCutUI shortCutUI))
            {
                shortCutUI.UseShortCutItem();
            }
        }


        //void OnExecuteHelpPopup()
        //{
        //    var helpPopup = UIManager.Singleton.GetUI<PopupA_UI>(UIList.PopupA_UI);
        //    OnHelpPopupToggle(!helpPopup.gameObject.activeSelf);
        //}

        //void OnHelpPopupToggle(bool isOn)
        //{
        //    if (isOn)
        //    {
        //        UIManager.Show<PopupA_UI>(UIList.PopupA_UI);
        //    }
        //    else
        //    {
        //        UIManager.Hide<PopupA_UI>(UIList.PopupA_UI);
        //    }
        //}
        public void OnExecuteReloadFinishEvent()
        {
            MainHudChangeEvent?.Invoke();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftAlt))
            {
                InputSystem.Singleton.ChangeCursorVisibility(false);
            }

            //if (Input.GetKeyDown(KeyCode.K))
            //{
            //    //IngameStartCinematic.Instance.SkipCinematic();
            //    CanvasAlhpaManager.Instance.FadeOut();
            //}

            if (Input.GetKeyDown(KeyCode.F9))
            {
                UIManager.Show<CheatUI>(UIList.CheatUI);
            }

            if (Input.GetKeyDown(KeyCode.F10))
            {
                UIManager.Hide<CheatUI>(UIList.CheatUI);
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                UIManager.Show<CraftingUI>(UIList.CraftingUI);
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                UIManager.Hide<CraftingUI>(UIList.CraftingUI);
            }
            
            if (Input.GetKeyDown(KeyCode.H))
            {
                linkedCharacter.DroneSetting();
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                NightVisionController.Instance.IsActiveVision = !NightVisionController.Instance.IsActiveVision;
            }

            //if (Input.GetKeyDown(KeyCode.G))
            //{
            //    linkedCharacter.IsThrowMode = !linkedCharacter.IsThrowMode;
            //}

            IsControlLocked = UIManager.Singleton.ActiveCursorVisibleUIsCount > 0 
                || InputSystem.Singleton.IsActiveCursorVisible 
                || CameraSystem.Instance.IsActiveWorldCamera;

                float inputX = Input.GetAxis("Horizontal");
                float inputY = Input.GetAxis("Vertical");

                if (currentInteractables.Count > 0)
                {
                    InteractionUI.Instance.ShowInteraction(currentInteractables[0]);
                }
                else
                {
                    InteractionUI.Instance.HideInteractionItem();
                }

                if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    linkedCharacter.IsSprint = !linkedCharacter.IsSprint;
                }
            
                if (Input.GetKeyDown(KeyCode.T))
                {
                    linkedCharacter.IsAutoRunMode = !linkedCharacter.IsAutoRunMode;
                }

                if (Input.GetKeyDown(KeyCode.CapsLock))
                {
                    linkedCharacter.IsWalk = !linkedCharacter.IsWalk;
                }

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    OptionManager.Singleton.IsGameStopped = !OptionManager.Singleton.IsGameStopped;
                }

                Vector3 aimingPoint = Vector3.zero;
                Ray screenCenterRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
                Debug.DrawRay(screenCenterRay.origin, screenCenterRay.direction * 1000.0f, Color.red);
                if (Physics.Raycast(screenCenterRay, out RaycastHit hitInfo, 1000f, aimingLayer, QueryTriggerInteraction.Ignore))
                {
                    aimingPoint = hitInfo.point;
                }
                else
                {
                    aimingPoint = screenCenterRay.GetPoint(1000f);
                }
            
                if (IsActiveControl_Movement && !IsControlLocked)
                {
                    linkedCharacter.Move(new Vector2(inputX, inputY), Camera.main.transform.eulerAngles.y);
                    bool rotateSuccess = linkedCharacter.Rotate(aimingPoint);
                    linkedCharacter.AimingPosition = rotateSuccess ? aimingPoint : screenCenterRay.GetPoint(1000f);
                }
                else
                {
                    linkedCharacter.Move(Vector2.zero, Camera.main.transform.eulerAngles.y);
                }

            #region Test
            //if (Input.GetKeyDown(KeyCode.H))
            //{
            //    linkedCharacter.DroneSetting();
            //}

            //if (Input.GetKeyDown(KeyCode.G))
            //{
            //    linkedCharacter.IsThrowMode = !linkedCharacter.IsThrowMode;
            //}
            #region Physic Scene 시뮬레이션 씬
            //if (linkedCharacter.IsThrowMode)
            //{
            //    List<Vector3> simulationResult = SimulationSystem.Instance.Simulate(
            //        linkedCharacter.CurrentThrowObject,
            //        linkedCharacter.throwStartPoint.position,
            //        linkedCharacter.transform.forward * 50,
            //        ForceMode.Impulse);

            //    trajectoryRenderer.positionCount = simulationResult.Count;
            //    for (int i = 0; i < simulationResult.Count; i++)
            //    {
            //        trajectoryRenderer.SetPosition(i, simulationResult[i]);
            //    }
            //}
            #endregion
            #region Skill Active
            //if (Input.GetKeyDown(KeyCode.T))
            //{
            //    CommandExecuteSkill(0);
            //}
            #endregion
            #endregion
        }

        public float interactionRange = 2f;
        [field : SerializeField]public List<IInteractable> currentInteractables = new List<IInteractable>();

        private void OnDrawGizmos()
        {
            Color transparentRed = new Color(0f, 1f, 0f, 0.1f);
            Gizmos.color = transparentRed;
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y, transform.position.z),
                interactionRange);
        }

        private void FixedUpdate()
        {
            currentInteractables.Clear();
            Collider[] overlappedObjects = Physics.OverlapSphere(transform.position, interactionRange);
            for (int i = 0; i < overlappedObjects.Length; i++)
            {
                if (overlappedObjects[i].TryGetComponent(out IInteractable interactable))
                {
                    if (false == currentInteractables.Contains(interactable))
                    {
                        currentInteractables.Add(interactable);
                    }
                }
            }
        }

        private void LateUpdate()
        {
            if (IsActiveControl_Camera && !IsControlLocked )
            {
                CameraRotation();
            }
        }

        public void PlayLootAnimation()
        {
            linkedCharacter.animator.SetTrigger("Loot Trigger");
        }
     
        public void AddRecoil()
        {
            if (linkedCharacter.IsArmed && linkedCharacter.currentWeapon.WeaponCurrentBulletAmount > 0)
            {
                currentRecoil += recoilAmount * Time.deltaTime;
                OptionManager.Singleton.crossHairCanvas.SetCrossHairRecoil(OptionManager.Singleton.CurrentCrossHairType, true);
            }

            currentRecoil = Mathf.Clamp(currentRecoil, 0.0f, recoilMaxThreshold);
        }

        public void PauseRecoil()
        {
            currentRecoil = 0.0f;

            OptionManager.Singleton.crossHairCanvas.SetCrossHairRecoil(OptionManager.Singleton.CurrentCrossHairType, false);
        }

        private void CameraRotation()
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            Vector2 look = new Vector2(mouseX, mouseY);

            if (look.sqrMagnitude > threshold)
            {
                float yaw = look.x;
                float pitch = -look.y;

                targetYaw = ClampAngle(targetYaw + yaw, float.MinValue, float.MaxValue);
                targetPitch = ClampAngle(targetPitch + pitch, bottomClampLimit, topClampLimit);
            }

            // Recoil 적용
            targetPitch -= currentRecoil;
            targetPitch = ClampAngle(targetPitch, bottomClampLimit, topClampLimit);

            // 카메라 회전 적용
            linkedCharacter.cameraPivot.transform.rotation = Quaternion.Euler(targetPitch, targetYaw , 0f);
            linkedCharacter.fpsCameraPivot.transform.rotation = Quaternion.Euler(targetPitch, targetYaw, 0f);

            //// Recoil 감소 (자연스럽게 원래 위치로 돌아가기) 추후 컨텐츠에 따라 선택하자 아직은 비활성화
            //currentRecoil = Mathf.Lerp(currentRecoil, 0f, Time.deltaTime * recoilSpeed);
        }

        private static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360)
            {
                angle += 360;
            }

            if (angle > 360)
            {
                angle -= 360;
            }

            return Mathf.Clamp(angle, min, max);
        }

        public void CommandExecuteSkill(int index)
        {
            linkedCharacter.ExecuteSkill(index);
        }
    }
}

