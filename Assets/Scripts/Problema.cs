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

    public string incognita;
    public string[] algoritmo;
    public string solucion;
    public int puntaje;

        public bool ValidarRespuesta (string respuesta)
        {
            if (respuesta == solucion)
            {
                return true;
            }
            else return false;
        }
    }

    public class ProblemaWrapper
    {
        public Problema[] bancoProblemas;
    }

   
}