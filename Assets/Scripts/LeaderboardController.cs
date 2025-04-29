using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class LeaderboardController : MonoBehaviour
{


    List<Text> listaUsersText = new List<Text>();

    public Text username1;
    public Text username2;
    public Text username3;
    public Text username4;
    public Text username5;
    public Text username6;
    public Text username7;
    public Text username8;
    public Text username9;
    public Text username10;

    


    List<Image> pfpList = new List<Image>();
    public Image pfp1;
    public Image pfp2;
    public Image pfp3;
    public Image pfp4;
    public Image pfp5;
    public Image pfp6;
    public Image pfp7;
    public Image pfp8;
    public Image pfp9;
    public Image pfp10;

    List<UsuarioLB> rankingUsuarios = new List<UsuarioLB>();




    public void Awake(){

    fillTextList();
    fillPfpList();
    fillUserList();   
    fillUI();        
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void fillTextList(){
        listaUsersText.Add(username1);
        listaUsersText.Add(username2);
        listaUsersText.Add(username3);
        listaUsersText.Add(username4);
        listaUsersText.Add(username5);
        listaUsersText.Add(username6);
        listaUsersText.Add(username7);
        listaUsersText.Add(username8);
        listaUsersText.Add(username9);
        listaUsersText.Add(username10);
    }

    public void fillPfpList(){
        pfpList.Add(pfp1);
        pfpList.Add(pfp2);
        pfpList.Add(pfp3);
        pfpList.Add(pfp4);
        pfpList.Add(pfp5);
        pfpList.Add(pfp6);
        pfpList.Add(pfp7);
        pfpList.Add(pfp8);
        pfpList.Add(pfp9);
        pfpList.Add(pfp10);
    }

    public void fillUserList(){
        StartCoroutine(GetRanking());
    }

    IEnumerator GetRanking(){
    string JSONurl = "https://10.22.158.116:7258/LeaderBoard/GetRanking"; 

    UnityWebRequest web = UnityWebRequest.Get(JSONurl);
    web.certificateHandler = new ForceAcceptAll(); // Solo si tienes HTTPS autofirmado
    yield return web.SendWebRequest();

    if (web.result != UnityWebRequest.Result.Success)
    {
        Debug.LogError("Error API: " + web.error);
    }
    else
    {
        rankingUsuarios = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UsuarioLB>>(web.downloadHandler.text);
        fillUI();
    }
}

    public void fillUI()
{
    int count = Mathf.Min(rankingUsuarios.Count, 10); 
    int currentUserId = PlayerPrefs.GetInt("usuario_id", -1);

    for (int i = 0; i < count; i++)
    {
        SetUsername(rankingUsuarios[i].nombre_usuario, listaUsersText[i]);
        StartCoroutine(LoadImage(rankingUsuarios[i].imagen, pfpList[i]));
        
        // Highlight the current user if they are in the top 10
        if (rankingUsuarios[i].id_usuario == currentUserId)
        {
            HighlightUserEntry(listaUsersText[i], pfpList[i]);
        }
    }
}

    private void HighlightUserEntry(Text usernameText, Image pfpImage)
    {
        // Change text color to highlight
        Color highlightColor = new Color(1f, 0.19f, 0.19f, 1f); // #FF3131
        usernameText.color = highlightColor;
        
        // Make text bigger
        usernameText.fontSize = (int)(usernameText.fontSize * 1.2f); // Increase size by 20%
    }

    IEnumerator LoadImage(string imageUrl, Image targetImage)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error loading image: " + request.error);
        }
        else
        {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector3(0.5f, 0.5f));
            targetImage.sprite = sprite;
        }
    }

    public void SetUsername(string userName,Text userText){
        userText.text= userName;
    }

}
