using Main.Main;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
/**
 * 
 * 
 * TODO Add constructors for common Texture Task Types
 * 
 **/
namespace Main.Geometry
{
    public enum TaskUsage
    {
        Preserve, FrameLimit, TimeLimit
    }

    public enum TaskSwitchType
    {
        Ping ,Pong, PingPongIn, PingPongOut, PingPongInOut
    }

    public enum TaskStage
    {
        PreRenderTarget = 0, PreOffScreenTarget = 1, PreScene = 2, PostProcess = 3, PreUI = 4
    }

    public enum TaskType
    {
        FillColor, FillFloat, CopyFloatTexture, CopyColorTexture, MergeMinFloat, MergeMaxFloat, GenerateNormals
    }

    public enum FloatParameter
    {
        ColorFloat, LerpValue, TextureWidth0, TextureWidth1, TextureHeight0, TextureHeight1
    }

    public enum TextureParameter
    {
        InputTexture, InputTexture2
    }

    public enum Vector4Parameter
    {
        Color
    }

    public class TextureTask
    {
        public Dictionary<Geometry3D, string> OutputGeometries3DPing { get; set; } = new Dictionary<Geometry3D, string>();
        public Dictionary<Geometry2D, string> OutputGeometries2DPing { get; set; } = new Dictionary<Geometry2D, string>();
        public Dictionary<Effect, string> OutputShadersPing { get; set; } = new Dictionary<Effect, string>();

        public Dictionary<Geometry3D, string> OutputGeometries3DPong { get; set; } = new Dictionary<Geometry3D, string>();
        public Dictionary<Geometry2D, string> OutputGeometries2DPong { get; set; } = new Dictionary<Geometry2D, string>();
        public Dictionary<Effect, string> OutputShadersPong { get; set; } = new Dictionary<Effect, string>();
        
        public RenderTarget2D TargetPing { get; set; }
        public RenderTarget2D TargetPong { get; set; }

        public Dictionary<TextureParameter, Texture2D> TextureParametersPing { get; set; } = new Dictionary<TextureParameter, Texture2D>();
        public Dictionary<FloatParameter, float> FloatParametersPing { get; set; } = new Dictionary<FloatParameter, float>();
        public Dictionary<Vector4Parameter, Vector4> Vector4ParametersPing { get; set; } = new Dictionary<Vector4Parameter, Vector4>();

        public Dictionary<TextureParameter, Texture2D> TextureParametersPong { get; set; } = new Dictionary<TextureParameter, Texture2D>();
        public Dictionary<FloatParameter, float> FloatParametersPong { get; set; } = new Dictionary<FloatParameter, float>();
        public Dictionary<Vector4Parameter, Vector4> Vector4ParametersPong { get; set; } = new Dictionary<Vector4Parameter, Vector4>();

        long StartTime = 0;
        int FrameCount = 0;
        public int FrameLimit = 1;
        public long TimeLimit = 0;
        public bool HasPing { get; private set; } = true;
        public bool HasDiscard { get; private set; } = false;

        public TaskSwitchType TaskSwitchType { get; set; } = TaskSwitchType.Ping;
        public TaskUsage TaskUsage { get; set; } = TaskUsage.FrameLimit;
        public TaskStage TaskStage { get; set; } = TaskStage.PreRenderTarget;
        public TaskType TaskType { get; set; } = TaskType.FillColor;
        public string TaskName { get; set; }

        public TextureTask(string Name)
        {
            TaskName = Name;
        }

        public TextureTask(string Name, TaskType Type)
        {
            TaskName = Name;
            TaskType = Type;
        }

        public TextureTask(string Name, TaskType Type, TaskStage Stage)
        {
            TaskName = Name;
            TaskType = Type;
            TaskStage = Stage;
        }
        public TextureTask(string Name, TaskType Type, TaskStage Stage, TaskUsage Usage)
        {
            TaskName = Name;
            TaskType = Type;
            TaskStage = Stage;
            TaskUsage = Usage;
        }

        public TextureTask(string Name, TaskType Type, TaskStage Stage, TaskUsage Usage, TaskSwitchType SwitchType)
        {
            TaskName = Name;
            TaskType = Type;
            TaskStage = Stage;
            TaskUsage = Usage;
            TaskSwitchType = SwitchType;
        }

        public TextureTask(string Name, RenderTarget2D RenderTarget, TaskType Type, TaskUsage Usage)
        {
            TaskName = Name;
            TargetPing = RenderTarget;
            TaskType = Type;
            TaskUsage = Usage;
        }

        public void SetRenderTarget(RenderTarget2D Target)
        {
            SetRenderTarget(Target, true);
        }

        public void SetRenderTarget(RenderTarget2D Target, bool IsPing)
        {
            if (IsPing)
            {
                TargetPing = Target;
            }
            else
            {
                TargetPong = Target;
            }
        }
        
