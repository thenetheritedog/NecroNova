using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;
public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;
    public static DataPersistenceManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null) 
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);

        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("load");
        this.dataPersistenceObjects = FindAllDataPersitenceObjects();
        LoadGame();
    }
    public void OnSceneUnloaded(Scene scene)
    {
        Debug.Log("unload");
        SaveGame();
    }
    public void NewGame() 
    { 
        this.gameData = new GameData();
    }
    public void LoadGame() 
    {
        this.gameData = dataHandler.Load();
        if (this.gameData == null)
        {
            Debug.Log("Go play a new game");
            NewGame();
        }

        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }
    }
    public void SaveGame() 
    {
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref gameData);
        }
        dataHandler.Save(gameData);
    }

    private List<IDataPersistence> FindAllDataPersitenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>(true).
            OfType<IDataPersistence>();
        return new List<IDataPersistence>(dataPersistenceObjects);
    }
    private void OnApplicationQuit()
    {
        SaveGame();
    }
    public void Test()
    {
        this.dataPersistenceObjects = FindAllDataPersitenceObjects();
    }
}
