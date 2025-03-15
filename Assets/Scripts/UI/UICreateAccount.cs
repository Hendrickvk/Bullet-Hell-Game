using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Define uma classe para gerenciar a interface de criação de contas.
public class UICreateAccount : MonoBehaviour
{
    // Campos serializados para serem configurados no Inspector do Unity.
    [SerializeField] Text errorText; // Exibe mensagens de erro.
    [SerializeField] Canvas canvas; // Controla a exibição da tela de criação de conta.

    // Variáveis para armazenar o e-mail, senha e nome de usuário.
    string emailAddress, password, username ;

    // Método chamado quando o objeto é ativado.
    void OnEnable()
    {
        // Adiciona listeners para os eventos de falha na criação da conta e sucesso no login.
        UserAccountManager.OnCreateAccountFailed.AddListener(OnCreateAccountFailed);
        UserAccountManager.OnSignInSuccess.AddListener(OnSignInSuccess);
    }

    // Método chamado quando o objeto é desativado.
    void OnDisable()
    {
        // Remove os listeners dos eventos ao desabilitar o objeto.
        UserAccountManager.OnCreateAccountFailed.AddListener(OnCreateAccountFailed);
        UserAccountManager.OnSignInSuccess.RemoveListener(OnSignInSuccess);
    }

    // Callback chamado em caso de falha na criação da conta.
    void OnCreateAccountFailed(string error)
    {
        // Exibe a mensagem de erro na tela.
        errorText.gameObject.SetActive(true);
        errorText.text = error;
    }

    // Callback chamado em caso de sucesso no login (após a criação da conta).
    void OnSignInSuccess()
    {
        // Oculta a tela de criação de conta desativando o Canvas.
        canvas.enabled = false;
    }

    // Métodos para atualizar os campos de e-mail, nome de usuário e senha a partir de inputs.
    public void UpdateEmailAddress(string _emailAddress)
    {
        emailAddress = _emailAddress;           
    }
    
    public void UpdateUsername(string _username)
    {
        username = _username;           
    }

    public void UpdatePassword(string _password)
    {
        password = _password;           
    }

    // Método para iniciar o processo de criação de conta chamando o UserAccountManager.
    public void CreateAccount (){
        UserAccountManager.Instance.CreateAccount(username, emailAddress , password);
    }
}
