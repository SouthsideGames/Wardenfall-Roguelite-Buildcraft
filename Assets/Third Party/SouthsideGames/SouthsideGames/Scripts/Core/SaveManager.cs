
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

        public static GameData GameData { get; private set; }

        private string dataPath;

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);

            dataPath = GetDataPath();

            DontDestroyOnLoad(this);
            Load();
        }

        private static string GetDataPath()
        {
#if UNITY_EDITOR
            return Application.dataPath + "/GameData.txt";
#else
            return Application.persistentDataPath + "/GameData.txt";
#endif
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

        private static void Save() => instance?.LocalSave();

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
            if (GameData == null)
            {
                value = null;
                return false;
            }

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
            return scriptType + "_" + key;
        }

        public static void Remove(object sender, string key)
        {
            string fullKey = GetFullKey(sender, key);
            GameData.Remove(fullKey);
        }

        [NaughtyAttributes.Button]
        public static void ClearData()
        {
            string path = GetDataPath();

            if (File.Exists(path))
                File.Delete(path);

            GameData = new GameData();

            if (instance != null)
                instance.LocalSave();

            Debug.LogWarning("🧹 All saved data has been cleared successfully.");
        }
    }

    [Serializable]
    struct SerializableObject<T>
    {
        [SerializeField] public T myObject;
    }
}