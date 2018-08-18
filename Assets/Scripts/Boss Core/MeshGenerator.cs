using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour {

    private GameObject generated;

    private float oldInnerRadius = 5f;
    private float oldRadius = 10f;

    public float innerRadius = 5f;
    public float radius = 10f;
    public int numSections = 360 / 180;

	// Use this for initialization
	void Start () {
        Generate2();
 	}

	void Update()
	{
        if (!Mathf.Approximately(oldRadius, radius) || !Mathf.Approximately(oldInnerRadius, innerRadius))
        {
            Destroy(generated);
            Generate2();
            oldRadius = radius;
            oldInnerRadius = innerRadius;
        }
	}

	// Generates a circle (or sector) with the inner radius at 0.
	private void Generate1()
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[numSections * 3];
        float thetaStep = 360f / numSections;
        for (int i = 0; i < numSections; i++)
        {
            float theta1 = Mathf.Deg2Rad * i * thetaStep;
            float theta2 = Mathf.Deg2Rad * (i + 1) * thetaStep;

            vertices[(3 * i) + 0] = Vector3.zero;
            vertices[(3 * i) + 1] = new Vector3(radius * Mathf.Cos(theta1), 0f, radius * Mathf.Sin(theta1));
            vertices[(3 * i) + 2] = new Vector3(radius * Mathf.Cos(theta2), 0f, radius * Mathf.Sin(theta2));
        }

        for (int i = 0; i < vertices.Length; i++)
        {
            Debug.Log(vertices[i]);
        }

        int[] triangles = new int[numSections * 3];
        for (int i = 0; i < numSections; i++)
        {
            triangles[(3 * i) + 0] = (3 * i) + 0;
            triangles[(3 * i) + 1] = (3 * i) + 2;
            triangles[(3 * i) + 2] = (3 * i) + 1;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        GameObject obj = new GameObject();
        MeshFilter meshFilter = obj.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = obj.AddComponent<MeshRenderer>();
        MeshCollider meshCollider = obj.AddComponent<MeshCollider>();

        meshFilter.mesh = mesh;
        meshRenderer.material = new Material(Shader.Find("Diffuse"));
        meshCollider.sharedMesh = mesh;

        generated = obj;
    }

    // Generates a circle or sector with an inner radius cut out.
    private void Generate2()
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[numSections * 4];
        float thetaStep = 360f / numSections;
        for (int i = 0; i < numSections; i++)
        {
            float theta1 = Mathf.Deg2Rad * i * thetaStep;
            float theta2 = Mathf.Deg2Rad * (i + 1) * thetaStep;

            vertices[(4 * i) + 0] = new Vector3(innerRadius * Mathf.Cos(theta1), 0f, innerRadius * Mathf.Sin(theta1));
            vertices[(4 * i) + 1] = new Vector3(radius * Mathf.Cos(theta1), 0f, radius * Mathf.Sin(theta1));

            vertices[(4 * i) + 2] = new Vector3(innerRadius * Mathf.Cos(theta2), 0f, innerRadius * Mathf.Sin(theta2));
            vertices[(4 * i) + 3] = new Vector3(radius * Mathf.Cos(theta2), 0f, radius * Mathf.Sin(theta2));
        }

        for (int i = 0; i < vertices.Length; i++)
        {
            Debug.Log(vertices[i]);
        }

        int[] triangles = new int[numSections * 6];
        for (int i = 0; i < numSections; i++)
        {
            int baseIndex = 4 * i;
            triangles[(6 * i) + 0] = baseIndex + 0;
            triangles[(6 * i) + 1] = baseIndex + 2;
            triangles[(6 * i) + 2] = baseIndex + 1;

            triangles[(6 * i) + 3] = baseIndex + 2;
            triangles[(6 * i) + 4] = baseIndex + 3;
            triangles[(6 * i) + 5] = baseIndex + 1;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        GameObject obj = new GameObject();
        MeshFilter meshFilter = obj.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = obj.AddComponent<MeshRenderer>();
        MeshCollider meshCollider = obj.AddComponent<MeshCollider>();

        meshFilter.mesh = mesh;
        meshRenderer.material = new Material(Shader.Find("Diffuse"));
        meshCollider.sharedMesh = mesh;

        generated = obj;
    }
}
