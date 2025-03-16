using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using PlayFab.ClientModels;
using PlayFab;
using UnityEditor.Compilation;
using UnityEditor.PackageManager;

// Define uma classe para gerenciar contas de usuário.
public class UserAccountManager : MonoBehaviour {

    // Instância única da classe para facilitar o acesso (Singleton).
    public static UserAccountManager Instance;

    // Eventos para notificar sucesso ou falha no login e criação de contas.
    public static UnityEvent OnSignInSuccess = new UnityEvent();
    public static UnityEvent<string> OnSignInFailed = new UnityEvent<string>();
    public static UnityEvent<string> OnCreateAccountFailed = new UnityEvent<string>();

    public static UnityEvent<string, string> OnUserDataRetrieved = new UnityEvent<string, string>();

    string playfabID;

    // Método chamado quando o script é iniciado.
    void Awake()
    {
        // Define a instância única como o objeto atual.
        Instance = this;
    }

    // Método para criar uma nova conta no PlayFab.
    public void CreateAccount( string username, string emailAddress, string password){
        // Chama a API do PlayFab para registrar um novo usuário.
        PlayFabClientAPI.RegisterPlayFabUser(
            new RegisterPlayFabUserRequest(){
                Username = username,
                Email = emailAddress,
                Password = password,
                RequireBothUsernameAndEmail = true
            },
            // Callback em caso de sucesso.
            response => {
                Debug.Log($"Sucessful Account Creation: {username}, {emailAddress}");
                // Faz login automaticamente após criar a conta.
                SignIn(username, password);
            },
            // Callback em caso de erro.
            error => {
                Debug.Log($"Unsucessful Account Creation: {username}, {emailAddress} \n {error.ErrorMessage}");
                // Dispara evento de falha com a mensagem de erro.
                OnCreateAccountFailed.Invoke(error.ErrorMessage);
        }
    );
}

// Método para fazer login com nome de usuário e senha.
public void SignIn(string username, string password){
        // Chama a API do PlayFab para login.
        PlayFabClientAPI.LoginWithPlayFab(new LoginWithPlayFabRequest(){
            Username = username,
            Password = password
        },
        // Callback em caso de sucesso.
        response =>{
            Debug.Log($"Sucessful Account Login: {username}");
            // Dispara evento de sucesso no login.
            playfabID = response.PlayFabId;
            OnSignInSuccess.Invoke();

        },
        // Callback em caso de erro.
        error =>{
            Debug.Log($"Unsucessful Account Login: {username} \n {error.ErrorMessage}");
            // Dispara evento de falha com a mensagem de erro.
            OnSignInFailed.Invoke(error.ErrorMessage);
        }
    );   
}
    // SIGN IN WITH DEVICE ID

    void GetDeviceID(out string android_id, out string ios_id, out string custom_id){
        android_id = string.Empty;
        ios_id = string.Empty;
        custom_id = string.Empty;

        if(Application.platform == RuntimePlatform.Android){
            AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = up.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject contentResolver = currentActivity.Call<AndroidJavaObject>("getContentResolver");
            AndroidJavaClass secure = new AndroidJavaClass("android.provider.Settings$Secure");
            android_id = secure.CallStatic<string>("getString", contentResolver, "android_id");

        }else if(Application.platform == RuntimePlatform.IPhonePlayer){
            ios_id = UnityEngine.iOS.Device.vendorIdentifier;

        }else{
            custom_id = SystemInfo.deviceUniqueIdentifier;
        }
    }

    public void SignInWithDevice(){
        GetDeviceID(out string android_id, out string ios_id, out string custom_id);

        if(!string.IsNullOrEmpty(android_id)){
            Debug.Log($"Logging in with Android Device ID");
            PlayFabClientAPI.LoginWithAndroidDeviceID(new LoginWithAndroidDeviceIDRequest() {
                AndroidDevice = android_id,
                OS = SystemInfo.operatingSystem,
                AndroidDeviceId = SystemInfo.deviceModel,
                TitleId = PlayFabSettings.TitleId,
                CreateAccount = true
            }, response => {
                Debug.Log($"Sucessful login with Android Device ID");
                playfabID = response.PlayFabId;
                OnSignInSuccess.Invoke();
            }, error => {
                Debug.Log($"Unsucessful login with Android Device ID : {error.ErrorMessage}");
                OnSignInFailed.Invoke(error.ErrorMessage);
            });
        }else if(!string.IsNullOrEmpty(ios_id)){
            Debug.Log($"Logging in with IOS Device ID");
            PlayFabClientAPI.LoginWithIOSDeviceID(new LoginWithIOSDeviceIDRequest() {
                DeviceId = ios_id,
                OS = SystemInfo.operatingSystem,
                DeviceModel = SystemInfo.deviceModel,
                TitleId = PlayFabSettings.TitleId,
                CreateAccount = true
            }, response => {
                Debug.Log($"Sucessful login with IOS Device ID");
                playfabID = response.PlayFabId;
                OnSignInSuccess.Invoke();
            }, error => {
                Debug.Log($"Unsucessful login with IOS Device ID : {error.ErrorMessage}");
                OnSignInFailed.Invoke(error.ErrorMessage);
            });
        }else if(!string.IsNullOrEmpty(custom_id)){
            Debug.Log($"Logging in with Custom Device ID");
            PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest() {
                CustomId = custom_id,
                TitleId = PlayFabSettings.TitleId,
                CreateAccount = true
            }, response => {
                Debug.Log($"Sucessful login with Custom Device ID");
                playfabID = response.PlayFabId;
                OnSignInSuccess.Invoke();
            }, error => {
                Debug.Log($"Unsucessful login with Custom Device ID : {error.ErrorMessage}");
                OnSignInFailed.Invoke(error.ErrorMessage);
            });
        }
    }

    //USERDATA

    public void GetUserData(string key) {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(){
            PlayFabId = playfabID,
            Keys = new List<string>(){
                key
            }
        }, response => {
            Debug.Log($"Successful GetUserData");
            if(response.Data.ContainsKey(key)) OnUserDataRetrieved.Invoke(key, response.Data[key].Value);
            else OnUserDataRetrieved.Invoke(key, null);
        }, error => {
            Debug.Log($"Unsuccessful GetUserData: {error.ErrorMessage}");
        });
    }

    public void SetUserData(string key, string value, UnityAction OnSuccess = null) {
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest(){
            Data = new Dictionary<string, string>(){
                {key, value}
            }
        }, responde => {
            Debug.Log($"Successful SetUserData");
            OnSuccess.Invoke();

        }, error => {
            Debug.Log($"Unsuccessful SetUserData: {error.ErrorMessage}");
        });

    }
    
}
