using UnityEngine;
public class Handimator : MonoBehaviour{
	public Animator Animator;
	public void PlayDropAnimation(){
		Animator.Play("Armature_Drop");
	}
	
	public void PlayHoldAnimation(){
		Animator.Play("Armature_Holding");
	}
	
	public void PlayThrowAnimation(){
		Animator.Play("Armature_Throw");
	}

	public void PlayIdleAnimation()
	{
		Animator.Play("Armature_Idle");
	}

	public void PlayPickupAnimation()
	{
		Animator.Play("Armature_PickUp");
	}
}
