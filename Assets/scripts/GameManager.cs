using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	[SerializeField]
	private GameObject m_gamePanel;
	[SerializeField]
	private Text m_gameTextForRandom;
	[SerializeField]
	private Text m_gameTopic;
	[SerializeField]
	private GameObject m_chooseTopic;
	[SerializeField]
	private float m_randomSpeed = 0.5f;
	[SerializeField]
	private GameObject m_settingPanel;
	[SerializeField]
	private InputField m_topic;
	[SerializeField]
	private InputField m_randomText;
	[SerializeField]
	private DisplayText m_displayTemplate;
	[SerializeField]
	private Transform m_content;

	[SerializeField]
	private Button m_newTopicButton;
	[SerializeField]
	private Button m_deleteTopicButton;
	[SerializeField]
	private Button m_saveTopicButton;
	[SerializeField]
	private Button m_addRandomTextButton;

	[SerializeField]
	private RectTransform m_contentTopic;

	private int m_countCreateCell = 0;

	private int m_countCreateCellTopic = 0;

	void Awake()
	{
		//		PlayerPrefs.DeleteAll();
		SaveTextManager.InitialListOfTopic();
	}



	#region choose topic

	[SerializeField]
	private GameObject m_displayTopicTemplate;


	private void DisplayAllTopic()
	{
		foreach(Transform child in m_contentTopic)
		{
			Destroy(child.gameObject);
		}
		m_countCreateCellTopic = 0;
		Dictionary<string, Dictionary<string,string>> dic = SaveTextManager.GetAllTopic();
		foreach (KeyValuePair<string, Dictionary<string,string>> topic in dic) {
			CreateTopic(topic.Key);
		}
	}
	
	private void CreateTopic(string topic)
	{
		GameObject displayText = (GameObject)Instantiate(m_displayTopicTemplate.gameObject);
		displayText.GetComponent<DisplayTextTopic>().DisplayThisText(topic, ChooseTopicCallback);
		displayText.transform.parent = m_contentTopic;
		displayText.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
		m_countCreateCellTopic++;
		
		int maxOfDisplay = 8;
		int gapPerEach = 125;
		float defaulSize = -(gapPerEach * maxOfDisplay);
		if (m_countCreateCellTopic > maxOfDisplay) {
			m_contentTopic.offsetMin = new Vector2 (m_contentTopic.offsetMin.x, defaulSize - (gapPerEach * (m_countCreateCellTopic - maxOfDisplay)));
		}
		else
		{
			m_contentTopic.offsetMin = new Vector2 (m_contentTopic.offsetMin.x, defaulSize);
		}
		
	}

	public void ChooseTopicCallback(string topic)
	{
		if(m_gamePanel.activeSelf)
		{
			ChooseTopicGameScene(topic);
		}
		else
		{
			ChooseTopicSettingScene(topic);
		}
		DisibleGameObject(m_chooseTopic);
	}
	#endregion

	private void AllowInputBeforeChooseTopic()
	{
		m_newTopicButton.interactable = true;
		m_saveTopicButton.interactable = false;
		m_topic.interactable = false;
		m_randomText.interactable = false;
		m_deleteTopicButton.interactable = false;
		m_addRandomTextButton.interactable = false;
	}

	private void AllowInputAfterChooseTopic()
	{
		m_newTopicButton.interactable = true;
		m_deleteTopicButton.interactable = true;
		m_randomText.interactable = true;
		m_topic.interactable = false;
		m_saveTopicButton.interactable = false;
		m_addRandomTextButton.interactable = true;
	}

	private void AllowInputAfterNewTopic()
	{
		m_deleteTopicButton.interactable = false;
		m_newTopicButton.interactable = true;
		m_topic.interactable = true;
		m_saveTopicButton.interactable = true;
		m_randomText.interactable = false;
		m_addRandomTextButton.interactable = false;
	}

	private void AllowInputAfterSaveTopic()
	{
		m_deleteTopicButton.interactable = true;
		m_newTopicButton.interactable = true;
		m_topic.interactable = false;
		m_saveTopicButton.interactable = false;
		m_randomText.interactable = true;
		m_addRandomTextButton.interactable = true;
	}

	public void OpenSetting()
	{
		StopRandom();
		EnableGameObject(m_settingPanel);
		DisibleGameObject(m_gamePanel);
		AllowInputBeforeChooseTopic();
		ClearAllDisplayForInput();
		ClearAllDisplayRandomText();
	}
	private bool m_isPause = false;

	public void PauseRandom()
	{
		m_isPause = !m_isPause;
	}
	private void StopRandom()
	{
		StopCoroutine("RandomText");
	}
	public void StartRandom()
	{
		StopRandom();
		StartCoroutine("RandomText");
	}

	private IEnumerator RandomText()
	{
		m_isPause = false;
		Dictionary<string,string> dic = SaveTextManager.GetAllStringByTopic(m_gameTopic.text);
		List<string> listString = new List<string>();
		foreach (KeyValuePair<string,string> k in dic) {
			listString.Add(k.Value);
		}
		while(true)
		{
			if(!m_isPause)
			{
				int ran = Random.Range(0, listString.Count);
				if(listString.Count <= 0) yield break;
				m_gameTextForRandom.text = listString[ran];
				yield return new WaitForSeconds(m_randomSpeed);
			}

			yield return 0;
		}
	}
	public void BackToGame()
	{
		EnableGameObject(m_gamePanel);
		DisibleGameObject(m_settingPanel);
		DisibleGameObject(m_chooseTopic);
	}

	public void OpenChooseTopic()
	{
		if(m_gamePanel.activeSelf)
		{
			PauseRandom();
		}
		EnableGameObject(m_chooseTopic);
		DisplayAllTopic();
	}



	private void ChooseTopicGameScene(string topic)
	{
		m_gameTopic.text = topic;
		StartRandom();
	}

	private void ChooseTopicSettingScene(string topic)
	{
		m_topic.text = topic;
		ClearAllDisplayRandomText();
		DisplayAllTextInTopic(topic);
		AllowInputAfterChooseTopic();
	}

	private void DisplayAllTextInTopic(string topic)
	{
		Dictionary<string,string> dic = SaveTextManager.GetAllStringByTopic(topic);
		foreach (KeyValuePair<string,string> k in dic) {
			CreateNewCellForDisplayWithText(topic, k.Value);
		}
	}

	private void CreateNewCellForDisplayWithText(string topic,string text)
	{
		GameObject displayText = (GameObject)Instantiate(m_displayTemplate.gameObject);
		displayText.GetComponent<DisplayText>().DisplayThisText(topic, text, DeleteCellCallback);
		displayText.transform.parent = m_content;
		displayText.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
		m_countCreateCell++;

		int maxOfDisplay = 5;
		int gapPerEach = 75;
		float defaulSize = -(gapPerEach * maxOfDisplay);
		if (m_countCreateCell > maxOfDisplay) {
			RectTransform rectrans = m_content.GetComponent<RectTransform>();
			rectrans.offsetMin = new Vector2 (rectrans.offsetMin.x, defaulSize - (gapPerEach * (m_countCreateCell - maxOfDisplay)));
		}
		else
		{
			RectTransform rectrans = m_content.GetComponent<RectTransform>();
			rectrans.offsetMin = new Vector2 (rectrans.offsetMin.x, defaulSize);
		}

	}
	private void ClearAllDisplayRandomText()
	{
		foreach(Transform child in m_content)
		{
			Destroy(child.gameObject);
		}
		m_countCreateCell = 0;
	}

	public void SaveTopic()
	{
		string topic = m_topic.text;
		if(string.IsNullOrEmpty(topic))
			return;

		SaveTextManager.SaveTopic(topic);
		AllowInputAfterSaveTopic();
	}

	public void NewTopic()
	{
		AllowInputAfterNewTopic();
		ClearAllDisplayRandomText();
		ClearAllDisplayForInput();
	}
	private void EnableGameObject(GameObject obj)
	{
		obj.SetActive(true);
	}
	private void DisibleGameObject(GameObject obj)
	{
		obj.SetActive(false);
	}

	public void AddRandomText()
	{
		if(string.IsNullOrEmpty(m_topic.text))
			return;
		if(string.IsNullOrEmpty(m_randomText.text))
			return;

		Dictionary<string,string> dic = SaveTextManager.GetAllStringByTopic(m_topic.text);
		if(!dic.ContainsValue(m_randomText.text))
		{
			SaveTextManager.SaveThisWordByTopic(m_topic.text, m_randomText.text);
			CreateNewCellForDisplayWithText(m_topic.text, m_randomText.text);
			m_randomText.text = "";
		}
	}



	private void DeleteCellCallback()
	{
		ClearAllDisplayRandomText();
		DisplayAllTextInTopic(m_topic.text);
	}
	
	public void ClearAllDisplayForInput()
	{
		m_topic.text = "";
		m_randomText.text = "";
	}

	public void DeleteTopic()
	{
		if(string.IsNullOrEmpty(m_topic.text))
			return;

		SaveTextManager.DeleteTopic(m_topic.text);
		AllowInputBeforeChooseTopic();
		m_countCreateCell = 0;
		ClearAllDisplayForInput();
		ClearAllDisplayRandomText();
	}

}
