using System;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

public class GoToGame : MonoBehaviour
{
   private UIDocument MainMenu;
   void OnEnable()
   {
       MainMenu = GetComponent<UIDocument>();
       var root = MainMenu.rootVisualElement;
       var button = root.Q<Button>("play");
       button.RegisterCallback<ClickEvent>(Play);
   }

    private void Play(ClickEvent evt)
    {
        SceneManager.LoadScene("City");
    }
}