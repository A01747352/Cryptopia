using System;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

public class PauseButton : MonoBehaviour
{
   private UIDocument Game;
   void OnEnable()
   {
       Game = GetComponent<UIDocument>();
       var root = Game.rootVisualElement;
       var button = root.Q<Button>("pauseButton");
       button.RegisterCallback<ClickEvent>(Regresar);
   }

    private void Regresar(ClickEvent evt)
    {
        SceneManager.LoadScene("Pause");
    }
}