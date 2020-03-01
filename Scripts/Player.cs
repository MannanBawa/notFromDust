using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Gatherer gatherer;
    private GatherableType currentType;

    // Start is called before the first frame update
    void Start()
    {
        gatherer = gameObject.GetComponentInChildren<Gatherer>();
        currentType = GatherableType.All;
    }

    // Update is called once per frame
    void Update()
    {
        currentType = gatherer.getBucketType();
        

        if (Input.GetKey (KeyCode.RightShift)) {
            //TODO: DEBUG THIS SO IT ONLY TAKES THE ONE TYPE
            gatherer.GatherTargets(currentType);
        }

        if (Input.GetKeyDown (KeyCode.LeftShift)) {
            // gatherer.dropFromBucket();
        }
    }
}
