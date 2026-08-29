using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public enum FacingDirection
{
    Up = 0,
    Left = 1,
    Down = 2,
    Right = 3
}

[System.Serializable]
public class LpcItem 
{
    public string name;
    public string color;
}

[System.Serializable]
public class LpcCharacter 
{
    public string id;
    public string gender;
    public string expression;
    public LpcItem hair;
    public LpcItem shirt;
    public LpcItem pants;
    public LpcItem shoes;
    public LpcItem hat;
}

public class CharacterIdentity : MonoBehaviour
{
    private static readonly string[] MaleNames = 
    { 
        "Arthur", "Bob", "Cedric", "Daniel", "Edward", "Felix", "George", 
        "Harry", "Isaac", "Jack", "Leo", "Miles", "Noah", "Oliver", 
        "Peter", "Robin", "Samuel", "Thomas", "Victor", "William" 
    };

    private static readonly string[] FemaleNames = 
    { 
        "Alice", "Bella", "Clara", "Daisy", "Elena", "Flora", "Grace", 
        "Hazel", "Iris", "Julia", "Luna", "Maya", "Nora", "Olivia", 
        "Penny", "Rose", "Stella", "Tessa", "Violet", "Willow" 
    };


    private static readonly HashSet<string> UsedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetUsedNames()
    {
        UsedNames.Clear();
    }

    // --- Option Pools ---
    private static readonly string[] Genders = { "male", "female" };
    private static readonly string[] Expressions = { "neutral", "happy", "anger", "sad", "blush", "shock" };
    private static readonly string[] Colors = { "Red", "Blue", "Green", "Black", "White", "Brown", "Yellow", "Purple", "Orange", "Navy", "Pink" };
    private static readonly string[] HairOptions = { "flat top fade", "bangs", "bob", "curtains", "messy", "long", "Bald" };
    private static readonly string[] ShirtOptions = { "shortsleeve", "longsleeve", "overalls" };
    private static readonly string[] PantsOptions = { "pants", "pantaloons", "hose", "leggings" };
    private static readonly string[] ShoesOptions = { "boots/rimmed", "boots/basic", "shoes/basic", "slippers" };
    private static readonly string[] HatOptions = { "bandana", "hood", "leather cap", "tophat", "wizard" };

    private LpcCharacter _data;
    public LpcCharacter Data 
    { 
        get 
        {
            if (_data == null && autoGenerateOnStart)
            {
                AutoInitialize();
            }
            return _data;
        } 
        private set => _data = value; 
    }    

    [Header("Identity")]
    [Tooltip("Leave blank to randomly generate based on gender")]
    public string characterName = "";

    [Header("Role Settings")]
    public bool truthTeller = false;

    [Header("Conversation Target")]
    public CharacterIdentity talkingAbout;

    [Header("Scene Placement Settings")]
    public bool autoGenerateOnStart = true;
    public string specificCharacterId = "";

    [Header("Animation Settings")]
    public FacingDirection facing = FacingDirection.Down;
    [Range(0.1f, 1f)]
    public float animationSpeed = 0.4f;

    private SpriteRenderer _sr;
    private Sprite[] _allSlices;
    private Sprite[] _currentFrames;
    private int _frameIndex = 0;
    private float _timer = 0f;

    // --- Cached Persistent Fake Data ---
    private bool _fakeDataGenerated = false;
    private string _fakeGender;
    private string _fakeHairStyle;
    private string _fakeHairColor;
    private string _fakeShirtName;
    private string _fakeShirtColor;
    private string _fakePantsName;
    private string _fakePantsColor;
    private string _fakeHatName;
    private string _fakeHatColor;
    private bool _fakeIsWearingHat;
    private string _fakeShoesName;
    private string _fakeShoesColor;

    private void Start()
    {
        if (_data == null && autoGenerateOnStart)
        {
            AutoInitialize();
        }
    }

    private void OnDestroy()
    {
        // Free up the name if this character is destroyed
        if (!string.IsNullOrEmpty(characterName))
        {
            UsedNames.Remove(characterName);
        }
    }

