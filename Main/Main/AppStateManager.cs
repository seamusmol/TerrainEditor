using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
namespace Main.Main
{
    public class AppStateManager
    {
        Game1 Game { get; set; }
        private List<AppState> appStateList = new List<AppState>();
        
        public AppStateManager(Game1 game)
        {
            Game = game;
        }    

        public void UpdatePreRender(GameTime GameTime)
        {
            for (int i = 0; i < appStateList.Count; i++)
            {
                if (appStateList[i].IsActive)
                {
                    appStateList.ElementAt(i).UpdatePreRender(GameTime);
                }
            }
        }

        public void Update(GameTime GameTime)
        {
            for(int i = 0; i < appStateList.Count; i++)
            {
                if (!appStateList[i].IsInitialized)
                {
                    appStateList[i].Init(Game);
                }
            }

            for ( int i = 0; i < appStateList.Count; i++)
            {
                if (appStateList[i].IsActive)
                {
                    appStateList.ElementAt(i).Update(GameTime);
                }
            }

        }

        public void Attach(AppState AppState)
        {
            if (!appStateList.Contains(AppState))
            {
                appStateList.Add(AppState);
            }
        }

        public void Detach(AppState AppState)
        {
            if (appStateList.Contains(AppState))
            {
                appStateList.Remove(AppState);
            }

        }

    }
    

}
