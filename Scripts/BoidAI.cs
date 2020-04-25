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

    // private SphereCollider sphereCollider;
    private Vector3 heading;

    private FlockSense myFlockSense;

    private List<GameObject> flock;

    private bool flocking;

    // Start is called before the first frame update
    void Start()
    {   
        spinSpeed = defaultSpinSpeed;
        state = "Search";

        myFlockSense = GetComponentInChildren<FlockSense>();
        flocking = true;
        
    }

    // Update is called once per frame
    void Update()
    {
        flock = myFlockSense.getFlock();
        birdMode();
        // Debug.Log(flock.Count);
        heading = transform.TransformDirection(Vector3.forward);
        if (flock.Count > 0 && flocking) {
            alignWithFlock();
        }

    }

    public Quaternion getRotation() {
        return transform.rotation;
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
        int layerMask = 1 << 8;

        transform.Translate (0, 0, defaultMoveSpeed * Time.deltaTime, Space.Self);
        Debug.DrawRay(transform.position, heading * 3, Color.green);
        if (Physics.Raycast(transform.position, heading * 3, out hit, 5, ~layerMask)) {
            // Debug.Log(hit.transform.tag);
            switch (hit.transform.tag) {
                case "Wall": {
                    // Debug.Log("CLOSE TO WALL!");
                    int bounce = Random.Range(0,360);
                    gameObject.transform.Rotate (0, bounce, 0);
                    StartCoroutine(resetFlocking());

                    break;
                }
                case "Boid": {
                    // Debug.Log("CLOSE TO BOID");
                    avoidBoid(hit.transform.gameObject);
                    break;
                }
            }
        }

    }

    IEnumerator resetFlocking() {
        flocking = false;
        Debug.Log("Flocking disabled");
        yield return new WaitForSeconds(3);
        Debug.Log("Flocking re-enabled");
        flocking = true;
    }

    void avoidBoid(GameObject boid) {
        int bounce = Random.Range(0, 360);
        gameObject.transform.Rotate (0, bounce, 0);
    }

    void alignWithFlock() {
        transform.rotation = averageFlockHeading();

    }

    Quaternion averageFlockHeading() {
        Vector3 sumVector = Vector3.zero;
        foreach (GameObject boid in flock) {
            BoidAI boidAI = boid.GetComponent<BoidAI>();
            Quaternion boidRotaion = boidAI.getRotation();
            sumVector += boidRotaion.eulerAngles;
        }
        sumVector += this.getRotation().eulerAngles;
        Vector3 avgVector = sumVector / (flock.Count + 1);
        return Quaternion.Euler(avgVector);
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
