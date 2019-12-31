using Main.Main;
using Main.PatternEditor;
using Main.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Structures
{
    public class SingleWall : Wall, ICloneable
    {
        public int[,] Voxels { get; set; } = new int[0,0];
        bool IsDestructible { get; set; } = true;
        public int VX { get; private set; } = 0;
        public int VY { get; private set; } = 0;

        public VoxelPattern Pattern { get; set; } = VoxelPattern.MarchingSquare;
        
        public SingleWall(Vector3 PA, Vector3 Rot, Vector3 Scale, VoxelPattern WallPatern, int SX, int SY, string Material, bool Destructible) : base(PA, Rot, Scale)
        {
            Pattern = WallPatern;
            IsDestructible = Destructible;
            VX = SX;
            VY = SY;
            
        }
          
        public override void Init(byte[] Data)
        {
            int ByteCount = 0;
            
            float PX = BitConverter.ToSingle(Data, 0);
            ByteCount += 4;
            float PY = BitConverter.ToSingle(Data, ByteCount);
            ByteCount += 4;
            float PZ = BitConverter.ToSingle(Data, ByteCount);
            ByteCount += 4;

            float RX = BitConverter.ToSingle(Data, ByteCount);
            ByteCount += 4;
            float RY = BitConverter.ToSingle(Data, ByteCount);
            ByteCount += 4;
            float RZ = BitConverter.ToSingle(Data, ByteCount);
            ByteCount += 4;

            float SX = BitConverter.ToSingle(Data, ByteCount);
            ByteCount += 4;
            float SY = BitConverter.ToSingle(Data, ByteCount);
            ByteCount += 4;
            float SZ = BitConverter.ToSingle(Data, ByteCount);
            ByteCount += 4;
            
            int PatternIndex = BitConverter.ToInt32(Data, ByteCount);
            ByteCount += 4;
            int VX = BitConverter.ToInt32(Data, ByteCount);
            ByteCount += 4;
            int VY = BitConverter.ToInt32(Data, ByteCount);
            ByteCount += 4;
            
            int VoxelDataLength = BitConverter.ToInt32(Data,ByteCount);
            ByteCount += 4;

            Voxels = CompressionUtil.UnPackToIntArray(VX,VY, Data, ByteCount, VoxelDataLength);
            
            ByteCount += VoxelDataLength * 8;
            
            A = new Vector3(PX,PY,PZ);
            Rotation = new Vector3(RX,RY,RZ);
            Scale = new Vector3(SX,SY,SZ);
        }
        
        public override byte[] ToBytes()
        {
            List<byte> Data = new List<byte>();
            Data.AddRange(BitConverter.GetBytes(A.X));
            Data.AddRange(BitConverter.GetBytes(A.Y));
            Data.AddRange(BitConverter.GetBytes(A.Z));

            Data.AddRange(BitConverter.GetBytes(Rotation.X));
            Data.AddRange(BitConverter.GetBytes(Rotation.Y));
            Data.AddRange(BitConverter.GetBytes(Rotation.Z));

            Data.AddRange(BitConverter.GetBytes(Scale.X));
            Data.AddRange(BitConverter.GetBytes(Scale.Y));
            Data.AddRange(BitConverter.GetBytes(Scale.Z));

            Data.AddRange(BitConverter.GetBytes((int)Pattern));
            Data.AddRange(BitConverter.GetBytes(Voxels.GetLength(0)));
            Data.AddRange(BitConverter.GetBytes(Voxels.GetLength(1)));

            object[] CompressedVoxels = CompressionUtil.Array2DToCompressedArrays(Voxels);
            List<int> WallQuantities = (List<int>)CompressedVoxels[0];
            List<int> WallValues = (List<int>)CompressedVoxels[0];

            int CompressedVoxelLength = WallQuantities.Count;
            
            Data.AddRange(BitConverter.GetBytes(CompressedVoxelLength));
            Data.AddRange(CompressionUtil.ListToBytes(WallQuantities));
            Data.AddRange(CompressionUtil.ListToBytes(WallValues));
            return Data.ToArray();
        }
        
        public void UpdateWall(int SX, int SY, string Material)
        {
            VX = SX;
            VY = SY;
            string[] Bits = Material.Split('-');

            int Mat = 0;
            bool CanParse = Int32.TryParse(Bits[0], out Mat);

            if ( !CanParse)
            {
                string[] PatternString = Bits[0].Split(',');
                string Pat = PatternString[0];
                
                switch (PatternString[1])
                {
                    case "N":
                        Voxels = PatternIOUtil.Import(Pat);
                        break;
                    case "S":
                        Voxels = VoxelUtil.ScaleVoxels( new int[SX,SY], PatternIOUtil.Import(Pat));
                        break;
                    case "O":
                        int[,] NewVoxels = PatternIOUtil.Import(Pat);
                        Voxels = new int[SX > NewVoxels.GetLength(0) ? NewVoxels.GetLength(0) : SX, SY > NewVoxels.GetLength(0) ? NewVoxels.GetLength(0) : SY];
                        Voxels = VoxelUtil.MergeFields( Voxels, PatternIOUtil.Import(Pat));
                        break;
                }
            }
            else
            {
                if (Mat == 0)
                {
                    return;
                }

                Voxels = new int[SX, SY];
                for (int i = 0; i < Voxels.GetLength(0); i++)
                {
                    for (int j = 0; j < Voxels.GetLength(1); j++)
                    {
                        Voxels[i, j] = Mat;
                    }
                }
            }
            NeedsGeomUpdate = true;
        }

        public SingleWall() { }

        
        public override void GenerateGeometry(RenderManager RenderManager)
        {
            Geom = new Geometry3D(Name);
            Geom.SetTextures(new string[] { "Structure"});
            BasicEffect BasicEffect = new BasicEffect(RenderManager.Graphics);
            BasicEffect.TextureEnabled = true;
            Geom.Shader = BasicEffect;
            Geom.HasCull = false;
            Geom.LocalPosition = A;
            Geom.WorldPosition = WorldPosition;

            Geom.Rotation = Rotation;
            Geom.RenderBucket = Geometry3D.RenderQueue.Solid;

            TransGeom = new Geometry3D("Trans" + Name);
            TransGeom.SetTextures(new string[] { "Structure" });
            TransGeom.Shader = BasicEffect;
            TransGeom.HasCull = false;
            TransGeom.LocalPosition = A;
            TransGeom.WorldPosition = WorldPosition;

            TransGeom.Rotation = Rotation;
            TransGeom.RenderBucket = Geometry3D.RenderQueue.Transparent;
            NeedsGeomUpdate = true;
        }
        

        public override void UpdateGeometry(RenderManager RenderManager)
        {
            Geom.SetName(Name);
            TransGeom.SetName(Name);
            Geom.LocalPosition = A;
            Geom.WorldPosition = WorldPosition;
            TransGeom.LocalPosition = A;
            TransGeom.WorldPosition = WorldPosition;

            WallGenerator.GenerateWall(RenderManager.Graphics, this);
        }
        
        public override object Clone()
        {
            SingleWall NewComponent = new SingleWall();
            
            NewComponent.A = new Vector3(A.X,A.Y,A.Z);
            NewComponent.WorldPosition = new Vector3(WorldPosition.X, WorldPosition.Y, WorldPosition.Z);
            NewComponent.Rotation = new Vector3(Rotation.X,Rotation.Y,Rotation.Z);
            NewComponent.Scale = new Vector3(Scale.X, Scale.Y, Scale.Z);
           
            NewComponent.NeedsGeomUpdate = NeedsGeomUpdate;
            NewComponent.Name = Name;

            NewComponent.Pattern = Pattern;
            NewComponent.Voxels = (int[,])Voxels.Clone();
            return NewComponent;
        }
        
        public void Modify(Vector3 ModifyPosition, int Material)
        {
            if (IsDestructible)
            {
                Matrix Rot = new Matrix();
                Rot += Matrix.CreateRotationZ(MathUtil.GetRadian(Rotation.Z));
                
                Rot *= Matrix.CreateRotationY(MathUtil.GetRadian(Rotation.Y));
                Rot *= Matrix.CreateRotationX(MathUtil.GetRadian(Rotation.X));

                Vector3 Offset = Vector3.Transform(new Vector3(), Rot);

                int PX = (int)Math.Round(MathUtil.Distance(ModifyPosition.X, A.X + WorldPosition.X) / Scale.X);
                int PY = (int)Math.Round(MathUtil.Distance(ModifyPosition.Y, A.Y + WorldPosition.Y) / Scale.Y);
                int PZ = (int)Math.Round(MathUtil.Distance(ModifyPosition.Z, A.Z + WorldPosition.Z) / Scale.Z);

                Vector3 Pos = Vector3.Transform(new Vector3(PX, PY, PZ), Rot);

                PX = (int)Math.Round(Math.Abs(Pos.X));
                PZ = (int)Math.Round(Math.Abs(Pos.Z));
                
                if (PX >= 0 && PX < Voxels.GetLength(0) && PZ >= 0 && PZ < Voxels.GetLength(1))
                {
                    Voxels[PX, PZ] = Material;
                    NeedsGeomUpdate = true;
                }
            }
            else
            {
                
            }

        }
        
    }
}
