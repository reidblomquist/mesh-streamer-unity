using UnityEngine;
using MiniJSON;
using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;

public class RequestWWW
{
	public bool IsDone
	{
		get;
		private set;
	}

	public string Text
	{
		get;
		private set;
	}

	public string Error
	{
		get;
		private set;
	}

	public IEnumerator doHttpPost(string url, byte[] payload, int meshKey)
	{
		Dictionary<string, string> headers = new Dictionary<string, string>();

		headers.Add("Content-Type", "application/octet-stream");
		headers.Add("Content-Length", payload.Length.ToString());

		if (meshKey != -1)
		{
			headers.Add("slot-key", meshKey.ToString());
		}

		WWW www = new WWW(url, payload, headers);
		yield return www;
		IsDone = true;
		Error = www.error;
		Text = www.text;
	}
}

public class MeshSenderHTTP : MonoBehaviour {
	private MeshSerializer serializer = new MeshSerializer();
	private RequestWWW wwwcall;

	string rootServerUrl = "";
	string authorName = "";
	string title = "";

	public bool isRegistered = false;

	bool needsToSend = true;

	int meshSlot = -1;
	int meshKey = 0;

	public void Construct(string rootServerUrl, string authorName, string title)
	{
		this.rootServerUrl = rootServerUrl;
		this.authorName = authorName;
		this.title = title;
	}

	public void Register() {
		var regMsg = new Dictionary<string, object>();

		regMsg.Add("author", this.authorName);
		regMsg.Add("title", this.title);
		regMsg.Add("platform", "Unity3D");

		byte[] registration = Encoding.UTF8.GetBytes(Json.Serialize(regMsg));

		wwwcall = new RequestWWW();
		StartCoroutine(wwwcall.doHttpPost(this.rootServerUrl + "/mesh/register", registration, -1));
		if (wwwcall != null && wwwcall.IsDone)
		{
			if (wwwcall.Error != null)
			{
				Debug.Log("error registering: " + wwwcall.Error);
			}
			else
			{
				var result = Json.Deserialize(wwwcall.Text) as Dictionary<string, object>;

				if ((bool)result["result"])
				{
					isRegistered = true;
					meshKey = (int)result["key"];
					meshSlot = (int)result["index"];
				}
				else
				{
					isRegistered = false;
					Debug.Log("Unable to register: " + (string)result["error"]);
				}
			}
			wwwcall = null;
		}

	}

	public void sendFrame(Mesh mesh)
	{

		if (this.needsToSend)
		{
			wwwcall = new RequestWWW();
			StartCoroutine(wwwcall.doHttpPost(this.rootServerUrl + "/mesh/" + this.meshSlot + "/frame", serializer.Serialize(mesh), meshKey));
		}

	}
}
