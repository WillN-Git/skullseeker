using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

/*
This class is responsible for generating the cave of the game.
It's implement a marching square algorithm
For more details : https://en.wikipedia.org/wiki/Marching_squares
*/

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class ProceduralTerrainGenerator : MonoBehaviour
{
    ///===============================================
    /// 
    ///              INTERAL TYPES
    /// 
    ///===============================================
    enum NoiseType
    {
        NONE,
        PERLIN
    }


    ///===============================================
    /// 
    ///                 MEMBERS
    /// 
    ///===============================================

    [Header("PTG")]
    [SerializeField] 
    private int m_size = 15;

    [SerializeField]
    private float m_noiseResolution = 0.1f;

    [SerializeField]
    private float m_gridResolution = 1f;

    [SerializeField]
    private float m_weightsThreshold = .5f;

    [SerializeField]
    private Material m_material;

    private float[,] m_scalars;

    [SerializeField]
    private MeshCollider m_collider;

    private MeshFilter m_meshFilter;

    private MeshRenderer m_meshRenderer;

    private List<Vector3> m_vertices = new List<Vector3>();
    
    private List<int> m_triangles = new List<int>();

    private List<Vector2> m_uvs = new List<Vector2>();

    [SerializeField]
    private GameObject m_player;

    [Header("Debug")]
    [SerializeField]
    private bool m_showScalarField = false;

    [SerializeField]
    private Transform m_debugVisualizerParent;

    [SerializeField]
    private Transform m_debugVisualizerPrefab;

    ///===============================================
    /// 
    ///                 FUNCTIONS
    /// 
    ///===============================================
    
    void Start()
    {
        m_meshFilter = GetComponent<MeshFilter>();
        m_meshRenderer = GetComponent<MeshRenderer>();
        m_collider = GetComponent<MeshCollider>();

#if DEBUG_SCALAR_FIELD
        _UpdateDebugVisualization();
#endif
    }

    void Update()
    {
    }

    private void _CreateMesh()
    {
        Mesh mesh = new Mesh();

        mesh.vertices = m_vertices.ToArray();
        mesh.triangles = m_triangles.ToArray();
        mesh.uv = m_uvs.ToArray();
        // mesh.RecalculateNormals();

        m_collider.sharedMesh = mesh;

        m_meshFilter.mesh = mesh;
        m_meshRenderer.material = m_material;
    }

    private void _MarchSquares()
    {
        m_vertices.Clear();
        m_triangles.Clear();
        m_uvs.Clear();

        for (int i = 0; i < m_size; i++)
        {
            for (int j = 0; j < m_size; j++)
            {
                // top: t, bottom : b, left: l, right: r
                int bl = _GetActivationValue(m_scalars[i, j]);
                int br = _GetActivationValue(m_scalars[i + 1, j]);
                int tr = _GetActivationValue(m_scalars[i + 1, j + 1]);
                int tl = _GetActivationValue(m_scalars[i, j + 1]);

                _MarchSquare(tl, tr, br, bl, i, j);
            }
        }
    }
    private void _MarchSquare(int v00, int v01, int v10, int v11, int offsetX, int offsetY)
    {
        int value = v00 | v01 << 1 | v10 << 2 | v11 << 3;

        int vertexCount = m_vertices.Count;

        Vector3[] localVertices = new Vector3[6];
        int[] localTriangles = new int[6];

        // Debug.Log($"{offsetX},{offsetY} : {value}");
        switch (value)
        {
            case 0:
                return;
            case 1:
                localVertices = new Vector3[]{ new Vector3(0, 1f), new Vector3(0, 0.5f), new Vector3(0.5f, 1) };
                localTriangles = new int[]{ 2, 1, 0};
                break;
            case 2:
                localVertices = new Vector3[]{ new Vector3(1, 1), new Vector3(1, 0.5f), new Vector3(0.5f, 1) };
                localTriangles = new int[]{ 0, 1, 2};
                break;
            case 3:
                localVertices = new Vector3[]{ new Vector3(0, 0.5f), new Vector3(0, 1), new Vector3(1, 1), new Vector3(1, 0.5f) };
                localTriangles = new int[]{ 0, 1, 2, 0, 2, 3};
                break;
            case 4:
                localVertices = new Vector3[]{ new Vector3(1, 0), new Vector3(0.5f, 0), new Vector3(1, 0.5f) };
                localTriangles = new int[]{ 0, 1, 2};
                break;
            case 5:
                localVertices = new Vector3[]{ new Vector3(0, 0.5f), new Vector3(0, 1), new Vector3(0.5f, 1), new Vector3(1, 0), new Vector3(0.5f, 0), new Vector3(1, 0.5f) };
                localTriangles = new int[]{ 0, 1, 2, 3, 4, 5};
                break;
            case 6:
                localVertices = new Vector3[]{ new Vector3(0.5f, 0), new Vector3(0.5f, 1), new Vector3(1, 1), new Vector3(1, 0) };
                localTriangles = new int[]{ 0, 1, 2, 0, 2, 3};
                break;
            case 7:
                localVertices = new Vector3[]{ new Vector3(0, 1), new Vector3(1, 1), new Vector3(1, 0), new Vector3(0.5f, 0), new Vector3(0, 0.5f) };
                localTriangles = new int[]{ 2, 3, 1, 3, 4, 1, 4, 0, 1};
                break;
            case 8:
                localVertices = new Vector3[]{ new Vector3(0, 0.5f), new Vector3(0, 0), new Vector3(0.5f, 0) };
                localTriangles = new int[]{ 2, 1, 0};
                break;
            case 9:
                localVertices = new Vector3[]{ new Vector3(0, 0), new Vector3(0.5f, 0), new Vector3(0.5f, 1), new Vector3(0, 1) };
                localTriangles = new int[]{ 1, 0, 2, 0, 3, 2};
                break;
            case 10:
                localVertices = new Vector3[]{ new Vector3(0, 0), new Vector3(0, 0.5f), new Vector3(0.5f, 0), new Vector3(1, 1), new Vector3(0.5f, 1), new Vector3(1, 0.5f) };
                localTriangles = new int[]{ 0, 1, 2, 5, 4, 3};
                break;
            case 11:
                localVertices = new Vector3[]{ new Vector3(0, 0), new Vector3(0, 1), new Vector3(1, 1), new Vector3(1, 0.5f), new Vector3(0.5f, 0) };
                localTriangles = new int[]{ 0, 1, 2, 0, 2, 3, 4, 0, 3};
                break;
            case 12:
                localVertices = new Vector3[]{ new Vector3(0, 0), new Vector3(1, 0), new Vector3(1, 0.5f), new Vector3(0, 0.5f) };
                localTriangles = new int[]{ 0, 3, 2, 0, 2, 1};
                break;
            case 13:
                localVertices = new Vector3[]{ new Vector3(0, 0), new Vector3(0, 1), new Vector3(0.5f, 1), new Vector3(1, 0.5f), new Vector3(1, 0) };
                localTriangles = new int[]{ 0, 1, 2, 0, 2, 3, 0, 3, 4};
                break;
            case 14:
                localVertices = new Vector3[]{ new Vector3(1, 1), new Vector3(1, 0), new Vector3(0, 0), new Vector3(0, 0.5f), new Vector3(0.5f, 1) };
                localTriangles = new int[]{ 0, 1, 4, 1, 3, 4, 1, 2, 3};
                break;
            case 15:
                localVertices = new Vector3[]{ new Vector3(0, 0), new Vector3(0, 1), new Vector3(1, 1), new Vector3(1, 0) };
                localTriangles = new int[]{ 0, 1, 2, 0, 2, 3};
                break;

        }

        foreach (var v in localVertices)
        {
            Vector3 vertex = new Vector3((v.x + offsetX) * m_gridResolution, (v.y + offsetY) * m_gridResolution, 0);
            m_vertices.Add(vertex);
            m_uvs.Add(new Vector2(vertex.x, vertex.y));
        }

        foreach (var t in localTriangles)
        {
            m_triangles.Add(t + /* offset= */ vertexCount);
        }
    }

    private float frac(float x)
    {
        return (x >= 0) ? x - Mathf.Floor(x) : x - Mathf.Ceil(x);
    }

    private int _GetActivationValue(float v)
    {
        return v < m_weightsThreshold ? 0 : 1;
    }

    private void _GenerateWeights()
    {
        m_scalars = new float[m_size + 1, m_size + 1];

        for (int i = 0; i <= m_size; i++)
            for (int j = 0; j <= m_size; j++)
                m_scalars[i,j] = Mathf.PerlinNoise(i * m_noiseResolution, j * m_noiseResolution);
    }

    private void _UpdateDebugVisualization()
    {
        StartCoroutine(_UpdateDebugVisualizationCO());
    }

    private IEnumerator _UpdateDebugVisualizationCO()
    {

        _GenerateWeights();
        _MarchSquares();
        _CreateMesh();
        _DrawScalarField();

        m_debugVisualizerParent.gameObject.SetActive(m_showScalarField);

        yield return new WaitForSeconds(0.3f);

        StartCoroutine(_UpdateDebugVisualizationCO());
    }

    private void _DrawScalarField()
    {
        foreach (Transform t in m_debugVisualizerParent)
        {
            Destroy(t.gameObject);
        }

        for (int i = 0; i <= m_size; i++)
            for (int j = 0; j <= m_size; j++)
            {
                Vector2 pos = transform.TransformPoint(new Vector2(i * m_gridResolution, j * m_gridResolution));
                Transform debugger = Instantiate(m_debugVisualizerPrefab, pos, new Quaternion(), m_debugVisualizerParent);
                debugger.localScale = Vector2.one * m_gridResolution / 2f;

                // shade the debugger visualizer
                debugger.GetComponent<SpriteRenderer>().color = new Color(m_scalars[i, j], m_scalars[i, j], m_scalars[i, j], 1);
            }
    }
}