    private void AutoInitialize()
    {
        if (CharacterManager.Instance == null)
        {
            Debug.LogError("Cannot auto-generate NPC: CharacterManager is not in the scene!");
            return;
        }

        LpcCharacter dataToLoad = !string.IsNullOrEmpty(specificCharacterId)
            ? CharacterManager.Instance.GetCharacterData(specificCharacterId)
            : CharacterManager.Instance.GetRandomCharacterData();

        if (dataToLoad != null)
        {
            Initialize(dataToLoad);
        }
    }

    public void Initialize(LpcCharacter characterData)
    {
        Data = characterData;
        _sr = GetComponent<SpriteRenderer>();

        // Generate persistent name and fake attributes
        GenerateName();
        GenerateFakeData();

        _allSlices = Resources.LoadAll<Sprite>($"sprites/{Data.id}");
        SetDirection(facing);
    }

    public string GetCharacterName()
    {
        if (string.IsNullOrEmpty(characterName))
        {
            GenerateName();
        }
        return characterName;
    }

    private void GenerateName()
    {
        // If a name was already set in inspector, register it so nobody else takes it
        if (!string.IsNullOrEmpty(characterName))
        {
            UsedNames.Add(characterName);
            return;
        }

        string gender = GetGender().ToLower();
        string[] pool = (gender == "female") ? FemaleNames : MaleNames;

        // Filter for names that haven't been assigned yet
        var availableNames = pool.Where(n => !UsedNames.Contains(n)).ToList();

        if (availableNames.Count > 0)
        {
            characterName = availableNames[UnityEngine.Random.Range(0, availableNames.Count)];
        }
        else
        {
            // Fallback if all names in pool are exhausted: append a counter
            string baseName = pool[UnityEngine.Random.Range(0, pool.Length)];
            int counter = 2;
            while (UsedNames.Contains($"{baseName} {counter}"))
            {
                counter++;
            }
            characterName = $"{baseName} {counter}";
        }

        UsedNames.Add(characterName);
        gameObject.name = characterName;
    }

    public void SetDirection(FacingDirection newDirection)
    {
        facing = newDirection;
        if (_allSlices == null || _allSlices.Length < 8) return;

        int startIndex = (int)facing * 2;
        _currentFrames = new Sprite[] { _allSlices[startIndex], _allSlices[startIndex + 1] };
        
        _frameIndex = 0;
        if (_sr != null)
        {
            _sr.sprite = _currentFrames[0];
        }
    }

    private void Update()
    {
        if (_currentFrames == null || _currentFrames.Length < 2) return;

        _timer += Time.deltaTime;
        if (_timer >= animationSpeed)
        {
            _timer = 0f;
            _frameIndex = (_frameIndex + 1) % _currentFrames.Length;
            _sr.sprite = _currentFrames[_frameIndex];
        }
    }

    public bool IsWearingHat() => Data is { hat: not null };
    public string GetGender() => Data != null ? Data.gender : "Unknown";
    public string GetHairStyle() => Data is { hair: not null } ? CleanName(Data.hair.name) : "Bald";
    public string GetHairColor() => Data is { hair: not null } ? Data.hair.color : "None";
    public string GetShirtName() => Data is { shirt: not null } ? CleanName(Data.shirt.name) : "None";
    public string GetShirtColor() => Data is { shirt: not null } ? Data.shirt.color : "None";
    public string GetPantsName() => Data is { pants: not null } ? CleanName(Data.pants.name) : "None";
    public string GetPantsColor() => Data is { pants: not null } ? Data.pants.color : "None";
    public string GetHatName() => IsWearingHat() ? CleanName(Data.hat.name) : "None";
    public string GetHatColor() => IsWearingHat() ? Data.hat.color : "None";
    public string GetShoesColor() => Data is { shoes: not null } ? Data.shoes.color : "None";

    public string GetShoesName()
    {
        if (Data?.shoes?.name is not { } name)
            return "None";

        return FormatShoeName(name);
    }

