using UnityEngine;
using System.Collections;

public class SquareGenerator : MonoBehaviour {

	//public GameObject[] pool;
	public	GameObject	squareBase;
	public	float		interval=0.2f;
	public	float		intervalBase;
	public	bool		isActive;
	private	float		contador;

	// Use this for initialization
	void Start () {
	
	}

	void createSquare(){
		Instantiate(squareBase,transform.position,Quaternion.identity);
	}

	// Update is called once per frame
	void Update () {
	//	interval=LocalData.speed;
		//interval = 0.5f;
		if(interval>0){
			isActive=true;
		}else{
			isActive=false;
		}
		if(isActive){
			contador+=Time.deltaTime;
			if(contador>=interval){
				createSquare();
				contador=0;
			}
		}
	}
}
