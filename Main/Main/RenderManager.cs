using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Main.Main;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using Main.Geometry;
using Main.Util;
using System.Threading;
using static Main.Geometry.TextureEditor;

namespace Main.Main
{
    /// <summary>
    /// This class manages all rendering inside the Program.
    /// 
    /// 
    /// Rendering Layout:
    /// 
    /// Pre-Pass:
    ///     Process All OffScreen Render Targets
    ///     
    /// Scene-Pass:
    ///     (WIP)Render Normal Maps
    ///     Render Depth Map  
    ///     (WIP)Render Shadow Map
    ///     (WIP)Render Light Maps
    ///     Render Opaque Scene
    ///     
    /// Post Process-Pass
    ///     Render Other Passes in Shaders
    ///     
    /// UI-Pass
    ///     Render UI Elements
    /// 
    /// 
    /// </summary>


    public class RenderManager
    {
        public Node3D RootNode { get; set; }
        public Node2D GuiNode { get; set; }

        //contains nodes for different Shader Effects
        //Reflection, Refraction, Snow
        public Node3D RenderTargetNode { get; set; }

        public List<SceneRenderTarget> RenderTargets = new List<SceneRenderTarget>();
        public List<OffScreenTarget> OffScreenTargets = new List<OffScreenTarget>();

        //public Dictionary<string, Geometry3D> ActiveGeometries = new Dictionary<string, Geometry3D>();
        public Dictionary<string, Texture2D> Textures { get; set; } = new Dictionary<string, Texture2D>();
        public Dictionary<string, Effect> Shaders { get; private set; } = new Dictionary<string, Effect>();
        public Dictionary<string, GeometryModel> Models { get; private set; } = new Dictionary<string, GeometryModel>();

        TextureEditor TextureEditor;
        ContentManager Content;
        public GraphicsDevice Graphics { get; }

        string TextureLocation = "GFX";
        string ShaderLocation = "Shaders";
        string ModelLocation = "Models";
        string ModelMaterialLocation = "ModelMaterials";

        public Camera Camera { get; set; }

        RenderTarget2D ScreenDepthCapture;
        RenderTarget2D ScreenColorCapture;
        RenderTarget2D ScreenCapture;

        SpriteBatch Batch2D;

        int VirtualResolutionX = 1920;
        int VirtualResolutionY = 1920;

        float ResolutionX = 1920;
        float ResolutionY = 1080;
        float FOV = 90;
        int FarPlane = 2000;
        
        public RenderManager(ContentManager ContentManager, GraphicsDevice GraphicsDevice)
        {   
            Content = ContentManager;
            Graphics = GraphicsDevice;
            LoadTextures();
            LoadShaders();
            LoadModels();

            TextureEditor = new TextureEditor(Graphics,Shaders["TextureEditor"]);
            Batch2D = new SpriteBatch(Graphics);
            RootNode = new Node3D("RootNode");
            GuiNode = new Node2D("GuiNode");
            RenderTargetNode = new Node3D("RenderTargetNode");

            ResolutionX = (int)(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width * 0.75f);
            ResolutionY = (int)(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height * 0.75f);

            VirtualResolutionX = (int)ResolutionX;
            VirtualResolutionY = (int)ResolutionY;

            //ScreenCapture = new RenderTarget2D(Graphics, (int)ResolutionX, (int)ResolutionY, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

            ScreenDepthCapture = new RenderTarget2D(Graphics, GraphicsDevice.PresentationParameters.BackBufferWidth,
                 GraphicsDevice.PresentationParameters.BackBufferHeight, false, SurfaceFormat.Single, DepthFormat.Depth24);

            ScreenColorCapture = new RenderTarget2D(GraphicsDevice, GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24, 0, RenderTargetUsage.PreserveContents);

            ScreenCapture = new RenderTarget2D(GraphicsDevice, GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24, 0, RenderTargetUsage.PreserveContents);

            FOV = MathHelper.PiOver4;
        }

        public void AddTextureModificationTask(TextureTask Task)
        {
            TextureEditor.AddTextureTask(Task);
        }

        public void UpdateCamera(Camera SetCamera)
        {
            SetCamera.Fov = FOV;
            SetCamera.SetResolution(ResolutionX, ResolutionY);
        }

