using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.WorldEditor
{
    public class SettingsContainer
    {
        public int[] LOD { get; set; } = new int[] { 128, 256, 512, 1024, 2048};
        public float[] TerrainLODResolution { get; set; } = new float[] { 0.5f, 1.0f, 4.0f, 8.0f, 16.0f };

        public int[] PropLOD { get; set; } = new int[] { 32, 128, 256};
        public int[] StructureLOD { get; set; } = new int[] { 32, 128, 256};

        public int LODMode = 1;
        
        public int ChunkLODCount = 5;
        public int ChunkDistance = 32;
        public int ChunkSize { get; set; } = 64;
        public float MinWorldScale { get; set; } = 0.125f;

        public Vector3 DebugLightPosition = new Vector3(-200,-200,200);
        public Vector3 DebugAmbientLightColor = new Vector3(0.2f, 0.2f, 0.2f);
        //public Vector3 DebugDirectLightColor = new Vector3(0.788f, 0.886f, 1.0f);
        //public Vector3 DebugDirectLightColor = new Vector3(0.7f, 0.886f, 1.0f);
        public Vector3 DebugDirectLightColor = new Vector3(0.99f, 0.99f, 0.99f);

        public int FromBelowClosePlane = 0;
        public int FromBelowFarPlane = 1000;

        public int FromBelowResolutionX = 1025;
        public int FromBelowResolutionY = 1025;

        public int ChunkSelectionRadius = 2;
        
        public float CameraClosePlane = 0.01f;
        public float CameraFarPlane = 2000.0f;
        
        public int WaterReflectionResolutionX = 1920;
        public int WaterReflectionResolutionY = 1080;

        public int WaterPassSetting = 2;
        public int WaterPassMipSetting = 1;

        public float WaterReflectionPlaneDirection = 1;
        public float WaterRefractionPlaneDirection = -1;
        public float ClipOffset = 0.05f;

        public float WaterReflectivity = 0.95f;
        public float WaterShineDamper = 15;
        public float MinimumSpecular = -0.05f;
        public float WaterRepeatTime = 20.0f;
        public float WaterSpeed = 3.0f;
        public float TextureSize = 16.0f;
        public float WaterFoamStart = 0.0f;
        public float WaterFoamEnd = 1.0f;

        public SettingsContainer()
        {

            
        }
        
        public byte[] ToBytes()
        {



            return new byte[] { };
        }

    }
}
