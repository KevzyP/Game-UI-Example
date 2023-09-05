using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.InputSystem;

public class RewardsPanel : MonoBehaviour
{
    public Image rewardIcon;
    public CanvasGroup textBoxesCanvasGroup;
    public InputActionAsset actionAsset;
    public List<Item> StoredRewards = new();

    [Space(10)]
    public float scaleUpFactor = 1.25f;
    public float scaleDownFactor = 1f;
    public float jiggleAmount = 10f;  
    public float scaleDuration = 0.3f;
    public float jiggleDuration = 0.1f;
    public float fadeInDuration = 0.5f;

    private Coroutine currentAnimation;

    // Subscribe to input action events when enabled
    private void OnEnable()
    {
        InputAction acceptAction = actionAsset.FindAction("AcceptReward");
        if (acceptAction != null)
        {
            acceptAction.performed += OnAcceptReward;
            acceptAction.Enable();
        }
    }

    // Unsubscribe from input action events when disabled
    void OnDisable()
    {
        InputAction acceptAction = actionAsset.FindAction("AcceptReward");
        if (acceptAction != null)
        {
            acceptAction.performed -= OnAcceptReward;
            acceptAction.Disable();
        }
    }

    // Handler for Accept Reward input action
    private void OnAcceptReward(InputAction.CallbackContext obj)
    {
        if (StoredRewards.Count > 0)
        {
            AcceptReward(StoredRewards[StoredRewards.Count - 1]);
        }
    }

    // Add new reward to stored rewards
    public void AddRewardToStoredList(Item reward)
    {
        StoredRewards.Add(reward);
    }

    // Accept the reward and add it to the inventory
    private void AcceptReward(Item reward)
    {
        InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();

        if (StoredRewards.Count > 0 && StoredRewards[StoredRewards.Count - 1] == reward)
        {
            inventoryManager.AddItemToInventory(reward);
            StoredRewards.RemoveAt(StoredRewards.Count - 1);
        }

        StartCoroutine(FadeOutAndProcessNext(reward));
    }

    // Process next reward if any
    public void ProcessNextRewardInStore()
    {
        if (StoredRewards.Count > 0)
        {
            if (currentAnimation != null)
            {
                StopCoroutine(currentAnimation);
            }

            UpdateRewardIcon(StoredRewards[StoredRewards.Count - 1].icon);
            MakeRewardIconInteractable(StoredRewards[StoredRewards.Count - 1]);
        }
        else
        {
            RemoveRewardIcon();
        }
    }

    // Coroutine to fade out reward icon and process the next reward
    private IEnumerator FadeOutAndProcessNext(Item reward)
    {
        yield return FadeOutRewardIcon();
        ProcessNextRewardInStore();
    }

    // Coroutine to fade out the reward icon and text boxes
    private IEnumerator FadeOutRewardIcon()
    {
        CanvasGroup itemIconCanvasGroup = rewardIcon.GetComponent<CanvasGroup>();
        float initialAlpha = itemIconCanvasGroup.alpha;

        // Fade-out the icon
        for (float t = 0; t <= 1; t += Time.deltaTime / fadeInDuration)
        {
            itemIconCanvasGroup.alpha = Mathf.Lerp(initialAlpha, 0, t);
            yield return null;
        }

        // Ensure the icon is fully transparent at the end
        itemIconCanvasGroup.alpha = 0;

        yield return new WaitForSeconds(0.1f);

        // Fade-out the text boxes
        initialAlpha = textBoxesCanvasGroup.alpha;
        for (float t = 0; t <= 1; t += Time.deltaTime / fadeInDuration)
        {
            textBoxesCanvasGroup.alpha = Mathf.Lerp(initialAlpha, 0, t);
            yield return null;
        }

        // Ensure the text boxes are fully transparent at the end
        textBoxesCanvasGroup.alpha = 0;
    }

