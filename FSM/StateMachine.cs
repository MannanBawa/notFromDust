using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;

public abstract class StateMachine : MonoBehaviour {
    
    protected State State;

    public void SetState(State newState) {
        State = newState;
        StartCoroutine(State.Start());
    }
}
