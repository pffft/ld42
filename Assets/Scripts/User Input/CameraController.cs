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
	private Transform[] followTargets;
	[SerializeField]
	private float minZoom = 10f;

	public float MinZoom
	{
		get { return minZoom; }
		set { minZoom = value; }
	}

	public bool IsFollowing { get { return IsFollowing; } set { isFollowing = value; } }

	private bool shaking;

	private Camera cam;
	private Transform zoomPoint;

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
		zoomPoint = transform.GetChild (0);
		cam = GetComponentInChildren<Camera> ();
	}

	public void Shake(float duration, Vector3 intensity, float decay = 0f)
	{
		if(!shaking)
			StartCoroutine (DoShake (duration, intensity, decay));
	}

	public void Goto(Vector3 position, float zoom)
	{
		if (!isFollowing)
		{
			StartCoroutine (GotoPostion (position, zoom));
		}
	}

	private IEnumerator GotoPostion(Vector3 position, float zoom)
	{
		while (Vector3.Distance (transform.position, position) > 0.001f && Mathf.Abs (cam.transform.localPosition.magnitude - zoom) > 0.001f)
		{
			transform.position = Vector3.Lerp (transform.position, position, followSpeed * Time.unscaledDeltaTime);
			Vector3 idealPos = zoomPoint.localPosition.normalized * Mathf.Max (zoom, minZoom);
			zoomPoint.localPosition = Vector3.Lerp (zoomPoint.localPosition, idealPos, 100 * Time.unscaledDeltaTime);
			yield return null;
		}
	}

	private IEnumerator DoShake(float duration, Vector3 intensity, float decay)
	{
		Vector3 startPos = cam.transform.localPosition;
		float x = Random.Range(5f, 10000f), w = 0.1f;

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
		if (isFollowing && followTargets.Length > 0)
		{
			//average postions
			Vector3 avgPos = Vector3.zero;
			int addedTargets = 0;
			for (int i = 0; i < followTargets.Length; i++)
			{
				if (followTargets[i] != null)
				{
					avgPos += followTargets[i].position;
					addedTargets++;
				}
			}
			avgPos /= addedTargets;

			//zoom out to fit all targets
			if (cam != null)
			{				
				float atb = Vector3.Distance (followTargets[0].position, followTargets[1].position);
				float idealDist = atb / (2 * Mathf.Tan (cam.fieldOfView * Mathf.Deg2Rad / 2));

                // Temporary hacky fuzzing factor; because of camera incline, we tend to
                // underestimate height on large distances. This roughly compensates for that.
                //
                // A better camera would take into account the angle of the camera, and
                // initial offset from the midpoint.
                //Debug.Log("ATB: " + atb);
                if (atb > 10) {
                    idealDist *= 1.2f;
                }

				//Debug.Log (idealDist);
				Vector3 idealPos = zoomPoint.localPosition.normalized * Mathf.Max(idealDist, minZoom);
				zoomPoint.localPosition = Vector3.Lerp (zoomPoint.localPosition, idealPos, 100 * Time.unscaledDeltaTime);
			}

			transform.position = Vector3.Lerp (transform.position, avgPos, followSpeed * Time.unscaledDeltaTime);
		}
	}
	private float sq(float v) { return v * v; }

	private Vector3 GetOutermostTarget(Vector3 avgPos)
	{
		Vector3 cameraDir = cam.transform.rotation * Vector3.forward;
		Vector3 outermost = Vector3.zero;
		float distance = 0f;

		for (int i = 0; i < followTargets.Length; i++)
		{
			if (followTargets[i] == null)
				continue;

			float dist;
			if ((dist = Vector3.Distance (avgPos, followTargets[i].position)) > distance)
			{
				outermost = followTargets[i].position;
				distance = dist;
			}
		}
		return outermost;
	}
}

