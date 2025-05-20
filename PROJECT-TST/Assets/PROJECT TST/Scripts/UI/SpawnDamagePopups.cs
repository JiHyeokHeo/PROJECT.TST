using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.UI.CanvasScaler;

namespace TST
{
    public class SpawnsDamagePopups : SingletonBase<SpawnsDamagePopups>
    {
        private ObjectPool<DamageLabel> _damageLabelPopupPool;

        [Header("Damage Label Popup")]
        [SerializeField] private DamageLabel damageLabelPrefab;

        [Header("Display Setup")]
        [Range(0.8f, 1.5f), SerializeField] public float displayLength = 1f;
        private Camera _mainCamera;

        protected override void Awake()
        {
            var register = UIManager.Singleton.GetUI<DamageLabelRegister>(UIList.DamageNumberUI);
            damageLabelPrefab = register.damageLabel;

            Canvas canvas = this.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = this.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = ScreenMatchMode.Expand;

            base.Awake();
            _damageLabelPopupPool = new ObjectPool<DamageLabel>(
                () =>
                {
                    DamageLabel damageLabel = Instantiate(damageLabelPrefab, transform);
                    damageLabel.Initialize(displayLength, this);
                    return damageLabel;
                },
                damageLabel => damageLabel.gameObject.SetActive(true),
                damageLabel => damageLabel.gameObject.SetActive(false)
            );
            _mainCamera = Camera.main;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            _mainCamera = Camera.main;
        }

        public void DamageDone(int damage, Vector3 position, bool isCrit)
        {
            Vector3 screenPosition = _mainCamera.WorldToScreenPoint(position);
            screenPosition.z = 0;
            bool direction = screenPosition.x < Screen.width * 0.5f;

            SpawnDamagePopup(damage, screenPosition, direction, isCrit);
        }

        private void SpawnDamagePopup(int damage, Vector3 position, bool direction, bool isCrit)
        {
            DamageLabel damageLabel = _damageLabelPopupPool.Get();
            damageLabel.Display(damage, position, direction, isCrit);
        }

        public void ReturnDamageLabelToPool(DamageLabel damageLabel3d)
        {
            _damageLabelPopupPool.Release(damageLabel3d);
        }
    }
}
