using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DonationProgressBar : MonoBehaviour
{
    public RewardsManager rewardsManager;
    public Slider donationSlider; 
    public TextMeshProUGUI DonationAmountText;  
    public GameObject RewardsIconArea; 
    public ScrollRect scrollRect;  

    [Header("Settings")]
    public float fillSpeedMultiplier = 3f;
    public float scrollStartThreshold = 0.5f;
    public float smoothFillSpeed = 0.5f;
    public float smoothScrollSpeed = 0.5f; 
    [Space(10)]
    public int currentDonations = 0;


    private bool isCoroutineRunning = false;

    // Method to update the donation progress bar
    public void UpdateProgressBar(int donationToAdd)
    {
        // If rewards are animating or if a coroutine is running, do not proceed
        if (rewardsManager.isRewardAnimating || isCoroutineRunning)
        {
            return;
        }

        // Basic error checking for positive donations
        if (donationToAdd > 0)
        {
            // If the coroutine is not running
            if (!isCoroutineRunning || rewardsManager.isRewardAnimating)
            {
                isCoroutineRunning = true; // Set flag to indicate coroutine is running

                // If adding the donation would exceed the maximum, only add up to the maximum
                if (currentDonations + donationToAdd > 20)
                {
                    donationToAdd = 20 - currentDonations;
                }

                currentDonations += donationToAdd; // Update the total donations
                StartCoroutine(SmoothFillAndScroll()); // Start the coroutine for smooth filling and scrolling

                // Update the donation amount text
                DonationAmountText.text = currentDonations.ToString();
            }
        }
    }

    // Coroutine for smooth filling of the slider and scrolling of the rewards
    IEnumerator SmoothFillAndScroll()
    {
        isCoroutineRunning = true; // Set flag to indicate coroutine is running

        // Calculate target value for the slider
        float targetValue = Mathf.Min(currentDonations * fillSpeedMultiplier, donationSlider.maxValue);
        float normalizedProgress = 0;
        float newNormalizedPosition = 0;

        // While the slider has not reached the target value
        while (Mathf.Abs(donationSlider.value - targetValue) > 0.01f)
        {
            // Smooth filling of the donation slider
            donationSlider.value = Mathf.Lerp(donationSlider.value, targetValue, smoothFillSpeed * Time.deltaTime);

            // Update the text for current donations
            DonationAmountText.text = currentDonations.ToString();

            // Calculate normalized progress and new scroll position
            normalizedProgress = donationSlider.value / donationSlider.maxValue;
            newNormalizedPosition = (normalizedProgress >= scrollStartThreshold) ? normalizedProgress - scrollStartThreshold : 0;

            // Smooth scrolling of the content
            if (normalizedProgress >= scrollStartThreshold)
            {
                scrollRect.horizontalNormalizedPosition = Mathf.Lerp(scrollRect.horizontalNormalizedPosition, newNormalizedPosition, smoothScrollSpeed * Time.deltaTime);
            }

            yield return null;
        }

        // Set the slider and scroll position to their final target values
        donationSlider.value = targetValue;
        scrollRect.horizontalNormalizedPosition = newNormalizedPosition;

        // Check and update all rewards based on the current donation amount
        rewardsManager.CheckAndUpdateAllRewards(currentDonations);

        // Reset the flag for the coroutine
        isCoroutineRunning = false;
        yield return null;
    }

    // For testing purposes: simulates a donation being added when a button is clicked
    public void TestButton()
    {
        UpdateProgressBar(1);
    }
}
