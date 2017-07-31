using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puck : MonoBehaviour {

    //  scorer
    private PlayerController lastTouch;
    //  assister
    private PlayerController secondLastTouch;

    private bool scored = false;

    private LayerMask netLayerMask;
    private LayerMask stickLayerMask;
    private Rigidbody2D body;
    private FadeAway fade;

    void Start() {
        fade = GetComponent<FadeAway>();
        body = GetComponent<Rigidbody2D>();
        netLayerMask = LayerMask.NameToLayer("Net");
        stickLayerMask = LayerMask.NameToLayer("Stick");
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(!scored) {
			if (collision.gameObject.layer == netLayerMask)
			{
				scored = true;
                MatchManager.instance.OnScore(collision.gameObject.tag, lastTouch, secondLastTouch);
			}
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        GameObject collider = collision.contacts[0].collider.gameObject;
		if (collider.layer == stickLayerMask)
		{
			if (lastTouch &&
			   lastTouch.gameObject != collider &&
			   lastTouch.gameObject.tag == collider.tag) {
				secondLastTouch = lastTouch;
			}
            lastTouch = collider.GetComponent<Stick>().owner;
		}
    }

    public void Reset() {
        transform.position = Vector3.zero;
        body.velocity = Vector2.zero;
        scored = false;
    }

    public void FadeAway() {
        gameObject.layer = LayerMask.NameToLayer("PuckOff");
        fade.Fade();
    }
}
