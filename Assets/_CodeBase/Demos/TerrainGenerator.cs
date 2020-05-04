using  UnityEngine;

namespace _CodeBase.Demos
{
    public class TerrainGenerator
    {

        private float maxHeight;


        public TerrainGenerator(float maxHeight)
        {

            this.maxHeight = maxHeight;

        }
 
        public float GetHeight()
        {
            var height = Random.Range(0, 1f);
            height = Mathf.Pow(2 * height - 1, 3);

            return height * maxHeight;
        }
        
    }
}