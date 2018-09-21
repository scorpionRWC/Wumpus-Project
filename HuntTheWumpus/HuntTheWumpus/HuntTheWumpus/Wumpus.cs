using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace HuntTheWumpus
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Wumpus : Microsoft.Xna.Framework.GameComponent
    {
        public bool isSlayed;
        public bool isAsleep;
        public int currentRoomW;

        public Wumpus(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
            isSlayed = false;
            isAsleep = true;
        }

        public void setCurrentRoomW(int r)
        {
            currentRoomW = r;
        }

        public bool willMove() // 75% chance of moving
        {
            Random rand = new Random();
            int chanceMove = rand.Next(100);
            if (chanceMove < 75)
            {
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }
    }
}
