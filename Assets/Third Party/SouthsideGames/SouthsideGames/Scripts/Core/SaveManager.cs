using System.IO;
using UnityEngine;
using System.Linq;
using Leguar.TotalJSON;
using System;
using System.Reflection;


namespace SouthsideGames.SaveManager
{
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager instance;

        private string dataPath;

        public static GameData GameData { get; private set; }

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);

#if UNITY_EDITOR
            dataPath = Application.dataPath + "/GameData.txt";
#else
        dataPath = Application.persistentDataPath + "/GameData.txt";
#endif

            DontDestroyOnLoad(this);
            Load();
        }

        private void Start()
        {
        }

        private void LocalSave()
        {
            StreamWriter writer = new StreamWriter(dataPath);

            JSON gameDataJSon = JSON.Serialize(GameData);
            string dataString = gameDataJSon.CreatePrettyString();

            writer.WriteLine(dataString);

            writer.Close();
        }

        private void Load()
        {
            if (!File.Exists(dataPath))
            {
                GameData = new GameData();
                LocalSave();
            }
            else
            {
                StreamReader reader = new StreamReader(dataPath);
                string dataString = reader.ReadToEnd();
                reader.Close();

                JSON gameDataJson = JSON.ParseString(dataString);
                GameData = gameDataJson.Deserialize<GameData>();

            }

            foreach (IWantToBeSaved saveable in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IWantToBeSaved>())
                saveable.Load();
            
        }

        private static void Save() => instance.LocalSave();

        public static void Save(object sender, string key, object data)
        {
            string fullKey = GetFullKey(sender, key);
            string jsonData;
            Type dataType = data.GetType();

            Type serializableOjectType = typeof(SerializableObject<>).MakeGenericType(dataType);
            object mySerializableObject = Activator.CreateInstance(serializableOjectType);

            FieldInfo myObjectField = serializableOjectType.GetField("myObject");

            myObjectField.SetValue(mySerializableObject, data);

            jsonData = JSON.Serialize(mySerializableObject).CreatePrettyString();

            dataType = serializableOjectType;

            GameData.Add(fullKey, dataType, jsonData);
            Save();
        }

        public static bool TryLoad(object sender, string key, out object value)
        {
           string fullKey = GetFullKey(sender, key);

            if (GameData.TryGetValue(fullKey, out Type dataType, out string data))
            {
                DeserializeSettings settings = new DeserializeSettings();

                JSON jsonObject = JSON.ParseString(data);
                value = jsonObject.zDeserialize(dataType, "fieldName", settings);

                FieldInfo myObject = value.GetType().GetField("myObject");
                value = myObject.GetValue(value);

                return true;
            }

            value = null;
            return false;
        }

        private static string GetFullKey(object sender, string key)
        {
            string scriptType = sender.GetType().ToString();
            string fullKey = scriptType + "_" + key;

            return fullKey;
        }

        public static void Remove(object sender, string key)
        {
            string fullKey = GetFullKey(sender, key);
            GameData.Remove(fullKey);
        }

        [NaughtyAttributes.Button]
        public static void ClearData()
        {
            if (File.Exists(instance.dataPath))
            {
                File.Delete(instance.dataPath); 
            }

            GameData = new GameData(); 
            instance.LocalSave();      
        }

    }

    [Serializable]
    struct SerializableObject<T>
    {
        [SerializeField] public T myObject;
    }
}