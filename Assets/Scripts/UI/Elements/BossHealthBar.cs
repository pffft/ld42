using CombatCore;
using System.Collections;
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
	private Color barColor = Color.red;

	[SerializeField]
	private Color barBackColor = Color.black;

	[Range(0f, 250f)]
	[SerializeField]
	private float activeBarWidth = 200f;

	[Range (0f, 250f)]
	[SerializeField]
	private float inactiveBarWidth = 10f;

	[SerializeField]
	private float phaseTransitionDuration = 1f;

	private int activeBarIndex;
	private ResourceBar activeBar;

	private bool transitioning;

	public void Start()
	{
		if (resourceBarPrefab != null)
		{
			for (int i = 0; i < numberOfPhases; i++)
			{
				GameObject barObj = Instantiate (resourceBarPrefab, gameObject.transform, false);
				ResourceBar bar = barObj.GetComponent<ResourceBar> ();
				bar.Front.color = barColor;
				bar.Back.color = barBackColor;
				bar.Front.fillAmount = 1f;
				bar.Width = inactiveBarWidth;
			}
		}

		activeBarIndex = 0;
		activeBar = transform.GetChild (activeBarIndex).GetComponent<ResourceBar> ();
		activeBar.Width = activeBarWidth;

		target.died += PhaseCompleted;

		transitioning = false;
	}

	public void Update()
	{
		activeBar.Front.fillAmount = target.HealthPerc;
	}

	private void PhaseCompleted()
	{
		if (!transitioning)
			StartCoroutine (TransitionPhases ());
	}

	private IEnumerator TransitionPhases()
	{
		transitioning = true;

		ResourceBar prevBar = activeBar;
		activeBarIndex++;
		activeBar = transform.GetChild (activeBarIndex).GetComponent<ResourceBar> ();
		prevBar.Front.fillAmount = 0f;

		Entity.HealEntity (target, float.PositiveInfinity);

		for (float dur = phaseTransitionDuration, initDur = dur; dur > 0f; dur -= Time.deltaTime)
		{
			prevBar.Width = Mathf.Lerp (inactiveBarWidth, activeBarWidth, dur / initDur);
			activeBar.Width = Mathf.Lerp (activeBarWidth, inactiveBarWidth, dur / initDur);
			yield return null;
		}

		prevBar.Width = inactiveBarWidth;
		activeBar.Width = activeBarWidth;

		transitioning = false;
	}
}
