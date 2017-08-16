using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ConnectButtonUI : MonoBehaviour
{
	[SerializeField]
	private GameObject prefab;

	public void Init()
	{
		foreach (var btn in this.gameObject.GetComponentsInChildren<Button> ())
			Destroy (btn.gameObject);	
	}

	public void Add(string deviceId, UnityAction onClickAction)
	{
		var obj = Instantiate (prefab, this.transform);
		obj.GetComponent<Button> ().onClick.AddListener (onClickAction);
		obj.GetComponentInChildren<Text> ().text = deviceId;
	}
}
