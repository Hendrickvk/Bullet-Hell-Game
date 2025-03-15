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
}