        public void LoadModels()
        {
            DirectoryInfo Folder = new DirectoryInfo(ModelLocation);
            if (Folder.Exists)
            {
                FileInfo[] ConfigFiles = Folder.GetFiles();

                foreach (FileInfo CurrentFile in ConfigFiles)
                {
                    if (CurrentFile.Extension.Equals(".obj"))
                    {
                        string Name = CurrentFile.Name.Remove(CurrentFile.Name.Length - 4, 4);
                        GeometryModel NewModel = ModelUtil.ImportOBJModel(CurrentFile.FullName);
                        
                        string[] ModelMaterialData = GetModelMaterialData(Name);
                        
                        if(ModelMaterialData.Length != 0)
                        {
                            for (int i = 0; i < ModelMaterialData.Length; i++)
                            {
                                if (ModelMaterialData[i].Length == 0 || ModelMaterialData[i].StartsWith("//") || ModelMaterialData[i].StartsWith("#"))
                                {
                                    continue;
                                }
                                string Key = ModelMaterialData[i].Split(new char[] {':'})[0].Trim();
                                string Value = ModelMaterialData[i].Split(new char[] {':'})[1].Trim();
                                
                                if (Key.ToLower().StartsWith("shader"))
                                {
                                    if (Shaders.ContainsKey(Value))
                                    {
                                        NewModel.Shader = Shaders[Value].Clone();
                                    }
                                 }
                                if (Key.ToLower().StartsWith("depthshader"))
                                {
                                    if (Shaders.ContainsKey(Value))
                                    {
                                        NewModel.DepthShader = Shaders[Value].Clone();
                                    }
                                }
                                else
                                {
                                    if (Textures.ContainsKey(Value))
                                    {
                                        NewModel.AddTexture(Key, Value);
                                    }
                                }

                            }
                            
                            
                            if (NewModel.Shader == null || NewModel.DepthShader == null)
                            {
                                continue;
                            }
                            //Debug.WriteLine(Name);
                            NewModel.Name = Name;
                            Models.Add(Name, NewModel);
                        }
                    }
                }
            }
        }
        
        private string[] GetModelMaterialData(String FileName)
        {
            DirectoryInfo Folder = new DirectoryInfo(ModelMaterialLocation);
            if (Folder.Exists)
            {
                FileInfo[] ConfigFiles = Folder.GetFiles();
                foreach (FileInfo CurrentFile in ConfigFiles)
                {
                    if (CurrentFile.Extension.Equals(".mmt"))
                    {
                        string Name = CurrentFile.Name.Remove(CurrentFile.Name.Length - 4, 4);
                        if (Name == FileName)
                        {
                            return File.ReadAllLines(CurrentFile.FullName);
                        }
                    }
                }
            }
            return new string[0];
        }

        public void LoadShaders()
        {
            BasicEffect BasicEffect = new BasicEffect(Graphics);
            Shaders.Add("Default", BasicEffect);

            DirectoryInfo Folder = new DirectoryInfo(ShaderLocation);

            if (Folder.Exists)
            {
                FileInfo[] ConfigFiles = Folder.GetFiles();

                foreach (FileInfo CurrentFile in ConfigFiles)
                {
                    if (CurrentFile.Extension.Equals(".mgfxo"))
                    {
                        string Name = CurrentFile.Name.Remove(CurrentFile.Name.Length - 6, 6);
                        
                        Effect Test = new Effect(Graphics, File.ReadAllBytes(CurrentFile.FullName.ToString()));
                        Test.Name = Name;
                       
                        Shaders.Add(Name, Test);
                        
                    }
                }
            }
        }

        public void LoadTextures()
        {
            DirectoryInfo Folder = new DirectoryInfo(TextureLocation);

            if (Folder.Exists)
            {
                FileInfo[] ConfigFiles = Folder.GetFiles();

                foreach (FileInfo File in ConfigFiles)
                {
                    if (File.Extension.Equals(".png"))
                    {
                        FileStream FileStream = new FileStream(File.FullName.ToString(), FileMode.Open);
                        Textures.Add(File.Name.Remove(File.Name.Length - 4, 4), Texture2D.FromStream(Graphics, FileStream));
                        FileStream.Close();
                        FileStream.Dispose();
                        //call garbage collection after loading image
                        System.GC.Collect();
                        System.GC.WaitForPendingFinalizers();
                    }
                }
            }
        }

