using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	private static CameraController instance;

	[SerializeField]
	private float followRadius;
	[SerializeField]
	private float followSpeed;
	[SerializeField]
	private bool isFollowing;
	[SerializeField]
	private bool matchRotation;
	[SerializeField]
	private Transform followTarget;

	private bool shaking;

	private Camera cam;

	public static CameraController GetInstance()
	{
		return instance;
	}

	public CameraController()
	{
		shaking = false;
	}

	public void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Debug.LogWarning ("Extra CameraController in scene!");
#if UNITY_EDITOR
			UnityEditor.EditorGUIUtility.PingObject (gameObject);
#endif
		}
	}

	public void Start()
	{
		cam = GetComponentInChildren<Camera> ();
	}

	public void Shake(float duration, Vector3 intensity, float decay = 0f)
	{
		if(!shaking)
			StartCoroutine (DoShake (duration, intensity, decay));
	}

	private IEnumerator DoShake(float duration, Vector3 intensity, float decay)
	{
		Vector3 startPos = cam.transform.localPosition;
		float x = 0f, w = Random.value;

		shaking = true;
		while (shaking && duration > 0f)
		{
			Vector3 dShake = new Vector3 (
				Mathf.PerlinNoise(x, 0f) - 0.5f,
				Mathf.PerlinNoise (x, 7f) - 0.5f,
				Mathf.PerlinNoise (x, 19f) - 0.5f);

			cam.transform.localPosition = Vector3.Scale (intensity, dShake) + startPos;
			intensity *= decay;

			x += w;

			duration -= Time.deltaTime;
			yield return null;
		}

		shaking = false;
		cam.transform.localPosition = startPos;
	}

	public void LateUpdate()
	{
		if (isFollowing && followTarget != null && Vector3.Distance(transform.position, followTarget.position) > followRadius)
		{
			transform.position = Vector3.Lerp (transform.position, followTarget.transform.position, followSpeed * Time.deltaTime);
			if (matchRotation)
				transform.rotation = followTarget.transform.rotation;
		}
	}
}

