using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using System.Text;

public class GameManager : MonoBehaviour
{
    public Dropdown tiendaDropdown;
    public InputField descripcionInput;

    private string apiUrl = "https://10.22.158.116:7258/api/NotasIrregularidades"; // Cambia IP si es necesario


    public void Start()
    {
        tiendaDropdown.onValueChanged.AddListener(delegate { OnDropdownChange(); });
    }

    void OnDropdownChange()
    {
        string tiendaSeleccionada = tiendaDropdown.options[tiendaDropdown.value].text;
        StartCoroutine(GetNotaDeTienda(tiendaSeleccionada));
    }

    IEnumerator GetNotaDeTienda(string tienda)
    {
        string getUrl = apiUrl + "?tienda=" + UnityWebRequest.EscapeURL(tienda);

        UnityWebRequest web = UnityWebRequest.Get(getUrl);
        web.certificateHandler = new ForceAcceptAll();
        yield return web.SendWebRequest();

        if (web.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error al obtener la nota: " + web.error);
        }
        else
        {
            Debug.Log("Nota obtenida: " + web.downloadHandler.text);

            List<NotaIrregularidad> notas = JsonConvert.DeserializeObject<List<NotaIrregularidad>>(web.downloadHandler.text);

            if (notas != null && notas.Count > 0)
            {
                // Si hay varias, podrías decidir cuál mostrar:
                // Por ahora vamos a mostrar la más reciente (última de la lista)
                NotaIrregularidad ultimaNota = notas[notas.Count - 1];
                descripcionInput.text = ultimaNota.descripcion;
            }
            else
            {
                descripcionInput.text = "No hay notas registradas para esta tienda.";
            }
        }
    }

  [System.Serializable]
public class NotaIrregularidad
{
    public int idNotas;
    public string tienda;
    public string descripcion;
    public string fecha;
}

    public void Enviar()
    {
        StartCoroutine(EnviarIrregularidad());
    }

    IEnumerator EnviarIrregularidad()
    {
        string tiendaSeleccionada = tiendaDropdown.options[tiendaDropdown.value].text;
        string descripcion = descripcionInput.text;

        // Crear un objeto
        var irregularidad = new
        {
            tienda = tiendaSeleccionada,
            descripcion = descripcion
        };

        string jsonData = JsonConvert.SerializeObject(irregularidad);
        byte[] jsonToSend = Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest web = new UnityWebRequest(apiUrl, "POST");
        web.uploadHandler = new UploadHandlerRaw(jsonToSend);
        web.downloadHandler = new DownloadHandlerBuffer();
        web.SetRequestHeader("Content-Type", "application/json");
        web.certificateHandler = new ForceAcceptAll(); // Aquí el bypass SSL

        yield return web.SendWebRequest();

        if (web.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error al enviar: " + web.error);
        }
        else
        {
            Debug.Log("Reporte enviado exitosamente");
        }
    }
}