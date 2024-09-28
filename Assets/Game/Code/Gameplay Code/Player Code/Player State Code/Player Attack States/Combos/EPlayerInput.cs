using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    //All enums below have to have the same name as their input actions
    public enum EPlayerInput
    {

        Punch = 0,
        Kick = 1,
        Grab = 2,
        Special = 3,
        RunningAttack = 4,

        Navigate_Up = 6,
        Navigate_Down = 7,
        Navigate_Left = 8,
        Navigate_Right = 9,
        Confirm = 10,
        Cancel = 11,        
    }
}
