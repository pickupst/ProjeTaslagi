using System.Collections.Generic;
using UnityEngine;

namespace _CodeBase.Demos
{
    public class TerrainPatch
    {
        private const int SizeZ = 2; //İleri doğru oluşacak yükselti sayısı ile uzunluğu belirliyor
        private const int SizeX = 2; //Yataydaki katman sayısı ile genişliği belirliyor

        public static float SpacingZ = 10f; //triangles uzunlukları
        public static float SpacingX = 5f; //triangles genişlikleri
        
        public float MaxHeight = 5f; //Ulaşılabilecek en yüksek tepe noktası

        private static int _tileSideCountZ = -1;
        
        public static float PatchSizeZ
        {
            get { return (SizeZ - 1) * SpacingZ; }
        }
        
        public static float PatchSizeX
        {
            get { return (SizeX - 1) * SpacingX; }
        }
        
        public static List<TerrainPatch> Patches { get; set; } //
        private List<Vector3> vertices = new List<Vector3>(); //Vertex üçgenlerin konumlarını tutacağız
        private List<Vector2> uvs = new List<Vector2>(); //
        private List<int> triangles = new List<int>(); //Poligonları sayısal olarak tutacağımız dizi
        
        private GameObject meshObject; //Yolumuzun objesini tanımladık
        
        private MeshFilter meshFilter; 
        private MeshCollider meshCollider;
        private Material material;
        private Mesh mesh;
        
        public Vector2 OffSet { get; set; }

        private Vector3 position;

        private static float oldPatchHight = 5f; //Başlangıç platform yüksekliği
        
        public static bool IsInitialize { get; private set; }

        private void Initialize()
        {
            
            Patches = new List<TerrainPatch>();
            
            
            IsInitialize = true;

        }
        
        public Vector3 Position
        {
            get => position;
            set
            {
                position = new Vector3(value.x * (SizeX - 1) * SpacingX, value.y, value.z * (SizeZ - 1) * SpacingZ);
            }
        }

        public TerrainPatch(Vector3 pos, int tileSideCountZ, float maxHeight)
        {
            MaxHeight = maxHeight; 
            
            if (_tileSideCountZ == -1)
            {
                _tileSideCountZ = tileSideCountZ;
            }
            _tileSideCountZ--;

            if (!IsInitialize)
            {
                Initialize();
            }
            
            OffSet = new Vector2(pos.x, pos.z);
            
            Position = pos;
            
            material = Resources.Load("m_UVTest") as Material;
            
            meshObject = new GameObject(string.Format("Mesh_{0}_{1}:{2}", (Patches.Count + 1).ToString().PadLeft(3, '0'), Position.x.ToString(), Position.z.ToString()));
            
            meshObject.AddComponent<MeshFilter>();
            meshObject.AddComponent<MeshRenderer>();
            meshObject.AddComponent<MeshCollider>();
            
            meshObject.transform.position = position;
            meshFilter = meshObject.GetComponent<MeshFilter>() as MeshFilter;
            meshCollider = meshObject.GetComponent<MeshCollider>() as MeshCollider;
            
            CreateGrid();
            CreateMesh();
            UpdateMesh();

            Patches.Add(this);
            
        } //TerrainPatch()

        private void CreateGrid()
        {
            vertices.Clear();
            uvs.Clear();
            
            var offsetZ = (SizeZ - 1) * SpacingZ / 2f;
            var offsetX = (SizeX - 1) * SpacingX / 2f;

            int verticesCount = 0;
            
            for (int z = 0; z < SizeZ; z++)
            {
                //Debug.Log("oldPatchHight : " + oldPatchHight + " : " + _tileSideCountZ);
                float height = oldPatchHight; //Patchler arası kopukluğu engellemek için bir önceki patchin son mesh yüksekliği neyse bir sonraki patch onunla başlar

                if (_tileSideCountZ <= 0) //Son yükseltiye geldik mi? 
                { 
                    if (verticesCount == 1) //Son yükseltinin ikinci noktasına geldik mi?
                    {
                        height = 5f; //Son zıplamayı yapıyorsak kesinlikle tepe olmalı. Çukur olursa son zıplama gerçekleşmez
                    }
                    verticesCount++;
                }
                
                if (z % 2 == 0) //Çift sayılı satırlar
                {
                    for (int x = 0; x < SizeX; x++)
                    {
                        
                        vertices.Add(new Vector3(x * SpacingX - offsetX, height, z * SpacingZ - offsetZ));
                        
                        var u = x * SpacingX / ((SizeX - 1) * SpacingX);
                        var v = z * SpacingZ / ((SizeZ - 1) * SpacingZ);
                        
                        uvs.Add(new Vector2(u, v));
                        
                    }    
                }
                else //Tek sayılı satırlar
                {
                    for (int x = 0; x < SizeX + 1; x++)
                    {
                        var posX = x * SpacingX - SpacingX / 2;

                        if (posX < 0)
                        {
                            
                            posX = 0;
                            
                        } else if (posX > (SizeX - 1) * SpacingX)
                        {
                            
                            posX = (SizeX - 1) * SpacingX;
                            
                        }
                        
                        vertices.Add(new Vector3(posX - offsetX, height, z * SpacingZ - offsetZ));
                        
                        var u = posX * SpacingX / ((SizeX - 1) * SpacingX);
                        var v = z * SpacingZ / ((SizeZ - 1) * SpacingZ);
                        
                        uvs.Add(new Vector2(u, v));
                        
                    }
                }

                if (z < SizeZ - 1 ) //Son yükseltiye gelmediysek
                {
                    oldPatchHight = TerrainDemo.terrainGenerator.GetHeight();
                }
            }

        } //CreateGrid()

