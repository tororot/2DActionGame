using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour {
    private Animator anim = null;
    private Rigidbody2D rb = null;
    public float speed;
    float xSpeed;
    // Start is called before the first frame update
    void Start () {
        anim = GetComponent<Animator> ();
        rb = GetComponent<Rigidbody2D> ();
        speed = 8.0f;
    }

    // Update is called once per frame
    void Update () {
        float horizontalKey = Input.GetAxis ("Horizontal");

        if (horizontalKey > 0) {
            transform.localScale = new Vector3 (1, 1, 1);
            anim.SetBool ("run", true);
            xSpeed = speed;
        } else if (horizontalKey < 0) {
            transform.localScale = new Vector3 (-1, 1, 1);
            anim.SetBool ("run", true);
            xSpeed = -speed;
        } else {
            anim.SetBool ("run", false);
            xSpeed = 0.0f;
        }
        rb.velocity = new Vector2(xSpeed, rb.velocity.y);
    }
}