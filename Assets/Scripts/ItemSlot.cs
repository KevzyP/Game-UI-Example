using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, ISelectHandler, IDeselectHandler, ISubmitHandler
{
    public Image itemIcon;
    public Image qtyBG;
    public TextMeshProUGUI qtyText;
    public Item item;  
    
    [SerializeField]
    private Image itemSlotBG;

    private InventoryManager inventoryManager;  
    private Vector3 originalScale;

    // Initialization
    private void Start()
    {
        // Cache original scale
        originalScale = transform.localScale;
        // Find and store a reference to the InventoryManager
        inventoryManager = FindObjectOfType<InventoryManager>();
        // Update UI
        UpdateUI();
    }

    // Initialize the item slot with an item
    public void InitializeItemSlot(Item newItem)
    {
        item = newItem;
        UpdateUI();
    }

    // Update the UI of this item slot based on its state
    public void UpdateUI()
    {
        // Cache the current background color
        Color currentBGColor = itemSlotBG.color;

        // Item-specific UI
        if (item != null)
        {
            // Populate the UI with item details
            itemIcon.sprite = item.icon;
            qtyText.text = item.quantity.ToString();
            itemIcon.enabled = true;
            qtyBG.enabled = item.quantity > 1;
            qtyText.enabled = item.quantity > 1;

            // Check if the item is selectable and set UI accordingly
            if (IsCategorySelectable())
            {
                itemIcon.color = Color.white;
                itemSlotBG.color = new Color(currentBGColor.r, currentBGColor.g, currentBGColor.b, 150f / 255f);

                // Reset the quantity panel to be fully visible
                Color qtyBGColor = qtyBG.color;
                qtyBG.color = new Color(qtyBGColor.r, qtyBGColor.g, qtyBGColor.b, 1f);
                Color qtyTextColor = qtyText.color;
                qtyText.color = new Color(qtyTextColor.r, qtyTextColor.g, qtyTextColor.b, 1f);
            }
            else
            {
                // If the item is not selectable, fade out the icon and text
                itemIcon.color = new Color(0.3f, 0.3f, 0.3f, 1);

                // Fade out the quantity panel
                Color qtyBGColor = qtyBG.color;
                qtyBG.color = new Color(qtyBGColor.r, qtyBGColor.g, qtyBGColor.b, 0.5f);
                Color qtyTextColor = qtyText.color;
                qtyText.color = new Color(qtyTextColor.r, qtyTextColor.g, qtyTextColor.b, 0.5f);

                // Set the background alpha to 100
                itemSlotBG.color = new Color(currentBGColor.r, currentBGColor.g, currentBGColor.b, 100f / 255f);
            }
        }
        else
        {
            // If no item, disable UI elements
            itemIcon.enabled = false;
            qtyBG.enabled = false;
            qtyText.enabled = false;

            // Set background alpha to 50
            itemSlotBG.color = new Color(currentBGColor.r, currentBGColor.g, currentBGColor.b, 50f / 255f);
        }
    }

    // Clear the item from the slot
    public void ClearItem()
    {
        item = null;
        UpdateUI();
    }

    // Check if the item's category is selectable
    public bool IsCategorySelectable()
    {
        return item != null &&
               (item.itemCategory == Item.Category.Insects ||
                item.itemCategory == Item.Category.Fishes ||
                item.itemCategory == Item.Category.Gemstones ||
                item.itemCategory == Item.Category.Fossils);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (IsCategorySelectable())
        {
            StopAllCoroutines();
            StartCoroutine(SmoothScale(originalScale * 1.2f, 0.2f));
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(SmoothScale(originalScale, 0.2f));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (item == null || !IsCategorySelectable())
        {
            return;
        }

        // Handle item selection and UI updating in InventoryManager
        inventoryManager.HandleItemSelection(this);

        // Refresh the UI
        UpdateUI();
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (IsCategorySelectable())
        {
            StopAllCoroutines();
            StartCoroutine(SmoothScale(originalScale * 1.2f, 0.2f));
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(SmoothScale(originalScale, 0.2f));
    }

    public void OnSubmit(BaseEventData eventData)
    {
        Debug.Log("Submit detected");
        if (item == null || !IsCategorySelectable())
        {
            return;
        }

        // Handle item selection and UI updating in InventoryManager
        inventoryManager.HandleItemSelection(this);

        // Refresh the UI
        UpdateUI();
    }

    // Coroutine for scaling smoothly
    private IEnumerator SmoothScale(Vector3 targetScale, float duration)
    {
        Vector3 originalScale = transform.localScale;
        float time = 0;

        while (time <= duration)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
    }
}