        public void AddRenderTarget(SceneRenderTarget Target)
        {
            if (!RenderTargets.Contains(Target))
            {
                RenderTargets.Add(Target);
            }
        }

        public void DetachRenderTarget(SceneRenderTarget Target)
        {
            if (RenderTargets.Contains(Target))
            {
                RenderTargets.Remove(Target);
            }
        }
       
        public void AddOffScreenTarget(OffScreenTarget Target)
        {
            if (!OffScreenTargets.Contains(Target))
            {
                OffScreenTargets.Add(Target);
            }
        }

        public void DetachOffScreenTarget(OffScreenTarget Target)
        {
            if (OffScreenTargets.Contains(Target))
            {
                OffScreenTargets.Remove(Target);
            }
        }

        public int GetScreenWidth()
        {
            return Graphics.Viewport.Width;
        }

        public int GetScreenHeight()
        {
            return Graphics.Viewport.Height;
        }
        
        public int GetFarPlane()
        {
            return FarPlane;
        }

        //Unlocks Image After Refresh
        public void RefreshImage(string TextureName, LockBitMap Image)
        {
            if (!Image.IsLocked)
            {
                Image.LockBits();
            }

            if (Textures.ContainsKey(TextureName))
            {
                Textures[TextureName].SetData(Image.Pixels);
            }
            else
            {
                Texture2D NewTexture = new Texture2D(Graphics, Image.Width, Image.Height);
                NewTexture.SetData(Image.Pixels);
                Textures.Add(TextureName, NewTexture);
            }
            Image.UnlockBits();
        }

        public void RefreshImage(string FileName, string TextureName)
        {
            if (Textures.ContainsKey(TextureName))
            {
                FileStream FileStream = new FileStream(FileName, FileMode.Open);
                Textures[TextureName] = Texture2D.FromStream(Graphics, FileStream);
                FileStream.Close();
                FileStream.Dispose();
            }
        }

        public void LoadTexturesFromFile(string FolderName, string Prefix)
        {
            DirectoryInfo folder = new DirectoryInfo(FolderName);

            if (folder.Exists)
            {
                FileInfo[] ConfigFiles = folder.GetFiles();

                foreach (FileInfo File in ConfigFiles)
                {
                    if (File.Extension.Equals(".png"))
                    {
                        FileStream FileStream = new FileStream(File.FullName.ToString(), FileMode.Open);

                        if (Textures.ContainsKey(Prefix + File.Name.Remove(File.Name.Length - 4, 4)))
                        {
                            Textures[Prefix + File.Name.Remove(File.Name.Length - 4, 4)] = Texture2D.FromStream(Graphics, FileStream);
                        }
                        else
                        {
                            Textures.Add(Prefix + File.Name.Remove(File.Name.Length - 4, 4), Texture2D.FromStream(Graphics, FileStream));
                        }

                        FileStream.Close();
                    }
                }
            }
        }

        public void Update(GameTime GameTime)
        {
            TextureEditor.ProcessTasks(Graphics, 0);
            UpdateRenderTargets();
            TextureEditor.ProcessTasks(Graphics, 1);
            ProcessOffScreenTargets();
            TextureEditor.ProcessTasks(Graphics, 2);
            RenderSceneDepth();
            RenderMainSceneColor(GameTime);
            TextureEditor.ProcessTasks(Graphics, 3);
            RenderMainScene(GameTime);
            TextureEditor.ProcessTasks(Graphics, 4);

            RenderUI();
        }
        
        public void UpdateRenderTargets()
        {
            if (Camera == null)
            {
                return;
            }

            for (int i = 0; i < RenderTargets.Count; i++)
            {
                if (RenderTargets[i].IsActive)
                {
                    RenderTargets[i].RenderScene(Graphics, Camera.Position);
                    
                    if (Textures.ContainsKey(RenderTargets[i].DebugTextureName))
                    {
                        //Textures[RenderTargets[i].DebugTextureName] = GraphicsUtil.CloneTexture(Graphics, RenderTargets[i].RenderTarget);
                        Textures[RenderTargets[i].DebugTextureName] = RenderTargets[i].RenderTarget;
                    }
                }
            }
        }

