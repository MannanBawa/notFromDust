using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidAI : MonoBehaviour
{

    public float defaultSpinSpeed;
    public float defaultMoveSpeed;

    private float spinSpeed;

    private string state;
    private GameObject target;

    private SphereCollider sphereCollider;

    // Start is called before the first frame update
    void Start()
    {   
        spinSpeed = defaultSpinSpeed;
        state = "Search";
    }

    // Update is called once per frame
    void Update()
    {
        birdMode();

        // switch(state) {
        //     case "Search": {
        //         searchMode();
        //         break;
        //     }
        //     case "Lock": {
        //         lockMode();
        //         break;
        //     }
        //     case "Bird": {
        //         birdMode();
        //         break;
        //     }
        //     default: {
        //         searchMode();
        //         break;
        //     }
        // }
        
    }

    void searchMode() {
        spin();
        castSearchRay("Player");
    }

    void lockMode() {
        castLockRay();
        faceTarget();
    }

    void birdMode() {
        RaycastHit hit;

        transform.Translate (0, 0, defaultMoveSpeed * Time.deltaTime, Space.Self);
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 5, Color.green);
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward) * 5, out hit, 5, 1)) {
            Debug.Log(hit.transform.tag);
            switch (hit.transform.tag) {
                case "Wall": {
                    // Debug.Log("CLOSE TO WALL!");
                    int bounce = Random.Range(0, 360);
                    gameObject.transform.Rotate (0, bounce, 0);
                    break;
                }
                case "Boid": {
                    Debug.Log("CLOSE TO BOID");
                    avoidBoid(hit.transform.gameObject);
                    break;
                }
            }


            
        }

    }

    void avoidBoid(GameObject boid) {
        Debug.Log("HOW TO AVOID BOID?");

        int bounce = Random.Range(0, 360);
        gameObject.transform.Rotate (0, bounce, 0);
    }


    void spin() {
        gameObject.transform.Rotate (0, spinSpeed, 0);
    }

    void castSearchRay(string searchTag) {
        int layerMask = 1;
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward) * 1000, out hit, Mathf.Infinity, layerMask))
        {
            Debug.Log(hit.transform.tag);
            Debug.Log(hit.transform.position);
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            
            switch (hit.transform.tag)  {
                case "Player": {
                    spinSpeed = 0;
                    target = hit.transform.gameObject;
                    state = "Lock";
                    break;
                }
                case "Wall": {
                    Debug.Log("WALL HIT!");
                    spinSpeed = 0;
                    state = "Bird";
                    break;
                }
                default: {
                    // Do nothing
                    break;
                }
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
