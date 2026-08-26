using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[RequireComponent(typeof(ModularCharacter))]
public class RuntimeCharacterBuilder : MonoBehaviour
{
    [SerializeField] private CharacterParrtsDB database; 
    [SerializeField] private bool genOnStart; 
    private ModularCharacter modularCharacter;

    private void Start()
    {
        if (genOnStart)
        {
            RandomizeCharacter();
        }
    }

    private void Awake()
    {
        modularCharacter = GetComponent<ModularCharacter>();
    }

    [ContextMenu("Randomize Character")]
    public void RandomizeCharacter()
    {
        if (database == null)
        {
            Debug.LogWarning("Please assign CharacterParrtsDB to the RuntimeCharacterBuilder!");
            return;
        }

        if (modularCharacter == null)
            modularCharacter = GetComponent<ModularCharacter>();

        // Random Body
        EquipmentItem randomBody = GetRandom(database.bodies);
        modularCharacter.Equip(randomBody);

        bool isSkeleton = randomBody != null && randomBody.itemName.ToLower().Contains("skeleton");

        // Head
        modularCharacter.Equip(GetRandom(database.heads, allowEmpty: false));
        
        // Hair or Helmet
        if (!isSkeleton)
        {
            if (Random.value > 0.5f)
            {
                modularCharacter.Equip(GetRandom(database.helmets, allowEmpty: false));
                modularCharacter.Unequip(EquipSlot.Hair);
            }
            else
            {
                modularCharacter.Equip(GetRandom(database.hairs, allowEmpty: false));
                modularCharacter.Unequip(EquipSlot.Helmet);
            }
        }
        else
        {
            modularCharacter.Unequip(EquipSlot.Hair);
            modularCharacter.Equip(GetRandom(database.helmets, allowEmpty: true));
        }

        // Armor, Legs, Weapon
        modularCharacter.Equip(GetRandom(database.torsos, allowEmpty: false));
        modularCharacter.Equip(GetRandom(database.legs, allowEmpty: false));
        modularCharacter.Equip(GetRandom(database.weapons, allowEmpty: false));

        // Display first idle frame
        modularCharacter.SetFrame(18);
    }

    private EquipmentItem GetRandom(EquipmentItem[] array, bool allowEmpty = false)
    {
        if (array == null || array.Length == 0) return null;
        if (allowEmpty && Random.value < 0.3f) return null;
        return array[Random.Range(0, array.Length)];
    }
}