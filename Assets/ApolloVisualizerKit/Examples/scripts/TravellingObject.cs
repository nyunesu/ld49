using UnityEngine;
using System.Collections;

public class TravellingObject : MonoBehaviour {

	private Animator myAnim;

	// Use this for initialization
	void Start () {
		myAnim = GetComponent<Animator>();
	}

	void OnTriggerEnter(Collider col){
		if(col.gameObject.tag=="destroyer"){
			Destroy(gameObject);
		}
	}
	// Update is called once per frame
	void Update () {

		myAnim.speed = 2;//LocalData.speed+2;

	}
}