        private void CreateMesh()
        {
            triangles.Clear();
            
            int index = 0;

            for (int z = 0; z < SizeZ - 1; z++)
            {
                if (z % 2 == 0) //Çift sayılı satırlar
                {
                    for (int x = 0; x < SizeX; x++)
                    {
                        triangles.Add(index);
                        triangles.Add(index + SizeX);
                        triangles.Add(index + SizeX + 1);
                        
                        /*
                        Debug.DrawLine(Vertices[index], Vertices[index + Size]);
                        Debug.DrawLine(Vertices[index + Size], Vertices[index + Size + 1]);
                        Debug.DrawLine(Vertices[index + Size + 1], Vertices[index]);
                        */
                        
                        if (x < SizeX - 1)
                        {
                            
                            triangles.Add(index);
                            triangles.Add(index + SizeX + 1);
                            triangles.Add(index + 1);
                            
                            /*
                            Debug.DrawLine(Vertices[index], Vertices[index + Size + 1]);
                            Debug.DrawLine(Vertices[index + Size + 1], Vertices[index + 1]);
                            Debug.DrawLine(Vertices[index + 1], Vertices[index]);
                            */
                            
                        }

                        index++;
                    }    
                }
                else //Tek sayılı satırlar
                {
                    for (int x = 0; x < SizeX; x++)
                    {
                        triangles.Add(index);
                        triangles.Add(index + SizeX + 1);
                        triangles.Add(index + 1);
                        /*
                        Debug.DrawLine(Vertices[index], Vertices[index + Size + 1]);
                        Debug.DrawLine(Vertices[index + Size + 1], Vertices[index + 1]);
                        Debug.DrawLine(Vertices[index + 1], Vertices[index]);
                        */
                        if (x < SizeX - 1)
                        {
                            triangles.Add(index + 1);
                            triangles.Add(index + SizeX + 1);
                            triangles.Add(index + SizeX + 2);
                            
                            /*Debug.DrawLine(Vertices[index + 1], Vertices[index + Size + 1]);
                            Debug.DrawLine(Vertices[index + Size + 1], Vertices[index + Size + 2]);
                            Debug.DrawLine(Vertices[index + Size + 2], Vertices[index + 1]);*/

                        }
                        
                        index++;
                    }

                    index++; //Çift sayılı ve tek sayılı satırlarda ki vertex sayılarında ki farkı önlemek için
                } //else
                
            } //for

        }

        private void UpdateMesh()
        {
            
            mesh = new Mesh();
            mesh.vertices = vertices.ToArray();

            mesh.uv = uvs.ToArray();
            
            mesh.triangles = triangles.ToArray();
            
            mesh.RecalculateNormals(); // ters yöne bakan yüzey normali olmaması için
            
            meshFilter.mesh = mesh;

            meshCollider.sharedMesh = mesh;

            if (MaxHeight > 0) //Engebeli yolsa
            {
                meshObject.tag = "Road";
            }
            else //Zıplama yoluysa
            {
                meshObject.tag = "JumpLine";
            }
            
            meshObject.GetComponent<MeshRenderer>().sharedMaterial = material;
            
            meshObject.GetComponent<MeshRenderer>().material.mainTextureScale = new Vector2(0.1f * (SizeX - 1), 0.1f * (SizeZ - 1));

        }

        public void Destroy()
        {
            vertices.Clear();
            uvs.Clear();
            triangles.Clear();
            
            Object.Destroy(meshFilter.mesh);
            Object.Destroy(meshCollider.sharedMesh);
            Object.Destroy(mesh);
            Object.Destroy(meshCollider);
            Object.Destroy(meshObject);
            Object.Destroy(meshFilter);
            Object.Destroy(meshObject);

            Patches.Remove(this);

        }
        
        
    }
    
    
}































