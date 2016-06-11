using UnityEngine;
using MiniJSON;
using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;

public class MeshSenderHTTP : MonoBehaviour {
	private MeshSerializer serializer = new MeshSerializer();

	string rootServerUrl = "";
	string authorName = "";
	string title = "";

	bool isRegistered = false;

	bool needsToSend = true;

	int meshSlot = -1;
	int meshKey = 0;

	public MeshSenderHTTP (string rootServerUrl, string authorName, string title)
	{
		this.rootServerUrl = rootServerUrl;
		this.authorName = authorName;
		this.title = title;

		var regMsg = new Dictionary<string, object>();

		regMsg.Add("author", this.authorName);
		regMsg.Add("title", this.title);
		regMsg.Add("platform", "Unity3D");

		byte[] registration = Encoding.UTF8.GetBytes(Json.Serialize(regMsg));

		string responseText = doHTTPPost(this.rootServerUrl + "/mesh/register", registration);

		var result = Json.Deserialize(responseText) as Dictionary<string,object>;

		if ((bool) result["result"])
		{
			isRegistered = true;
			meshKey = (int) result["key"];
			meshSlot = (int) result["index"];
		}
		else
		{
			isRegistered = false;
			Debug.Log("Unable to register: " + (string) result["error"]);
		}
	}

	public IEnumerator sendFrame(Mesh mesh)
	{

		if (this.isRegistered && this.needsToSend)
		{
			doHTTPPost(this.rootServerUrl + "/mesh/" + this.meshSlot + "/frame", serializer.Serialize(mesh), meshKey);
		}
		yield return null;

	}

	string doHTTPPost(string urlString, byte[] payload)
	{
		return doHTTPPost(urlString, payload, -1);
	}

	string doHTTPPost(string urlString, byte[] payload, int meshKey)
	{
		Dictionary<string, string> headers = new Dictionary<string, string>();

		headers.Add("Content-Type", "application/octet-stream");
		headers.Add("Content-Length", payload.Length.ToString());

		if (meshKey != -1)
		{
			headers.Add("slot-key", meshKey.ToString());
		}

		WWW www = new WWW(urlString, payload, headers);

		if (www.error != null)
		{
			return www.error;
		} else {
			return www.text;
		}
	}
}
