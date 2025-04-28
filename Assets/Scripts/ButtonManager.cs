using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    
    public void CambiarAEscenaLogin()
    {
        SceneManager.LoadScene("Login");
    }

    public void CambiarAEscenaLeaderboard()
    {
        SceneManager.LoadScene("Leaderboard");
    }

    public void CambiarAEscenaReporte()
    {
        SceneManager.LoadScene("Reporte");
    }
}
