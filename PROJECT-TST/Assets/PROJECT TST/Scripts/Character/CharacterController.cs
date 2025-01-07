using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public class CharacterController : MonoBehaviour
    {
        public CharacterBase linkedCharacter;
        public Transform cameraPivot;

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
        }

        private void Start()
        {
            //transform.position = UserDataModel.Singleton.IngamePlayerData.Values[0].Position;
            //transform.rotation = UserDataModel.Singleton.IngamePlayerData.PlayerRotation;
            InputSystem.Singleton.OnInput_Jump += OnExecuteJump;
            InputSystem.Singleton.OnInput_MainWeapon += OnExecuteMainWeaponSwap;
            InputSystem.Singleton.OnInput_SubWeapon += OnExecuteSubWeaponSwap;
        }

        void OnExecuteJump()
        {
            linkedCharacter.Jump();
        }

        void OnExecuteMainWeaponSwap()
        {
            linkedCharacter.ArmedType = EArmedType.Rifle; 
            linkedCharacter.IsArmed = !linkedCharacter.IsArmed;
        }

        void OnExecuteSubWeaponSwap()
        {
            linkedCharacter.ArmedType = EArmedType.Pistol;
            linkedCharacter.IsArmed = !linkedCharacter.IsArmed;
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

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                CameraSystem.Instance.IsCameraSideOnRight = !CameraSystem.Instance.IsCameraSideOnRight;
            }

            if (Input.GetMouseButtonDown(1))
            {
                CameraSystem.Instance.IsCameraZoom = true;
            }

            if (Input.GetMouseButtonUp(1))
            {
                CameraSystem.Instance.IsCameraZoom = false;
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
                //for (int i = 0; i < currentInteractables.Count; i++)
                //{
                //    currentInteractables[i].Interact(linkedCharacter.gameObject);
                //}
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
        }


        public float interactionRange = 2f;
        //public List<IInteractable> currentInteractables = new List<IInteractable>();

        private void FixedUpdate()
        {
            //currentInteractables.Clear();
            //Collider[] overlappedObjects = Physics.OverlapSphere(transform.position, interactionRange);
            //for (int i = 0; i < overlappedObjects.Length; i++)
            //{
            //    if (overlappedObjects[i].TryGetComponent(out IInteractable interactable))
            //    {
            //        if (false == currentInteractables.Contains(interactable))
            //        {
            //            currentInteractables.Add(interactable);
            //        }
            //    }
            //}
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

     
        public void AddRecoil()
        {
            if (linkedCharacter.IsArmed && linkedCharacter.currentWeapon.CurrentAmmo > 0)
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
    }
}

