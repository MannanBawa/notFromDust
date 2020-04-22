using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{

    public float defaultSpinSpeed;

    private float spinSpeed;

    private string state;
    private GameObject target;

    // Start is called before the first frame update
    void Start()
    {   
        spinSpeed = defaultSpinSpeed;
        state = "Search";
    }

    // Update is called once per frame
    void Update()
    {

        switch(state) {
            case "Search": {
                searchMode();
                break;
            }
            case "Lock": {
                lockMode();
                break;
            }
        }
        
       
        
    }

    void searchMode() {
        spin();
        castSearchRay();
    }

    void lockMode() {
        castLockRay();
        faceTarget();
    }


    void spin() {
        gameObject.transform.Rotate (0, spinSpeed, 0);
    }

    void castSearchRay() {
        int layerMask = 1;
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
        {
            Debug.Log(hit.transform.position);
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            if (hit.transform.tag == "Player") {
                spinSpeed = 0;
                target = hit.transform.gameObject;
                state = "Lock";
            }
        }
        else
        {
            
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.green);
            spinSpeed = defaultSpinSpeed;
        }
    }

    void faceTarget() {
        Vector3 myGlobalPos = transform.position;
        Vector3 targetGlobalPos = target.transform.position;

        Vector3 direction = (targetGlobalPos - myGlobalPos).normalized;

        Quaternion targetRotation = Quaternion.LookRotation (direction);

        transform.rotation = targetRotation;

    }

    void castLockRay() {
        Vector3 myGlobalPos = transform.position;
        Vector3 targetGlobalPos = target.transform.position;

        Vector3 direction = (targetGlobalPos - myGlobalPos).normalized;


        // TODO: See why this is a 90 angle from the player
        Debug.DrawRay(transform.position, direction * 500, Color.red);

    }
}
