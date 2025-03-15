using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Define uma classe para gerenciar a interface de login.
public class UISignIn : MonoBehaviour {

    // Campos serializados para serem configurados no Inspector do Unity.
    [SerializeField] Text errorText; // Exibe mensagens de erro.
    [SerializeField] Canvas canvas; // Controla a exibição da tela de login.

    // Variáveis para armazenar o nome de usuário e a senha.
    string username, password ;

    // Método chamado quando o objeto é ativado.
    void OnEnable()
    {
        // Adiciona listeners para os eventos de login do UserAccountManager.
        UserAccountManager.OnSignInFailed.AddListener(OnSignInFailed);
        UserAccountManager.OnSignInSuccess.AddListener(OnSignInSuccess);

    }

    // Método chamado quando o objeto é desativado.
    void OnDisable()
    {
        // Remove os listeners dos eventos ao desabilitar o objeto.
        UserAccountManager.OnSignInFailed.RemoveListener(OnSignInFailed);
        UserAccountManager.OnSignInSuccess.RemoveListener(OnSignInSuccess);

    }

    // Callback chamado em caso de falha no login.
    void OnSignInFailed(string error)
    {
        // Exibe a mensagem de erro na tela.
        errorText.gameObject.SetActive(true);
        errorText.text = error;
    }

    // Callback chamado em caso de sucesso no login.
    void OnSignInSuccess()
    {
        // Oculta a tela de login desativando o Canvas.
        canvas.enabled = false;
    }
    
    // Método para atualizar o nome de usuário a partir de um campo de input.
    public void UpdateUsername(string _username)
    {
     username = _username;           
    }

    // Método para atualizar a senha a partir de um campo de input.
        public void UpdatePassword(string _password)
    {
     password = _password;           
    }

    // Método para iniciar o processo de login chamando o UserAccountManager.
    public void SignIn (){
        UserAccountManager.Instance.SignIn(username, password);
    }

}
