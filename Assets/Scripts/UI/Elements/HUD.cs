using CombatCore;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class HUD : MonoBehaviour
{
	[SerializeField]
	private Entity player, shield;

	[SerializeField]
	private Image shieldIndicator;

	[SerializeField]
	private Color[] shieldIndicatorColors = new Color[2];

	[SerializeField]
	private Color unavailableShieldIndicatorColor;
	public Color UnavailableShieldIndicatorColor
	{
		get { return unavailableShieldIndicatorColor; }
		set { unavailableShieldIndicatorColor = value; }
	}

	public bool shieldAvailable = true;

	[SerializeField]
	private Image dashIndicator;

	[SerializeField]
	private Color dashIndicatorColor;
	public Color DashIndicatorColor
	{
		get { return dashIndicatorColor; }
		set { dashIndicatorColor = value; }
	}

	[SerializeField]
	private Vector3 followOffset;

	public void Update()
	{
		if (dashIndicator != null)
		{
			dashIndicator.color = dashIndicatorColor;
		}

		if (shieldIndicator != null && shield != null)
		{
			if (shieldAvailable)
				shieldIndicator.color = Color.Lerp (shieldIndicatorColors[1], shieldIndicatorColors[0], shield.ShieldPerc);
			else
				shieldIndicator.color = unavailableShieldIndicatorColor;

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
