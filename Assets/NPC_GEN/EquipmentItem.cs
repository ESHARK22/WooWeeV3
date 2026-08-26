using UnityEngine;

public enum EquipSlot { Body, Head, Pants, Armor, Hair, Helmet, Weapon }

[CreateAssetMenu(fileName = "NewEquipment", menuName = "Modular Character/Equipment Item")]
public class EquipmentItem : ScriptableObject
{
    public string itemName;
    public EquipSlot slot;
    public Sprite[] sprites; 
}