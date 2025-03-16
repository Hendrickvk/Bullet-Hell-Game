using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIProfile : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] InputField playerNameText;
    [SerializeField] Text playerLevelText;
    [SerializeField] Text playerXPText;
    [SerializeField] Image playerXPFill;

    void OnEnable()
    {
        UserAccountManager.OnSignInSuccess.AddListener(SignIn);
        UserProfile.OnProfileDataUpdated.AddListener(ProfileDataUpdated);
    }


    void OnDisable()
    {
        UserProfile.OnProfileDataUpdated.RemoveListener(ProfileDataUpdated);
    }
    void SignIn()
    {
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
    }

    void ProfileDataUpdated(ProfileData profileData)
    {
        float level = (Mathf.Floor(profileData.level));
        float xp = profileData.level - level;
        
        playerNameText.text = profileData.playerName;
        playerLevelText.text = level.ToString();
        playerXPText.text = ((int)(xp * UserProfile.Instance.xpThreshold)).ToString();
        playerXPFill.fillAmount = xp;
    }
}
