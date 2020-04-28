using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockSense : MonoBehaviour
{

    List<GameObject> nearbyBoids = new List<GameObject>();

    List<GameObject> visibleBoids = new List<GameObject>();

    float maxViewAngle;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(nearbyBoids.Count);
    }

    public List<GameObject> getFlock() {
        return nearbyBoids;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Boid") {
            nearbyBoids.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag == "Boid") {
            nearbyBoids.Remove(other.gameObject);
        }
    }
}
