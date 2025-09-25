using UnityEngine;

public class HealItem : ItemBase {
    public int healAmount;

    public HealItem(string name, int id, int healAmount, Sprite icon = null)
        : base(name, id, eItemType.Heal, icon) {
        this.healAmount = healAmount;
    }

    public override void Use(GameObject user) {
        PlayerBase player = user.GetComponent<PlayerBase>();
        if (player != null) {
            player.Heal(healAmount);
            Debug.Log($"{itemName} ‚ğg‚Á‚Ä HP ‚ğ {healAmount} ‰ñ•œI");
        }
    }
}