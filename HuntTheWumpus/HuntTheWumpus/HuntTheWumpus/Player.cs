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
    public class Player : Microsoft.Xna.Framework.GameComponent
    {
        public int numArrows;
        public bool isAlive;
        string input;
        public int[] arrowPath;
        public int currentRoomP;

        public Player(Game game)
            : base(game)
        {
            // TODO: Construct any child components here

            numArrows = 5;
            isAlive = true;
            arrowPath = new int[5];
        }

        public void chooseArrowPath()
        {
            for(int i = 0; i < 5; i++)
            {
                arrowPath[i] = -1;
            }
            int num; // used to get number of rooms in arrow path
            int choice; // used to get room numbers for arrowPath[]
            while (true)
            {
                System.Console.WriteLine("How many rooms would you like to shoot through? (1 - 5)");
                if (Int32.TryParse(System.Console.ReadLine(), out num))
                {
                    if (num > 0 && num < 6)
                    {
                        for (int i = 0; i < num; i++)
                        {
                            System.Console.WriteLine("Enter Room " + (i + 1) + ": ");
                            if (Int32.TryParse(System.Console.ReadLine(), out choice))
                            {
                                if(choice > 0 && choice < 21)
                                {
                                    arrowPath[i] = choice - 1;
                                }
                                else
                                {
                                    i--;
                                    System.Console.WriteLine("Invalid...(1 - 20)");
                                }
                            }
                            else
                            {
                                i--;
                                System.Console.WriteLine("Invalid...(1 - 20)");
                            }
                        }
                        numArrows--;
                        break;
                    }
                }
                System.Console.WriteLine("Invalid...(1 - 5)");
            }
        }

        public void setCurrentRoomP(int r)
        {
            currentRoomP = r;
        }

        public char moveOrShoot()
        {
            while (true)
            {
                input = System.Console.ReadLine();
                if (String.Compare(input, 0, "Move", 0, 1, true) == 0)
                {
                    return 'm';
                }
                if (String.Compare(input, 0, "Shoot", 0, 1, true) == 0)
                {
                    return 's';
                }
                else
                {
                    System.Console.WriteLine("Do you want to (M)ove or (S)hoot?");
                }
            }
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
