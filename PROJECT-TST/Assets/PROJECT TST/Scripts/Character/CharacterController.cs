using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public class CharacterController : MonoBehaviour
    {
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

        private void Awake()
        {
            linkedCharacter = GetComponent<CharacterBase>();

            linkedCharacter.eventHandler.OnDamagedAction += OnLinkedCharacterDamaged;
        }

        private void OnLinkedCharacterDamaged()
        {
            if (linkedCharacter.CurrentHp <= 0)
            {
                GameManager.Instance.OnPlayerDead();
            }
        }

        private void Start()
        {
            //transform.position = UserDataModel.Singleton.IngamePlayerData.Values[0].Position;
            //transform.rotation = UserDataModel.Singleton.IngamePlayerData.PlayerRotation;
            InputSystem.Singleton.OnInput_Jump += OnExecuteJump;
            InputSystem.Singleton.OnInput_MainWeapon += OnExecuteMainWeaponSwap;
            InputSystem.Singleton.OnInput_SubWeapon += OnExecuteSubWeaponSwap;
            InputSystem.Singleton.OnInput_ToggleFpsRightButtonTransition += OnExcuteFpsZoomTransition;
            InputSystem.Singleton.OnInput_MaintainZoom += OnExecuteMaintainZoom;
            InputSystem.Singleton.OnInput_ReturnToTps += OnExecuteReturnToTps;
            InputSystem.Singleton.OnInput_InventoryToggle += OnExecuteInventoryUI;
            // += CommandExecuteSkill // input 연동

            MainHudUI mainHud = UIManager.Singleton.GetUI<MainHudUI>(UIList.MainUI);
            mainHud.SetLinkedCharacter(linkedCharacter);

            InventoryUI inventoryUI = UIManager.Singleton.GetUI<InventoryUI>(UIList.InventoryUI);
            inventoryUI.SetLinkedCharacter(linkedCharacter);
            // 
            //GameDataModel.Singleton.GetSkillData("SlingShot", out SkillData slingShotData);
            //linkedCharacter.RegisterSkill(0, new CharacterSkill_SlingShot(slingShotData));
        }

        void OnExecuteJump()
        {
            linkedCharacter.Jump();
        }

        void OnExecuteMainWeaponSwap()
        {
            // 1번 키를 눌럿을 때, 들어오는 이벤트
            // 1번 키를 눌렀을 때 => 1번 무기로 변경하는 명령만 CharacterBase 에게 전달
            linkedCharacter.ToggleEquipPrimaryWeapon();

            ReturnToTPSModeCheck();
        }

        void OnExecuteSubWeaponSwap()
        {
            // 2번 키를 눌럿을 때, 들어오는 이벤트
            // 2번 키를 눌렀을 때 => 1번 무기로 변경하는 명령만 CharacterBase 에게 전달
            linkedCharacter.ToggleEquipSecondaryWeapon();

            ReturnToTPSModeCheck();
        }

        void OnExcuteFpsZoomTransition()
        {
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

        void OnExecuteMaintainZoom()
        {
            CameraSystem.Instance.IsCameraZoom = true;
        }

        void OnExecuteReturnToTps()
        {
            CameraSystem.Instance.IsCameraZoom = false;
        }

        bool OnExecuteInventoryUI()
        {
            var inventoryUI = UIManager.Singleton.GetUI<InventoryUI>(UIList.InventoryUI);
            if (inventoryUI == null)
                return false;

            if (true == inventoryUI.gameObject.activeSelf)
            {
                UIManager.Hide<InventoryUI>(UIList.InventoryUI);
                return false;
            }
            else 
            {
                UIManager.Show<InventoryUI>(UIList.InventoryUI);
                return true;
            }
        }

        private void ReturnToTPSModeCheck()
        {
            if (CameraSystem.Instance.IsFpsMode == true)
                CameraSystem.Instance.IsFpsMode = !CameraSystem.Instance.IsFpsMode;
        }

        private void OnDestroy()
        {
            //InputSystem.Singleton.OnInput_HelpPopupToggle -= OnExecuteHelpPopup;
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


        private void Update()
        {
            //if (Input.GetKeyDown(KeyCode.F1))
            //{
            //    UIManager.Show<PopupA_UI>(UIList.PopupA_UI);
            //}


            float inputX = Input.GetAxis("Horizontal");
            float inputY = Input.GetAxis("Vertical");

            if (currentInteractables.Count > 0)
            {
                InteractionUI.Instance.ShowInteractionItem(currentInteractables[0]);
            }
            else
            {
                InteractionUI.Instance.HideInteractionItem();
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                CameraSystem.Instance.IsCameraSideOnRight = !CameraSystem.Instance.IsCameraSideOnRight;
            }

            //if (Input.GetKeyDown(KeyCode.Alpha2))
            //{
            //    linkedCharacter.IsArmed = !linkedCharacter.IsArmed;
            //}

            //if (Input.GetKeyDown(KeyCode.Alpha3))
            //{
            //    linkedCharacter.IsArmed = !linkedCharacter.IsArmed;
            //}

            //if (Input.GetKeyDown(KeyCode.Alpha4))
            //{
            //    linkedCharacter.IsArmed = !linkedCharacter.IsArmed;
            //}

            //if (currentInteractables.Count > 0)
            //{
            //    InteractionUI.Instance.ShowInteractionItem();
            //}
            //else
            //{
            //    InteractionUI.Instance.HideInteractionItem();
            //}

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

            if (Input.GetKeyDown(KeyCode.R))
            {
                linkedCharacter.Reload();
            }

            if (Input.GetMouseButton(0))
            {
                linkedCharacter.Shoot();
            }

            if (Input.GetMouseButtonUp(0))
            {
                linkedCharacter.ShootFinished();
            }

            if (Input.GetKeyDown(KeyCode.V))
            {
                linkedCharacter.Roll();
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                linkedCharacter.Crouch();
            }

            //if (Input.GetKeyDown(KeyCode.H))
            //{
            //    linkedCharacter.DroneSetting();
            //}


            if (Input.GetKeyDown(KeyCode.F))
            {
                if (currentInteractables.Count > 0) 
                {
                    currentInteractables[0].Interact(linkedCharacter.gameObject);
                }
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                linkedCharacter.IsThrowMode = !linkedCharacter.IsThrowMode;
            }

            if (linkedCharacter.IsThrowMode)
            {
                List<Vector3> simulationResult = SimulationSystem.Instance.Simulate(
                    linkedCharacter.CurrentThrowObject,
                    linkedCharacter.throwStartPoint.position,
                    linkedCharacter.transform.forward * 50,
                    ForceMode.Impulse);

                trajectoryRenderer.positionCount = simulationResult.Count;
                for (int i = 0; i < simulationResult.Count; i++)
                {
                    trajectoryRenderer.SetPosition(i, simulationResult[i]);
                }
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
            
            linkedCharacter.Move(new Vector2(inputX, inputY), Camera.main.transform.eulerAngles.y);
            bool rotateSuccess = linkedCharacter.Rotate(aimingPoint);
            linkedCharacter.AimingPosition = rotateSuccess ? aimingPoint : screenCenterRay.GetPoint(1000f);

            if (Input.GetKeyDown(KeyCode.T))
            {
                CommandExecuteSkill(0);
            }
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
            CameraRotation();
        }

     
        public void AddRecoil()
        {
            if (linkedCharacter.IsArmed && linkedCharacter.currentWeapon.Ammo.CurrentAmmo > 0)
            {
                currentRecoil += recoilAmount * Time.deltaTime;
                //OptionManager.Singleton.usingCrossHairComponent.IsRecoilChange = true;
            }

            currentRecoil = Mathf.Clamp(currentRecoil, 0.0f, recoilMaxThreshold);
        }

        public void PauseRecoil()
        {
            //OptionManager.Singleton.usingCrossHairComponent.IsRecoilChange = false;
            currentRecoil = 0.0f;
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

