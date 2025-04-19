using UnityEngine;

public class CatStore : MonoBehaviour
{
    public int playerTokens = 100; // Cantidad inicial de tokens del jugador
    public int cat1Price = 50; // Precio del gato 1
    public int cat2Price = 75; // Precio del gato 2

    public GameObject cat1Sprite; // Sprite del gato 1
    public GameObject cat2Sprite; // Sprite del gato 2

    private bool cat1Purchased = false; // Estado de compra del gato 1
    private bool cat2Purchased = false; // Estado de compra del gato 2

    // Método para manejar la compra de gatos
    public void BuyCat(int catNumber)
    {
        if (catNumber == 1 && !cat1Purchased && playerTokens >= cat1Price)
        {
            playerTokens -= cat1Price;
            cat1Purchased = true;
            cat1Sprite.SetActive(true); // Habilitar el sprite del gato 1
            Debug.Log("Gato 1 comprado!");
        }
        else if (catNumber == 2 && !cat2Purchased && playerTokens >= cat2Price)
        {
            playerTokens -= cat2Price;
            cat2Purchased = true;
            cat2Sprite.SetActive(true); // Habilitar el sprite del gato 2
            Debug.Log("Gato 2 comprado!");
        }
        else
        {
            Debug.Log("No se puede comprar este gato.");
        }
    }
}
