using System.Collections.Generic;
using UnityEngine;

public class ModularCharacter : MonoBehaviour
{
    [Header("Renderers")]
    [SerializeField] private SpriteRenderer bodyRenderer;
    [SerializeField] private SpriteRenderer legsRenderer;
    [SerializeField] private SpriteRenderer armourRenderer;
    [SerializeField] private SpriteRenderer hairRenderer;
    [SerializeField] private SpriteRenderer helmetRenderer;
    [SerializeField] private SpriteRenderer weaponRenderer;

    [Header("Equipped Gear")]
    public EquipmentItem equippedBody;
    public EquipmentItem equippedLegs;
    public EquipmentItem equippedArmour;
    public EquipmentItem equippedHair;
    public EquipmentItem equippedHelmet;
    public EquipmentItem equippedWeapon;

    private Dictionary<EquipSlot, EquipmentItem> currentEquipment = new Dictionary<EquipSlot, EquipmentItem>();

    private void Start()
    {
        if (equippedBody != null) Equip(equippedBody);
        if (equippedLegs != null) Equip(equippedLegs);
        if (equippedArmour != null) Equip(equippedArmour);
        if (equippedHair != null) Equip(equippedHair);
        if (equippedHelmet != null) Equip(equippedHelmet);
        if (equippedWeapon != null) Equip(equippedWeapon);
    }

    public void Equip(EquipmentItem item)
    {
        if (item == null) return;
        currentEquipment[item.slot] = item;
    }

    public void Unequip(EquipSlot slot)
    {
        if (currentEquipment.ContainsKey(slot))
        {
            currentEquipment[slot] = null;
            SpriteRenderer renderer = GetRendererBySlot(slot);
            if (renderer != null) renderer.sprite = null;
        }
    }

    public void SetFrame(int spriteIndex)
    {
        UpdateSlotFrame(EquipSlot.Body, bodyRenderer, spriteIndex);
        UpdateSlotFrame(EquipSlot.Pants, legsRenderer, spriteIndex);
        UpdateSlotFrame(EquipSlot.Armor, armourRenderer, spriteIndex);
        UpdateSlotFrame(EquipSlot.Hair, hairRenderer, spriteIndex);
        UpdateSlotFrame(EquipSlot.Helmet, helmetRenderer, spriteIndex);
        UpdateSlotFrame(EquipSlot.Weapon, weaponRenderer, spriteIndex);
    }

    private void UpdateSlotFrame(EquipSlot slot, SpriteRenderer renderer, int frameIndex)
    {
        if (renderer == null) return;

        if (currentEquipment.TryGetValue(slot, out EquipmentItem item) && item != null)
        {
            if (item.sprites != null && frameIndex >= 0 && frameIndex < item.sprites.Length)
            {
                renderer.sprite = item.sprites[frameIndex];
                renderer.enabled = true;
                return;
            }
        }
        
        if (slot != EquipSlot.Body)
        {
            renderer.sprite = null;
        }
    }

    private SpriteRenderer GetRendererBySlot(EquipSlot slot)
    {
        return slot switch
        {
            EquipSlot.Body => bodyRenderer,
            EquipSlot.Pants => legsRenderer,
            EquipSlot.Armor => armourRenderer,
            EquipSlot.Hair => hairRenderer,
            EquipSlot.Helmet => helmetRenderer,
            EquipSlot.Weapon => weaponRenderer,
            _ => null
        };
    }
}