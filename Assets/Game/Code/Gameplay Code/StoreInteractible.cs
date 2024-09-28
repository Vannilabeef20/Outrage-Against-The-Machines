using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    public class StoreInteractible : BaseInteractble
    {
        [Header("PURCHASE"), HorizontalLine(2f, EColor.Yellow)]
        [SerializeField] GameObject item;
        public override void OnInteract(int playerNumber)
        {
            Instantiate(item, transform.position.ToXYY(), Quaternion.identity);
        }
    }
}