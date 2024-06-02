using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class JsonManager : MonoBehaviour
{
    private string filePath;

    private void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "data.json");
    }

    public void SaveData(MyData data)
    {
        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(filePath, json);
    }

    public MyData LoadData()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<MyData>(json);
        }

        return null;
    }
}

[System.Serializable]
public class MyData
{
    public string name;
    public int score;
}


public class GameManager : MonoBehaviour
{
    private JsonManager jsonManager;
    private XmlManager xmlManager;

    private void Start()
    {
        jsonManager = GetComponent<JsonManager>();
        xmlManager = GetComponent<XmlManager>();

        MyData data = new MyData { name = "Player1", score = 100 };
        jsonManager.SaveData(data); 

        MyData loadedData = jsonManager.LoadData(); 
        if (loadedData != null)
        {
            Debug.Log($"Name: {loadedData.name}, Score: {loadedData.score}");
        }
    }
}