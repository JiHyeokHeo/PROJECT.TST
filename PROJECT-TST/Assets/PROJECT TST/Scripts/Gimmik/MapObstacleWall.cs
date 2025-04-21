using UnityEngine;

namespace TST
{
    public class MapObstacleWall : MonoBehaviour, IInteractable
    {
        public string Message => "키를 사용해주세요";

        public InteractType InteractType => InteractType.Door;

        public void Interact(GameObject go)
        {
            // 만약 내가 키를 갖고 있다면 벽과 상호작용이 잘 되며, access가 되도록 설정
            if (go.TryGetComponent<CharacterController>(out var characterController))
            {
                if (GameDataModel.Singleton.GetItemData("KeyA", out ItemData resultData))
                {
                    GameManager.Instance.UseItem(-1, resultData);
                    Destroy(this.gameObject);
                }
            }
        }
    }
}
