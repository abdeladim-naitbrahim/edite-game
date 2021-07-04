using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.Advertisements;

public class FoodTaker : MonoBehaviour
{
	List<GameObject> foods;
	public bool takeit=true;
	Transform target;
	float b =0.01f;
	float c =0.01f;
	private Vector3 movementVelocity; 
	
       void OnTriggerEnter(Collider obj)
    {
        if (takeit && obj.transform.tag == "foodmover")
        {
            takeit=false;
			target=obj.transform.parent.transform;
        }
        //	##### added by Morgan #####
        
    }
	void FixedUpdate()
    {
		
		 if (!takeit )
		 {
			b+=c;
			if(b>(-transform.position+target.position).magnitude)
				b=(-transform.position+target.position).magnitude;
			transform.position += (-transform.position+target.position).normalized*b;
		 }
                
	}

    
}