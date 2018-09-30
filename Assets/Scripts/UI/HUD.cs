using CombatCore;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
	[SerializeField]
	private Entity player, shield;

	[SerializeField]
	private Image shieldIndicator;

	[SerializeField]
	private Image dashIndicator;

	[SerializeField]
	private Vector3 followOffset;

	public void Update()
	{
		if (shield != null && shieldIndicator != null)
		{
			shieldIndicator.fillAmount = shield.ShieldPerc;
		}
	}

	public void LateUpdate()
	{
		if (player != null)
		{
			transform.position = player.transform.position + followOffset;

			if (dashIndicator != null)
			{
				dashIndicator.transform.position = player.GetComponent<Controller> ().GetDashTargetPoint () + Vector3.up * 0.01f;
			}
		}
	}
}