        public void ProcessOffScreenTargets()
        {
            for (int i = 0; i < OffScreenTargets.Count; i++)
            {
                if (OffScreenTargets[i].IsActive)
                {
                    OffScreenTargets[i].Process(Graphics,this);
                    if (Textures.ContainsKey(OffScreenTargets[i].DebugTextureName))
                    {
                        //Textures[RenderTargets[i].DebugTextureName] = GraphicsUtil.CloneTexture(Graphics, RenderTargets[i].RenderTarget);
                        if (OffScreenTargets[i].HasPing)
                        {
                            Textures[OffScreenTargets[i].DebugTextureName] = OffScreenTargets[i].TargetPing;
                        }
                        else
                        {
                            Textures[OffScreenTargets[i].DebugTextureName] = OffScreenTargets[i].TargetPong;
                        }
                    }
                }
            }
        }

        public void UpdateRenderTarget(SceneRenderTarget RenderTarget, GameTime GameTime)
        {
            if (Camera == null)
            {
                return;
            }
            RenderTarget.RenderScene(Graphics, Camera.Position);

            Graphics.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);
        }

        public void RenderSceneDepth()
        {
            if (Camera != null)
            {
                UpdateCamera(Camera);

                Graphics.SetRenderTarget(ScreenDepthCapture);
                Graphics.DepthStencilState = DepthStencilState.Default;

                List<Geometry3D> RootNodeGeometries = RootNode.GetAllGeometries(Camera.Position);

                Matrix CameraProjection = Camera.ProjectionMatrix;
                Matrix CameraView = Camera.ViewMatrix;
                Vector3 CameraPosition = Camera.Position;

                Graphics.Clear(Color.CornflowerBlue);

                for (int j = 0; j < RootNodeGeometries.Count; j++)
                {
                    Geometry3D Geom = RootNodeGeometries[j];
                    Geom.RenderDepth(Graphics, CameraView, CameraProjection, null, new Vector3(), new Vector4(), 0, 0, Camera.FarPlaneClip, Camera.IsOrthogonal);
                }
                Graphics.SetRenderTarget(null);
            }
        }

        public void RenderMainSceneColor(GameTime GameTime)
        {
            if (Camera != null)
            {

                UpdateCamera(Camera);

                Matrix CameraProjection = Camera.ProjectionMatrix;
                Matrix CameraView = Camera.ViewMatrix;
                Vector3 Position = Camera.Position;

                List<Geometry3D> RootNodeGeometries = RootNode.GetAllGeometries(Position);
               
                Graphics.SetRenderTarget(ScreenColorCapture);
                Graphics.DepthStencilState = DepthStencilState.Default;
                Graphics.Clear(Color.CornflowerBlue);

                for (int i = 0; i < RootNodeGeometries.Count; i++)
                {
                    
                    Geometry3D Geom = RootNodeGeometries[i];
                    Effect Shader = Geom.Shader;
                    
                    if (Geom.HasTextureUpdate)
                    {
                        List<string> TextureNames = Geom.TextureNames;
                        
                        if (Shader.GetType() == typeof(BasicEffect))
                        {
                            BasicEffect CastShader = (BasicEffect)Shader;
                            if (TextureNames.Count > 0)
                            {
                                CastShader.Parameters["Texture"].SetValue(Textures[TextureNames[0]]);
                            }
                        }
                        else
                        {
                            for (int j = 0; j < TextureNames.Count; j++)
                            {
                                Shader.Parameters["Texture" + j].SetValue(Textures[TextureNames[j]]);
                            }
                        }
                        Geom.HasTextureUpdate = false;
                    }
                    Geom.RenderFirstPassColor(Graphics, CameraView, CameraProjection, null, new Vector3(), new Vector4(), 0, 0, 0);
                }
                Graphics.SetRenderTarget(null);

            }

        }

        public void RenderPostProcess()
        {

        }

