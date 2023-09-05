using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DonationPopup : MonoBehaviour
{
    public CanvasGroup popupCanvasGroup; // Reference to the CanvasGroup component
    public Image itemIcon;
    public TextMeshProUGUI categoryNameText;
    public TextMeshProUGUI itemNameText;

    public float fadeInDuration = 1.0f;
    public float displayDuration = 2.0f;
    public float fadeOutDuration = 1.0f;

    private bool isAnimating;
    private Coroutine activeCoroutine;

    // Show the popup with specified details
    public void ShowPopup(Sprite icon, string categoryName, string itemName)
    {
        // Stop ongoing animation if any
        if (isAnimating && activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
        }

        // Set the item icon and texts
        itemIcon.sprite = icon;
        categoryNameText.text = categoryName;
        itemNameText.text = itemName;

        // Initialize the CanvasGroup properties
        popupCanvasGroup.alpha = 0; // set this to zero initially
        popupCanvasGroup.interactable = true;
        popupCanvasGroup.blocksRaycasts = true;

        // Start the animation coroutine
        activeCoroutine = StartCoroutine(AnimatePopup());
    }

    // Coroutine for handling the popup animation
    private IEnumerator AnimatePopup()
    {
        isAnimating = true;

        // Fade in the popup
        float elapsedTime = 0.0f;
        while (elapsedTime < fadeInDuration)
        {
            popupCanvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / fadeInDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        popupCanvasGroup.alpha = 1;

        // Display the popup for a set duration
        yield return new WaitForSeconds(displayDuration);

        // Fade out the popup
        elapsedTime = 0.0f;
        while (elapsedTime < fadeOutDuration)
        {
            popupCanvasGroup.alpha = Mathf.Lerp(1, 0, elapsedTime / fadeOutDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        popupCanvasGroup.alpha = 0;

        // Disable the CanvasGroup's interactivity and raycasting properties
        popupCanvasGroup.interactable = false;
        popupCanvasGroup.blocksRaycasts = false;

        // Reset the animation state
        isAnimating = false;
        activeCoroutine = null;
    }
}