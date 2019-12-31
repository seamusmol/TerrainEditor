using Main.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Geometry
{
    public static class ModelUtil
    {
        public static GeometryModel ImportOBJModel(string FileName)
        {
            GeometryModel Model = LoadModel(FileName);


            return Model;
        }

        //TODO 
        //Add Vertex Normal Smoothing
        
        public static GeometryModel LoadModel(String ModelName)
        {
            List<Vector3> VertexBuffer = new List<Vector3>();
            List<Vector2> TexCoordBuffer = new List<Vector2>();
            List<Vector3> NormalBuffer = new List<Vector3>();
            try
            {
                StreamReader file = new StreamReader(ModelName);

                string[] Lines = File.ReadAllLines(ModelName);

                List<Vector3> VertexLookupBuffer = new List<Vector3>();
                List<Vector2> TexCoordLookupBuffer = new List<Vector2>();
                List<Vector3> NormalLookupBuffer = new List<Vector3>();
                List<Vector3> SmoothNormalLookUp = new List<Vector3>();
 
                for (int i = 0; i < Lines.Length; i++)
                {
                    if (Lines[i].StartsWith("v "))
                    {
                        String[] bits = Lines[i].Replace("v ", "").Split();
                        VertexLookupBuffer.Add(new Vector3(float.Parse(bits[0]), float.Parse(bits[1]), float.Parse(bits[2])));
                    }
                    else if (Lines[i].StartsWith("vt "))
                    {
                        String[] bits = Lines[i].Replace("vt ", "").Split();
                        TexCoordLookupBuffer.Add(new Vector2(float.Parse(bits[0]), 1.0f - float.Parse(bits[1])));
                    }
                    else if (Lines[i].StartsWith("vn "))
                    {
                        String[] bits = Lines[i].Replace("vn ", "").Split();
                        NormalLookupBuffer.Add(new Vector3(float.Parse(bits[0]), float.Parse(bits[1]), float.Parse(bits[2])));
                    }
                }
                
                /*
                for (int i = 0; i < Lines.Length; i++)
                {
                    if (Lines[i].StartsWith("v "))
                    {
                        String[] bits = Lines[i].Replace("v ", "").Split();
                        VertexLookupBuffer.Add(new Vector3(float.Parse(bits[0]), float.Parse(bits[1]), float.Parse(bits[2])));
                    }
                    else if (Lines[i].StartsWith("vt "))
                    {
                        TexCoordStartIndex = i;
                        break;
                    }
                }

                for (int i = TexCoordStartIndex; i < Lines.Length; i++)
                {
                    if (Lines[i].StartsWith("vt "))
                    {
                        String[] bits = Lines[i].Replace("vt ", "").Split();
                        TexCoordLookupBuffer.Add(new Vector2(float.Parse(bits[0]), 1.0f - float.Parse(bits[1])));
                    }
                    else if (Lines[i].StartsWith("vn "))
                    {
                        NormalStartIndex = i;
                        break;
                    }
                }

                for (int i = NormalStartIndex; i < Lines.Length; i++)
                {
                    if (Lines[i].StartsWith("vn "))
                    {
                        String[] bits = Lines[i].Replace("vn ", "").Split();
                        NormalLookupBuffer.Add(new Vector3(float.Parse(bits[0]), float.Parse(bits[1]), float.Parse(bits[2])));
                    }
                    else if (Lines[i].StartsWith("f "))
                    {
                        FaceStartIndex = i;
                        break;
                    }
                }
                
                //generate smooth normals

                for (int i = 0; i < VertexLookupBuffer.Count; i++)
                {
                    int NormalCount = 0;
                    Vector3 NormalSum = new Vector3();

                    for (int j = FaceStartIndex; j < Lines.Length; j++)
                    {
                        if (Lines[j].StartsWith("f "))
                        {
                            String[] bits = Lines[j].Replace("f ", "").Replace(" ", "/").Split('/');
                           
                            int Index0 = int.Parse(bits[2]) - 1;
                            int Index1 = int.Parse(bits[5]) - 1;
                            int Index2 = int.Parse(bits[8]) - 1;

                            if (Index0 == i || Index1 == i || Index2 == i)
                            {
                                //Shares Vertex with lookup vertex
                                NormalSum += NormalLookupBuffer.ElementAt(int.Parse(bits[2]) - 1);
                                NormalCount++;
                            }
                        }
                    }
                    NormalSum /= NormalCount;
                    SmoothNormalLookUp.Add( NormalSum);
                }
                */


                for (int i = 0; i < Lines.Length; i++)
                {
                    if (Lines[i].StartsWith("f "))
                    {
                        String[] bits = Lines[i].Replace("f ", "").Replace(" ", "/").Split('/');

                        for (int j = 0; j < bits.Length; j += 3)
                        {
                            VertexBuffer.Add(VertexLookupBuffer.ElementAt(int.Parse(bits[j]) - 1));
                            TexCoordBuffer.Add(TexCoordLookupBuffer.ElementAt(int.Parse(bits[j + 1]) - 1));
                            NormalBuffer.Add(NormalLookupBuffer.ElementAt(int.Parse(bits[j+2]) - 1));
                        }
                    }


                }
                
            }
            catch (FileNotFoundException e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine("Model " + ModelName + " Not found");
            }

            //check if smooth normals
            
            //grab 3 normal values for polygon bias

            //search all 


            GeometryModel Model = new GeometryModel();
            
            Model.VertexList = VertexBuffer;
            Model.TexcoordList = TexCoordBuffer;
            Model.NormalList = NormalBuffer;
            
            List<Vector3> Tangents = GraphicsUtil.GenerateTangents(VertexBuffer, TexCoordBuffer, NormalBuffer);
            return Model;
        }
        

    }

}
