using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeAway : MonoBehaviour {

    SpriteRenderer spriteRenderer;

    private bool fading = false;
     
	// Use this for initialization
	void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
        if (fading) {
			Color color = spriteRenderer.color;
			color.a = Mathf.Lerp(color.a, 0, Time.deltaTime * 2.0f);
			spriteRenderer.color = color;   
        }
	}

    public void Fade() {
        fading = true;
    }
}
