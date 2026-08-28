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

    private void Start()
    {
        AssignSingleTruthTeller();
        AssignConversationTargets();
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

    public void AssignSingleTruthTeller()
    {
        CharacterIdentity[] allNpcs = FindObjectsByType<CharacterIdentity>();
        if (allNpcs.Length == 0) return;

        var truthTellerIndex = Random.Range(0, allNpcs.Length);

        for (var i = 0; i < allNpcs.Length; i++)
        {
            allNpcs[i].truthTeller = (i == truthTellerIndex);
        }

        Debug.Log($"[Role Assignment] '{allNpcs[truthTellerIndex].gameObject.name}' is the TRUTH TELLER!");
    }

    public void AssignConversationTargets()
    {
        CharacterIdentity[] allNpcs = FindObjectsByType<CharacterIdentity>();
        if (allNpcs.Length < 2) return;

        List<CharacterIdentity> shuffled = allNpcs.OrderBy(x => Random.value).ToList();

        for (int i = 0; i < shuffled.Count; i++)
        {
            CharacterIdentity speaker = shuffled[i];
            CharacterIdentity targetSubject = shuffled[(i + 1) % shuffled.Count];

            speaker.talkingAbout = targetSubject;
            Debug.Log($"[Gossip Setup] '{speaker.gameObject.name}' is talking about '{targetSubject.gameObject.name}'");
        }
    }
}