    // Function to update the reward icon in the RewardsPanel
    public void UpdateRewardIcon(Sprite newRewardIcon)
    {
        if (newRewardIcon != null)
        {
            rewardIcon.sprite = newRewardIcon;
            rewardIcon.enabled = true; 
            currentAnimation = StartCoroutine(AnimateRewardAppearance());
        }
        else
        {
            // No new reward, so remove the current one
            RemoveRewardIcon();
        }
    }

    // Function to remove the reward icon
    public void RemoveRewardIcon()
    {
        rewardIcon.sprite = null;
        rewardIcon.enabled = false;
        textBoxesCanvasGroup.alpha = 0;
    }

    // Function to make the reward icon interactable, allowing the player to accept it
    public void MakeRewardIconInteractable(Item reward)
    {
        // Get the Button component and make it interactable
        Button rewardButton = rewardIcon.GetComponent<Button>();
        rewardButton.interactable = true;

        // Attach the function to execute when button is clicked
        rewardButton.onClick.AddListener(() => AcceptReward(reward));
    }



    // Coroutine to handle animations related to the reward appearance
    private IEnumerator AnimateRewardAppearance()
    {
        CanvasGroup itemIconCanvasGroup = rewardIcon.gameObject.GetComponent<CanvasGroup>();

        // Make sure everything is active and at initial states
        rewardIcon.gameObject.SetActive(true);
        rewardIcon.transform.localScale = Vector3.one;

        // Initialize the icon's CanvasGroup alpha to 0 if not already visible
        if (Mathf.Approximately(itemIconCanvasGroup.alpha, 0))
        {
            itemIconCanvasGroup.alpha = 0f;
        }

        // Fade-in the icon
        for (float t = 0; t <= 1; t += Time.deltaTime / fadeInDuration)
        {
            itemIconCanvasGroup.alpha = Mathf.Lerp(0, 1, t);
            yield return null;
        }

        // Scale up the icon
        Vector3 originalScale = rewardIcon.transform.localScale;
        Vector3 targetScale = originalScale * scaleUpFactor;
        for (float t = 0; t <= 1; t += Time.deltaTime / scaleDuration)
        {
            rewardIcon.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }

        // Jiggle (rotate back and forth)
        float originalRotation = rewardIcon.transform.rotation.eulerAngles.z;
        float leftRotation = originalRotation - jiggleAmount;
        float rightRotation = originalRotation + jiggleAmount;

        // Rotate left
        for (float t = 0; t <= 1; t += Time.deltaTime / jiggleDuration)
        {
            float zRotation = Mathf.Lerp(originalRotation, leftRotation, t);
            rewardIcon.transform.rotation = Quaternion.Euler(0, 0, zRotation);
            yield return null;
        }

        // Rotate right
        for (float t = 0; t <= 1; t += Time.deltaTime / jiggleDuration)
        {
            float zRotation = Mathf.Lerp(leftRotation, rightRotation, t);
            rewardIcon.transform.rotation = Quaternion.Euler(0, 0, zRotation);
            yield return null;
        }

        // Rotate back to original
        for (float t = 0; t <= 1; t += Time.deltaTime / jiggleDuration)
        {
            float zRotation = Mathf.Lerp(rightRotation, originalRotation, t);
            rewardIcon.transform.rotation = Quaternion.Euler(0, 0, zRotation);
            yield return null;
        }

        // Fade in the textboxes while scaling back down, only if they're not already visible
        if (textBoxesCanvasGroup.alpha == 0)
        {
            for (float t = 0; t <= 1; t += Time.deltaTime / scaleDuration)
            {
                // Scale down
                rewardIcon.transform.localScale = Vector3.Lerp(targetScale, originalScale, t);

                // Fade in textboxes
                textBoxesCanvasGroup.alpha = Mathf.Lerp(0, 1, t / fadeInDuration);
                yield return null;
            }
        }
        else
        {
            // If the textboxes are already visible, just do the scale-down animation
            for (float t = 0; t <= 1; t += Time.deltaTime / scaleDuration)
            {
                // Scale down
                rewardIcon.transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
                yield return null;
            }
        }

        rewardIcon.transform.localScale = originalScale;
        textBoxesCanvasGroup.alpha = 1;

        currentAnimation = null;
    }
}
