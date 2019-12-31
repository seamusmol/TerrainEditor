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
    public class DoubleWall : Wall
    {
        public int[,] Voxels { get; set; } = new int[0, 0];
        public int[,] Voxels2 { get; set; } = new int[0, 0];

        public int VX { get; private set; } = 0;
        public int VY { get; private set; } = 0;

        bool IsDestructible { get; set; } = true;
        bool IsDestructible2 { get; set; } = true;

        public float WallWidth = 0.4f;
        public float WallWidth2 = 0.4f;
        public float WallGapWidth = 0.2f;

        public VoxelPattern Pattern { get; set; } = VoxelPattern.MarchingSquare;
        public VoxelPattern Pattern2 { get; set; } = VoxelPattern.MarchingSquare;

        public DoubleWall(Vector3 PA, Vector3 Rot, Vector3 Scale, VoxelPattern WallPattern, VoxelPattern WallPattern2, int SX, int SY, string Material, bool Destructible, bool Destructible2) : base(PA, Rot, Scale)
        {
            Pattern = WallPattern;
            Pattern2 = WallPattern2;
            
            IsDestructible = Destructible;
            IsDestructible2 = Destructible2;

            VX = SX;
            VY = SY;

            UpdateWall(SX, SY, Material);
        }

        public DoubleWall() { }

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
            
            IsDestructible = BitConverter.ToBoolean(Data, ByteCount);
            ByteCount++;
            IsDestructible2 = BitConverter.ToBoolean(Data, ByteCount);
            ByteCount++;

            int VX = BitConverter.ToInt32(Data, ByteCount);
            ByteCount += 4;
            int VY = BitConverter.ToInt32(Data, ByteCount);
            ByteCount += 4;
            
            Pattern = (VoxelPattern)BitConverter.ToInt32(Data, ByteCount);
            ByteCount += 4;
            Pattern2 = (VoxelPattern)BitConverter.ToInt32(Data, ByteCount);
            ByteCount += 4;

            int VoxelDataLength = BitConverter.ToInt32(Data, ByteCount);
            ByteCount += 4;
            Voxels = CompressionUtil.UnPackToIntArray(VX, VY, Data, ByteCount, VoxelDataLength);
            ByteCount += VoxelDataLength * 8;

            int VoxelDataLength2 = BitConverter.ToInt32(Data, ByteCount);
            ByteCount += 4;
            Voxels2 = CompressionUtil.UnPackToIntArray(VX, VY, Data, ByteCount, VoxelDataLength2);
            ByteCount += VoxelDataLength2 * 8;

            A = new Vector3(PX, PY, PZ);
            Rotation = new Vector3(RX, RY, RZ);
            Scale = new Vector3(SX, SY, SZ);

        }

        //Vector3 PA, Vector3 Rot, Vector3 Scale, StructureType Type, VoxelPattern WallPatern, int SX, int SY, int[,] voxels, bool Destructible
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

            Data.AddRange(BitConverter.GetBytes(IsDestructible));
            Data.AddRange(BitConverter.GetBytes(IsDestructible2));

            Data.AddRange(BitConverter.GetBytes(Voxels.GetLength(0)));
            Data.AddRange(BitConverter.GetBytes(Voxels.GetLength(1)));

            Data.AddRange(BitConverter.GetBytes((int)Pattern));
            Data.AddRange(BitConverter.GetBytes((int)Pattern2));

            object[] CompressedVoxels = CompressionUtil.Array2DToCompressedArrays(Voxels);
            List<int> Quantities = (List<int>)CompressedVoxels[0];
            List<int> Values = (List<int>)CompressedVoxels[1];
            int CompressedVoxelLength = Quantities.Count;
            
            Data.AddRange(BitConverter.GetBytes(CompressedVoxelLength));
            Data.AddRange(CompressionUtil.ListToBytes(Quantities));
            Data.AddRange(CompressionUtil.ListToBytes(Values));

            object[] CompressedVoxels2 = CompressionUtil.Array2DToCompressedArrays(Voxels2);
            List<int> Quantities2 = (List<int>)CompressedVoxels2[0];
            List<int> Values2 = (List<int>)CompressedVoxels2[1];

            int CompressedVoxelLength2 = Quantities2.Count;

            Data.AddRange(BitConverter.GetBytes(CompressedVoxelLength2));
            Data.AddRange(CompressionUtil.ListToBytes(Quantities2));
            Data.AddRange(CompressionUtil.ListToBytes(Values2));

            return Data.ToArray();
        }

        
        public void UpdateWall(int SX, int SY, string Material)
        {
            string[] Bits = Material.Split('-');

            int SingleMat = 0;
            bool CanParseSingle = Int32.TryParse(Bits[0], out SingleMat);
            
            if (!CanParseSingle)
            {
                string[] SinglePatternString = Bits[0].Split(',');
                string SinglePat = SinglePatternString[0];

                switch (SinglePatternString[1])
                {
                    case "N":
                        Voxels = PatternIOUtil.Import(SinglePat);
                        break;
                    case "S":
                        Voxels = VoxelUtil.ScaleVoxels(new int[SX, SY], PatternIOUtil.Import(SinglePat));
                        break;
                    case "O":
                        int[,] NewVoxels = PatternIOUtil.Import(SinglePat);
                        Voxels = new int[SX > NewVoxels.GetLength(0) ? NewVoxels.GetLength(0) : SX, SY > NewVoxels.GetLength(0) ? NewVoxels.GetLength(0) : SY];
                        Voxels = VoxelUtil.MergeFields(Voxels, PatternIOUtil.Import(SinglePat));
                        break;
                }
            }
            else
            {
                if (SingleMat == 0)
                {
                    return;
                }
                Voxels = new int[SX, SY];
                for (int i = 0; i < Voxels.GetLength(0); i++)
                {
                    for (int j = 0; j < Voxels.GetLength(1); j++)
                    {
                        Voxels[i, j] = SingleMat;
                    }
                }
            }
            
            int DoubleMat = 0;
            bool CanParseDouble = Int32.TryParse(Bits[1], out DoubleMat);
            if (!CanParseDouble)
            {
                string[] DoublePatternString = Bits[1].Split(',');
                string DoublePat = DoublePatternString[0];

                switch (DoublePatternString[1])
                {
                    case "N":
                        Voxels2 = PatternIOUtil.Import(DoublePat);
                        break;
                    case "S":
                        Voxels2 = VoxelUtil.ScaleVoxels(new int[SX, SY], PatternIOUtil.Import(DoublePat));
                        break;
                    case "O":
                        int[,] NewVoxels = PatternIOUtil.Import(DoublePat);
                        Voxels2 = new int[SX > NewVoxels.GetLength(0) ? NewVoxels.GetLength(0) : SX, SY > NewVoxels.GetLength(0) ? NewVoxels.GetLength(0) : SY];
                        Voxels2 = VoxelUtil.MergeFields(Voxels2, PatternIOUtil.Import(DoublePat));
                        break;
                }
            }
            else
            {
                if (DoubleMat == 0)
                {
                    return;
                }
                Voxels2 = new int[SX, SY];
                for (int i = 0; i < Voxels2.GetLength(0); i++)
                {
                    for (int j = 0; j < Voxels2.GetLength(1); j++)
                    {
                        Voxels2[i, j] = DoubleMat;
                    }
                }
            }
            
            int NX1 = Voxels.GetLength(0);
            int NX2 = Voxels2.GetLength(0);
            int NY1 = Voxels.GetLength(1);
            int NY2 = Voxels2.GetLength(1);

            int MNX = Math.Min(NX1, NX2);
            int MNY = Math.Min(NY1, NY2);

            if (NX1 != NX2 || NY1 != NY2)
            {
                Voxels = VoxelUtil.MergeFields( new int[MNX, MNY], Voxels);
                Voxels2 = VoxelUtil.MergeFields(new int[MNX, MNY], Voxels2);
            }
            VX = MNX;
            VY = MNY;

            NeedsGeomUpdate = true;
        }
        
        public override void GenerateGeometry(RenderManager RenderManager)
        {
            Geom = new Geometry3D(Name);
            Geom.SetTextures(new string[] { "Structure" });
            BasicEffect BasicEffect = new BasicEffect(RenderManager.Graphics);
            BasicEffect.TextureEnabled = true;
            Geom.Shader = BasicEffect;
            Geom.HasCull = false;
            Geom.LocalPosition = A;
            Geom.WorldPosition = WorldPosition;
            Geom.Rotation = Rotation;
            Geom.RenderBucket = Geometry3D.RenderQueue.Solid;

            TransGeom = new Geometry3D("Trans" + Name);
            TransGeom.SetTextures(new string[] {"Structure"});
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

            WallGenerator.GenerateWall(RenderManager.Graphics, this);

        }

        public override object Clone()
        {
            DoubleWall NewComponent = new DoubleWall();

            NewComponent.A = new Vector3(A.X, A.Y, A.Z);
            NewComponent.Rotation = new Vector3(Rotation.X, Rotation.Y, Rotation.Z);
            NewComponent.Scale = new Vector3(Scale.X, Scale.Y, Scale.Z);

            NewComponent.NeedsGeomUpdate = NeedsGeomUpdate;
            NewComponent.Name = Name;

            NewComponent.IsDestructible = IsDestructible;
            NewComponent.IsDestructible2 = IsDestructible2;

            NewComponent.Pattern = Pattern;
            NewComponent.Pattern2 = Pattern2;
            NewComponent.Voxels = (int[,])Voxels.Clone();
            NewComponent.Voxels2 = (int[,])Voxels2.Clone();

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

                int PX = (int)Math.Round(MathUtil.Distance(ModifyPosition.X, A.X));
                int PY = (int)Math.Round(MathUtil.Distance(ModifyPosition.Y, A.Y));
                int PZ = (int)Math.Round(MathUtil.Distance(ModifyPosition.Z, A.Z));

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
