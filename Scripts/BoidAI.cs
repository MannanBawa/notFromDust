using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidAI : MonoBehaviour
{

    public float defaultSpinSpeed;
    public float defaultMoveSpeed;
    public float visibilityAngle;


    private float spinSpeed;


    // private SphereCollider sphereCollider;
    private Vector3 heading;

    private FlockSense myFlockSense;

    private List<GameObject> visibleFlock;

    private bool flocking;
    private bool coheding;

    // Start is called before the first frame update
    void Start()
    {   
        spinSpeed = defaultSpinSpeed;
        myFlockSense = GetComponentInChildren<FlockSense>();
        flocking = true;
        coheding = true;
        
    }

    // Update is called once per frame
    void Update()
    {
        List<GameObject> wholeFlock = myFlockSense.getFlock();
        birdMode();
        // Debug.Log(flock.Count);
        heading = transform.TransformDirection(Vector3.forward);

        if (wholeFlock.Count > 0) {
            this.visibleFlock = getVisibleFlock(wholeFlock);
            if (this.visibleFlock.Count > 0) {
                if (flocking) {
                    alignWithFlock();
                }
                if (coheding) {
                    Vector3 flockCenter = findFlockCenter();
                    transform.LookAt(flockCenter);
                    float distance = Mathf.Abs(Vector3.Distance(transform.position, flockCenter));
                    if (distance <= 3.0f) {
                        StartCoroutine(resetCoheding());
                    } 
                }               
            }
        }

    }

    List<GameObject> getVisibleFlock(List<GameObject> wholeFlock) {
        List<GameObject> visibleFlock = new List<GameObject>();
        foreach (GameObject boid in wholeFlock)
        {
            Vector3 vectorToBoid = boid.transform.position - transform.position;
            float angle = Vector3.Angle(heading, vectorToBoid);

            if (angle < visibilityAngle) {
                visibleFlock.Add(boid);
            }
        }
        return visibleFlock;
    }

    public Quaternion getRotation() {
        return transform.rotation;
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
        yield return new WaitForSeconds(3);
        flocking = true;
    }

     IEnumerator resetCoheding() {
        coheding = false;
        yield return new WaitForSeconds(3);
        coheding = true;
    }



    void avoidBoid(GameObject boid) {
        int bounce = Random.Range(-45, 45);
        gameObject.transform.Rotate (0, bounce, 0);
    }

    void alignWithFlock() {
        transform.rotation = averageFlockHeading();

    }

    Vector3 findFlockCenter() {
        Vector3 flockCenter;
        Vector3 positionSum = Vector3.zero;
        foreach (GameObject boid in visibleFlock) {
            Vector3 boidPosition = boid.transform.position;
            positionSum += boidPosition;
        }
        Vector3 avgPos = positionSum / visibleFlock.Count;
        return avgPos;
    }

    Quaternion averageFlockHeading() {
        Vector3 sumVector = Vector3.zero;
        foreach (GameObject boid in visibleFlock) {
            BoidAI boidAI = boid.GetComponent<BoidAI>();
            Quaternion boidRotaion = boidAI.getRotation();
            sumVector += boidRotaion.eulerAngles;
        }
        sumVector += this.getRotation().eulerAngles;
        Vector3 avgVector = sumVector / (visibleFlock.Count + 1);
        return Quaternion.Euler(avgVector);
    }


    void spin() {
        gameObject.transform.Rotate (0, spinSpeed, 0);
    }



}
