using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Main.Main;

namespace Main.Geometry
{
    

    public class TextureEditor
    { 
        List<TextureTask> TextureTasks = new List<TextureTask>();
        SpriteBatch Batch2D;
        public Effect Shader { get; set; }
        Texture2D BaseTexture;
        
        public TextureEditor(GraphicsDevice Graphics, Effect TextureEditorShader)
        {
            Batch2D = new SpriteBatch(Graphics);
            Shader = TextureEditorShader.Clone();

            BaseTexture = new Texture2D(Graphics,1,1);
        }
        
        public void ProcessTasks(GraphicsDevice Graphics, int Stage)
        {
            for (int i = 0; i < TextureTasks.Count; i++)
            {
                if ((int)TextureTasks[i].TaskStage == Stage)
                {
                    Graphics.SetRenderTarget(TextureTasks[i].GetRenderTarget());
                    TextureTasks[i].SetShaderParameters(Shader);
                    Shader.CurrentTechnique = Shader.Techniques[TextureTasks[i].TaskType.ToString()];
                    
                    Batch2D.Begin(0, BlendState.Opaque, null, null, null, Shader);

                    for (int j = 0; j < Shader.CurrentTechnique.Passes.Count; j++)
                    {
                        Shader.CurrentTechnique.Passes[j].Apply();
                        Batch2D.Draw(BaseTexture, TextureTasks[i].GetTargetBounds(), Microsoft.Xna.Framework.Color.Wheat);
                    }
                    Batch2D.End();
                    Graphics.SetRenderTarget(null);
                    
                    TextureTasks[i].OnFrameCompletion();
                    if (TextureTasks[i].HasDiscard)
                    {
                        TextureTasks.RemoveAt(i);
                    }
                }
            }
        }
        
        public void AddTextureTask(TextureTask NewTextureTask)
        {
            if (!TextureTasks.Contains(NewTextureTask))
            {
                TextureTasks.Add(NewTextureTask);
            }
        }

    }
}