    public bool IsTruthTeller() => truthTeller;

    public string GetFakeGender() { EnsureFakeData(); return _fakeGender; }
    public string GetFakeHairStyle() { EnsureFakeData(); return _fakeHairStyle; }
    public string GetFakeHairColor() { EnsureFakeData(); return _fakeHairColor; }
    public string GetFakeShirtName() { EnsureFakeData(); return _fakeShirtName; }
    public string GetFakeShirtColor() { EnsureFakeData(); return _fakeShirtColor; }
    public string GetFakePantsName() { EnsureFakeData(); return _fakePantsName; }
    public string GetFakePantsColor() { EnsureFakeData(); return _fakePantsColor; }
    public string GetFakeHatName() { EnsureFakeData(); return _fakeHatName; }
    public string GetFakeHatColor() { EnsureFakeData(); return _fakeHatColor; }
    public bool GetFakeIsWearingHat() { EnsureFakeData(); return _fakeIsWearingHat; }
    public string GetFakeShoesName() { EnsureFakeData(); return _fakeShoesName; }
    public string GetFakeShoesColor() { EnsureFakeData(); return _fakeShoesColor; }

    private void EnsureFakeData()
    {
        if (!_fakeDataGenerated)
        {
            GenerateFakeData();
        }
    }

    private void GenerateFakeData()
    {
        if (Data == null) return;

        _fakeGender = GetRandomDifferent(Genders, GetGender());
        _fakeHairStyle = GetRandomDifferent(HairOptions, GetHairStyle());
        _fakeHairColor = GetRandomDifferent(Colors, GetHairColor());
        _fakeShirtName = GetRandomDifferent(ShirtOptions, GetShirtName());
        _fakeShirtColor = GetRandomDifferent(Colors, GetShirtColor());
        _fakePantsName = GetRandomDifferent(PantsOptions, GetPantsName());
        _fakePantsColor = GetRandomDifferent(Colors, GetPantsColor());

        string currentHat = GetHatName();
        string[] hatPool = IsWearingHat() ? HatOptions.Append("None").ToArray() : HatOptions;
        _fakeHatName = GetRandomDifferent(hatPool, currentHat);
        _fakeHatColor = GetRandomDifferent(Colors, GetHatColor());
        _fakeIsWearingHat = !IsWearingHat();

        string currentShoe = GetShoesName();
        var formattedShoeOptions = ShoesOptions.Select(FormatShoeName).ToArray();
        _fakeShoesName = GetRandomDifferent(formattedShoeOptions, currentShoe);
        _fakeShoesColor = GetRandomDifferent(Colors, GetShoesColor());

        _fakeDataGenerated = true;
    }

    private static string GetRandomDifferent(string[] pool, string actualValue)
    {
        string cleanActual = CleanName(actualValue);
        var candidates = pool
            .Select(CleanName)
            .Where(item => !string.Equals(item, cleanActual, StringComparison.OrdinalIgnoreCase))
            .Distinct()
            .ToArray();

        if (candidates.Length == 0) return cleanActual;
        return candidates[UnityEngine.Random.Range(0, candidates.Length)];
    }

    private static string CleanName(string raw)
    {
        if (string.IsNullOrEmpty(raw) || raw == "None" || raw == "Bald") 
            return raw ?? "None";

        int slashIndex = raw.LastIndexOf('/');
        if (slashIndex >= 0) raw = raw.Substring(slashIndex + 1);

        raw = raw.Replace('_', ' ');
        raw = System.Text.RegularExpressions.Regex.Replace(raw, @"\d+$", "");
        return raw.Trim();
    }

    private static string FormatShoeName(string rawName)
    {
        if (string.IsNullOrEmpty(rawName) || rawName == "None") return "None";
        rawName = rawName.Replace('_', ' ');
        int slashIndex = rawName.IndexOf('/');
        return slashIndex >= 0 ? $"{rawName[(slashIndex + 1)..]} {rawName[..slashIndex]}" : rawName;
    }
}