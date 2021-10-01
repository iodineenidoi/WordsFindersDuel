using System;
using UnityEditor.Animations;
using UnityEngine;

namespace Entities
{
    [Serializable]
    public class PlayerAnimatorSettings
    {
        public RuntimeAnimatorController animator;
        public Sprite sprite;
    }
}