using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardsManager : MonoBehaviour
{
    public List<Reward> rewards;
    public RewardsPanel rewardsPanel;
    public float displayRewardDelay = 1.0f; 

    [HideInInspector] public bool isRewardAnimating = false;
    private int nextRewardIndex = 0;

    // Coroutine for handling reward unlocking animation and additional actions
    private IEnumerator HandleRewardAnimationAndDisplay(Reward reward)
    {
        isRewardAnimating = true;
        
        yield return StartCoroutine(reward.AnimateRewardUnlock());
        yield return new WaitForSeconds(displayRewardDelay);

        rewardsPanel.AddRewardToStoredList(reward.rewardItem);

        rewardsPanel.ProcessNextRewardInStore();

        isRewardAnimating = false;
    }

    // Check all rewards to see if any should be unlocked based on current donations
    public void CheckAndUpdateAllRewards(int currentDonations)
    {
        // Only proceed if we have rewards left and the donation level satisfies the next reward
        if (nextRewardIndex < rewards.Count && currentDonations >= rewards[nextRewardIndex].donationGoal)
        {
            // Handle reward unlocking animation and further actions
            StartCoroutine(HandleRewardAnimationAndDisplay(rewards[nextRewardIndex]));
            nextRewardIndex++;
        }
    }
}
