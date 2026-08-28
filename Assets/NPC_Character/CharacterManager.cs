using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance;

    [Header("Dependencies")]
    public TextAsset masterJsonFile;        
    public GameObject characterPrefab;      

    private Dictionary<string, LpcCharacter> _database = new Dictionary<string, LpcCharacter>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else 
        {
            Destroy(gameObject);
            return;
        }

        LoadDatabase();
    }

    private void LoadDatabase()
    {
        if (masterJsonFile == null)
        {
            Debug.LogError("Master JSON file is not assigned on CharacterManager!");
            return;
        }

        var loadedData = JsonConvert.DeserializeObject<List<LpcCharacter>>(masterJsonFile.text);
        _database.Clear();
        foreach (var c in loadedData)
        {
            _database[c.id] = c;
        }
        Debug.Log($"Loaded {_database.Count} characters into database.");
    }

    // --- Data Queries ---

    public LpcCharacter GetCharacterData(string charId)
    {
        if (_database.TryGetValue(charId, out LpcCharacter data))
        {
            return data;
        }
        Debug.LogError($"Character ID '{charId}' not found in database!");
        return null;
    }

    public LpcCharacter GetRandomCharacterData()
    {
        if (_database.Count == 0) return null;
        int randomIndex = Random.Range(0, _database.Count);
        string randomId = _database.Keys.ElementAt(randomIndex);
        return _database[randomId];
    }

    // --- Dynamic Spawning Methods ---

    public CharacterIdentity SpawnCharacter(string charId, Vector3 position)
    {
        LpcCharacter data = GetCharacterData(charId);
        if (data != null)
        {
            GameObject newChar = Instantiate(characterPrefab, position, Quaternion.identity);
            CharacterIdentity identity = newChar.GetComponent<CharacterIdentity>();
            identity.Initialize(data);
            return identity;
        }
        return null;
    }

    public CharacterIdentity SpawnRandomCharacter(Vector3 position)
    {
        LpcCharacter data = GetRandomCharacterData();
        if (data != null)
        {
            GameObject newChar = Instantiate(characterPrefab, position, Quaternion.identity);
            CharacterIdentity identity = newChar.GetComponent<CharacterIdentity>();
            identity.Initialize(data);
            return identity;
        }
        return null;
    }
}