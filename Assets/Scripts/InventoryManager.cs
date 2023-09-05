using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public Transform itemSlotParent;  
    public GameObject itemSlotPrefab;  
    public DonationPopup donationPopup;
    public List<Item> items;
    public float fadeInDuration = 0.5f;
    public float animationDuration = 1.5f;
    public int numberOfSlots = 30;  
    
    [HideInInspector] public List<ItemSlot> itemSlots;  
    private DonationProgressBar donationProgressBar;  
    private bool isAnimationPlaying = false;


    void Start()
    {
        // Find the DonationProgressBar instance
        donationProgressBar = FindObjectOfType<DonationProgressBar>();

        // Initialize item slots
        itemSlots = new List<ItemSlot>();
        for (int i = 0; i < numberOfSlots; i++)
        {
            GameObject newSlot = Instantiate(itemSlotPrefab, itemSlotParent);
            newSlot.name = "ItemSlot" + (i + 1);  // Name it sequentially

            ItemSlot itemSlot = newSlot.GetComponent<ItemSlot>();
            if (itemSlot)
            {
                itemSlots.Add(itemSlot);
            }
        }

        // Populate item slots with items
        for (int i = 0; i < items.Count && i < itemSlots.Count; i++)
        {
            itemSlots[i].InitializeItemSlot(items[i]);

            // Clear the slot if the item's quantity is zero
            if (items[i].quantity <= 0)
            {
                itemSlots[i].ClearItem();
                items.RemoveAt(i);
                i--;  // Decrement i to keep the index valid after removing an item
            }
        }

    }

    public void HandleItemSelection(ItemSlot selectedSlot)
    {

        Item item = selectedSlot.item;
        // Validate the selected item
        if (item == null || item.quantity <= 0 || isAnimationPlaying)
        {
            return;
        }

        // Update the donation progress bar
        if (donationProgressBar != null)
        {
            donationProgressBar.UpdateProgressBar(1);
        }

        // Update the item's quantity
        item.quantity--;
        if (item.quantity <= 0)
        {
            selectedSlot.item = null;
        }

        // Refresh the UI and display the donation popup
        RefreshInventoryUI();
        donationPopup.ShowPopup(item.icon, item.itemCategory.ToString(), item.itemName);
        StartCoroutine(WaitForAnimationToEnd());
    }

    private IEnumerator WaitForAnimationToEnd()
    {
        isAnimationPlaying = true;
        yield return new WaitForSeconds(animationDuration);
        isAnimationPlaying = false;
    }

    public void AddItemToInventory(Item item)
    {
        // Add the new item to the inventory list.
        items.Add(item);
        ItemSlot emptySlot = null;

        // Find the first empty slot and populate it
        foreach (ItemSlot slot in itemSlots)
        {
            if (slot.item == null)
            {
                slot.InitializeItemSlot(item);
                emptySlot = slot;
                break;
            }
        }

        if (emptySlot != null)
        {
            StartCoroutine(FadeInNewItem(emptySlot));
        }

        // Refresh the UI.
        RefreshInventoryUI();
    }

    private IEnumerator FadeInNewItem(ItemSlot itemSlot)
    {
        CanvasGroup itemCanvasGroup = itemSlot.GetComponentInChildren<CanvasGroup>();  // Assuming the CanvasGroup is directly under the ItemSlot in the hierarchy

        if (itemCanvasGroup != null)
        {
            itemCanvasGroup.alpha = 0;  // Initially set it to transparent

            for (float t = 0; t <= 1; t += Time.deltaTime / fadeInDuration)
            {
                itemCanvasGroup.alpha = Mathf.Lerp(0, 1, t);
                yield return null;
            }

            itemCanvasGroup.alpha = 1;  // Fully opaque
        }
    }

    public void RefreshInventoryUI()
    {
        // Update each item slot's UI
        foreach (ItemSlot slot in itemSlots)
        {
            slot.UpdateUI();
        }
    }
}