        public void AddParameter(FloatParameter ValueParameter, float Value)
        {
            AddParameter(ValueParameter, Value, true);
        }

        public void AddParameter(TextureParameter ValueParameter, Texture2D Value)
        {
            AddParameter(ValueParameter, Value, true);
        }
        public void AddParameter(Vector4Parameter ValueParameter, Vector4 Value)
        {
            AddParameter(ValueParameter, Value, true);
        }

        public void AddParameter(FloatParameter ParameterName, float Value, bool IsPing)
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

        public void AddParameter(Vector4Parameter ParameterName, Vector4 Value, bool IsPing)
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

        public void AddParameter(TextureParameter ParameterName, Texture2D Value, bool IsPing)
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
        
        public void TogglePingPong()
        {
            HasPing = !HasPing;
        }

        public void OnFrameCompletion()
        {
            FrameCount++;
            if (TaskUsage == TaskUsage.TimeLimit)
            {
                HasDiscard = DateTime.Now.Millisecond - StartTime > TimeLimit;
            }
            else if (TaskUsage == TaskUsage.FrameLimit)
            {
                HasDiscard = FrameCount == FrameLimit;
                FrameCount = 0;
                FrameLimit = 0;
            }

            if (TaskSwitchType != TaskSwitchType.Ping && TaskSwitchType != TaskSwitchType.Pong)
            {
                HasPing = !HasPing;
            }
            UpdateGeometries();
        }

        public RenderTarget2D GetRenderTarget()
        {
            if (HasPing)
            {
                return TargetPing;
            }
            else
            {
                return TargetPong;
            }
        }
        
        public Rectangle GetTargetBounds()
        {
            if (HasPing)
            {
                return new Rectangle(0,0, TargetPing.Width, TargetPing.Height);
            }
            else
            {
                return new Rectangle(0, 0, TargetPong.Width, TargetPong.Height);
            }
        }

        public void SetShaderParameters(Effect Shader)
        {
            if (TaskSwitchType == TaskSwitchType.Ping)
            {
                foreach (KeyValuePair<TextureParameter, Texture2D> Parameter in TextureParametersPing)
                {
                    if (Shader.Parameters[Parameter.Key.ToString()] != null)
                    {
                        Shader.Parameters[Parameter.Key.ToString()].SetValue(Parameter.Value);

                    }
                }

                foreach (KeyValuePair<FloatParameter, float> Parameter in FloatParametersPing)
                {
                    if (Shader.Parameters[Parameter.Key.ToString()] != null)
                    {
                        Shader.Parameters[Parameter.Key.ToString()].SetValue(Parameter.Value);
                    }
                }
                
                foreach (KeyValuePair<Vector4Parameter, Vector4> Parameter in Vector4ParametersPing)
                {
                    if (Shader.Parameters[Parameter.Key.ToString()] != null)
                    {
                        Shader.Parameters[Parameter.Key.ToString()].SetValue(Parameter.Value);
                    }
                }
            }
            else
            {
                if (HasPing)
                {
                    foreach (KeyValuePair<FloatParameter, float> Parameter in FloatParametersPing)
                    {
                        if (Shader.Parameters[Parameter.Key.ToString()] != null)
                        {
                            Shader.Parameters[Parameter.Key.ToString()].SetValue(Parameter.Value);
                        }
                    }

                    foreach (KeyValuePair<TextureParameter, Texture2D> Parameter in TextureParametersPing)
                    {
                        if (Shader.Parameters[Parameter.Key.ToString()] != null)
                        {
                            Shader.Parameters[Parameter.Key.ToString()].SetValue(Parameter.Value);
                        }
                    }

                    foreach (KeyValuePair<Vector4Parameter, Vector4> Parameter in Vector4ParametersPing)
                    {
                        if (Shader.Parameters[Parameter.Key.ToString()] != null)
                        {
                            Shader.Parameters[Parameter.Key.ToString()].SetValue(Parameter.Value);
                        }
                    }
                }
                else
                {
                    foreach (KeyValuePair<FloatParameter, float> Parameter in FloatParametersPong)
                    {
                        if (Shader.Parameters[Parameter.Key.ToString()] != null)
                        {
                            Shader.Parameters[Parameter.Key.ToString()].SetValue(Parameter.Value);
                        }
                    }

                    foreach (KeyValuePair<TextureParameter, Texture2D> Parameter in TextureParametersPong)
                    {
                        if (Shader.Parameters[Parameter.Key.ToString()] != null)
                        {
                            Shader.Parameters[Parameter.Key.ToString()].SetValue(Parameter.Value);
                        }
                    }

                    foreach (KeyValuePair<Vector4Parameter, Vector4> Parameter in Vector4ParametersPong)
                    {
                        if (Shader.Parameters[Parameter.Key.ToString()] != null)
                        {
                            Shader.Parameters[Parameter.Key.ToString()].SetValue(Parameter.Value);
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



    }
}
