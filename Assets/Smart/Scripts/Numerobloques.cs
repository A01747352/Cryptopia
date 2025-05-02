using UnityEngine;

public class Numerobloques : MonoBehaviour
{
    void Update()
    {
        CryptoMine cryptoMine = FindAnyObjectByType<CryptoMine>();
        if (cryptoMine != null)
        {
            // Asegúrate de que el valor de bloques minados se guarde en PlayerPrefs
            PlayerPrefs.SetInt("TotalMinedBlocks", cryptoMine.totalMinedBlocks);
            PlayerPrefs.Save();  // Guardamos los cambios en PlayerPrefs
        }
    }
}