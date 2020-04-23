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
        
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Debug.Break();
        }

        float move = Input.GetAxis ("Vertical");
		float turn = Input.GetAxis ("Horizontal");
		gameObject.transform.Rotate (0, turn, 0);

		if (Input.GetKey (KeyCode.Space)) {
			moveUp (2);		
		}

        if (Input.GetKey (KeyCode.N)) {
            moveUp(-2);
        }

        moveForward(move);

    }

    public void moveForward (float speed) {
		transform.Translate (0, 0, speed * Time.deltaTime * moveSpeed, Space.Self);
	}

    void moveUp (float speed) {
        transform.Translate (0, speed * Time.deltaTime * moveSpeed, 0, Space.Self);
    }
}
