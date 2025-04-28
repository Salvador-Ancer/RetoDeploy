using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Diagnostics;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Debug = UnityEngine.Debug;
using TMPro;

public class LoginController : MonoBehaviour
{
    public TMP_InputField userField;
    public TMP_InputField passwordField;
    public Text error;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Submit() {
        string userInput = userField.text;
        string passInput = passwordField.text;

        StartCoroutine(VerificarUsuario(userInput, passInput));
    }


    IEnumerator VerificarUsuario (string usr, string contra) {
        string JSONurl = "https://10.22.158.116:7258/Oxxo/VerificarUsuario/" + usr + "/" + contra;
        UnityWebRequest web = UnityWebRequest.Get(JSONurl);
        web.certificateHandler = new ForceAcceptAll();
        yield return web.SendWebRequest ();

        if (web.result != UnityWebRequest.Result.Success) {
            UnityEngine.Debug.Log("Error API: " + web.error);
            error.text = "Error de conexión. Por favor intente más tarde.";
        }
        else {
            bool sesionCorrecta = false;
            sesionCorrecta = JsonConvert.DeserializeObject<bool>(web.downloadHandler.text);
            if (sesionCorrecta) {
                // Get user info immediately after successful login
                yield return StartCoroutine(GetUserInfo(usr));
            } else {
                error.text = "Usuario o contraseña incorrectos";
                userField.text = "";
                passwordField.text = "";
            }
        }
    }

    IEnumerator GetUserInfo (string userName) {
        string JSONurl = "https://10.22.158.116:7258/Oxxo/GetUsuario/" + userName;
        UnityWebRequest web = UnityWebRequest.Get(JSONurl);
        web.certificateHandler = new ForceAcceptAll();
        yield return web.SendWebRequest ();

        if (web.result != UnityWebRequest.Result.Success) {
            UnityEngine.Debug.Log("Error API: " + web.error);
            error.text = "Error al obtener información del usuario";
        }
        else {
            Usuario usuario = JsonConvert.DeserializeObject<Usuario>(web.downloadHandler.text);
            if (usuario != null && usuario.id_usuario > 0) {
                definirUsuario(usuario);
                PlayerPrefs.SetString("userName", userName);
                SceneManager.LoadScene("Reporte");
            } else {
                error.text = "Error: Usuario no encontrado";
            }
        }
    }

    public void definirUsuario(Usuario usr1) {
        PlayerPrefs.SetInt("usuario_id", usr1.id_usuario);
        PlayerPrefs.SetInt("gameCoins", usr1.monedas);
        PlayerPrefs.SetInt("nivel", usr1.nivel);
        PlayerPrefs.Save();
    }
}
