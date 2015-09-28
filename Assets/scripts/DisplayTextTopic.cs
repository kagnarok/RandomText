using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DisplayTextTopic : MonoBehaviour {

	[SerializeField]
	private Text m_text;
	public delegate void SelectTopicCallBack(string topic);
	private SelectTopicCallBack m_callback;
	// Use this for initialization
	public void DisplayThisText (string topic,SelectTopicCallBack callback) {
		m_callback = callback;
		m_text.text = topic;
	}
	
	public void SelectThisTopic()
	{
		m_callback(m_text.text);
	}
}
