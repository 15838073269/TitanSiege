using System;
using UnityEngine;
using SQLite4Unity3d;
using System.Collections.Generic;

namespace Framework
{
    /// <summary>
    /// 玩家持久化数据管理器
    /// </summary>
    public class PlayerPrefsManager : MonoBehaviour
    {
        private SQLiteConnection connection;
        public SQLiteConnection Connection { get { return connection; } }

        public virtual void Awake()
        {
            Init();
        }

        public virtual void Init()
        {
            var dbFilePath = Application.persistentDataPath + "/" + Application.productName + ".db";
            connection = new SQLiteConnection(dbFilePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
            connection.CreateTable<GameData>();
        }

        public int CreateTable<T>(CreateFlags createFlags = CreateFlags.None)
        {
            return connection.CreateTable<T>(createFlags);
        }

        public int CreateTable(Type type, CreateFlags createFlags = CreateFlags.None)
        {
            return connection.CreateTable(type, createFlags);
        }

        public bool HasKey(string key) 
        {
            return connection.Table<GameData>().Where(x => x.Key == key).Count() > 0;
        }

        public void DeleteKey(string key) 
        {
            connection.Delete<GameData>(key);
        }

        public void DeleteAll() 
        {
            connection.DeleteAll<GameData>();
        }

        public void SetValue(string key, object value) 
        {
            var gameData = new GameData()
            {
                Key = key,
                Data = Newtonsoft_X.Json.JsonConvert.SerializeObject(value),
            };
            connection.InsertOrReplace(gameData);
        }

        public T GetValue<T>(string key) => GetValue<T>(key, default);

        public T GetValue<T>(string key, T defaultValue)
        {
            TryGetValue(key, out var value, defaultValue);
            return value;
        }

        public bool TryGetValue<T>(string key, out T value) => TryGetValue(key, out value, default);

        public bool TryGetValue<T>(string key, out T value, T defaultValue)
        {
            var data = connection.Table<GameData>().Where(x => x.Key == key).FirstOrDefault();
            if (data == null)
            {
                value = defaultValue;
                return false;
            }
            value = Newtonsoft_X.Json.JsonConvert.DeserializeObject<T>(data.Data);
            return true;
        }

        public virtual void OnDestroy()
        {
            connection?.Dispose();
            connection = null;
        }

        public List<T> Query<T>(string query, params object[] args) where T : new()
        {
            return connection.Query<T>(query, args);
        }

        public void Insert(object obj)
        {
            connection.Insert(obj);
        }

        public void UpdateRow(object obj)
        {
            connection.Update(obj);
        }

        public void Delete(object objectToDelete)
        {
            connection.Delete(objectToDelete);
        }

        public int GetTableCount<T>()
        {
            var map = connection.GetMapping(typeof(T));
            return connection.ExecuteScalar<int>($"SELECT COUNT(*) FROM `{map.TableName}`");
        }
    }

    [Table("GameData")]
    public class GameData
    {
        [PrimaryKey, MaxLength(255)]
        public string Key { get; set; }
        [MaxLength(65536)]
        public string Data { get; set; }

        public override string ToString()
        {
            return $"Key:{Key} Data:{Data}";
        }
    }
}