        //TODO Add Support for:
        //Normal Map
        //Scene Depth Map
        //Scene Shadow Map
        //
        public void RenderMainScene(GameTime GameTime)
        {
            if (Camera != null)
            {
                UpdateCamera(Camera);

                Matrix ProjectionMatrix = Camera.ProjectionMatrix;
                Matrix ViewMatrix = Camera.ViewMatrix;
                Vector3 Position = Camera.Position;

                List<Geometry3D> RootNodeGeometries = RootNode.GetAllGeometries(Position);
                //Debug.WriteLine(RootNodeGeometries.Count);

                Graphics.SetRenderTarget(ScreenCapture);
                Graphics.DepthStencilState = DepthStencilState.Default;
                Graphics.Clear(Color.TransparentBlack);
                
                //Render Post Processing
                for (int i = 0; i < RootNodeGeometries.Count; i++)
                {
                    Geometry3D Geom = RootNodeGeometries[i];
                    Effect Shader = Geom.Shader;
                    Geom.RenderPostProcessColor(Graphics,ViewMatrix,ProjectionMatrix,ScreenDepthCapture,ScreenColorCapture,Camera.FarPlaneClip);
                }
                Graphics.SetRenderTarget(null);

                //Textures["SceneColorDebug"] = ScreenColorCapture;
                //Textures["SceneDepthDebug"] = ScreenDepthCapture;
                
                Batch2D.Begin();
                Batch2D.Draw(ScreenColorCapture, new Rectangle(0, 0, VirtualResolutionX, VirtualResolutionY), Color.White);
                Batch2D.End();
                
                Batch2D.Begin();
                Batch2D.Draw(ScreenCapture, new Rectangle(0, 0, VirtualResolutionX, VirtualResolutionY), Color.White);
                Batch2D.End();
            }
        }

        public void RenderUI()
        {
            List<Geometry2D> GuiGeometries = GuiNode.GetAllGeometries();
            GuiGeometries = GuiGeometries.OrderBy(x => x.Z).ToList();

            //GuiGeometries = GuiGeometries.OrderBy(x => x.Shader == null).ThenBy(x => x.Z).ToList();
            for (int i = 0; i < GuiGeometries.Count; i++)
            {
                if (GuiGeometries[i].Shader == null)
                {
                    Batch2D.Begin();
                    //Debug.WriteLine(GuiGeometries[i].TextureName);
                    Texture2D Texture = Textures[GuiGeometries[i].TextureName];
                    
                    Microsoft.Xna.Framework.Rectangle TextureBounds = new Microsoft.Xna.Framework.Rectangle((int)(GuiGeometries[i].TX * Texture.Width), (int)(GuiGeometries[i].TY * Texture.Height), (int)(GuiGeometries[i].TSX * Texture.Width), (int)(GuiGeometries[i].TSY * Texture.Height));
                    Microsoft.Xna.Framework.Rectangle Bounds = new Microsoft.Xna.Framework.Rectangle((int)GuiGeometries[i].PX, (int)GuiGeometries[i].PY, (int)GuiGeometries[i].SX, (int)GuiGeometries[i].SY);

                    Batch2D.Draw(Texture, Bounds, TextureBounds, Microsoft.Xna.Framework.Color.White);
                    Batch2D.End();
                }
                else
                {

                    Geometry2D Geom = GuiGeometries[i];
                    Effect Shader = GuiGeometries[i].Shader;
                    Texture2D Texture = Textures["StandardMenu"];

                    Microsoft.Xna.Framework.Rectangle Bounds = new Microsoft.Xna.Framework.Rectangle((int)GuiGeometries[i].PX, (int)GuiGeometries[i].PY, (int)GuiGeometries[i].SX, (int)GuiGeometries[i].SY);

                    Batch2D.Begin(0, BlendState.Opaque, null, null, null, Shader);

                    for (int j = 0; j < Shader.CurrentTechnique.Passes.Count; j++)
                    {
                        Shader.CurrentTechnique.Passes[j].Apply();
                        Batch2D.Draw(Texture, Bounds, Color.Transparent);

                    }
                    Batch2D.End();
                }
            }
        }

    }




}
