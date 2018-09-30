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
	private Color defaultShieldIndicatorColor;
	public Color DefaultShieldIndicatorColor
	{
		get { return defaultShieldIndicatorColor; }
		set { defaultShieldIndicatorColor = value; }
	}

	[SerializeField]
	private Color flashShieldIndicatorColor;
	public Color FlashShieldIndicatorColor
	{
		get { return flashShieldIndicatorColor; }
		set { flashShieldIndicatorColor = value; }
	}

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

	public void Awake()
	{
		shield.tookDamage += OnShieldDamageTaken;
	}

	private void OnShieldDamageTaken(Entity victim, Entity attacker, float rawDamage, float calcDamage, bool damageApplied, bool hitShields)
	{
		StopAllCoroutines ();
		StartCoroutine (FlashShieldIndicator (flashShieldIndicatorColor));
	}

	public void Update()
	{
		if (shieldIndicator != null && dashIndicator != null)
		{
			if (shieldAvailable)
				shieldIndicator.color = defaultShieldIndicatorColor;
			else
				shieldIndicator.color = unavailableShieldIndicatorColor;
			dashIndicator.color = dashIndicatorColor;
		}

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

	private IEnumerator FlashShieldIndicator(Color flashColor)
	{
		shieldIndicator.color = flashColor;

		for (float duration = 1f, initDur = duration; duration > 0f; duration -= Time.deltaTime)
		{
			shieldIndicator.color = Color.Lerp (shieldIndicator.color, DefaultShieldIndicatorColor, duration / initDur);
			yield return null;
		}
	}
}
