using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    [System.Serializable]
    public class RootDataDTO
    {
        [JsonProperty(Order = 0)]
        public int ID;
    }

    [System.Serializable]
    public class UserDataDTO : RootDataDTO
    {
       
    }

    [System.Serializable]
    public class MonsterDataDTO : RootDataDTO
    {

    }

    [System.Serializable]
    public class IngamePlayerDataDTO : UserDataDTO
    {
        public string Name;
        public float Hp;
        public float AttackPower;
        public float Speed;
        public List<int> SkillData;
        public Vector3 VecPosition;
        public Quaternion QuatRotation;
        public List<string> Equipments;
    }

    [System.Serializable]
    public class IngameMonsterDataDTO : MonsterDataDTO
    {
        public string Name;
        public Vector3 Position;
        public Quaternion Rotation;
        public List<int> Skills;
    }

    [System.Serializable]
    public class UserItemDTO : UserDataDTO
    {
        [System.Serializable]
        public class UserItemData
        {
            public int slotID;
            public string itemID;
            public int itemCount;
        }

        public List<UserItemData> Items = new List<UserItemData>();
    }
}
    