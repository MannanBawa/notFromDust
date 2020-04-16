using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State 
{
    protected BattleSystem battleSystem;

    public State(BattleSystem battleSystem) {
        this.battleSystem = battleSystem;
    }

    public virtual IEnumerator Start() {
        Debug.Log("CALLED STATE START??");
        yield break;
    } 

    public virtual IEnumerator Heal() {
        Debug.Log("CALLED STATE DOT HEAL IN STATE");
        yield break;
    } 

    public virtual IEnumerator Attack() {
        Debug.Log("CALLED STATE DOT ATTACK IN STATE");
        yield break;
    } 


}
