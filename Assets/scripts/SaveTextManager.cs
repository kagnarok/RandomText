using UnityEngine;
using System.Collections.Generic;

public class SaveTextManager
{
	//	private const string KEY_TO_SAVE_WORD = "Save_text";
	private const string KEY_TO_SAVE_TOPIC = "Save_topic";
	//	private static List<int> m_lastIndexForWordEachTopic = new List<int>();
	private static int m_lastIndexForTopic = 0;
	private static bool m_isInitialze;
	private static Dictionary<string, Dictionary<string,string>> m_dicTopic;
	//	private static Dictionary<string, string> m_dicOfWord;
	
	public static void InitialListOfTopic ()
	{
		if (!m_isInitialze) {
			m_isInitialze = true;
			int maxFor = 10000;
			m_dicTopic = new Dictionary<string, Dictionary<string, string>>();
			for (int topicIndex = 0; topicIndex < maxFor; topicIndex++) {
				string keyTopic = KEY_TO_SAVE_TOPIC + topicIndex;
				string runningTopicString = PlayerPrefs.GetString (keyTopic, "#");
				if(runningTopicString == "#")
					break;

				Dictionary<string, string> dicOfWord = new Dictionary<string, string> ();
				
				for(int wordIndex = 0; wordIndex < maxFor; wordIndex++)
				{
					//topic_x
					string keyTextForSaveWord = runningTopicString+ "_" + wordIndex;
					string runnigWordForThisTopic = PlayerPrefs.GetString (keyTextForSaveWord, "#");
					//Debug.Log("runnigWordForThisTopic "+runnigWordForThisTopic);
					if (runnigWordForThisTopic == "#")
						break;
					dicOfWord.Add (keyTextForSaveWord, runnigWordForThisTopic);
				}

				m_dicTopic.Add(runningTopicString, dicOfWord);
				m_lastIndexForTopic++;
			}
			InitDefaultIfNeed();
		} 
	}

	private static void InitDefaultIfNeed()
	{
		bool isNotTopic = m_dicTopic.Count == 0;
		if(isNotTopic) InitDefaultTopicAndText();
	}
	private static void InitDefaultTopicAndText()
	{
		string TopicName = "Default";
		SaveTextManager.SaveTopic(TopicName);
		SaveTextManager.SaveThisWordByTopic(TopicName, "Text1");
		SaveTextManager.SaveThisWordByTopic(TopicName, "Text2");
		SaveTextManager.SaveThisWordByTopic(TopicName, "Text3");
		SaveTextManager.SaveThisWordByTopic(TopicName, "Text4");
		SaveTextManager.SaveThisWordByTopic(TopicName, "Text5");
		SaveTextManager.SaveThisWordByTopic(TopicName, "Text6");
	}
	public static void SaveTopic(string topic)
	{
		if(!m_dicTopic.ContainsKey(topic))
		{
			//Debug.Log("SaveTopic "+(KEY_TO_SAVE_TOPIC + m_lastIndexForTopic));
			PlayerPrefs.SetString (KEY_TO_SAVE_TOPIC + m_lastIndexForTopic, topic);
			m_dicTopic.Add(topic, new Dictionary<string, string> ());
			m_lastIndexForTopic++;
		}
	}

	public static void SaveThisWordByTopic (string topic, string text)
	{
		if(!m_dicTopic[topic].ContainsValue(text))
		{
			Dictionary<string, string> dicOfWord = m_dicTopic[topic];
			string keyTextForSaveWord = topic + "_"+dicOfWord.Count;
			//Debug.Log("keyTextForSaveWord "+keyTextForSaveWord);
			//Debug.Log("SaveThisWordByTopic "+topic+" word "+text);
			PlayerPrefs.SetString (keyTextForSaveWord, text);
			dicOfWord.Add (keyTextForSaveWord, text);

			if(m_dicTopic.ContainsKey(topic))
				m_dicTopic.Remove(topic);
			m_dicTopic.Add(topic, dicOfWord);
		}
	}

	public static void DeleteThisWordByTopic (string topic, string text)
	{
		if(m_dicTopic.ContainsKey(topic))
		{
			Dictionary<string, string> dicOfWord = new Dictionary<string, string>(m_dicTopic[topic]);
			string KeepKeyForSwapValue = "";
			bool beginRepeat = false;
			foreach (KeyValuePair<string,string> k in dicOfWord) {
				if (beginRepeat) {
					PlayerPrefs.SetString (KeepKeyForSwapValue, k.Value);
					m_dicTopic[topic][KeepKeyForSwapValue] = k.Value;
					KeepKeyForSwapValue = k.Key;
				}
				
				if (k.Value.Equals (text)) {
					KeepKeyForSwapValue = k.Key;
					beginRepeat = true;
				}
			}
			PlayerPrefs.DeleteKey (KeepKeyForSwapValue);
			m_dicTopic[topic].Remove(KeepKeyForSwapValue);
		}
	}

	public static void DeleteTopic(string topic)
	{
		if(m_dicTopic.ContainsKey(topic))
		{
			bool beginRepeat = false;
			int maxFor = 10000;
			for (int topicIndex = 0; topicIndex < maxFor; topicIndex++) {
				string keyTopic = KEY_TO_SAVE_TOPIC + topicIndex;
				string runningTopicString = PlayerPrefs.GetString (keyTopic, "#");
				if(runningTopicString == "#")
					break;
				if (beginRepeat) {
//					Dictionary<string, Dictionary<string,string>> oldValueInDic = m_dicTopic[runningTopicString];
//					m_dicTopic.Remove(
					PlayerPrefs.DeleteKey(keyTopic);
					PlayerPrefs.SetString((KEY_TO_SAVE_TOPIC + (topicIndex-1)), runningTopicString);
				}
				if(runningTopicString == topic)
				{
					beginRepeat = true;
					PlayerPrefs.DeleteKey(keyTopic);
					m_dicTopic.Remove(runningTopicString);

					for(int wordIndex = 0; wordIndex < maxFor; wordIndex++)
					{
						//topic_x
						string keyTextForSaveWord = runningTopicString+ "_" + wordIndex;
						string runnigWordForThisTopic = PlayerPrefs.GetString (keyTextForSaveWord, "#");
						if (runnigWordForThisTopic == "#")
							break;
						PlayerPrefs.DeleteKey(keyTextForSaveWord);
					}
					m_lastIndexForTopic--;
				}
			}
		}
	}
	public static Dictionary<string, Dictionary<string,string>> GetAllTopic()
	{
		InitDefaultIfNeed();
		return m_dicTopic;
	}
	public static Dictionary<string,string> GetAllStringByTopic (string topic)
	{
		return m_dicTopic [topic];
	}
}
