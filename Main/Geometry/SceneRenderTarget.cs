using Main.Main;
using Main.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Main.Geometry
{
    
    public class SceneRenderTarget
    {

        public enum RenderFrequency
        {
            Off = 0, PerFrame = 1, PerBool = 2, PerTime = 3
        }

        public enum RenderType
        {
            ColorCubeMap = 0, SingleColor = 1, SingleDepth = 2
        }

        public enum RenderPositionState
        {
            Static = 0, RenderCamera = 1, LookatPositon = 2
        }
        
        //north,east,south,west,top,bottom
        Vector3[] CubeDirections = new Vector3[]
        {
            new Vector3(0, 10,0),
            new Vector3(0, -10,0),
            new Vector3(10, 0,0),
            new Vector3(-10, 0,0),
            new Vector3(0, 0, 10),
            new Vector3(0, 0, -10)
        };

        Vector3[] CubeUpVectors = new Vector3[]
        {
            new Vector3( 0, 0, -1.0f),
            new Vector3( 0, 0, 1.0f),
            new Vector3( 0, 1.0f, 0),
            new Vector3( 0, 1.0f, 0),
            new Vector3( 0, 1.0f, 0),
            new Vector3( 0, 1.0f, 0)
            
        };

        CubeMapFace[] CubeFaces = new CubeMapFace[]
        {
            CubeMapFace.PositiveY,
            CubeMapFace.NegativeY,
            CubeMapFace.PositiveX,
            CubeMapFace.NegativeX,
            CubeMapFace.PositiveZ,
            CubeMapFace.NegativeZ,
        };

        private Camera RenderCamera { get; set; }
        public RenderTarget2D RenderTarget { get; private set; }
        
        public RenderType RenderMode { get; private set; } = RenderType.ColorCubeMap;
        public RenderFrequency Frequency { get; set; } = RenderFrequency.PerFrame;
        public RenderPositionState PositionState { get; set; } = RenderPositionState.Static;
        public Color BackGroundColor { get; set; } = Color.Blue;

        public bool NeedsRender { get; private set; } = false;
        public bool HasRendered { get; private set; } = false;
        public int RenderTick = 33;
        public int LastRender = 0;
        public bool IsActive { get; private set; } = true;
        
        //public Texture2D[] CubeMap;
        public TextureCube CubeMap;
        //public Texture2D ColorTarget;
        //public Texture2D DepthTarget;
        
        public float ClipPlaneDirection;
        public float ClipOffset;
        
        Dictionary<string, Texture2D> ClipMapStorage = new Dictionary<string, Texture2D>();
        Dictionary<string, Vector3> ClipMapPositionStorage = new Dictionary<string, Vector3>();
        Dictionary<string, Vector4> ClipMapRangeStorage = new Dictionary<string, Vector4>();
        
        public Node3D SceneNode { get; private set; }
        public string ParameterName { get; set; }

        public string DebugTextureName { get; set; } = "";
        public string Name { get; private set; }

        public List<Geometry3D> GeometryUpdateList = new List<Geometry3D>();
        public List<Effect> OffScreenUpdateList = new List<Effect>();

        public SceneRenderTarget(string Name, string SName, Node3D Scene, GraphicsDevice Graphics, Vector3 CameraPosition, float Close, float Far, float PlaneDirection, int ResolutionX, int ResolutionY, RenderType TargetType)
        {
           
            ParameterName = SName;
            SceneNode = Scene;
            ClipPlaneDirection = PlaneDirection;
            
            RenderCamera = new Camera(CameraPosition, ResolutionX, ResolutionY, Far, Close, MathHelper.PiOver4);
            RenderMode = TargetType;
            
            GenerateTarget(Graphics, ResolutionX, ResolutionY);
        }

        public SceneRenderTarget( string SName, Node3D Scene, GraphicsDevice Graphics, Vector3 CameraPosition, float Close, float Far, float PlaneDirection, int ResolutionX, int ResolutionY, RenderType TargetType)
        {
            Name = SName;
            ParameterName = SName;
            SceneNode = Scene;
            ClipPlaneDirection = PlaneDirection;

            RenderMode = TargetType;
            
            RenderCamera = new Camera(CameraPosition, ResolutionX, ResolutionY, Far, Close, MathHelper.PiOver4);
            RenderCamera.SetResolution(ResolutionX, ResolutionY);
            RenderMode = TargetType;
            
            GenerateTarget(Graphics, ResolutionX, ResolutionY);
        }

        public SceneRenderTarget(string SName, Node3D Scene, GraphicsDevice Graphics, Vector3 CameraPosition, float Close, float Far, float PlaneDirection, int ResolutionX, int ResolutionY, RenderType TargetType, int FrustrumWidth, int FrustrumHeight)
        {
            Name = SName;
            ParameterName = SName;
            SceneNode = Scene;
            ClipPlaneDirection = PlaneDirection;

            RenderMode = TargetType;

            RenderCamera = new Camera(CameraPosition, FrustrumWidth, FrustrumHeight, Far, Close);
            RenderMode = TargetType;

            GenerateTarget(Graphics, ResolutionX, ResolutionY);
        }

        public void GenerateTarget(GraphicsDevice Graphics, int Width, int Height)
        {
            switch (RenderMode)
            {
                case RenderType.ColorCubeMap:
                    CubeMap = new TextureCube(Graphics, Width, false, SurfaceFormat.Color);
                    RenderTarget = new RenderTarget2D(Graphics, Width, Height, false, SurfaceFormat.Color, DepthFormat.Depth24);
                    break;
                case RenderType.SingleColor:
                    RenderTarget = new RenderTarget2D(Graphics, Width, Height, false, SurfaceFormat.Color, DepthFormat.Depth24);
                    break;
                case RenderType.SingleDepth:
                    RenderTarget = new RenderTarget2D(Graphics, Width, Height, false, SurfaceFormat.Single, DepthFormat.Depth24);
                    break;
            }
        }

        public void Dispose()
        {
            if (CubeMap != null)
            {
                CubeMap.Dispose();
                CubeMap = null;
            }
            if (RenderTarget != null)
            {
                RenderTarget.Dispose();
                RenderTarget = null;
            }
        }

        public void SetIsActive(bool Value)
        {
            IsActive = Value;
        }

        //TODO
        //Add LOD check
        public object[] GetClosestClipMap(Vector3 Position)
        {
            foreach (KeyValuePair<string, Texture2D> entry in ClipMapStorage)
            {
                Vector3 ClipMapPosition = ClipMapPositionStorage[entry.Key];
                Vector4 Range = ClipMapRangeStorage[entry.Key];

                if (Position.X >= ClipMapPosition.X && Position.Y >= ClipMapPosition.Y && Position.X <= ClipMapPosition.X + Range.X && Position.Y <= ClipMapPosition.Y + Range.Y)
                {
                    return new object[] { entry.Value, ClipMapPositionStorage[entry.Key], ClipMapRangeStorage[entry.Key] };
                }
            }
            return new object[] { };
        }

        
        public void AddClipMap(string ClipMapName, Texture2D ClipMap, Vector3 ClipMapPosition, Vector4 ClipMapRange)
        {
            if (!ClipMapStorage.ContainsKey(ClipMapName))
            {   
                ClipMapStorage.Add(ClipMapName, ClipMap);
                ClipMapPositionStorage.Add(ClipMapName, ClipMapPosition);
                ClipMapRangeStorage.Add(ClipMapName, ClipMapRange);
            }
        }
        
        public void SetClipPlaneMap(string ClipMapName, Texture2D NewMap)
        {
            if (ClipMapStorage.ContainsKey(ClipMapName))
            {
                Texture2D Map = ClipMapStorage[ClipMapName];
                
                ClipMapStorage.Remove(ClipMapName);
                Map.Dispose();
            }
            ClipMapStorage.Add(ClipMapName, NewMap);
        }
        
        public void SetClipMapPosition(string ClipMapName, Vector3 Position)
        {
            if (ClipMapPositionStorage.ContainsKey(ClipMapName))
            {
                ClipMapPositionStorage[ClipMapName] = Position;
            }
        }

        public void SetClipMapRange(string ClipMapName, Vector4 Range)
        {
            if (ClipMapRangeStorage.ContainsKey(ClipMapName))
            {
                ClipMapRangeStorage[ClipMapName] = Range;
            }
        }
        
        public void RenderScene(GraphicsDevice Graphics, Vector3 NewPosition)
        {
            if (!IsActive)
            {
                return;
            }
            
            HasRendered = false;
            Graphics.Clear(Color.CornflowerBlue);
            switch (RenderMode)
            {
                case RenderType.ColorCubeMap:
                    RenderCubeMap(Graphics, NewPosition);
                    UpdateGeometries(CubeMap);
                    break;
                case RenderType.SingleColor:
                    RenderSingleTarget(Graphics, NewPosition);
                    UpdateGeometries(RenderTarget);
                    break;
                case RenderType.SingleDepth:
                    RenderSingleTargetDepth(Graphics, NewPosition);
                    UpdateGeometries(RenderTarget);
                    break;
            }
        }
        
        public void UpdateGeometries(Texture2D SingleTarget)
        {
            for (int i = 0; i < GeometryUpdateList.Count; i++)
            {
                if (!GeometryUpdateList[i].HasCull)
                {
                    if (GeometryUpdateList[i].Shader != null)
                    {
                        if (GeometryUpdateList[i].Shader.Parameters[ParameterName] != null)
                        {
                            GeometryUpdateList[i].Shader.Parameters[ParameterName].SetValue(SingleTarget);
                        }
                    }
                }
            }

            for (int i = 0; i < OffScreenUpdateList.Count; i++)
            {
                if (OffScreenUpdateList[i].Parameters[ParameterName] != null)
                {
                    OffScreenUpdateList[i].Parameters[ParameterName].SetValue(SingleTarget);
                }
            }
        }

        public void UpdateGeometries(TextureCube CubeMap)
        {
            for (int i = 0; i < GeometryUpdateList.Count; i++)
            {
                if (!GeometryUpdateList[i].HasCull)
                {
                    if (GeometryUpdateList[i].Shader != null)
                    {
                        if (GeometryUpdateList[i].Shader.Parameters[ParameterName] != null)
                        {
                            GeometryUpdateList[i].Shader.Parameters[ParameterName].SetValue(CubeMap);
                        }
                    }
                }
            }
        }
        
        
        public void SetPositionDirection(Vector3 NewPosition, Vector3 Direction, Vector3 Axis)
        {
            SetPositionDirection(NewPosition, Direction, Axis, false);
        }

        public void SetPositionDirection( Vector3 NewPosition, Vector3 Direction, Vector3 Axis, bool IsOrthogonal)
        {
            RenderCamera.Position = NewPosition;
            if (IsOrthogonal)
            {
                RenderCamera.GenerateOrthogonalViewMatrix(RenderCamera.Position, RenderCamera.Position + Direction, Axis);
            }
            else
            {
                RenderCamera.GenerateLookAtViewMatrix(RenderCamera.Position, RenderCamera.Position + Direction, Axis);
            }
        }
        
        private void RenderSingleTargetDepth(GraphicsDevice Graphics, Vector3 Position)
        {
            Graphics.SetRenderTarget(RenderTarget);
            Graphics.DepthStencilState = DepthStencilState.Default;
            
            List<Geometry3D> RenderTargetGeometries = SceneNode.GetAllGeometries(Position);

            Matrix CameraProjection = RenderCamera.ProjectionMatrix;
            Matrix CameraView = RenderCamera.ViewMatrix;
            //Vector3 CameraPosition = RenderCamera.Position;
            
            Graphics.Clear(Color.White);
            
            for (int j = 0; j < RenderTargetGeometries.Count; j++)
            {
                object[] ClipMapData = GetClosestClipMap(RenderTargetGeometries[j].Position);
                if (ClipMapData.Length != 0)
                {
                    Geometry3D Geom = RenderTargetGeometries[j];
                    Geom.RenderDepth(Graphics, CameraView, CameraProjection, (Texture2D)ClipMapData[0], (Vector3)ClipMapData[1], (Vector4)ClipMapData[2], ClipPlaneDirection, ClipOffset, RenderCamera.FarPlaneClip, RenderCamera.IsOrthogonal);
                }
                else
                {
                    Geometry3D Geom = RenderTargetGeometries[j];
                    Geom.RenderDepth(Graphics, CameraView, CameraProjection, null, new Vector3(), new Vector4(), ClipPlaneDirection, ClipOffset, RenderCamera.FarPlaneClip, RenderCamera.IsOrthogonal);
                }
            }
            if (RenderCamera.IsOrthogonal)
            {
                //Debug.WriteLine(SceneNode.Count);
            }

            Graphics.SetRenderTarget(null);

            HasRendered = true;

        }
        
        private void RenderSingleTarget(GraphicsDevice Graphics, Vector3 Position)
        {
            Graphics.SetRenderTarget(RenderTarget);
            Graphics.DepthStencilState = DepthStencilState.Default;
            
            List<Geometry3D> RenderTargetGeometries = SceneNode.GetAllGeometries(Position);
            
            Matrix CameraProjection = RenderCamera.ProjectionMatrix;
            Matrix CameraView = RenderCamera.ViewMatrix;
            Graphics.Clear(Color.CornflowerBlue);
            
            for (int j = 0; j < RenderTargetGeometries.Count; j++)
            {
                object[] ClipMapData = GetClosestClipMap(RenderTargetGeometries[j].Position);

                if (ClipMapData.Length != 0)
                {
                    Geometry3D Geom = RenderTargetGeometries[j];
                    Geom.RenderFirstPassColor(Graphics, CameraView, CameraProjection, (Texture2D)ClipMapData[0], (Vector3)ClipMapData[1], (Vector4)ClipMapData[2], ClipPlaneDirection, ClipOffset, RenderCamera.FarPlaneClip);
                }
            }
            Graphics.SetRenderTarget(null);
            HasRendered = true;
        }

        private void RenderCubeMap(GraphicsDevice Graphics, Vector3 Position)
        {
            List<Geometry3D> RenderTargetGeometries = SceneNode.GetAllGeometries(Position);
            
            for (int i = 0; i < CubeDirections.Length; i++)
            {
                Graphics.SetRenderTarget( RenderTarget);
                Graphics.DepthStencilState = DepthStencilState.Default;
                
                RenderCamera.GenerateLookAtViewMatrix(RenderCamera.Position, RenderCamera.Position + CubeDirections[i], CubeUpVectors[i]);
                
                Matrix CameraProjection = RenderCamera.ProjectionMatrix;
                Matrix CameraView = RenderCamera.ViewMatrix;
                Graphics.Clear(Color.CornflowerBlue);

                for (int j = 0; j < RenderTargetGeometries.Count; j++)
                {
                    Geometry3D Geom = RenderTargetGeometries[j];
                    Effect Shader = Geom.Shader;

                    if (Geom.HasCull)
                    {
                        continue;
                    }

                    if (Shader.GetType() == typeof(BasicEffect))
                    {
                        BasicEffect CastShader = (BasicEffect)Shader;
                        CastShader.Projection = CameraProjection;
                        CastShader.View = CameraView;
                        CastShader.World = Matrix.CreateTranslation(Geom.Position);
                    }
                    else
                    {
                        Geom.Shader.Parameters["WorldViewProjection"].SetValue(CameraView * CameraProjection);
                    }
                    
                    foreach (var pass in Shader.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        Graphics.SetVertexBuffer(Geom.VertexBuffer);
                        Graphics.DrawPrimitives(PrimitiveType.TriangleList, 0, Geom.VertexBuffer.VertexCount / 3);
                    }
                }
                Graphics.SetRenderTarget(null);
                Color[] Data = new Color[RenderTarget.Width * RenderTarget.Height];
                RenderTarget.GetData(Data);
                
                CubeMap.SetData(CubeFaces[i], Data);

            }
            HasRendered = true;
            
        }

        public void AttachRange(List<Effect> AddList)
        {
            for (int i = 0; i < AddList.Count; i++)
            {
                if (OffScreenUpdateList.Contains(AddList[i]))
                {
                    OffScreenUpdateList.Add(AddList[i]);
                }
            }
        }

        public void Attach(Effect AttachShader)
        {
            if (!OffScreenUpdateList.Contains(AttachShader))
            {
                OffScreenUpdateList.Add(AttachShader);
            }
        }

        public void Detach(Effect RemoveShader)
        {
            if (OffScreenUpdateList.Contains(RemoveShader))
            {
                OffScreenUpdateList.Remove(RemoveShader);
            }
        }

        public void AttachRange(List<Geometry3D> AddList)
        {
            for (int i = 0; i < AddList.Count; i++)
            {
                if (GeometryUpdateList.Contains(AddList[i]))
                {
                    GeometryUpdateList.Add(AddList[i]);
                }
            }
        }
        
        public int GetWidth()
        {
            return RenderTarget.Width;
        }

        public int GetHeight()
        {
            return RenderTarget.Height;
        }

        public void Attach(Geometry3D Geometry)
        {
            if (!GeometryUpdateList.Contains(Geometry))
            {   
                GeometryUpdateList.Add(Geometry);
            }
        }
        
        public void Detach(Geometry3D Geometry)
        {
            if (GeometryUpdateList.Contains(Geometry))
            {
                GeometryUpdateList.Remove(Geometry);
            }
        }

        public void DetachChildNamed(string Name)
        {
            for (int i = 0; i < GeometryUpdateList.Count; i++)
            {
                if (GeometryUpdateList[i].Name == Name)
                {
                    GeometryUpdateList.RemoveAt(i);
                    return;
                }
            }
        }

        public Color[] FlipImage(Color[] Color, int Width)
        {
            Color[] NewColor = new Color[Color.Length];
            
            int Count = 0;
            
            for (int j = 0; j < Width; j++)
            {
                for (int i = Width-1; i >= 0; i--)
                {
                    int Index = ((j * Width) + i);
                    
                    NewColor[Index] = Color[Count++];
                }
            }
            return NewColor;
        }
        
        
    }


}
