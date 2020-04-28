using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class GeneralUtil {
    // public static void moveForward (GameObject gameObject, float speed) {
	// 	transform.Translate (0, 0, speed * Time.deltaTime * moveSpeed, Space.Self);
	// }


    // void castSearchRay(string searchTag) {
    //     int layerMask = 1;
    //     RaycastHit hit;
    //     // Does the ray intersect any objects excluding the player layer
    //     if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward) * 1000, out hit, Mathf.Infinity, layerMask))
    //     {
    //         Debug.Log(hit.transform.tag);
    //         Debug.Log(hit.transform.position);
    //         Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            
    //         switch (hit.transform.tag)  {
    //             case "Player": {
    //                 spinSpeed = 0;
    //                 target = hit.transform.gameObject;
    //                 state = "Lock";
    //                 break;
    //             }
    //             case "Wall": {
    //                 Debug.Log("WALL HIT!");
    //                 spinSpeed = 0;
    //                 state = "Bird";
    //                 break;
    //             }
    //             default: {
    //                 // Do nothing
    //                 break;
    //             }
    //         }
    //     }
    //     else
    //     {
            
    //         Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.green);
    //         spinSpeed = defaultSpinSpeed;
    //     }
    // }

        // void castSearchRay(string searchTag) {
    //     int layerMask = 1;
    //     RaycastHit hit;
    //     // Does the ray intersect any objects excluding the player layer
    //     if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward) * 1000, out hit, Mathf.Infinity, layerMask))
    //     {
    //         Debug.Log(hit.transform.tag);
    //         Debug.Log(hit.transform.position);
    //         Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            
    //         switch (hit.transform.tag)  {
    //             case "Player": {
    //                 spinSpeed = 0;
    //                 target = hit.transform.gameObject;
    //                 state = "Lock";
    //                 break;
    //             }
    //             case "Wall": {
    //                 Debug.Log("WALL HIT!");
    //                 spinSpeed = 0;
    //                 state = "Bird";
    //                 break;
    //             }
    //             default: {
    //                 // Do nothing
    //                 break;
    //             }
    //         }
    //     }
    //     else
    //     {
            
    //         Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.green);
    //         spinSpeed = defaultSpinSpeed;
    //     }
    // }

}
