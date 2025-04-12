using UnityEngine;


namespace Cryptopia.Problema {
    public class Problema
    {
        /*public string incognita { get; private set; }
        public string solucion { get; private set; }
        public int puntaje { get; private set; }
        public string[] algoritmo { get; private set; }

        public Problema(string Incognita, string[] Algoritmo, string Solucion, int Puntaje)
            => (incognita, algoritmo, solucion, puntaje) = (Incognita, Algoritmo, Solucion, Puntaje);*/

    public int idPalabra;
    public string palabra;
    public string encriptacion;
    public string respuestaCorrecta;
    public int puntos;
    public int idMinijuego;

        public bool ValidarRespuesta (string respuesta)
        {
            if (respuesta == respuestaCorrecta)
            {
                return true;
            }
            else return false;
        }
    }

   
   
}