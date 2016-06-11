using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter), typeof(MeshSenderHTTP))]
public class MeshFilterStreamer : MonoBehaviour {

    public string serverUrl = "localhost:8080";
    public string authorName = "Joe Shmoe";
    public string title = "Really broken proc mesh";
    internal MeshFilter _filter;
    internal MeshSenderHTTP _sender;

    void Start () {
        _filter = GetComponent<MeshFilter>();
        _sender = GetComponent<MeshSenderHTTP>();
        _sender.Construct(serverUrl, authorName, title);
        _sender.Register();
    }
	
	void Update () {
        _sender.SetMesh(_filter.mesh);
	}

}
