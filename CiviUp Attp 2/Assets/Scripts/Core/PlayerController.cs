using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CountryData playerCountry;

    public bool isPlayerTurn => GameManager.Instance.currentCountry == playerCountry;

    public void EndTurn()
    {
        if (!isPlayerTurn)
            return;

        GameManager.Instance.EndTurn();
        
    }
}
