using System;
using _CodeBase.DemoFramework;
using UnityEngine;

namespace _CodeBase.Demos
{
    public class TerrainDemo: IDemo
    {
        public static TerrainGenerator terrainGenerator { get; set; }
        
        public GameObject PlayerController;

        private const int TileSideCountZ = 81;
        private const int TileSideCountX = 0;
        
        private const int TileSideCountLineZ = 30;
        
        public void Awake()
        {
            PlayerController = GameObject.FindWithTag("Player");
            
            
        } //Awake

        public void Start()
        {
            terrainGenerator = new TerrainGenerator(5f);
            CreateInitialTerrain(0, TileSideCountZ, 5f); //Yolu oluştur
            terrainGenerator = new TerrainGenerator(0f);
            CreateInitialTerrain(TileSideCountZ, TileSideCountLineZ + TileSideCountZ, 0f); //Atlama mesafesini oluşturuyor

        } //Start

        public void Update()
        {

        } //Update

       

        public void OnGUI()
        {
        } //OnGUI

        public void OnApplicationQuit()
        {
        } //OnApplicationQuit

        private void CreateInitialTerrain(int minZ, int maxZ, float maxHeight)
        {
            for (int x = -TileSideCountX; x <= TileSideCountX; x++)
            {
                for (int z = minZ; z < maxZ; z++)
                {
                    new TerrainPatch(new Vector3(x,0, z), maxZ, maxHeight);
                }
            }
            
        } //CreateInitialTerrain
        
        
    } //class
} //namespace