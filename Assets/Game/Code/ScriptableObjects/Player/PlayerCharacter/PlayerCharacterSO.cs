using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    [CreateAssetMenu(fileName = "Character", menuName = "Player/Character")]
    public class PlayerCharacterSO : ScriptableObject
    {
        [field: SerializeField, ShowAssetPreview] public Sprite CharacterSprite { get; private set; }
        [field: SerializeField, ShowAssetPreview] public GameObject CharacterPrefab { get; private set; }
    }
}
