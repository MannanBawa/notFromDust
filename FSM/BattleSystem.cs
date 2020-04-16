using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : StateMachine {
    
    
    void Start() {
        this.SetState(new Begin(this));
    }

    void Update() {
        
        if (Input.GetKeyDown (KeyCode.A)) {
            //TODO: DEBUG THIS SO IT ONLY TAKES THE ONE TYPE
            this.onAttackButton();
        }

        if (Input.GetKeyDown (KeyCode.H)) {
            //TODO: DEBUG THIS SO IT ONLY TAKES THE ONE TYPE
            this.onHealButton();
        }



    }

    public void onAttackButton() {
        StartCoroutine(this.State.Attack());
    }

    public void onHealButton() {
        StartCoroutine(State.Heal());
    }


}
