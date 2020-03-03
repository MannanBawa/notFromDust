using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gatherer : MonoBehaviour
{
    public GameObject gatherPoint;
    List<GameObject> targets;
    List<GameObject> gatherBucket;

    public GameObject dirtPrefab;
    public GameObject waterPrefab;

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
                if (target.transform.name != "Plane") {
                    Gatherable targetGScript = target.GetComponent<Gatherable>();
                    GatherableType targetType = targetGScript.gatherType;
                    Transform targetParent = target.transform.parent;

                    // If gathering All types, then hand is empty, assign choice to first object type
                    if (typeChoice.Equals(GatherableType.All)) {
                        typeChoice = targetType;
                    }
                    if (targetType == typeChoice && (targetParent == null || (targetParent != null && targetParent.gameObject.name != "GatherPoint"))) {
                        this.suckToBucket(target.gameObject);
                    }
                }
            }
        }
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

    private void suckToBucket(GameObject targetObj) {
        
        Gatherable targetGScript = targetObj.GetComponent<Gatherable>();
        GatherableType targetType = targetGScript.gatherType;

        GameObject gatheredObj;

        if (gatherBucket.Count == 0) {
            // Empty bucket, generate gatheredObj
            gatheredObj = generateBaseGathered(targetType);
            gatherBucket.Add(gatheredObj);
            gatheredObj.transform.parent = gameObject.transform;
        } else {
            gatheredObj = gatherBucket[0];
        }

        float targetScaleX = targetObj.transform.localScale.x;
        float targetScaleY = targetObj.transform.localScale.y;
        float targetScaleZ = targetObj.transform.localScale.z;
        
        if (targetScaleX > 0 && targetScaleY > 0 && targetScaleZ > 0) {
            targetObj.transform.localScale = 
                new Vector3 (targetObj.transform.localScale.x - 0.01f, 
                             targetObj.transform.localScale.y - 0.01f, 
                             targetObj.transform.localScale.z - 0.01f);

            gatheredObj.transform.localScale =
                new Vector3 (gatheredObj.transform.localScale.x + 0.01f,
                             gatheredObj.transform.localScale.y + 0.01f,
                             gatheredObj.transform.localScale.z + 0.01f);
        }
    
    }

    private GameObject generateBaseGathered(GatherableType ofType) {
        GameObject generatedObject;

        switch (ofType) {
            case GatherableType.Dirt: {
                generatedObject = Instantiate(dirtPrefab, gatherPoint.transform.position, Quaternion.identity);
                break;
            }
            case GatherableType.Water: {
                generatedObject = Instantiate(waterPrefab, gatherPoint.transform.position, Quaternion.identity);
                break;
            }
            default: {
                generatedObject = Instantiate(waterPrefab, gatherPoint.transform.position, Quaternion.identity);
                break;
            }
        }

        Rigidbody targetRb = generatedObject.GetComponent<Rigidbody>();
        targetRb.isKinematic = true;

        generatedObject.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

        return generatedObject;
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
