using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gatherer : MonoBehaviour
{
    public GameObject gatherPoint;
    List<GameObject> targets;
    List<GameObject> gatherBucket;

    // Start is called before the first frame update
    void Start()
    {
        targets = new List<GameObject>();
        gatherBucket = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other) {
        targets.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other) {
        targets.Remove(other.gameObject);
    }

    public void testOut() {
        Debug.Log("CALLED TEST OUT");
    }

    /*
        Changes the transforms of targets in the gatherer to that of the gatherPoint.
        Takes in the gatherableType in the player's hand, or ALL, returning the first available
        Returns the type of the object it has gathered.  
     */
    public void GatherTargets(GatherableType type) {
        if (targets.Count > 0) {
            GatherableType typeChoice = type;
            
            foreach (GameObject target in targets)
            {
                Gatherable targetGScript = target.GetComponent<Gatherable>();
                GatherableType targetType = targetGScript.gatherType;
                Transform targetParent = target.transform.parent;

                if (typeChoice.Equals(GatherableType.All)) {
                    typeChoice = targetType;
                }
                if (targetType == typeChoice && (targetParent == null || (targetParent != null && targetParent.gameObject.name != "GatherPoint"))) {
                    this.addToBucket(target.gameObject);
                }
            }
            // return typeChoice;
        }
        // return type;
    }

    private void addToBucket(GameObject targetObj) {
        gatherBucket.Add(targetObj);

        Rigidbody targetRb = targetObj.GetComponent<Rigidbody>();
        targetRb.isKinematic = true;

        targetObj.transform.parent = gatherPoint.transform;
        targetObj.transform.localPosition = new Vector3(0,0,0);
        targetObj.transform.localScale = new Vector3(0,0,0);
        this.scaleBucket();
    }

    public void dropFromBucket() {
        int bucketSize = gatherBucket.Count;
        if (bucketSize > 0) {
            GameObject bottomOfBucket = gatherBucket[bucketSize - 1];
            GameObject topOfBucket = gatherBucket[0];
            Rigidbody topOfBucketRb = topOfBucket.GetComponent<Rigidbody>();

            bottomOfBucket.transform.parent = null;
            // bottomOfBucket.transform.localPosition = gameObject.transform.position;
            bottomOfBucket.transform.localScale = new Vector3(1,1,1);

            topOfBucketRb.isKinematic = false;

            gatherBucket.RemoveAt(bucketSize - 1);
            this.scaleBucket();
        }
    }

    private void scaleBucket() {
        int bucketSize = gatherBucket.Count;
        if (bucketSize > 0) {
            gatherBucket[0].transform.localScale = new Vector3 (bucketSize * 1.1f, bucketSize * 1.1f, bucketSize * 1.1f);
        }
    }

    public int getBucketCount() {
        return gatherBucket.Count;
    }

    public GatherableType getBucketType() {
        if (this.getBucketCount() > 0) {
            Gatherable firstGatherable = gatherBucket[0].GetComponent<Gatherable>();
            return firstGatherable.gatherType;
        } else {
            return GatherableType.All;
        }
    }
}
