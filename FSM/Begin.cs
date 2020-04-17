using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Begin : State 
{

    public Begin(BattleSystem battleSystem) : base(battleSystem) {}

    public override IEnumerator Start() {
        Debug.Log("CALLED STATE START??");
        yield return new WaitForSeconds(2f);
    } 

    public virtual IEnumerator Heal() {
        yield break;
    } 

    public virtual IEnumerator Attack() {
        Debug.Log("CALLED STATE DOT ATTACK IN BEGIN");
        yield break;
    } 


}
