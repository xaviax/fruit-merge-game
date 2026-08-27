using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InAppLoadingMenu : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

	public void destroyMe()
	{
		Destroy (this.transform.root.gameObject);
	}
	
	// Update is called once per frame

}
