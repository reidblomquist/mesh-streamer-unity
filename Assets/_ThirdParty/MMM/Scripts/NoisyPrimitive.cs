using UnityEngine;
using System.Collections;
using ProceduralToolkit;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class NoisyPrimitive : MonoBehaviour {

    public float noiseSpeed = 0.25f;
    internal float _noiseSpeedDelta;
    public float noiseScale = 0.1f;
    internal MeshFilter _filter;
    internal MeshDraft _draft;
    internal MeshDraft _draftOriginal;
    internal bool viewingOriginal = true;

	void Start () {
        _filter = GetComponent<MeshFilter>();
        _draft = GenerateMesh();
        _draftOriginal = new MeshDraft(_draft.ToMesh());
        _filter.mesh = _draft.ToMesh();
        InvokeRepeating("ToggleNoise", noiseSpeed, noiseSpeed);
	}

    MeshDraft GenerateMesh()
    {
        float radius = 1f;
        MeshDraft draft = MeshDraft.Octahedron(radius);

        // Randomize Colors
        int numColors = draft.vertices.Count;
        draft.colors.Clear();
        for (int i = 0; i < numColors; i++)
        {
            draft.colors.Add(RandomE.colorHSV);
        }
        return draft;
    }

    MeshDraft AddNoiseToDraft()
    {
        MeshDraft draft = new MeshDraft(_draftOriginal.ToMesh());
        var noiseOffset = new Vector2(Random.Range(0f, 100f), Random.Range(0f, 100f));
        for (int i = 0; i < draft.triangles.Count; i++)
        {
            // Calculate noise value
            int v1 = draft.triangles[i];
            Vector3 vertex = draft.vertices[v1];
            float x = vertex.x + (Random.Range(-noiseScale, noiseScale));
            float y = vertex.y + (Random.Range(-noiseScale, noiseScale));
            float z = vertex.z + (Random.Range(-noiseScale, noiseScale));
            draft.vertices[i] = new Vector3(x, y, z);
        }
        return draft;
    }

    void ToggleNoise()
    {
        if (viewingOriginal)
        {
            _draft = AddNoiseToDraft();
            _filter.mesh = _draft.ToMesh();
        } else
        {
            _filter.mesh = _draftOriginal.ToMesh();
        }
        viewingOriginal = !viewingOriginal;
    }
	
	void Update () {
        if(noiseSpeed != _noiseSpeedDelta && noiseSpeed > 0.0001f)
        {
            CancelInvoke("ToggleNoise");
            InvokeRepeating("ToggleNoise", noiseSpeed, noiseSpeed);
            _noiseSpeedDelta = noiseSpeed;
        }
    }

}