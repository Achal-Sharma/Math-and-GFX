using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Mesh_Generator : MonoBehaviour
{
    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;

    private Color[] colors;
    public Gradient gradient;
    private float minTerrainHeight;
    private float maxTerrainHeight;

    public int xSize = 20;
    public int zSize = 20;

    public float perlinRandomise;
    public float perlinRandX;
    public float perlinRandZ;

    [SerializeField] private TMP_InputField terrainHeightIF;
    [SerializeField] private TMP_InputField terrainWidthIF;

    // Start is called before the first frame update
    void Start()
    {
        perlinRandomise += Random.Range(0.5f, 5f);
        perlinRandX += Random.Range(0.1f, 0.5f);
        perlinRandZ += Random.Range(0.1f, 0.5f);

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void Update()
    {
        xSize = int.Parse(terrainWidthIF.text);
        zSize = int.Parse(terrainHeightIF.text);
    }

    public void Generate()
    {
        CreateShape();
        UpdateMesh();
    }

    void CreateShape()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = Mathf.PerlinNoise(x * perlinRandX, z * perlinRandZ) * perlinRandomise;
                vertices[i] = new Vector3(x, y, z);
                
                //to set the maximum and minimum range for the terrain height to set the gradient
                if (y > maxTerrainHeight) maxTerrainHeight = y;
                if (y < minTerrainHeight) minTerrainHeight = y;
                
                i++;
            }
        }

        triangles = new int[xSize * zSize * 6];

        int verts = 0;
        int tris = 0;

        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = verts + 0;
                triangles[tris + 1] = verts + xSize + 1;
                triangles[tris + 2] = verts + 1;
                triangles[tris + 3] = verts + 1;
                triangles[tris + 4] = verts + xSize + 1;
                triangles[tris + 5] = verts + xSize + 2;

                verts++;
                tris += 6;
            }
            verts++;
        }

        colors = new Color[vertices.Length];
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float height = Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, vertices[i].y);
                colors[i] = gradient.Evaluate(height);
                i++;
            }
        }

    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;
        
        mesh.RecalculateNormals();
    }

}
