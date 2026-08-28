using UnityEngine;

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
    [Header("Role Settings")]
    public bool truthTeller = false;

    [Header("Scene Placement Settings")]
    [Tooltip("If true and placed directly in the scene, this NPC generates on Start.")]
    public bool autoGenerateOnStart = true;
    [Tooltip("Leave blank for a random character, or type an exact ID like 'char_005'.")]
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

    private void Start()
    {
        if (_data == null && autoGenerateOnStart)
        {
            AutoInitialize();
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

        // Load all slices for this character from Resources
        _allSlices = Resources.LoadAll<Sprite>($"sprites/{Data.id}");
        
        SetDirection(facing);
    }

    public void SetDirection(FacingDirection newDirection)
    {
        facing = newDirection;

        if (_allSlices == null || _allSlices.Length < 8) return;

        // 2x4 sheet: Up=0,1 | Left=2,3 | Down=4,5 | Right=6,7
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

    // --- Helper Methods ---
    public bool IsWearingHat() => Data is { hat: not null };
    public string GetShirtName() => Data is { shirt: not null } ? Data.shirt.name : "None";
    public string GetHairStyle() => Data is { hair: not null } ? Data.hair.name : "Bald";
    public string GetPantsName() => Data is { pants: not null } ? Data.pants.name : "None";
    public string GetShirtColor() => Data is { shirt: not null } ? Data.shirt.color : "None";
    public string GetPantsColor() => Data is { pants: not null } ? Data.pants.color : "None";
    
    public string GetHatName() => IsWearingHat() ? Data.hat.name : "None";
    public string GetHairColor() => Data is { hair: not null } ? Data.hair.color : "None";
    public string GetHatColor() => IsWearingHat() ? Data.hat.color : "None";
    
    public string GetShoesName()
    {
        if (Data?.shoes?.name is not { } name)
            return "None";

        int slashIndex = name.IndexOf('/');
    
        return slashIndex >= 0 
            ? $"{name[(slashIndex + 1)..]} {name[..slashIndex]}" 
            : name;
    }    public string GetShoesColor() => Data is { shoes: not null } ? Data.shoes.color : "None";
    public string GetGender() => Data != null ? Data.gender : "Unknown";
    
    public bool IsTruthTeller() => truthTeller;
    
}