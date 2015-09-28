using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DisplayText : MonoBehaviour {
	[SerializeField]
	private Text m_text;
	private string m_thisTopic;
	public delegate void DeleteTextCallBack();
	private DeleteTextCallBack m_callback;
	// Use this for initialization
	public void DisplayThisText (string topic,string text,DeleteTextCallBack callback) {
		m_callback = callback;
		m_thisTopic = topic;
		m_text.text = text;
	}

	public void RemoveThisText()
	{
		SaveTextManager.DeleteThisWordByTopic(m_thisTopic, m_text.text);
		m_callback();
	}

}
