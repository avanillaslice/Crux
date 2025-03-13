using System.Collections;
using System.Collections.Generic;
using Project.UI.Loadout.Events;
using UnityEngine;

namespace Project.UI.Loadout.Controllers
{
	/// <summary>
	/// Controller for handling all animations in the Loadout UI system.
	/// </summary>
	public class LoadoutAnimationController : MonoBehaviour
	{
		// Animation timing settings
		[Header("Animation Timing")]
		[SerializeField] private float nodeAnimationDelay = 0.35f;
		[SerializeField] private float connectionLineDrawDuration = 0.5f;

		// State tracking
		private int pendingNodeAnimations = 0;
		private int pendingConnectionLines = 0;
		private bool isInitializationComplete = false;

		// References
		private List<WeaponNode> weaponNodes = new List<WeaponNode>();

		void OnEnable()
		{
			// Subscribe to events
			LoadoutEvents.OnNodeAnimationComplete += HandleNodeAnimationComplete;
			LoadoutEvents.OnConnectionLineDrawComplete += HandleConnectionLineDrawComplete;
		}

		void OnDisable()
		{
			// Unsubscribe from events
			LoadoutEvents.OnNodeAnimationComplete -= HandleNodeAnimationComplete;
			LoadoutEvents.OnConnectionLineDrawComplete -= HandleConnectionLineDrawComplete;
		}

		/// <summary>
		/// Initializes the animation controller with the list of weapon nodes.
		/// </summary>
		/// <param name="nodes">The list of weapon nodes to animate</param>
		public void Initialize(List<WeaponNode> nodes)
		{
			weaponNodes = nodes;
			pendingNodeAnimations = nodes.Count;
			pendingConnectionLines = nodes.Count;
			isInitializationComplete = false;
		}

		/// <summary>
		/// Starts the animation sequence for all weapon nodes.
		/// </summary>
		public void PlayStartupAnimations()
		{
			StartCoroutine(AnimateNodesSequentially());
		}

		/// <summary>
		/// Coroutine that animates each weapon node with a delay between them.
		/// </summary>
		private IEnumerator AnimateNodesSequentially()
		{
			foreach (WeaponNode weaponNode in weaponNodes)
			{
				StartCoroutine(AnimateNode(weaponNode));
				yield return new WaitForSeconds(nodeAnimationDelay);
			}
		}

		/// <summary>
		/// Coroutine that animates a single weapon node and activates its connection line.
		/// </summary>
		/// <param name="weaponNode">The weapon node to animate</param>
		private IEnumerator AnimateNode(WeaponNode weaponNode)
		{
			// Get the Animator component
			Animator animator = weaponNode.GetComponent<Animator>();
			if (animator != null)
			{
				// Play the initialization animation
				animator.Play("WeaponNodeInit");

				// Wait for the animation to complete
				AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
				float animationLength = stateInfo.length;
				yield return new WaitForSeconds(animationLength);
			}

			// Notify that the node animation is complete
			LoadoutEvents.TriggerNodeAnimationComplete(weaponNode);

			// Activate the connection line
			weaponNode.ActivateConnectionLine();
		}

		/// <summary>
		/// Handles the completion of a node animation.
		/// </summary>
		/// <param name="node">The node that completed its animation</param>
		private void HandleNodeAnimationComplete(WeaponNode node)
		{
			pendingNodeAnimations--;
			CheckInitializationComplete();
		}

		/// <summary>
		/// Handles the completion of a connection line drawing.
		/// </summary>
		/// <param name="node">The node that completed its connection line drawing</param>
		private void HandleConnectionLineDrawComplete(WeaponNode node)
		{
			pendingConnectionLines--;
			CheckInitializationComplete();
		}

		/// <summary>
		/// Checks if all animations are complete and triggers the appropriate event.
		/// </summary>
		private void CheckInitializationComplete()
		{
			if (pendingNodeAnimations <= 0 && pendingConnectionLines <= 0 && !isInitializationComplete)
			{
				isInitializationComplete = true;
				LoadoutEvents.TriggerAllAnimationsComplete();
				Debug.Log("Loadout UI animation sequence complete");
			}
		}
	}
}