using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float move = Input.GetAxis ("Vertical");
		float turn = Input.GetAxis ("Horizontal");
		gameObject.transform.Rotate (0, turn, 0);

		if (Input.GetKey (KeyCode.LeftShift)) {
			moveForward (move * 2);		
		} else {
			moveForward (move);
		}

    }

    void moveForward (float speed) {
		transform.Translate (0, 0, speed * Time.deltaTime * moveSpeed, Space.Self);
	}
}
