using UnityEngine;
using System.Collections;

public class ApplyRootMotionToParent : CoreBehaviour {

	void OnAnimatorMove() {
		Animator animator = GetComponent<Animator>();
		if (animator != null) {

			//transform.parent.localPosition = animator.rootPosition;
			//transform.parent.localRotation = animator.rootRotation;
		//	animator.Get
	//		animator.GetCurrentAnimatorStateInfo().

		//	transform.parent = transform;

			if ((!Mathf.Approximately (0f, animator.deltaPosition.sqrMagnitude) 
				//	|| !Mathf.Approximately (0f, animator.deltaPosition.y)
				//	|| !Mathf.Approximately (0f, animator.deltaPosition.y)
			    )) {
					Vector3 newPosition = transform.parent.position;
					newPosition += animator.deltaPosition;
					transform.parent.localPosition = newPosition;
			}
			/*
		  
			if ((!Mathf.Approximately (0f, animator.deltaRotation.x) 
			     || !Mathf.Approximately (0f, animator.deltaRotation.y)
			     || !Mathf.Approximately (0f, animator.deltaRotation.y))) {
				Quaternion newPosition = transform.parent.rotation;
				newPosition *= animator.deltaRotation; 
				transform.parent.rotation = newPosition;
			}
*/


		}
	}
}
