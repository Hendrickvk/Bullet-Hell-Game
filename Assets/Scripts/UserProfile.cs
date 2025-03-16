using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class UserProfile : MonoBehaviour
{
    public static UserProfile Instance;

    public static UnityEvent<ProfileData> OnProfileDataUpdated = new UnityEvent<ProfileData>();
    [SerializeField] ProfileData profileData;
    public float xpThreshold = 1000;

    void Awake()
    {
        Instance = this;
    }

    void OnEnable()
    {
        UserAccountManager.OnSignInSuccess.AddListener(SignIn);
        UserAccountManager.OnUserDataRetrieved.AddListener(UserDataRetrieved);
    }


    void OnDisable()
    {
        UserAccountManager.OnSignInSuccess.RemoveListener(SignIn);
        UserAccountManager.OnUserDataRetrieved.RemoveListener(UserDataRetrieved);
    }

    void SignIn()
    {
        GetUserData();
    }

    [ContextMenu("Get Profile Data")]
    void GetUserData(){
        UserAccountManager.Instance.GetUserData("ProfileData");
    }

    void UserDataRetrieved(string key, string value)
    {
        if(key == "ProfileData"){
            profileData = JsonUtility.FromJson<ProfileData>(value);
            OnProfileDataUpdated.Invoke(profileData);
        }
    }


    [ContextMenu("Set Profile Data")]
    void SetUserData(){
        UserAccountManager.Instance.SetUserData("ProfileData", JsonUtility.ToJson(profileData));
    }
}



[System.Serializable]
public class ProfileData {
    public string playerName;
    public float level;

}

//Ver o por que do botao nao funcionar
//SetUserData nao consegue ser mais acessado pelo UserProfile