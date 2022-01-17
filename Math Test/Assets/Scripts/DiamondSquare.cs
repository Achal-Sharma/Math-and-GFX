using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;

public class DiamondSquare : MonoBehaviour
{
    public int terrainPartitions;
    public float terrainHeight;
    public float terrainSize;

    [SerializeField] private TMP_InputField partitionIF;
    [SerializeField] private TMP_InputField sizeIF;
    [SerializeField] private TMP_InputField heightIF;

    Color[] colors;
    public Gradient gradient;
    float maxTerrainHeight;
    float minTerrainHeight;

    Vector3[] terrainVertices;
    int numberOfVertices;

    void Start()
    {
        //GenerateTerrain();
    }

    private void Update()
    {
        terrainPartitions = int.Parse(partitionIF.text);
        terrainHeight = float.Parse(heightIF.text);
        terrainSize = float.Parse(sizeIF.text);
    }

    public void GenerateTerrain()
    {
        numberOfVertices = (int)Mathf.Pow(terrainPartitions + 1, 2);
        terrainVertices = new Vector3[numberOfVertices];
        Vector2[] ab = new Vector2[numberOfVertices];
        int[] triangles = new int[((int)Mathf.Pow(terrainPartitions, 2))*6];
        int triangleOffset = 0;

        float terrainHalfSize = terrainSize / 2f;
        float partitionSize = terrainSize / terrainPartitions;

        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        for(int i = 0; i <= terrainPartitions; i++)
        {
            for (int j = 0; j <= terrainPartitions; j++)
            {
                terrainVertices[(i * (terrainPartitions + 1)) + j] = new Vector3(-terrainHalfSize + (j * partitionSize), 0f, terrainHalfSize - (i*partitionSize));
                ab[(i * (terrainPartitions + 1)) + j] = new Vector2((float) i / terrainPartitions, (float) j / terrainPartitions);

                if (i < terrainPartitions && j < terrainPartitions)
                {
                    int topLeft = (i * (terrainPartitions + 1)) + j;
                    int bottomLeft = ((i + 1) * (terrainPartitions + 1)) +j;

                    triangles[triangleOffset] = topLeft;
                    triangles[triangleOffset + 1] = topLeft + 1;
                    triangles[triangleOffset + 2] = bottomLeft + 1;

                    triangles[triangleOffset + 3] = topLeft;
                    triangles[triangleOffset + 4] = bottomLeft + 1;
                    triangles[triangleOffset + 5] = bottomLeft;

                    triangleOffset += 6;
                }
            }
        }

        // setting corner points to initial value
        terrainVertices[0].y = Random.Range(-terrainHeight, terrainHeight);
        terrainVertices[terrainPartitions].y = Random.Range(-terrainHeight, terrainHeight);
        terrainVertices[(terrainVertices.Length) - 1].y = Random.Range(-terrainHeight, terrainHeight);
        terrainVertices[((terrainVertices.Length) - 1) - terrainPartitions].y = Random.Range(-terrainHeight, terrainHeight);

        int iterations = (int)Mathf.Log(terrainPartitions, 2);
        int numberOfSquares = 1;
        int squareSize = terrainPartitions;
        for(int i = 0; i < iterations; i++)
        {
            int row = 0;
            for(int j= 0; j < numberOfSquares; j++)
            {
                int column = 0;
                for(int k = 0; k < numberOfSquares; k++)
                {
                    DiamondSquareAlgorithm(row, column, squareSize, terrainHeight);
                    column += squareSize;
                }
                row += squareSize;
            }
            numberOfSquares *= 2;
            squareSize /= 2;
            terrainHeight /= 2.0f;
        }

        maxTerrainHeight = -terrainHeight;
        minTerrainHeight = terrainHeight;
        for (int i = 0; i < numberOfVertices; i++)
        {
            if (terrainVertices[i].y > maxTerrainHeight)
            {
                maxTerrainHeight = terrainVertices[i].y;
            }
            if (terrainVertices[i].y < minTerrainHeight)
            {
                minTerrainHeight = terrainVertices[i].y;
            }
        }

        colors = new Color[numberOfVertices];
        for(int i = 0; i < numberOfVertices; i++)
        {      
            float vertexHeight = Mathf.InverseLerp(maxTerrainHeight, minTerrainHeight, terrainVertices[i].y);
            colors[i] = gradient.Evaluate(vertexHeight);
        }

        mesh.vertices = terrainVertices;
        mesh.uv = ab;
        mesh.triangles = triangles;
        mesh.colors = colors;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();        
    }

    void DiamondSquareAlgorithm(int row, int column, int size, float heightOffset)
    {
        int halfSize = (int)(size / 2.0f);
        int topLeft = (row * (terrainPartitions + 1)) + column;
        int bottomLeft = ((row + size) * (terrainPartitions + 1)) + column;
        int midpoint = ((int)(row + halfSize) * (terrainPartitions + 1)) + (int)(column + halfSize);
        terrainVertices[midpoint].y = ((terrainVertices[topLeft].y + terrainVertices[topLeft + size].y + terrainVertices[bottomLeft].y + terrainVertices[bottomLeft + size].y) / 4.0f) + Random.Range(-heightOffset, heightOffset);
        terrainVertices[topLeft + halfSize].y = ((terrainVertices[topLeft].y + terrainVertices[topLeft + size].y + terrainVertices[midpoint].y) / 3.0f) + Random.Range(-heightOffset, heightOffset);
        terrainVertices[midpoint - halfSize].y = ((terrainVertices[topLeft].y + terrainVertices[bottomLeft].y + terrainVertices[midpoint].y) / 3.0f) + Random.Range(-heightOffset, heightOffset);
        terrainVertices[midpoint + halfSize].y = ((terrainVertices[topLeft + size].y + terrainVertices[bottomLeft + size].y + terrainVertices[midpoint].y) / 3.0f) + Random.Range(-heightOffset, heightOffset);
        terrainVertices[bottomLeft + halfSize].y = ((terrainVertices[bottomLeft].y + terrainVertices[bottomLeft + size].y + terrainVertices[midpoint].y) / 3.0f) + Random.Range(-heightOffset, heightOffset);
    }
}
