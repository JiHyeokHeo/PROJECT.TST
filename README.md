# Unity3D_ShootingProject_Portfolio

This project is for **Unity Game Development Practice (3D TPS)**.  
The goal is to develop and structure core gameplay mechanics, data systems, and UI functionality while documenting the entire development process.  
All logs are written weekly and updated in GitHub Repository.


## Week 1 (2024.10.25)

- 📌 **Today’s Focus**
  - TPS Core  
  - Character Player => Weapon Base
    - Modeling
    - Animations => Run/Walk/Sprint 
  - CharacterController <=> CharacerBase
  - AIController <=> CharacterBase
  - Camera System TPS View [Cinemachine]
  - Shooting/Character
  - Weapon [Modeling + Animation]
  - Holster & Equip
  - Input System [Legacy Vesion]
  - Reload & IK [Inverse Kinematic]
  - C# Interface [IInteraction / IDamage]
---

## Week 2 (2024.11.01)

- 📌 **Today’s Focus**
  - Animations Added [Roll & Crouch]
  - Shooting
    - Hold
    - Aiming [FPS View]
    - Cinemachine Camera Shift
  - Animation Blend Tree
  - Advanced IK [Reload Hand On/Off]
  - EffectManager
    - Projectile Impact Effect
---

## Week 3 (2024.11.08)

- 📌 **Today’s Focus**
  - CharacterBase Move & Rotation Logic Change // 내적 활용 Using Dot Mathematical
  - Interactable Interface
    - Door/NPC/Item Interaction
    - Interaction UI Added
  - Ragdoll
    - Dead Ragdoll
    - Ragdoll Activate -> Animator OFF/CharacterController[:Collider] OFF
  - CrossHair Spread
  - Gun Recoil
---

## Week 4 (2024.11.22)

- 📌 **Today’s Focus**
  - Minimap
    - RenderTexture
  - Indicator
    - Camera Viewport
  - Trajectory Prediction
    - Grenade + Explosion
    - Simulation Scene // 과부하가 심함 Hard to use 
    - PhysicsScene
    - Ghost Object
---

## Week 5 (2024.11.30)

- 📌 **Today’s Focus**
  - Game Data/User Data
  - Game Data : 게임 자체에 종속되어있는 고유한 데이터 → 변하지 않는 데이터  
  - User Data : 계정이나 플레이어의 정보에 종속되어 있는 데이터 -> 변하는 데이터
  - UIManager
    - UIBase
    - UIPanel/UIPopup
    - Show<T>(), Hide<T>()
    - Cursor Visible
---

## Week 6 (2024.12.06)

- 📌 **Today’s Focus**
  - SceneManagement
    - MainScene -> 어플리케이션 실행 됐을 때 처음 불려지는 씬 (Main Object 1개만 존재)
    - EmptyScene -> 전환 시 필요한 씬 (Object 0개)
    - TitleScene -> 게임 콘텐츠 씬
    - IngameScene -> 게임 콘텐츠 씬
  - Scene Loading
    - Async Scene Loading
    - SceneBase -> 추상 클래스의 씬 OnStart(), OnEnd() :abstract
    - TitleScene 
    - GameScene
---

## Week 7 (2024.12.13)

- 📌 **Today’s Focus**
  - *(No specific tasks documented)*

---

## Week 8 (2024.12.20)

- 📌 **Today’s Focus**
  - *(No specific tasks documented)*

---

## Week 9 (2024.12.27)

- 📌 **Today’s Focus**
  - *(No specific tasks documented)*
