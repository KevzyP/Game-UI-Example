using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Reward : MonoBehaviour
{
    public Image background;  
    public Transform rewardTransform;  
    public Image rewardIcon; 

    [Space(10)]
    public int donationGoal; 
    public Item rewardItem; 
    public bool isAchieved; 

    [Space(10)]
    public float scaleUpFactor = 1.2f;  
    public float scaleUpDuration = 0.3f;  
    public float scaleDownFactor = 1f;  
    public float scaleDownDuration = 0.2f;


    // Update the state of the reward based on the number of current donations.
    public void UpdateRewardState(int currentDonations)
    {
        if (currentDonations >= donationGoal && !isAchieved)
        {
            StartCoroutine(AnimateRewardUnlock());
            isAchieved = true;
        }
    }

    // Coroutine to animate the reward when it gets unlocked.
    public IEnumerator AnimateRewardUnlock()
    {
        // Step 1: Scale Up
        Vector3 originalScale = rewardTransform.localScale;
        Vector3 targetScale = originalScale * scaleUpFactor;
        float timeToScale = scaleUpDuration;

        // Scaling up animation.
        for (float t = 0; t <= 1; t += Time.deltaTime / timeToScale)
        {
            rewardTransform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }

        // Step 2 & 3: Reveal Actual Sprite and Make BG Opaque
        rewardIcon.sprite = rewardItem.icon; // actualRewardSprite is assumed to be the sprite you want to reveal
        Color opaqueColor = new Color(1f, 1f, 1f, 1f); // Full opacity
        background.color = opaqueColor;

        // Step 4: Scale Back Down
        timeToScale = scaleDownDuration;  // Use the user-configurable scaleDownDuration
        
        // Scaling down animation.
        for (float t = 0; t <= 1; t += Time.deltaTime / timeToScale)
        {
            rewardTransform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }
    }
}
