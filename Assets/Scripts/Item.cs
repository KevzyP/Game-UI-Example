using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public enum Category
    {
        Tools,
        Fruits,
        Vegetables,
        Ingredient,
        Food,
        Flowers,
        Insects,
        Fishes,
        Gemstones,
        Fossils
    }

    public Category itemCategory;
    public string itemName;
    public Sprite icon;
    public int quantity;
}
