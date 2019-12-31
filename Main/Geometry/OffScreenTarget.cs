using Main.Main;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Main.Geometry
{
    public enum TargetSwitchType
    {
        Ping, Pong, PingPongIn, PingPongOut, PingPongInOut
    }
    
    public class OffScreenTarget
    { 
        Dictionary<Geometry3D, string> OutputGeometries3DPing = new Dictionary<Geometry3D, string>();
        Dictionary<Geometry2D, string> OutputGeometries2DPing = new Dictionary<Geometry2D, string>();
        Dictionary<Effect, string> OutputShadersPing = new Dictionary<Effect, string>();

        Dictionary<Geometry3D, string> OutputGeometries3DPong = new Dictionary<Geometry3D, string>();
        Dictionary<Geometry2D, string> OutputGeometries2DPong = new Dictionary<Geometry2D, string>();
        Dictionary<Effect, string> OutputShadersPong = new Dictionary<Effect, string>();

        public Dictionary<string, Texture2D> TextureParametersPing { get; private set; } = new Dictionary<string, Texture2D>();
        public Dictionary<string, float> FloatParametersPing { get; private set; } = new Dictionary<string, float>();
        public Dictionary<string, Vector4> Vector4ParametersPing { get; private set; } = new Dictionary<string, Vector4>();

        public Dictionary<string, Texture2D> TextureParametersPong { get; private set; } = new Dictionary<string, Texture2D>();
        public Dictionary<string, float> FloatParametersPong { get; private set; } = new Dictionary<string, float>();
        public Dictionary<string, Vector4> Vector4ParametersPong { get; private set; } = new Dictionary<string, Vector4>();

        public RenderTarget2D TargetPing { get; private set; }
        public RenderTarget2D TargetPong { get; private set; }
        
        public string DebugTextureName { get; set; } = "";

        public TargetSwitchType TargetSwitchType = TargetSwitchType.Ping;
        
        public Effect Shader { get; private set; }
        SpriteBatch Batch2D;

        public bool IsActive { get; private set; } = true;
        public bool HasPing { get; private set; } = true;

        public int Width { get; private set; }
        public int Height { get; private set; }

        public string Name { get; private set; } = "";
        
        public OffScreenTarget(String TargetName, GraphicsDevice Graphics, Effect TargetShader)
        {
            Name = TargetName;
            Shader = TargetShader;
            Batch2D = new SpriteBatch(Graphics);
        }

        public OffScreenTarget(String TargetName, GraphicsDevice Graphics, Effect TargetShader, TargetSwitchType SwitchType)
        {
            Name = TargetName;
            Shader = TargetShader;
            Batch2D = new SpriteBatch(Graphics);
            TargetSwitchType = SwitchType;
        }
        
        public void Process(GraphicsDevice Graphics,RenderManager Render)
        {
            SetShaderParameters(Shader);
           
            if (HasPing)
            {
                Graphics.SetRenderTarget(TargetPing);
            }
            else
            {
                Graphics.SetRenderTarget(TargetPong);
            }
            Microsoft.Xna.Framework.Rectangle Bounds = new Microsoft.Xna.Framework.Rectangle(0, 0, TargetPing.Width, TargetPing.Height);
            Microsoft.Xna.Framework.Rectangle Bounds2 = new Microsoft.Xna.Framework.Rectangle(0, 0, 0,0);

            Texture2D Texture = Render.Textures["StandardMenu"];
            Batch2D.Begin(0, BlendState.Opaque, null, null, null, Shader);

            for (int j = 0; j < Shader.CurrentTechnique.Passes.Count; j++)
            {
                Shader.CurrentTechnique.Passes[j].Apply();
                Batch2D.Draw(Texture, Bounds, Bounds2, Color.Red);
            }
            Batch2D.End();
            
            Graphics.SetRenderTarget(null);
            
            UpdateGeometries();
        }

        public void SetRenderTargets(RenderTarget2D NewPing, RenderTarget2D NewPong)
        {
            TargetPing = NewPing;
            TargetPong = NewPong;
        }
        
        public void SetTargetPing(RenderTarget2D NewPing)
        {
            TargetPing = NewPing;
        }

        public void SetTargetPong(RenderTarget2D NewPong)
        {
            TargetPong = NewPong;
        }

        public void AddParameter(string ParameterName, float Value)
        {
            AddParameter(ParameterName, Value, true);
        }

        public void AddParameter(string ParameterName, Texture2D Value)
        {
            AddParameter(ParameterName, Value, true);
        }
        public void AddParameter(string ParameterName, Vector4 Value)
        {
            AddParameter(ParameterName, Value, true);
        }

        public void AddParameter(string ParameterName, float Value, bool IsPing)
        {
            if (IsPing)
            {
                if (FloatParametersPing.ContainsKey(ParameterName))
                {
                    FloatParametersPing.Remove(ParameterName);
                }
                FloatParametersPing.Add(ParameterName, Value);
            }
            else
            {
                if (FloatParametersPong.ContainsKey(ParameterName))
                {
                    FloatParametersPong.Remove(ParameterName);
                }
                FloatParametersPong.Add(ParameterName, Value);
            }
        }

        public void AddParameter(string ParameterName, Vector4 Value, bool IsPing)
        {
            if (IsPing)
            {
                if (Vector4ParametersPing.ContainsKey(ParameterName))
                {
                    Vector4ParametersPing.Remove(ParameterName);
                }
                Vector4ParametersPing.Add(ParameterName, Value);
            }
            else
            {
                if (Vector4ParametersPong.ContainsKey(ParameterName))
                {
                    Vector4ParametersPong.Remove(ParameterName);
                }
                Vector4ParametersPong.Add(ParameterName, Value);
            }
        }

        public void AddParameter(string ParameterName, Texture2D Value, bool IsPing)
        {
            if (IsPing)
            {
                if (TextureParametersPing.ContainsKey(ParameterName))
                {
                    TextureParametersPing.Remove(ParameterName);
                }
                TextureParametersPing.Add(ParameterName, Value);
            }
            else
            {
                if (TextureParametersPong.ContainsKey(ParameterName))
                {
                    TextureParametersPong.Remove(ParameterName);
                }
                TextureParametersPong.Add(ParameterName, Value);
            }
        }

        public void AttachOutPutRange(Dictionary<Effect, string> AddList)
        {
            AttachOutPutRange(AddList, true);
        }

        public void AttachOutPutRange(Dictionary<Geometry2D, string> AddList)
        {
            AttachOutPutRange(AddList, true);
        }
        public void AttachOutPutRange(Dictionary<Geometry3D, string> AddList)
        {
            AttachOutPutRange(AddList, true);
        }

        public void AttachOutPut(string ShaderParameterName, Effect AttachShader)
        {
            AttachOutPut(ShaderParameterName, AttachShader, true);
        }

        public void DetachOutPut(string ShaderParameterName, Geometry3D DetachGeometry)
        {
            DetachOutPut(DetachGeometry, true);
        }

        public void DetachOutPut(string ShaderParameterName, Geometry2D DetachGeometry)
        {
            DetachOutPut(DetachGeometry, true);
        }

        public void DetachOutPut(string ShaderParameterName, Effect DetachShader)
        {
            DetachOutPut(DetachShader, true);
        }

        public void AttachOutPutRange(Dictionary<Effect, string> AddList, bool IsPing)
        {
            if (IsPing)
            {
                foreach (KeyValuePair<Effect, string> Entry in AddList)
                {
                    if (!OutputShadersPing.ContainsKey(Entry.Key))
                    {
                        OutputShadersPing.Add(Entry.Key, Entry.Value);
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<Effect, string> Entry in AddList)
                {
                    if (!OutputShadersPong.ContainsKey(Entry.Key))
                    {
                        OutputShadersPong.Add(Entry.Key, Entry.Value);
                    }
                }
            }
        }

        public void AttachOutPutRange(Dictionary<Geometry3D, string> AddList, bool IsPing)
        {
            if (IsPing)
            {
                foreach (KeyValuePair<Geometry3D, string> Entry in AddList)
                {
                    if (!OutputGeometries3DPing.ContainsKey(Entry.Key))
                    {
                        OutputGeometries3DPing.Add(Entry.Key, Entry.Value);
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<Geometry3D, string> Entry in AddList)
                {
                    if (!OutputGeometries3DPing.ContainsKey(Entry.Key))
                    {
                        OutputGeometries3DPing.Add(Entry.Key, Entry.Value);
                    }
                }
            }
        }

        public void AttachOutPutRange(Dictionary<Geometry2D, string> AddList, bool IsPing)
        {
            if (IsPing)
            {
                foreach (KeyValuePair<Geometry2D, string> Entry in AddList)
                {
                    if (!OutputGeometries2DPing.ContainsKey(Entry.Key))
                    {
                        OutputGeometries2DPing.Add(Entry.Key, Entry.Value);
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<Geometry2D, string> Entry in AddList)
                {
                    if (!OutputGeometries2DPong.ContainsKey(Entry.Key))
                    {
                        OutputGeometries2DPong.Add(Entry.Key, Entry.Value);
                    }
                }
            }
        }

        public void AttachOutPut(string ShaderParameterName, Effect AttachShader, bool IsPing)
        {
            if (IsPing)
            {
                if (!OutputShadersPing.ContainsKey(AttachShader))
                {
                    OutputShadersPing.Add(AttachShader, ShaderParameterName);
                }
            }
            else
            {
                if (!OutputShadersPong.ContainsKey(AttachShader))
                {
                    OutputShadersPong.Add(AttachShader, ShaderParameterName);
                }
            }
        }

        public void AttachOutPut(string ShaderParameterName, Geometry3D Geometry, bool IsPing)
        {
            if (IsPing)
            {
                if (!OutputGeometries3DPing.ContainsKey(Geometry))
                {
                    OutputGeometries3DPing.Add(Geometry, ShaderParameterName);
                }
            }
            else
            {
                if (!OutputGeometries3DPong.ContainsKey(Geometry))
                {
                    OutputGeometries3DPong.Add(Geometry, ShaderParameterName);
                }
            }
        }

        public void AttachOutPut(string ShaderParameterName, Geometry2D Geometry, bool IsPing)
        {
            if (IsPing)
            {
                if (!OutputGeometries2DPing.ContainsKey(Geometry))
                {
                    OutputGeometries2DPing.Add(Geometry, ShaderParameterName);
                }
            }
            else
            {
                if (!OutputGeometries2DPong.ContainsKey(Geometry))
                {
                    OutputGeometries2DPong.Add(Geometry, ShaderParameterName);
                }
            }
        }

        public void DetachOutPut(Effect RemoveShader, bool IsPing)
        {
            if (IsPing)
            {
                if (OutputShadersPing.ContainsKey(RemoveShader))
                {
                    OutputShadersPing.Remove(RemoveShader);
                }
            }
            else
            {
                if (OutputShadersPong.ContainsKey(RemoveShader))
                {
                    OutputShadersPong.Remove(RemoveShader);
                }
            }
        }

        public void DetachOutPut(Geometry3D Geometry, bool IsPing)
        {
            if (IsPing)
            {
                if (OutputGeometries3DPing.ContainsKey(Geometry))
                {
                    OutputGeometries3DPing.Remove(Geometry);
                }
            }
            else
            {
                if (OutputGeometries3DPong.ContainsKey(Geometry))
                {
                    OutputGeometries3DPong.Remove(Geometry);
                }
            }
        }

        public void DetachOutPut(Geometry2D Geometry, bool IsPing)
        {
            if (IsPing)
            {
                if (OutputGeometries2DPing.ContainsKey(Geometry))
                {
                    OutputGeometries2DPing.Remove(Geometry);
                }
            }
            else
            {
                if (OutputGeometries2DPong.ContainsKey(Geometry))
                {
                    OutputGeometries2DPong.Remove(Geometry);
                }
            }
        }
        
        public void SetShaderParameters(Effect Shader)
        {
            if (TargetSwitchType == TargetSwitchType.Ping)
            {
                foreach (KeyValuePair<string, float> Parameter in FloatParametersPing)
                {
                    if (Shader.Parameters[Parameter.Key] != null)
                    {
                        Shader.Parameters[Parameter.Key].SetValue(Parameter.Value);
                    }
                }
                
                foreach (KeyValuePair<string, Texture2D> Parameter in TextureParametersPing)
                {
                    if (Shader.Parameters[Parameter.Key] != null)
                    {
                        Shader.Parameters[Parameter.Key].SetValue(Parameter.Value);
                    }
                }

                foreach (KeyValuePair<string, Vector4> Parameter in Vector4ParametersPing)
                {
                    if (Shader.Parameters[Parameter.Key] != null)
                    {
                        Shader.Parameters[Parameter.Key].SetValue(Parameter.Value);
                    }
                }
            }
            else
            {
                if (HasPing)
                {
                    foreach (KeyValuePair<string, float> Parameter in FloatParametersPing)
                    {
                        if (Shader.Parameters[Parameter.Key] != null)
                        {
                            Shader.Parameters[Parameter.Key].SetValue(Parameter.Value);
                        }
                    }

                    foreach (KeyValuePair<string, Texture2D> Parameter in TextureParametersPing)
                    {
                        if (Shader.Parameters[Parameter.Key] != null)
                        {
                           
                            Shader.Parameters[Parameter.Key].SetValue(Parameter.Value);
                        }
                    }

                    foreach (KeyValuePair<string, Vector4> Parameter in Vector4ParametersPing)
                    {
                        if (Shader.Parameters[Parameter.Key] != null)
                        {
                            Shader.Parameters[Parameter.Key].SetValue(Parameter.Value);
                        }
                    }
                }
                else
                {
                    foreach (KeyValuePair<string, float> Parameter in FloatParametersPong)
                    {
                        if (Shader.Parameters[Parameter.Key] != null)
                        {
                            Shader.Parameters[Parameter.Key].SetValue(Parameter.Value);
                        }
                    }

                    foreach (KeyValuePair<string, Texture2D> Parameter in TextureParametersPong)
                    {
                        if (Shader.Parameters[Parameter.Key] != null)
                        {
                            Shader.Parameters[Parameter.Key].SetValue(Parameter.Value);
                        }
                    }

                    foreach (KeyValuePair<string, Vector4> Parameter in Vector4ParametersPong)
                    {
                        if (Shader.Parameters[Parameter.Key] != null)
                        {
                            Shader.Parameters[Parameter.Key].SetValue(Parameter.Value);
                        }
                    }
                }
            }
        }
        public void UpdateGeometries()
        {
            if (HasPing)
            {
                foreach (KeyValuePair<Geometry2D, string> Entry in OutputGeometries2DPing)
                {
                    if (!Entry.Key.HasCull)
                    {
                        Entry.Key.Shader.Parameters[Entry.Value].SetValue(TargetPing);
                    }
                }
                foreach (KeyValuePair<Geometry3D, string> Entry in OutputGeometries3DPing)
                {
                    if (!Entry.Key.HasCull)
                    {
                        Entry.Key.Shader.Parameters[Entry.Value].SetValue(TargetPing);
                    }
                }

                foreach (KeyValuePair<Effect, string> Entry in OutputShadersPing)
                {
                    if (Entry.Key != null)
                    {
                        Entry.Key.Parameters[Entry.Value].SetValue(TargetPing);
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<Geometry2D, string> Entry in OutputGeometries2DPong)
                {
                    if (!Entry.Key.HasCull)
                    {
                        Entry.Key.Shader.Parameters[Entry.Value].SetValue(TargetPong);
                    }
                }
                foreach (KeyValuePair<Geometry3D, string> Entry in OutputGeometries3DPong)
                {
                    if (!Entry.Key.HasCull)
                    {
                        Entry.Key.Shader.Parameters[Entry.Value].SetValue(TargetPong);
                    }
                }

                foreach (KeyValuePair<Effect, string> Entry in OutputShadersPong)
                {
                    if (Entry.Key != null)
                    {
                        Entry.Key.Parameters[Entry.Value].SetValue(TargetPong);
                    }
                }
            }
        }

    }
}
