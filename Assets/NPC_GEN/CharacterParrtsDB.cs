using UnityEngine;


[CreateAssetMenu(fileName = "CharacterParrtsDB", menuName = "Modular Character/Parts Database")]
public class CharacterParrtsDB : ScriptableObject
{
    [Header("Bases")]
    public EquipmentItem[] bodies;
    public EquipmentItem[] heads;
    public EquipmentItem[] hairs;
    public EquipmentItem[] beards;

    [Header("Equipment")]
    public EquipmentItem[] helmets;
    public EquipmentItem[] torsos;
    public EquipmentItem[] legs;
    public EquipmentItem[] boots;
    public EquipmentItem[] weapons;
    public EquipmentItem[] shields;
    public EquipmentItem[] capes;
}