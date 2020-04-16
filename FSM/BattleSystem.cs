using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BattleSystem : StateMachine {
    protected State State;

    public void onAttackButton() {
        StartCoroutine(State.Attack());
    }

    public void onHealButton() {
        StartCoroutine(State.Heal());
    }


}
