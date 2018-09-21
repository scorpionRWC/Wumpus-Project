using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HuntTheWumpus
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        //Text t;
        Map m = new Map(null);
        Player p = new Player(null);
        Wumpus w = new Wumpus(null);
        Texture2D pixel;
        Texture2D map1;
        Texture2D [] roomSprite;
        Rectangle[] arrowPath = new Rectangle[5];
        Rectangle [,] line;
        double [,] angle;
        Vector2 [] roomLoc = new Vector2[20];
        Texture2D batSprite;
        Texture2D pitSprite;
        Texture2D wumpusSprite;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        double mapScale;
        SpriteFont font;
        Vector2 fontPos;
        String output = "";
        //things added for 3D game
        Model [] cube = new Model[20];
        Model [] pipe = new Model[30];
        Texture2D black;
        Texture2D white;
        Matrix [] roomLoc3D = new Matrix[20];
        Vector3 [] pipeLoc3D = new Vector3[30];
        Vector3 [] pipeRot3D = new Vector3[30];
        Vector3 [] pipeScale3D = new Vector3[30];
        Texture2D [] pipeSprite = new Texture2D[30];
        Vector3 [] roomColor = new Vector3[20];
        Texture2D [] cubeSprite = new Texture2D[20];
        Vector3 [,] pipeColor2;
        Vector3 [] pipeColor = new Vector3[30];
        Vector3 thirdPersonReference = new Vector3(5, 0, 75);
        float avatarYaw = 0;// MathHelper.PiOver2;
        Matrix rotationMatrix;
        Vector3 transformedReference;
        Vector3 avatarPosition;
        Vector3 cameraPosition;
        //Model[,] prepipe;
        //int pipenum;
        //Material material1; 
        /*Effect effectInstance;
        GraphicsDevice device;
        ContentManager content;
        private Texture diffuseTexture = null;
        private Texture specularTexture = null;*/

        // for gameLoop() logic
        bool keepPlaying = true;
        string again; // just to check how the player wants to continue at the end
        char input; // for multiple choice questions
        int value = -5; // arbitrary initialization
        int prevArrow; // for arrow path logic
        int currArrow; // for arrow path logic
        int arrowPathNum = 0;

        // for Update() logic
        bool isChoosing = true;
        bool isShooting = false;
        bool doneShooting = false;
        bool isMoving = false;
        bool gameOver = false;
        bool isNext = false; // for the nextButton; used after pit/bat/wumpus statements
        //[DllImport("kernel32")]
        //static extern bool AllocConsole();

        // for buttons
        Rectangle moveButton;
        Rectangle shootButton;
        Rectangle move1Button;
        Rectangle move2Button;
        Rectangle move3Button;
        Rectangle nextButton;
        int buttonOffset = 0; // to avoid accidental button

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = 1200;
            graphics.PreferredBackBufferHeight = 650;
            graphics.ApplyChanges();
            mapScale = 1.5;
           // AllocConsole();
            IsMouseVisible = true;
            // System.Console.WriteLine("Sup");
            // TODO: Add your initialization logic here

            //t = GetComponent<Text>();
            //string s;
            //s = m.printMap();
            //t.text = s;
            //System.Console.WriteLine(s);
            //gameLoop();
            for(int i = 0; i < 30; i++)
            {
                pipeSprite[i] = black;
            }
          //  pipenum = 0;
            base.Initialize();
        }

        void choosing()
        {
            output = "";
            p.setCurrentRoomP(m.sendPlayerRoomNumber());
            w.setCurrentRoomW(m.sendWumpusRoomNumber());
            output += "\nIn room " + ((p.currentRoomP) + 1);
            output += m.checkRooms(p.currentRoomP);
            output += "Do you want to Move or Shoot?";
        }

        void moving()
        {
            output = m.moveCoices(p.currentRoomP);
        }

        void shooting()
        {
            output = m.arrowChoices(currArrow, prevArrow, arrowPathNum);
        }

        void afterShooting()
        {
            if (w.isSlayed)
            {
                output = "Woohoo! You got the Wumpus";
            }
            else if(!p.isAlive)
            {
                output = "Ouch... You shot yourself!";
            }
            else if(p.numArrows == 0)
            {
                output = "Out of Arrows... Game Over!!!";
            }
            else
            {
                buttonOffset = 50;
                output = "You missed...";
                if(w.isAsleep)
                {
                    output += "\nAnd you woke the Wumpus!";
                }
            }
        }

        void playAgainMenu()
        {
            output = "";
            output += "Play again? Or Quit?";
            output += "\nA: Play Again with Same Map";
            output += "\nB: Play Again with Different Map";
            output += "\nC: Quit";
        }

        void wumpusLogic()
        {
            if (w.isAsleep == false) // if the Wumpus is awake, it may move
            {
                if (w.willMove() == true)
                {
                    w.setCurrentRoomW(m.movingWumpus(w.currentRoomW));
                }
            }
            if (p.currentRoomP == w.currentRoomW) // in the same room as Wumpus
            {
                if (w.isAsleep == false)
                {
                    p.isAlive = false;
                    isChoosing = false;
                    isMoving = false;
                    isNext = true;
                    output = "The Wumpus got you... Game Over!!!";
                }
                else
                {
                    w.isAsleep = false;
                    isChoosing = false;
                    isMoving = false;
                    isNext = true;
                    output = "You bumped the Wumpus... Run!!!";
                }
            }
        }

        void gameLoop()
        {
            //bool keepPlaying = true;
            //string again; // just to check how the player wants to continue at the end
            //char input; // for multiple choice questions
            //int value = -5; // arbitrary initialization
            //int prevArrow; // for arrow path logic
            //int currArrow; // for arrow path logic
            //do // play again (yes / no)
            //{
                //do // game logic loop
                //{
                    p.setCurrentRoomP(m.sendPlayerRoomNumber());
                    w.setCurrentRoomW(m.sendWumpusRoomNumber());
                //System.Console.WriteLine("\nIn room " + ((p.currentRoomP) + 1));
                output += "\nIn room " + ((p.currentRoomP) + 1);
                //System.Console.WriteLine(m.checkRooms(p.currentRoomP));
                output += m.checkRooms(p.currentRoomP);
                //System.Console.WriteLine("Do you want to (M)ove or (S)hoot?");
                output += "Do you want to Move or Shoot?";
                    input = p.moveOrShoot();
                    if (input == 'm') // player decides to move
                    {
                        //System.Console.WriteLine("\nTesting game so far... in move\n");
                        p.setCurrentRoomP(m.adjacentRooms(p.currentRoomP));
                        value = m.checkCurrentSafety(p.currentRoomP);
                        if (value == -1)
                        {
                            p.isAlive = false;
                        }
                        else if (value > -1)
                        {
                            p.setCurrentRoomP(value);
                        }
                    }
                    else if (input == 's') // player decides to shoot
                    {
                        prevArrow = -3; // -1 if hit Wumpus, -2 if hit self
                        //System.Console.WriteLine("\nTesting game so far... in shoot\n");
                        currArrow = p.currentRoomP;
                        p.chooseArrowPath();
                        for (int i = 0; i < 5; i++)
                        {
                            if (p.arrowPath[i] == -1)
                            {
                                break;
                            }
                            value = m.checkArrow(currArrow, p.arrowPath[i], prevArrow);
   System.Console.WriteLine("Arrow going through room: " + (value + 1));
                            if (value == -1)
                            {
                                w.isSlayed = true;
                                w.isAsleep = true;
                                break;
                            }
                            else if (value == -2)
                            {
                                p.isAlive = false;
                                break;
                            }
                            else
                            {
                                prevArrow = currArrow;
                                currArrow = value;
                            }
                        } // end for loop
                        if (value >= 0)
                        {
                            System.Console.WriteLine("The arrow missed!!!");
                            if (w.isAsleep == true)
                            {
                                w.isAsleep = false;
                                System.Console.WriteLine("You woke the Wumpus!!!");
                            }
                            if (p.numArrows == 0)
                            {
                                System.Console.WriteLine("You ran out of arrows... GameOver!!!");
                            }
                        }
                    }
                    if (w.isAsleep == false) // if the Wumpus is awake, it may move
                    {
                        if (w.willMove() == true)
                        {
                            w.setCurrentRoomW(m.movingWumpus(w.currentRoomW));
                        }
                    }
                    if (p.currentRoomP == w.currentRoomW) // in the same room as Wumpus
                    {
                        if (w.isAsleep == false)
                        {
                            p.isAlive = false;
                            System.Console.WriteLine("The Wumpus got you... Game Over!!!");
                        }
                        else
                        {
                            w.isAsleep = false;
                            System.Console.WriteLine("You bumped the Wumpus... Run!!!");
                        }
                    }
                //} while (p.isAlive == true && w.isSlayed == false && p.numArrows > 0);
                System.Console.WriteLine("Play again? Or Quit?");
                System.Console.WriteLine("A: Play Again with Same Map");
                System.Console.WriteLine("B: Play Again with Different Map");
                System.Console.WriteLine("Other: Quit");
                again = System.Console.ReadLine();
                // reset values
                p.isAlive = true;
                p.numArrows = 5;
                w.isAsleep = true;
                w.isSlayed = false;
                if (String.Compare(again, 0, "A", 0, 1, true) == 0)
                {
                    m.replaySameMap();
                }
                else if (String.Compare(again, 0, "B", 0, 1, true) == 0)
                {
                    m.playDifferentMap();
                }
                else
                    keepPlaying = false;
            //} while (keepPlaying == true);
            //Environment.Exit(0);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            //for 3D models
            for (int i = 0; i < 20; i++)
            {
                cube[i] = Content.Load<Model>("Cube");
            }
            for (int i = 0; i < 30; i++)
            {
                pipe[i] = Content.Load<Model>("Cylinder");
            }
            black = Content.Load<Texture2D>("black");
            white = Content.Load<Texture2D>("white");
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            map1 = Content.Load<Texture2D>("dodecapath");
            batSprite = Content.Load<Texture2D>("bat");
            pitSprite = Content.Load<Texture2D>("pit");
            wumpusSprite = Content.Load<Texture2D>("wumpus");
            pixel = Content.Load<Texture2D>("pix");
            roomSprite = new Texture2D[20];
            roomSprite[0] = Content.Load<Texture2D>("r1");
            roomSprite[1] = Content.Load<Texture2D>("r2");
            roomSprite[2] = Content.Load<Texture2D>("r3");
            roomSprite[3] = Content.Load<Texture2D>("r4");
            roomSprite[4] = Content.Load<Texture2D>("r5");
            roomSprite[5] = Content.Load<Texture2D>("r6");
            roomSprite[6] = Content.Load<Texture2D>("r7");
            roomSprite[7] = Content.Load<Texture2D>("r8");
            roomSprite[8] = Content.Load<Texture2D>("r9");
            roomSprite[9] = Content.Load<Texture2D>("r10");
            roomSprite[10] = Content.Load<Texture2D>("r11");
            roomSprite[11] = Content.Load<Texture2D>("r12");
            roomSprite[12] = Content.Load<Texture2D>("r13");
            roomSprite[13] = Content.Load<Texture2D>("r14");
            roomSprite[14] = Content.Load<Texture2D>("r15");
            roomSprite[15] = Content.Load<Texture2D>("r16");
            roomSprite[16] = Content.Load<Texture2D>("r17");
            roomSprite[17] = Content.Load<Texture2D>("r18");
            roomSprite[18] = Content.Load<Texture2D>("r19");
            roomSprite[19] = Content.Load<Texture2D>("r20");
            font = Content.Load<SpriteFont>("CourierNew");

            // TODO: Load your game content here            
            fontPos = new Vector2(graphics.GraphicsDevice.Viewport.Width / 4 * 3,
                graphics.GraphicsDevice.Viewport.Height / 2);
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        /*protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }*/

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            if (keepPlaying == false)
                Environment.Exit(0);
            if(isChoosing)
            {
                choosing();
                //isChoosing = false;
            }
            if(isMoving)
            {
                moving();
            }
            if(isShooting)
            {
                shooting();
            }
            if(doneShooting)
            {
                afterShooting();
            }

            //checking buttons
            if(Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                if (isNext)
                {
                    if (Mouse.GetState().X > nextButton.X && Mouse.GetState().X < nextButton.X + nextButton.Width && Mouse.GetState().Y > nextButton.Y && Mouse.GetState().Y < nextButton.Y + nextButton.Height)
                    {
                        if (p.isAlive == false || p.numArrows == 0)
                        {
                            isNext = false;
                            doneShooting = false;
                            gameOver = true;
                        }
                        else if(p.currentRoomP == w.currentRoomW)
                        {
                            isNext = false;
                            isChoosing = true;
                        }
                        else if(doneShooting)
                        {
                            w.isAsleep = false;
                            if(w.isSlayed)
                            {
                                isNext = false;
                                doneShooting = false;
                                gameOver = true;
                            }
                            else
                            {
                                doneShooting = false;
                                isNext = false;
                                isChoosing = true;
                            }
                            if(!w.isSlayed)
                                wumpusLogic();
                        } // end next doneShooting
                        else // super bat snatch!
                        {
                            m.snatched(p.currentRoomP);
                            p.setCurrentRoomP(m.sendPlayerRoomNumber());
                            isNext = false;
                            isChoosing = true;
                            wumpusLogic();
                        }
                        buttonOffset = 0;
                        for (int i = 0; i < 5; i++)
                        {
                            arrowPath[i] = new Rectangle(0,0,0,0);
                        }
                    } // end click next
                } // end isNext
                if (isChoosing)
                {
                    // click move button
                    if (Mouse.GetState().X > moveButton.X && Mouse.GetState().X < moveButton.X + moveButton.Width && Mouse.GetState().Y > moveButton.Y && Mouse.GetState().Y < moveButton.Y + moveButton.Height)
                    {
                        isChoosing = false;
                        isMoving = true;
                    }
                    // click shoot button
                    if (Mouse.GetState().X > shootButton.X && Mouse.GetState().X < shootButton.X + shootButton.Width && Mouse.GetState().Y > shootButton.Y && Mouse.GetState().Y < shootButton.Y + shootButton.Height)
                    {
                        prevArrow = -1;
                        currArrow = p.currentRoomP;
                        arrowPathNum = 1;
                        isChoosing = false;
                        isShooting = true;
                    }
                } // end isChoosing
                if (isMoving)
                {
                    // chose move A
                    if (Mouse.GetState().X > move1Button.X && Mouse.GetState().X < move1Button.X + move1Button.Width && Mouse.GetState().Y > move1Button.Y && Mouse.GetState().Y < move1Button.Y + move1Button.Height)
                    {
                        p.setCurrentRoomP(m.chosenMove(p.currentRoomP, 'A'));
                        isChoosing = true;
                        isMoving = false;
                        wumpusLogic();
                    }
                    // chose move B
                    if (Mouse.GetState().X > move2Button.X && Mouse.GetState().X < move2Button.X + move2Button.Width && Mouse.GetState().Y > move2Button.Y && Mouse.GetState().Y < move2Button.Y + move2Button.Height)
                    {
                        p.setCurrentRoomP(m.chosenMove(p.currentRoomP, 'B'));
                        isChoosing = true;
                        isMoving = false;
                        wumpusLogic();
                    }
                    // chose move C
                    if (Mouse.GetState().X > move3Button.X && Mouse.GetState().X < move3Button.X + move3Button.Width && Mouse.GetState().Y > move3Button.Y && Mouse.GetState().Y < move3Button.Y + move3Button.Height)
                    {
                        p.setCurrentRoomP(m.chosenMove(p.currentRoomP, 'C'));
                        isChoosing = true;
                        isMoving = false;
                        wumpusLogic();
                    }
                } // end isMoving
                if (isShooting)
                {
                    // chose move A
                    if (Mouse.GetState().X > move1Button.X && Mouse.GetState().X < move1Button.X + move1Button.Width && Mouse.GetState().Y > move1Button.Y && Mouse.GetState().Y < move1Button.Y + move1Button.Height && !(isShooting && m.isPrevArrowRoom(currArrow, prevArrow, 'A')))
                    {
                        prevArrow = currArrow;
                        currArrow = m.getNextArrowRoom(currArrow, 'A');
                        if (prevArrow < currArrow)
                            arrowPath[arrowPathNum - 1] = line[prevArrow, currArrow];
                        else
                            arrowPath[arrowPathNum - 1] = line[currArrow, prevArrow];
                        //Console.Out.WriteLine("Arrow " + (arrowPathNum) + ": " + arrowPath[arrowPathNum - 1]);
                        arrowPathNum++;
                        if(currArrow == p.currentRoomP)
                        {
                            p.isAlive = false;
                        }
                        else if(currArrow == w.currentRoomW)
                        {
                            w.isSlayed = true;
                        }
                        if (arrowPathNum % 2 == 0)
                        {
                            buttonOffset = -50;
                        }
                        else
                            buttonOffset = 0;
                    }
                    // chose move B
                    if (Mouse.GetState().X > move2Button.X && Mouse.GetState().X < move2Button.X + move2Button.Width && Mouse.GetState().Y > move2Button.Y && Mouse.GetState().Y < move2Button.Y + move2Button.Height && !(isShooting && m.isPrevArrowRoom(currArrow, prevArrow, 'B')))
                    {
                        prevArrow = currArrow;
                        currArrow = m.getNextArrowRoom(currArrow, 'B');
                        if (prevArrow < currArrow)
                            arrowPath[arrowPathNum - 1] = line[prevArrow, currArrow];
                        else
                            arrowPath[arrowPathNum - 1] = line[currArrow, prevArrow];
                        arrowPathNum++;
                        if (currArrow == p.currentRoomP)
                        {
                            p.isAlive = false;
                        }
                        else if (currArrow == w.currentRoomW)
                        {
                            w.isSlayed = true;
                        }
                        if (arrowPathNum % 2 == 0)
                        {
                            buttonOffset = -50;
                        }
                        else
                            buttonOffset = 0;
                    }
                    // chose move C
                    if (Mouse.GetState().X > move3Button.X && Mouse.GetState().X < move3Button.X + move3Button.Width && Mouse.GetState().Y > move3Button.Y && Mouse.GetState().Y < move3Button.Y + move3Button.Height && !(isShooting && m.isPrevArrowRoom(currArrow, prevArrow, 'C')))
                    {
                        prevArrow = currArrow;
                        currArrow = m.getNextArrowRoom(currArrow, 'C');
                        if (prevArrow < currArrow)
                            arrowPath[arrowPathNum - 1] = line[prevArrow, currArrow];
                        else
                            arrowPath[arrowPathNum - 1] = line[currArrow, prevArrow];
                        arrowPathNum++;
                        if (currArrow == p.currentRoomP)
                        {
                            p.isAlive = false;
                        }
                        else if (currArrow == w.currentRoomW)
                        {
                            w.isSlayed = true;
                        }
                        if (arrowPathNum % 2 == 0)
                        {
                            buttonOffset = -50;
                        }
                        else
                            buttonOffset = 0;
                    }
                    if(arrowPathNum > 5 || w.isSlayed || !p.isAlive)
                    {
                        p.numArrows--;
                        doneShooting = true;
                        isNext = true;
                        isShooting = false;
                        buttonOffset = 0;
                    }
                } // end isShooting
                if (gameOver)
                {
                    // chose move A
                    if (Mouse.GetState().X > move1Button.X && Mouse.GetState().X < move1Button.X + move1Button.Width && Mouse.GetState().Y > move1Button.Y && Mouse.GetState().Y < move1Button.Y + move1Button.Height)
                    {
                        m.replaySameMap();
                        p.setCurrentRoomP(m.sendPlayerRoomNumber());
                        w.setCurrentRoomW(m.sendWumpusRoomNumber());
                        isChoosing = true;
                        gameOver = false;

                        // reset values
                        p.isAlive = true;
                        p.numArrows = 5;
                        w.isAsleep = true;
                        w.isSlayed = false;
                    }
                    // chose move B
                    if (Mouse.GetState().X > move2Button.X && Mouse.GetState().X < move2Button.X + move2Button.Width && Mouse.GetState().Y > move2Button.Y && Mouse.GetState().Y < move2Button.Y + move2Button.Height)
                    {
                        m.playDifferentMap();
                        p.setCurrentRoomP(m.sendPlayerRoomNumber());
                        w.setCurrentRoomW(m.sendWumpusRoomNumber());
                        isChoosing = true;
                        gameOver = false;

                        // reset values
                        p.isAlive = true;
                        p.numArrows = 5;
                        w.isAsleep = true;
                        w.isSlayed = false;
                    }
                    // chose move C
                    if (Mouse.GetState().X > move3Button.X && Mouse.GetState().X < move3Button.X + move3Button.Width && Mouse.GetState().Y > move3Button.Y && Mouse.GetState().Y < move3Button.Y + move3Button.Height)
                    {
                        Environment.Exit(0);
                    }
                } // end gameOver
                if (gameOver)
                {
                    playAgainMenu();
                }
            }// end mouse left button
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                avatarYaw += 0.01f;
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                avatarYaw -= 0.01f;
            base.Update(gameTime);
        }

        /*void placePipe(int p1, int p2)
        {
            int pipeLength = getDistance3D(p1, p2);
            double pipeAngle = getAngle(roomLoc3D[p1].Translation.X, roomLoc3D[p1].Translation.Y, roomLoc3D[p2].Translation.X, roomLoc3D[p2].Translation.Y);
            pipeAngle *= 1.08;
            pipeLoc3D[pipenum] = new Vector3((int)((roomLoc3D[p1].Translation.X + roomLoc3D[p2].Translation.X) / 2), (int)((roomLoc3D[p1].Translation.Y + roomLoc3D[p2].Translation.Y) / 2), 0);
            pipeRot3D[pipenum] = new Vector3(MathHelper.PiOver2, (float)pipeAngle, 0);
            pipeScale3D[pipenum] = new Vector3(0.3f, pipeLength, 0.3f);
            //pipe[pipenum] = prepipe[p1, p2];
            pipenum++;
        }*/
        
        void placeLine(int p1, int p2)
        {
            // Console.Out.WriteLine("Room 1: " + p1 + " -- Room 2: " + p2);
            //Console.Out.WriteLine("RoomLoc\t" + roomLoc[p1] + " " + roomLoc[p2]);

            // Rectangle buffer = new Rectangle((int)((roomLoc[p1].X + roomLoc[p2].X) / 2), (int)((roomLoc[p1].Y + roomLoc[p2].Y) / 2), getDistance(p1, p2), 10);
            Rectangle buffer = new Rectangle((int)roomLoc[p1].X + 15, (int)roomLoc[p1].Y + 15, getDistance(p1, p2), 10);
            line[p1, p2] = buffer;
            //Console.Out.WriteLine("Hi");
            angle[p1, p2] = getAngle(roomLoc[p1].X, roomLoc[p1].Y, roomLoc[p2].X, roomLoc[p2].Y);
        }

        int getDistance(int p1, int p2)
        {
            int distance = (int)Math.Sqrt(Math.Pow((roomLoc[p1].X - roomLoc[p2].X), 2) + Math.Pow((roomLoc[p1].Y - roomLoc[p2].Y), 2));
            return distance;
        }

        /*int getDistance3D(int p1, int p2)
        {
            int distance = (int)Math.Sqrt(Math.Pow((roomLoc3D[p1].Translation.X - roomLoc3D[p2].Translation.X), 2) + Math.Pow((roomLoc3D[p1].Translation.Y - roomLoc3D[p2].Translation.Y), 2));
            return (int)(distance * 0.5);
        }*/

        //this returns the angle between two points in radians 
        private double getAngle(float x1, float y1, float x2, float y2)
        {
            float adj = x1 - x2;
            float opp = y1 - y2;
            float tan = opp / adj;
            double ang = MathHelper.ToDegrees((float)Math.Atan2(opp, adj));
            ang = (ang - 180) % 360;
            if (ang < 0)
            {
                ang += 360;
            }
            ang = MathHelper.ToRadians((float)ang);
            return ang;
        }
        
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            // TODO: Add your drawing code here
            Vector2 mapLoc = new Vector2(0, 0);
            line = new Rectangle[20, 20];
            angle = new double[20, 20];
            pipeColor2 = new Vector3[20, 20];
           // prepipe = new Model[20, 20];

            //locations for room sprites
            roomLoc[0] = new Vector2((int)(192 * mapScale), (int)(10 * mapScale));
            roomLoc[1] = new Vector2((int)(380 * mapScale), (int)(145 * mapScale));
            roomLoc[2] = new Vector2((int)(310 * mapScale), (int)(380 * mapScale));
            roomLoc[3] = new Vector2((int)(75 * mapScale), (int)(380 * mapScale));
            roomLoc[4] = new Vector2((int)(15 * mapScale), (int)(145 * mapScale));
            roomLoc[5] = new Vector2((int)(65 * mapScale), (int)(175 * mapScale));
            roomLoc[6] = new Vector2((int)(100 * mapScale), (int)(250 * mapScale));
            roomLoc[7] = new Vector2((int)(120 * mapScale), (int)(330 * mapScale));
            roomLoc[8] = new Vector2((int)(192 * mapScale), (int)(325 * mapScale));
            roomLoc[9] = new Vector2((int)(264 * mapScale), (int)(330 * mapScale));
            roomLoc[10] = new Vector2((int)(285 * mapScale), (int)(250 * mapScale));
            roomLoc[11] = new Vector2((int)(320 * mapScale), (int)(175 * mapScale));
            roomLoc[12] = new Vector2((int)(254 * mapScale), (int)(130 * mapScale));
            roomLoc[13] = new Vector2((int)(192 * mapScale), (int)(77 * mapScale));
            roomLoc[14] = new Vector2((int)(130 * mapScale), (int)(130 * mapScale));
            roomLoc[15] = new Vector2((int)(165 * mapScale), (int)(175 * mapScale));
            roomLoc[16] = new Vector2((int)(145 * mapScale), (int)(240 * mapScale));
            roomLoc[17] = new Vector2((int)(192 * mapScale), (int)(280 * mapScale));
            roomLoc[18] = new Vector2((int)(239 * mapScale), (int)(240 * mapScale));
            roomLoc[19] = new Vector2((int)(219 * mapScale), (int)(175 * mapScale));

            for (int i = 0; i < 19; i++)
            {
                placeLine(i, (i + 1));
            }
            placeLine(15, 19);
            placeLine(5, 14);
            placeLine(0, 4);
            for (int i = 0, j = 13; i < 4; i++, j -= 2)
            {
                placeLine(i, j);
            }
            for (int i = 6, j = 16; i < 13; i += 2, j++)
            {
                placeLine(i, j);
            }
            
            moveButton = new Rectangle(graphics.GraphicsDevice.Viewport.Width / 7 * 4, graphics.GraphicsDevice.Viewport.Height / 3 * 2 + 50, 100, 30);
            shootButton = new Rectangle(graphics.GraphicsDevice.Viewport.Width / 7 * 5, graphics.GraphicsDevice.Viewport.Height / 3 * 2 + 50, 100, 30);
            move1Button = new Rectangle(graphics.GraphicsDevice.Viewport.Width / 7 * 4, graphics.GraphicsDevice.Viewport.Height / 3 * 2 + buttonOffset, 100, 30);
            move2Button = new Rectangle(graphics.GraphicsDevice.Viewport.Width / 7 * 5, graphics.GraphicsDevice.Viewport.Height / 3 * 2 + buttonOffset, 100, 30);
            move3Button = new Rectangle(graphics.GraphicsDevice.Viewport.Width / 7 * 6, graphics.GraphicsDevice.Viewport.Height / 3 * 2 + buttonOffset, 100, 30);
            nextButton = new Rectangle(graphics.GraphicsDevice.Viewport.Width / 7 * 6, graphics.GraphicsDevice.Viewport.Height / 3 * 2 + 50 + buttonOffset, 100, 30);
            spriteBatch.Begin();
            /*
            //first draw the lines
            for (int i = 0; i < 19; i++)
            {
                spriteBatch.Draw(pixel, line[i, i + 1], null, Color.Black, (float)angle[i,i + 1], Vector2.Zero, SpriteEffects.None, 0.0f);
            }
            spriteBatch.Draw(pixel, line[0, 4], null, Color.Black, (float)angle[0, 4], Vector2.Zero, SpriteEffects.None, 0.0f);
            spriteBatch.Draw(pixel, line[15, 19], null, Color.Black, (float)angle[15, 19], Vector2.Zero, SpriteEffects.None, 0.0f);
            spriteBatch.Draw(pixel, line[5, 14], null, Color.Black, (float)angle[5, 14], Vector2.Zero, SpriteEffects.None, 0.0f);
            for (int i = 0, j = 13; i < 4; i++, j -= 2)
            {
                spriteBatch.Draw(pixel, line[i, j], null, Color.Black, (float)angle[i, j], Vector2.Zero, SpriteEffects.None, 0.0f);
            }
            for (int i = 6, j = 16; i < 13; i += 2, j++)
            {
                spriteBatch.Draw(pixel, line[i, j], null, Color.Black, (float)angle[i, j], Vector2.Zero, SpriteEffects.None, 0.0f);
            }
            */
            //for arrowPath tint
            for (int k = 0; k < 5; k++)
            {
                for (int i = 0; i < 20; i++)
                {
                    for (int j = 0; j < 20; j++)
                    {
                        if (arrowPath[k] != new Rectangle(0,0,0,0))
                        {
                            if (line[i, j] == arrowPath[k])
                            {
                                pipeColor2[i, j] = new Vector3(1, 0, 0);
                                //              spriteBatch.Draw(pixel, line[i, j], null, Color.Red, (float)angle[i, j], Vector2.Zero, SpriteEffects.None, 0.0f);
                            }
                        }
                        else pipeColor2[i, j] = new Vector3(0.5f, 0, 1);
                    }
                }
            }
            //for (int i = 0; i < 30; i++) pipeColor[i] = new Vector3(0, 0, 0);
            //pipeColor[29] = new Vector3(1, 0, 0);
            // match the pipeColor[i,j] with the pipeColor[i]
            pipeColor[0] = pipeColor2[0, 4];
            pipeColor[1] = pipeColor2[0, 1];
            pipeColor[2] = pipeColor2[1, 2];
            pipeColor[3] = pipeColor2[2, 3];
            pipeColor[4] = pipeColor2[3, 4];
            pipeColor[5] = pipeColor2[1, 11];
            pipeColor[6] = pipeColor2[2, 9];
            pipeColor[7] = pipeColor2[3, 7];
            pipeColor[8] = pipeColor2[4, 5];
            pipeColor[9] = pipeColor2[0, 13];
            pipeColor[10] = pipeColor2[12, 13];
            pipeColor[11] = pipeColor2[11, 12];
            pipeColor[12] = pipeColor2[10, 11];
            pipeColor[13] = pipeColor2[9, 10];
            pipeColor[14] = pipeColor2[8, 9];
            pipeColor[15] = pipeColor2[7, 8];
            pipeColor[16] = pipeColor2[6, 7];
            pipeColor[17] = pipeColor2[5, 6];
            pipeColor[18] = pipeColor2[5, 14];
            pipeColor[19] = pipeColor2[13, 14];
            pipeColor[20] = pipeColor2[12, 19];
            pipeColor[21] = pipeColor2[10, 18];
            pipeColor[22] = pipeColor2[8, 17];
            pipeColor[23] = pipeColor2[6, 16];
            pipeColor[24] = pipeColor2[14, 15];
            pipeColor[25] = pipeColor2[18, 19];
            pipeColor[26] = pipeColor2[17, 18];
            pipeColor[27] = pipeColor2[16, 17];
            pipeColor[28] = pipeColor2[15, 16];
            pipeColor[29] = pipeColor2[15, 19];
            // then draw rooms on map
            for (int i = 0; i < 20; i++)
            {
                cubeSprite[i] = roomSprite[i];
            }
            for (int i = 0; i < 20; i++)
            {
                if (p.currentRoomP == i && m.checkCurrentSafety(p.currentRoomP) == -1 && gameOver == false)
                {
                    //spriteBatch.Draw(pitSprite, roomLoc[i], Color.White);
                    cubeSprite[i] = pitSprite;
                    output = "Aaaaaahhhh...Fell in a Pit.";
                    isChoosing = false;
                    isMoving = false;
                    isShooting = false;
                    p.isAlive = false;
                    isNext = true;
                }
                else if (p.currentRoomP == i && m.checkCurrentSafety(p.currentRoomP) > -1)
                {
                    //spriteBatch.Draw(batSprite, roomLoc[i], Color.White);
                    cubeSprite[i] = batSprite;
                    output = "Fwoop! Snatched by a Super Bat!";
                    isChoosing = false;
                    isMoving = false;
                    isNext = true;
                }
                else if (p.currentRoomP == i && p.currentRoomP == w.currentRoomW)
                {
                    //spriteBatch.Draw(wumpusSprite, roomLoc[i], Color.White);
                    cubeSprite[i] = wumpusSprite;
                }
                else if (p.currentRoomP == i && gameOver == false)
                {
                    //spriteBatch.Draw(roomSprite[i], roomLoc[i], Color.Green);
                    roomColor[i] = new Vector3(0, 1, 0);
                    avatarPosition = new Vector3(roomLoc3D[i].Translation.X, roomLoc3D[i].Translation.Y, roomLoc3D[i].Translation.Z + 1);
                }
                else
                    //spriteBatch.Draw(roomSprite[i], roomLoc[i], Color.White);
                    roomColor[i] = new Vector3(1, 1, 1);   
            }// end room draw for loop

            //for arrowPath rooms tint
            for (int k = 0; k < 5; k++)
            {
                for (int i = 0; i < 20; i++)
                {
                    for (int j = 0; j < 20; j++)
                    {
                        if (arrowPath[k] != new Rectangle(0, 0, 0, 0))
                        {
                            if (line[i, j] == arrowPath[k])
                            {
                                if (j != p.currentRoomP)
                                    roomColor[j] = new Vector3(1, 0, 0);
                    //            spriteBatch.Draw(roomSprite[j], roomLoc[j], Color.Red);

                                if(i != p.currentRoomP)
                                    roomColor[i] = new Vector3(1, 0, 0);
                                //            spriteBatch.Draw(roomSprite[i], roomLoc[i], Color.Red);

                                //else if (i == currArrow)
                                //spriteBatch.Draw(roomSprite[i], roomLoc[i], Color.Red);
                                //if (j == currArrow && (p.currentRoomP == j || w.currentRoomW == j))
                                //{
                                //  spriteBatch.Draw(roomSprite[j], roomLoc[j], Color.Purple);
                                //}
                                //else if (i == currArrow && (p.currentRoomP == i || w.currentRoomW == i))
                                //{
                                //  spriteBatch.Draw(roomSprite[i], roomLoc[i], Color.Purple);
                                // }
                            }
                        }
                    }
                }
            }

            // draw buttons and button labels
            Vector2 FontOrigin;
            if (isChoosing)
            {
                // move button
                FontOrigin = font.MeasureString("Move") / 2;
                Vector2 movePos = new Vector2((moveButton.Width / 2 + moveButton.Location.X), (moveButton.Height / 2 + moveButton.Location.Y));
                spriteBatch.Draw(pixel, moveButton, Color.White);
                spriteBatch.DrawString(font, "Move", movePos, Color.Black,
                0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);

                // shoot button
                FontOrigin = font.MeasureString("Shoot") / 2;
                Vector2 shootPos = new Vector2((shootButton.Width / 2 + shootButton.Location.X), (shootButton.Height / 2 + shootButton.Location.Y));
                spriteBatch.Draw(pixel, shootButton, Color.White);
                spriteBatch.DrawString(font, "Shoot", shootPos, Color.Black,
                0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);
            }
            if (isMoving || gameOver || isShooting)
            {
                // move1 button
                if (!isShooting || !(isShooting && m.isPrevArrowRoom(currArrow, prevArrow, 'A')))
                {
                    FontOrigin = font.MeasureString("A") / 2;
                    Vector2 move1Pos = new Vector2((move1Button.Width / 2 + move1Button.Location.X), (move1Button.Height / 2 + move1Button.Location.Y));
                    spriteBatch.Draw(pixel, move1Button, Color.White);
                    spriteBatch.DrawString(font, "A", move1Pos, Color.Black,
                    0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);
                }

                // move2 button
                if (!isShooting || !(isShooting && m.isPrevArrowRoom(currArrow, prevArrow, 'B')))
                {
                    FontOrigin = font.MeasureString("B") / 2;
                    Vector2 move2Pos = new Vector2((move2Button.Width / 2 + move2Button.Location.X), (move2Button.Height / 2 + move2Button.Location.Y));
                    spriteBatch.Draw(pixel, move2Button, Color.White);
                    spriteBatch.DrawString(font, "B", move2Pos, Color.Black,
                    0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);
                }

                // move3 button
                if (!isShooting || !(isShooting && m.isPrevArrowRoom(currArrow, prevArrow, 'C')))
                {
                    FontOrigin = font.MeasureString("C") / 2;
                    Vector2 move3Pos = new Vector2((move3Button.Width / 2 + move3Button.Location.X), (move3Button.Height / 2 + move3Button.Location.Y));
                    spriteBatch.Draw(pixel, move3Button, Color.White);
                    spriteBatch.DrawString(font, "C", move3Pos, Color.Black,
                    0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);
                }
            }
            if(isNext)
            {
                // next button
                FontOrigin = font.MeasureString("Next") / 2;
                Vector2 nextPos = new Vector2((nextButton.Width / 2 + nextButton.Location.X), (nextButton.Height / 2 + nextButton.Location.Y));
                spriteBatch.Draw(pixel, nextButton, Color.White);
                spriteBatch.DrawString(font, "Next", nextPos, Color.Black,
                0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);
            }

            // Find the center of the string
            FontOrigin = font.MeasureString(output) / 2;
            // Draw the string
            spriteBatch.DrawString(font, output, fontPos, Color.Black,
                0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);
            spriteBatch.End();
            
            // Create all translations and scaling.
            // Rooms:
            roomLoc3D[0] = Matrix.CreateTranslation(new Vector3(-10, 10, 0));
            roomLoc3D[1] = Matrix.CreateTranslation(new Vector3(-1, 3, 0));
            roomLoc3D[2] = Matrix.CreateTranslation(new Vector3(-4, -9, 0));
            roomLoc3D[3] = Matrix.CreateTranslation(new Vector3(-17, -9, 0));
            roomLoc3D[4] = Matrix.CreateTranslation(new Vector3(-20, 3, 0));
            roomLoc3D[5] = Matrix.CreateTranslation(new Vector3(-17, 2, 0));
            roomLoc3D[6] = Matrix.CreateTranslation(new Vector3(-16, -2, 0));
            roomLoc3D[7] = Matrix.CreateTranslation(new Vector3(-14, -7, 0));
            roomLoc3D[8] = Matrix.CreateTranslation(new Vector3(-10, -7, 0));
            roomLoc3D[9] = Matrix.CreateTranslation(new Vector3(-7, -7, 0));
            roomLoc3D[10] = Matrix.CreateTranslation(new Vector3(-5, -2, 0));
            roomLoc3D[11] = Matrix.CreateTranslation(new Vector3(-4, 2, 0));
            roomLoc3D[12] = Matrix.CreateTranslation(new Vector3(-7, 4, 0));
            roomLoc3D[13] = Matrix.CreateTranslation(new Vector3(-10, 7, 0));
            roomLoc3D[14] = Matrix.CreateTranslation(new Vector3(-14, 4, 0));
            roomLoc3D[15] = Matrix.CreateTranslation(new Vector3(-12, 2, 0));
            roomLoc3D[16] = Matrix.CreateTranslation(new Vector3(-13, -2, 0));
            roomLoc3D[17] = Matrix.CreateTranslation(new Vector3(-10, -4, 0));
            roomLoc3D[18] = Matrix.CreateTranslation(new Vector3(-8, -2, 0));
            roomLoc3D[19] = Matrix.CreateTranslation(new Vector3(-9, 2, 0));
            
            //Pipe Location:
            pipeLoc3D[0] = new Vector3(-15, 7, 0);
            pipeLoc3D[1] = new Vector3(-6, 7, 0);
            pipeLoc3D[2] = new Vector3(-2.5f, -3, 0);
            pipeLoc3D[3] = new Vector3(-10.5f, -9, 0);
            pipeLoc3D[4] = new Vector3(-18.2f, -3, 0);
            pipeLoc3D[5] = new Vector3(-2.2f, 2.8f, 0);
            pipeLoc3D[6] = new Vector3(-6, -8f, 0);
            pipeLoc3D[7] = new Vector3(-15.8f, -8, 0);
            pipeLoc3D[8] = new Vector3(-18.3f, 2.5f, 0);
            pipeLoc3D[9] = new Vector3(-10, 8.5f, 0);
            pipeLoc3D[10] = new Vector3(-8.5f, 5.5f, 0);
            pipeLoc3D[11] = new Vector3(-5.5f, 2.9f, 0);
            pipeLoc3D[12] = new Vector3(-4.5f, 0, 0);
            pipeLoc3D[13] = new Vector3(-6, -4.5f, 0);
            pipeLoc3D[14] = new Vector3(-8, -6.9f, 0);
            pipeLoc3D[15] = new Vector3(-12, -6.9f, 0);
            pipeLoc3D[16] = new Vector3(-15, -4.5f, 0);
            pipeLoc3D[17] = new Vector3(-16.3f, 0f, 0);
            pipeLoc3D[18] = new Vector3(-15.5f, 2.8f, 0);
            pipeLoc3D[19] = new Vector3(-12, 5.75f, 0);
            pipeLoc3D[20] = new Vector3(-8, 3, 0);
            pipeLoc3D[21] = new Vector3(-6.5f, -2, 0);
            pipeLoc3D[22] = new Vector3(-10, -5.5f, 0);
            pipeLoc3D[23] = new Vector3(-14.5f, -2f, 0);
            pipeLoc3D[24] = new Vector3(-13, 3, 0);
            pipeLoc3D[25] = new Vector3(-8.2f, 0, 0);
            pipeLoc3D[26] = new Vector3(-9, -3, 0);
            pipeLoc3D[27] = new Vector3(-12, -2.8f, 0);
            pipeLoc3D[28] = new Vector3(-12.5f, 0, 0);
            pipeLoc3D[29] = new Vector3(-10.5f, 1.7f, 0);
            //Pipe Rotation:
            pipeRot3D[0] = new Vector3(MathHelper.PiOver2, 0.95f, 0);
            pipeRot3D[1] = new Vector3(MathHelper.PiOver2, -0.93f, 0);
            pipeRot3D[2] = new Vector3(MathHelper.PiOver2, 0.3f, 0);
            pipeRot3D[3] = new Vector3(MathHelper.PiOver2, 1.58f, 0);
            pipeRot3D[4] = new Vector3(MathHelper.PiOver2, -0.25f, 0);
            pipeRot3D[5] = new Vector3(MathHelper.PiOver2, 1.1f, 0);
            pipeRot3D[6] = new Vector3(MathHelper.PiOver2, -0.9f, 0);
            pipeRot3D[7] = new Vector3(MathHelper.PiOver2, 0.9f, 0);
            pipeRot3D[8] = new Vector3(MathHelper.PiOver2, -1.1f, 0);
            pipeRot3D[9] = new Vector3(MathHelper.PiOver2, 0f, 0);
            pipeRot3D[10] = new Vector3(MathHelper.PiOver2, -0.8f, 0);
            pipeRot3D[11] = new Vector3(MathHelper.PiOver2, -1.0f, 0);
            pipeRot3D[12] = new Vector3(MathHelper.PiOver2, 0.4f, 0);
            pipeRot3D[13] = new Vector3(MathHelper.PiOver2, 0.3f, 0);
            pipeRot3D[14] = new Vector3(MathHelper.PiOver2, -1.5f, 0);
            pipeRot3D[15] = new Vector3(MathHelper.PiOver2, 1.5f, 0);
            pipeRot3D[16] = new Vector3(MathHelper.PiOver2, -0.3f, 0);
            pipeRot3D[17] = new Vector3(MathHelper.PiOver2, -0.4f, 0);
            pipeRot3D[18] = new Vector3(MathHelper.PiOver2, 1f, 0);
            pipeRot3D[19] = new Vector3(MathHelper.PiOver2, 0.9f, 0);
            pipeRot3D[20] = new Vector3(MathHelper.PiOver2, 0.7f, 0);
            pipeRot3D[21] = new Vector3(MathHelper.PiOver2, -1.5f, 0);
            pipeRot3D[22] = new Vector3(MathHelper.PiOver2, 0f, 0);
            pipeRot3D[23] = new Vector3(MathHelper.PiOver2, 1.5f, 0);
            pipeRot3D[24] = new Vector3(MathHelper.PiOver2, -0.7f, 0);
            pipeRot3D[25] = new Vector3(MathHelper.PiOver2, -0.3f, 0);
            pipeRot3D[26] = new Vector3(MathHelper.PiOver2, 0.7f, 0);
            pipeRot3D[27] = new Vector3(MathHelper.PiOver2, -0.8f, 0);
            pipeRot3D[28] = new Vector3(MathHelper.PiOver2, 0.35f, 0);
            pipeRot3D[29] = new Vector3(MathHelper.PiOver2, 1.55f, 0);
            //Pipe Scaling:
            pipeScale3D[0] = new Vector3(0.3f, 6, 0.3f);
            pipeScale3D[1] = new Vector3(0.3f, 5.8f, 0.3f);
            pipeScale3D[2] = new Vector3(0.3f, 5.8f, 0.3f);
            pipeScale3D[3] = new Vector3(0.3f, 6f, 0.3f);
            pipeScale3D[4] = new Vector3(0.3f, 6, 0.3f);
            pipeScale3D[5] = new Vector3(0.3f, 1.5f, 0.3f);
            pipeScale3D[6] = new Vector3(0.3f, 2, 0.3f);
            pipeScale3D[7] = new Vector3(0.3f, 2, 0.3f);
            pipeScale3D[8] = new Vector3(0.3f, 1.3f, 0.3f);
            pipeScale3D[9] = new Vector3(0.3f, 1.3f, 0.3f);
            pipeScale3D[10] = new Vector3(0.3f, 2, 0.3f);
            pipeScale3D[11] = new Vector3(0.3f, 2, 0.3f);
            pipeScale3D[12] = new Vector3(0.3f, 2, 0.3f);
            pipeScale3D[13] = new Vector3(0.3f, 2, 0.3f);
            pipeScale3D[14] = new Vector3(0.3f, 1.6f, 0.3f);
            pipeScale3D[15] = new Vector3(0.3f, 2, 0.3f);
            pipeScale3D[16] = new Vector3(0.3f, 2, 0.3f);
            pipeScale3D[17] = new Vector3(0.3f, 2, 0.3f);
            pipeScale3D[18] = new Vector3(0.3f, 2, 0.3f);
            pipeScale3D[19] = new Vector3(0.3f, 2, 0.3f);
            pipeScale3D[20] = new Vector3(0.3f, 2, 0.3f);
            pipeScale3D[21] = new Vector3(0.3f, 1, 0.3f);
            pipeScale3D[22] = new Vector3(0.3f, 1, 0.3f);
            pipeScale3D[23] = new Vector3(0.3f, 1, 0.3f);
            pipeScale3D[24] = new Vector3(0.3f, 1, 0.3f);
            pipeScale3D[25] = new Vector3(0.3f, 2, 0.3f);
            pipeScale3D[26] = new Vector3(0.3f, 2, 0.3f);
            pipeScale3D[27] = new Vector3(0.3f, 2, 0.3f);
            pipeScale3D[28] = new Vector3(0.3f, 2, 0.3f);
            pipeScale3D[29] = new Vector3(0.3f, 1, 0.3f);

            rotationMatrix = Matrix.CreateRotationZ(avatarYaw);
            transformedReference = Vector3.Transform(thirdPersonReference, rotationMatrix);
            cameraPosition = transformedReference + avatarPosition;
            // Draw the pipe models.
            for (int i = 0; i < 30; i++)
            {
                foreach (ModelMesh mesh in pipe[i].Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.World = Matrix.CreateScale(pipeScale3D[i]) * Matrix.CreateFromYawPitchRoll(pipeRot3D[i].X, pipeRot3D[i].Y, pipeRot3D[i].Z) * Matrix.CreateTranslation(pipeLoc3D[i]);
                        //effect.View = Matrix.CreateLookAt(new Vector3(0, 0, 230), Vector3.Zero, Vector3.Up);
                       effect.View = Matrix.CreateLookAt(cameraPosition, avatarPosition,
    new Vector3(0.0f, 0.0f, 1.0f));
                        effect.Projection = Matrix.CreatePerspectiveFieldOfView(0.1f, graphics.GraphicsDevice.Viewport.AspectRatio, 0.1f, 10000.0f);
                        //effect.EnableDefaultLighting();
                        effect.DiffuseColor = pipeColor[i];
                        effect.Texture = white;
                        effect.TextureEnabled = true;
                    }
                    mesh.Draw();
                }
            }

            // Draw the cube models.
            for (int i = 0; i < 20; i++)
            {
                foreach (ModelMesh mesh in cube[i].Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.World = roomLoc3D[i];
                        //effect.View = Matrix.CreateLookAt(new Vector3(0, 0, 230), Vector3.Zero, Vector3.Up);
                        effect.View = Matrix.CreateLookAt(cameraPosition, avatarPosition,
    new Vector3(0.0f, 0.0f, 1.0f));
                        effect.Projection = Matrix.CreatePerspectiveFieldOfView(0.1f, graphics.GraphicsDevice.Viewport.AspectRatio, 0.1f, 10000.0f);
                        //effect.EnableDefaultLighting();
                        effect.DiffuseColor = roomColor[i];
                        effect.Texture = cubeSprite[i];
                        effect.TextureEnabled = true;
                    }
                    mesh.Draw();
                }
            }
            
            //cube.Draw(rotation, view, projection);
           // pipenum = 0;
            base.Draw(gameTime);
        }
    }// end Class Game1
}
