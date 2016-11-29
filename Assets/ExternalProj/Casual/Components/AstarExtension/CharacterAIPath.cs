using UnityEngine;
using System.Collections;
using Pathfinding.RVO;

namespace Pathfinding {
	/** AI controller specifically made for the spider robot.
	 * The spider robot (or mine-bot) which is got from the Unity Example Project
	 * can have this script attached to be able to pathfind around with animations working properly.\n
	 * This script should be attached to a parent GameObject however since the original bot has Z+ as up.
	 * This component requires Z+ to be forward and Y+ to be up.\n
	 * 
	 * It overrides the AIPath class, see that class's documentation for more information on most variables.\n
	 * Animation is handled by this component. The Animation component refered to in #anim should have animations named "awake" and "forward".
	 * The forward animation will have it's speed modified by the velocity and scaled by #animationSpeed to adjust it to look good.
	 * The awake animation will only be sampled at the end frame and will not play.\n
	 * When the end of path is reached, if the #endOfPathEffect is not null, it will be instantiated at the current position. However a check will be
	 * done so that it won't spawn effects too close to the previous spawn-point.
	 * \shadowimage{mine-bot.png}
	 * 
	 * \note This script assumes Y is up and that character movement is mostly on the XZ plane.
	 */
	[RequireComponent(typeof(Seeker))]
	public class CharacterAIPath : CustomAIPath {

		/** Animation component.
		 * Should hold animations "awake" and "forward"
		 */
		public Animation anim;

		Animator animator;
		/*Animator _animator;
		Animator animator {
			get {
				if (_animator == null)
					_animator = GetComponent<Animator>();
				return _animator;
			}
			set {
				_animator = value;
			}
		}*/

		/** Minimum velocity for moving */
		public float sleepVelocity = 0.4F;
		
		/** Speed relative to velocity with which to play animations */
		public float animationSpeed = 0.2F;
		
		/** Effect which will be instantiated when end of path is reached.
		 * \see OnTargetReached */
		public GameObject endOfPathEffect;
		 
		public new void Start () {
			animator = GetComponentInChildren<Animator>();
		//	animator.SetFloat("Speed",1 );
			//Prioritize the walking animation
/*			anim["forward"].layer = 10;
			
			//Play all animations
			anim.Play ("awake");
			anim.Play ("forward");
			
			//Setup awake animations properties
			anim["awake"].wrapMode = WrapMode.Clamp;
			anim["awake"].speed = 0;
			anim["awake"].normalizedTime = 1F;
			*/
			//Call Start in base script (AIPath)
			base.Start ();
		}

		/** Point for the last spawn of #endOfPathEffect */
		protected Vector3 lastTarget;
		private Vector3 lastDirection;

		
		/**
		 * Called when the end of path has been reached.
		 * An effect (#endOfPathEffect) is spawned when this function is called
		 * However, since paths are recalculated quite often, we only spawn the effect
		 * when the current position is some distance away from the previous spawn-point
		*/
		public override void OnTargetReached () {
			animator.SetFloat("Speed",0 );
			if (endOfPathEffect != null && Vector3.Distance (tr.position, lastTarget) > 1) {
				GameObject.Instantiate (endOfPathEffect,tr.position,tr.rotation);
				lastTarget = tr.position;
			}
			base.OnTargetReached();
			//OnTargetReachedCallback ();
		}

		//TODO : TODO : move all set animation mode to one function (with switch)
		//public void SetToAnimation() {}
		public override void SetToIdle() {
			animator.SetFloat("Speed",0 );
			animator.SetBool("Working", false);
		}

		public override void SetToActivityAnim(Activity activity) {
			animator.SetFloat("Speed",0 );
			animator.SetInteger ("ActivityIndex", activity.activityIndex);
			animator.SetBool("Working", true);
		}

		public override Vector3 GetFeetPosition ()
		{
			return tr.position;
		}

		public void startMoving(Vector3 position) {
			if(target==null)
				target = new GameObject().transform;
			target.position = BuildingModeGrid3D.GetInstance().SnapWorldPositionToGrid(position);
			canMove = true;
		}

		public void startTeleporting(Vector3 position) {
			if(target==null)
				target = new GameObject().transform;
			transform.position = position;
			canMove = false;
			OnTargetReached();
		}

		public new void Update () {
			
			if (!canMove) { return; }
			
			Vector3 dir = CalculateVelocity (GetFeetPosition());
			
			//Rotate towards targetDirection (filled in by CalculateVelocity)
			RotateTowards (targetDirection);
			
			if (navController != null) {
			} else if (controller != null) {
				controller.SimpleMove (dir);

				if(dir.sqrMagnitude > 0.1)
				  animator.SetFloat("Speed",1 );
			//	else
			//		animator.SetBool("Working", true);

			} else if (rigid != null) {
				rigid.AddForce (dir);
			} else {
				transform.Translate (dir*Time.deltaTime, Space.World);
			}
		}

	}

}