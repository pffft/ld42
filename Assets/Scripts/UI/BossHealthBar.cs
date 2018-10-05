using CombatCore;
using UnityEngine;

public class BossHealthBar : MonoBehaviour
{
	[SerializeField]
	private Entity target;

	[Range(0f, 50f)]
	[SerializeField]
	private int numberOfPhases = 5;
	
	[SerializeField]
	private GameObject resourceBarPrefab;

	[SerializeField]
	private Color barColor;

	[Range(0f, 250f)]
	[SerializeField]
	private float activeBarWidth;

	[Range (0f, 250f)]
	[SerializeField]
	private float inactiveBarWidth;

	private int activeBarIndex;
	private ResourceBar activeBar;

	public void Start()
	{
		if (resourceBarPrefab != null)
		{
			for (int i = 0; i < numberOfPhases; i++)
			{
				GameObject barObj = Instantiate (resourceBarPrefab, gameObject.transform, false);
				ResourceBar bar = barObj.GetComponent<ResourceBar> ();
				bar.Front.color = barColor;
				bar.Front.fillAmount = 1f;
				bar.Width = inactiveBarWidth;
			}
		}

		activeBarIndex = 0;
		activeBar = transform.GetChild (activeBarIndex).GetComponent<ResourceBar> ();
		activeBar.Width = activeBarWidth;

		target.died += PhaseCompleted;
	}

	public void Update()
	{
		if (Input.GetKeyDown (KeyCode.T))
			Entity.DamageEntity (target, null, 10f, true);

		activeBar.Front.fillAmount = target.HealthPerc;
	}

	private void PhaseCompleted()
	{
		activeBar.Front.fillAmount = 0f;
		activeBar.Width = inactiveBarWidth;

		activeBarIndex++;
		activeBar = transform.GetChild (activeBarIndex).GetComponent<ResourceBar> ();

		activeBar.Width = activeBarWidth;
		Entity.HealEntity (target, float.PositiveInfinity);
	}
}
