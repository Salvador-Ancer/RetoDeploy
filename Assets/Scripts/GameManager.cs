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
    public Dropdown tipoIrregularidadDropdown;
    public InputField descripcionInput;

    private string apiUrl = "https://10.22.158.116:7258/api/NotasIrregularidades"; // Cambia IP si es necesario


    public void Start()
    {
        tiendaDropdown.onValueChanged.AddListener(delegate { OnDropdownChange(); });
        tipoIrregularidadDropdown.onValueChanged.AddListener(delegate { OnDropdownChange(); }); // Escuchar ambos dropdowns
    }

    void OnDropdownChange()
    {
        string tiendaSeleccionada = tiendaDropdown.options[tiendaDropdown.value].text;
        string tipoSeleccionado = tipoIrregularidadDropdown.options[tipoIrregularidadDropdown.value].text;
        StartCoroutine(GetNotaDeTienda(tiendaSeleccionada, tipoSeleccionado));
    }

    IEnumerator GetNotaDeTienda(string tienda, string tipoIrregularidad)
    {
        string getUrl = apiUrl + "?tienda=" + UnityWebRequest.EscapeURL(tienda) + "&tipoIrregularidad=" + UnityWebRequest.EscapeURL(tipoIrregularidad);

        UnityWebRequest web = UnityWebRequest.Get(getUrl);
        web.certificateHandler = new ForceAcceptAll();
        yield return web.SendWebRequest();

        if (web.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error al obtener la nota: " + web.error);
        }
        else
        {
            Debug.Log("Notas obtenidas: " + web.downloadHandler.text);

            List<NotaIrregularidad> notas = JsonConvert.DeserializeObject<List<NotaIrregularidad>>(web.downloadHandler.text);

            if (notas != null && notas.Count > 0)
            {
                // Mostrar la más reciente
                NotaIrregularidad ultimaNota = notas[notas.Count - 1];
                descripcionInput.text = ultimaNota.descripcion;
            }
            else
            {
                descripcionInput.text = "No hay notas registradas, empieze anotando una nueva";
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
        public string tipoIrregularidad;
    }

    public void Enviar()
    {
        StartCoroutine(EnviarIrregularidad());
    }


        IEnumerator EnviarIrregularidad()
        {
            string tiendaSeleccionada = tiendaDropdown.options[tiendaDropdown.value].text;
            string tipoIrregularidad = tipoIrregularidadDropdown.options[tipoIrregularidadDropdown.value].text;
            string descripcion = descripcionInput.text;
            int idUsuario = PlayerPrefs.GetInt("usuario_id");
            Debug.Log("idUsuario: " + idUsuario);

            var irregularidad = new
            {
                Tienda = tiendaSeleccionada,
                Descripcion = descripcion,
                TipoIrregularidad = tipoIrregularidad,
                IdUsuario = idUsuario
            };

            string jsonData = JsonConvert.SerializeObject(irregularidad);
            byte[] jsonToSend = Encoding.UTF8.GetBytes(jsonData);

            UnityWebRequest web = new UnityWebRequest(apiUrl, "POST");
            web.uploadHandler = new UploadHandlerRaw(jsonToSend);
            web.downloadHandler = new DownloadHandlerBuffer();
            web.SetRequestHeader("Content-Type", "application/json");
            web.certificateHandler = new ForceAcceptAll(); // Solo para bypass SSL en pruebas

            yield return web.SendWebRequest();

            if (web.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error al enviar: " + web.error);
                Debug.LogError("Respuesta completa: " + web.downloadHandler.text);
            }
            else
            {
                Debug.Log("Reporte enviado exitosamente");
                Debug.Log("Respuesta completa: " + web.downloadHandler.text);
            }
        }
    
}