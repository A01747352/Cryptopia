using UnityEngine;
    public class FlyingCoins : MonoBehaviour
    {
        
        public ParticleSystem flyingCoins;
        void OnEnable()
        {
            flyingCoins = GetComponent<ParticleSystem>();
        }
        public void FlyCoins()
        {
            flyingCoins.Play();
        }
        
        
    }
