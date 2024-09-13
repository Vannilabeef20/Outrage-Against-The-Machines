using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    //All enums below have to have the same name as their input actions
    public enum EPlayerInput
    {

        Punch,
        Kick,
        Grab,
        Special,
        RunningAttack_L,
        RunningAttack_R,

        Navigate_Up,
        Navigate_Down,
        Navigate_Left,
        Navigate_Right,
        Confirm,
        Cancel        
    }
}
