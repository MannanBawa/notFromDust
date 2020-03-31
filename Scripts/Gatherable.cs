using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gatherable : MonoBehaviour
{

    public GatherableType gatherType;

    // RigidBody rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        // rigidBody = gameObject.GetComponet(typeof(RigidBody)) as RigidBody;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other) {
        GameObject otherGameObject = other.gameObject;
        Gatherable otherGatherable = other.gameObject.GetComponent<Gatherable>();
        Transform otherTransform = otherGameObject.transform;

        Transform myTransform = gameObject.transform;

        string myParent = myTransform.parent == null ? "" : myTransform.parent.name;
        string otherParent = otherTransform.parent == null ? "" : otherTransform.parent.name;

        if (otherGatherable != null && 
            otherGatherable.gatherType == this.gatherType &&
            myParent != "GatherTarget" && otherParent != "GatherTarget") {

            float myVolume = myTransform.localScale.x * myTransform.localScale.y * myTransform.localScale.z;
            float otherVolume = otherTransform.localScale.x * otherTransform.localScale.y * otherTransform.localScale.z;

            if (myVolume >= otherVolume) {
            
                myTransform.localScale = new Vector3(myTransform.localScale.x +otherTransform.localScale.x,
                                                     myTransform.localScale.y + otherTransform.localScale.y,
                                                     myTransform.localScale.z + otherTransform.localScale.z);
                Destroy(otherGameObject);
            }
        }        
    }



}
