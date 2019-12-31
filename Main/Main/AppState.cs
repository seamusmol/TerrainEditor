using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Main
{
    public class AppState
    {
        public Game1 Game { get; set; }

        public enum ThreadType
        {
            Serial = 0, Parallel = 1
        }

        public bool IsActive { get; set; } = true;
        public bool IsInitialized { get; set; } = false;
        public AppState()
        {

        }

        public virtual void Init(Game1 game) {IsInitialized = true; Game = game; }
        public virtual void Update(GameTime GameTime) { }
        public virtual void UpdatePreRender(GameTime GameTime) { }
        public virtual void Close() { }

    }
    
}
