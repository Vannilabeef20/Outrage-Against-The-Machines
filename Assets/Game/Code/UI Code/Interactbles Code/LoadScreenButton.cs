using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game
{
	public class LoadScreenButton : BaseUIInteractive
	{
        [SerializeField] MenuIdEvent targetScreen;
        [SerializeField] EMenuId targetMenuIds;
        [Space]
        [SerializeField] bool hasTransition;
        [SerializeField, ShowIf("hasTransition")] LevelTransition transition;

        public void LoadScreen()
        {
            AudioManager.instance.PlayUiClickSfx();
            PlayInteractionAnimation();

            if(hasTransition)
            TransitionManager.Instance.LoadScreen(targetMenuIds, transition);
        }
    }
}