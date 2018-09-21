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
    public class Map : Microsoft.Xna.Framework.GameComponent
    {
        class Room
        {
            public bool hasBat;
            public bool hasWumpus;
            public bool hasPit;
            public bool hasYou;
            public bool openRoom;
            public int roomNumber;
            public int roomA;
            public int roomB;
            public int roomC;
            public Room copy;
        }
        
        int random;
        Room[] dungeon = new Room[20];

        public Map(Game game)
            : base(game)
        {
            // starts off with creating room objects for an array
            // as well as copies to implement "starting again with same map"
            for (int i = 0; i < 20; i++)
            {
                dungeon[i] = new Room();
                dungeon[i].copy = new Room();
                dungeon[i].roomNumber = i;
            }
            // then connects the rooms into a dodecahedron
            for (int i = 1; i < 19; i++)
            {
                dungeon[i].roomA = i - 1;
                dungeon[i].roomB = i + 1;
            }
            dungeon[0].roomA = 4;
            dungeon[0].roomB = 1;
            dungeon[4].roomC = 0;
            dungeon[19].roomA = 18;
            dungeon[19].roomB = 15;
            dungeon[15].roomC = 19;
            dungeon[5].roomC = 14;
            dungeon[14].roomC = 5;
            for (int i = 0, j = 13; i < 4; i++, j -= 2)
            {
                dungeon[i].roomC = j;
                dungeon[j].roomC = i;
            }
            for (int i = 6, j = 16; i < 13; i += 2, j++)
            {
                dungeon[i].roomC = j;
                dungeon[j].roomC = i;
            } // finished with dodecahedron
            setTraps(); // now need to set traps (and player starting position)
        }

        void blankMap() // make sure all rooms are empty
        {
            for (int i = 0; i < 20; i++)
            {
                dungeon[i].hasBat = false;
                dungeon[i].hasPit = false;
                dungeon[i].hasWumpus = false;
                dungeon[i].hasYou = false;
                dungeon[i].openRoom = true;
            }
        }

        void setCopy() // sets a copy of the map
        {
            for(int i = 0; i < 20; i++)
            {
                if (dungeon[i].hasBat == true)
                    dungeon[i].copy.hasBat = true;
                if (dungeon[i].hasPit == true)
                    dungeon[i].copy.hasPit = true;
                if (dungeon[i].hasWumpus == true)
                    dungeon[i].copy.hasWumpus = true;
                if (dungeon[i].hasYou == true)
                    dungeon[i].copy.hasYou = true;
            }
        }

        void getCopy() // gets the copy of the map
        {
            for (int i = 0; i < 20; i++)
            {
                if (dungeon[i].copy.hasBat == true)
                    dungeon[i].hasBat = true;
                if (dungeon[i].copy.hasPit == true)
                    dungeon[i].hasPit = true;
                if (dungeon[i].copy.hasWumpus == true)
                    dungeon[i].hasWumpus = true;
                if (dungeon[i].copy.hasYou == true)
                    dungeon[i].hasYou = true;
            }
        }

        public void replaySameMap()
        {
            blankMap();
            getCopy();
        }

        public void playDifferentMap()
        {
            eraseCopy();
            setTraps();
        }

        void eraseCopy() // resets copied traps to a blank room
        {
            for (int i = 0; i < 20; i++)
            {
                dungeon[i].copy.hasBat = false;
                dungeon[i].copy.hasPit = false;
                dungeon[i].copy.hasWumpus = false;
                dungeon[i].copy.hasYou = false;
                dungeon[i].copy.openRoom = true;
            }
        }

        void setTraps()
        {
            // first make all the rooms empty
            blankMap();

            // then fill the rooms with 2 pits, 2 bats, 1 Wumpus, and 1 player
            dungeon[findOpenRoom()].hasPit = true;
            dungeon[findOpenRoom()].hasPit = true;
            dungeon[findOpenRoom()].hasBat = true;
            dungeon[findOpenRoom()].hasBat = true;
         //   dungeon[findOpenRoom()].hasBat = true; ////////////////////////
         //   dungeon[findOpenRoom()].hasBat = true; // higher difficulty
         //   dungeon[findOpenRoom()].hasBat = true; ////////////////////////
            dungeon[findOpenRoom()].hasWumpus = true;
            dungeon[findOpenRoom()].hasYou = true;
            setCopy(); // allows player to replay same map
        }

        int findOpenRoom() // randomly picks an empty room
        {
            Random rand = new Random();
            int p;
            do
            {
                random = rand.Next(20);
                p = random;
            } while (dungeon[p].openRoom == false);
            dungeon[p].openRoom = false;
            return p;
        }

        public bool isPrevArrowRoom(int currArrow, int prevArrow, char r)
        {
            if (r == 'A')
            {
                return dungeon[currArrow].roomA == prevArrow;
            }
            else if (r == 'B')
            {
                return dungeon[currArrow].roomB == prevArrow;
            }
            else if (r == 'C')
            {
                return dungeon[currArrow].roomC == prevArrow;
            }
            else return false;
        }

        public string arrowChoices(int currentRoom, int prevRoom, int i)
        {
            string s = "Select Room " + i + " of the Arrow's Path";
            if (dungeon[currentRoom].roomA != prevRoom)
                s += "\nA: Room #" + (dungeon[currentRoom].roomA + 1);
            if (dungeon[currentRoom].roomB != prevRoom)
                s += "\nB: Room #" + (dungeon[currentRoom].roomB + 1);
            if (dungeon[currentRoom].roomC != prevRoom)
                s += "\nC: Room #" + (dungeon[currentRoom].roomC + 1);
            return s;
        }

        public int getNextArrowRoom(int current, char r)
        {
            if (r == 'A')
            {
                return dungeon[current].roomA;
            }
            else if (r == 'B')
            {
                return dungeon[current].roomB;
            }
            else if (r == 'C')
            {
                return dungeon[current].roomC;
            }
            else return -1;
        }

        // checks the validity of an arrow's immediate next path
        public int checkArrow(int currentRoom, int nextRoom, int prevRoom)
        {
            Random rand = new Random();
            int r; // to hold the random number
            int randRoom = -1;
            int input;
            
            // check if arrow is going to an adjacent room (not previous room)
            if((dungeon[currentRoom].roomA == nextRoom  ||
                dungeon[currentRoom].roomB == nextRoom  ||
                dungeon[currentRoom].roomC == nextRoom) &&
                nextRoom != prevRoom)
            {
                if(dungeon[nextRoom].hasWumpus == true)
                {
                    System.Console.WriteLine("You got the Wumpus!!!");
                    return -1;
                }
                else if(dungeon[nextRoom].hasYou == true)
                {
                    System.Console.WriteLine("Ouch!... Shot yourself...");
                    return -2;
                }
                else
                {
                    return nextRoom;
                }
            }
            else // teleporting arrow or "super crooked" arrow
            {
                if (nextRoom == prevRoom || nextRoom == currentRoom)
                {
                    System.Console.WriteLine("Arrows aren't that crooked!!!");
                    while (true)
                    {
                        System.Console.WriteLine("Enter another Room #: ");
                        if (Int32.TryParse(System.Console.ReadLine(), out input))
                        {
                            return checkArrow(currentRoom, input, prevRoom);
                        }
                    }
                }
                do
                { // randomly choose a path (as long as it's not a previous path)
                    r = rand.Next(3);
                    if (r == 0)
                    {
                        randRoom = dungeon[currentRoom].roomA;
                    }
                    else if (r == 1)
                    {
                        randRoom = dungeon[currentRoom].roomB;
                    }
                    else if (r == 2)
                    {
                        randRoom = dungeon[currentRoom].roomC;
                    }
                } while (randRoom == prevRoom);
                return randRoom;
            }
        }

        // Wumpus chooses a random path
        public int movingWumpus(int r)
        {
            Random rand = new Random();
            int num = rand.Next(3);
            if(num == 2)
            {
                dungeon[r].hasWumpus = false;
                dungeon[dungeon[r].roomA].hasWumpus = true;
                return dungeon[r].roomA;
            }
            else if(num == 1)
            {

                dungeon[r].hasWumpus = false;
                dungeon[dungeon[r].roomB].hasWumpus = true;
                return dungeon[r].roomB;
            }
            else
            {
                dungeon[r].hasWumpus = false;
                dungeon[dungeon[r].roomB].hasWumpus = true;
                return dungeon[r].roomB;
            }
        }

        public string moveCoices(int r)
        {
            string s = "";
            s += "Which room would you like to explore?\n";
            s += "A: " + (1 + dungeon[r].roomA) + "\n";
            s += "B: " + (1 + dungeon[r].roomB) + "\n";
            s += "C: " + (1 + dungeon[r].roomC) + "\n";
            return s;
        }

        public int chosenMove(int r, char c)
        {
            if (c == 'A')
            {
                dungeon[r].hasYou = false;
                dungeon[r].openRoom = true;
                dungeon[dungeon[r].roomA].hasYou = true;
                return dungeon[r].roomA;
            }
            else if (c == 'B')
            {
                dungeon[r].hasYou = false;
                dungeon[r].openRoom = true;
                dungeon[dungeon[r].roomB].hasYou = true;
                return dungeon[r].roomB;
            }
            else if (c == 'C')
            {
                dungeon[r].hasYou = false;
                dungeon[r].openRoom = true;
                dungeon[dungeon[r].roomC].hasYou = true;
                return dungeon[r].roomC;
            }
            else
                return -1;
        }

        // player chooses a path
        public int adjacentRooms(int r)
        {
            string s = "\n";
            string input;
            while (true)
            {
                s += "Which room would you like to explore?\n";
                s += "A: " + (1 + dungeon[r].roomA) + "\n";
                s += "B: " + (1 + dungeon[r].roomB) + "\n";
                s += "C: " + (1 + dungeon[r].roomC) + "\n";
                System.Console.WriteLine(s);
                input = System.Console.ReadLine();
                if (String.Compare(input, 0, "A", 0, 1, true) == 0)
                {
                    dungeon[r].hasYou = false;
                    dungeon[r].openRoom = true;
                    dungeon[dungeon[r].roomA].hasYou = true;
                    return dungeon[r].roomA;
                }
                else if (String.Compare(input, 0, "B", 0, 1, true) == 0)
                {
                    dungeon[r].hasYou = false;
                    dungeon[r].openRoom = true;
                    dungeon[dungeon[r].roomB].hasYou = true;
                    return dungeon[r].roomB;
                }
                else if (String.Compare(input, 0, "C", 0, 1, true) == 0)
                {
                    dungeon[r].hasYou = false;
                    dungeon[r].openRoom = true;
                    dungeon[dungeon[r].roomC].hasYou = true;
                    return dungeon[r].roomC;
                }
                else
                {
                    System.Console.WriteLine("Invalid Choice...A, B, or C");
                    s = "";
                }
            }
        }

        public int snatched(int r)
        {
            dungeon[r].hasYou = false;
            int newRoom = findOpenRoom();
            dungeon[newRoom].hasYou = true;
            return newRoom;
        }

        // outputs and returns state of current room
        public int checkCurrentSafety(int r)
        {
            if(dungeon[r].hasPit)
            {
               // System.Console.WriteLine("Aaaaaahhhh... Fell in a Pit.");
                return -1;
            }
            else if(dungeon[r].hasBat)
            {
                // System.Console.WriteLine("Fwoop! Snatched by a Super Bat!");
                //dungeon[r].hasYou = false;
                //int newRoom = findOpenRoom();
                //dungeon[newRoom].hasYou = true;
                //return newRoom;
                return 1;
            }
            else
            {
                return -2;
            }
        }

        // outputs nearby traps
        public string checkRooms(int r) // passing in player's current room
        {
            string s = "\n";
            if (dungeon[dungeon[r].roomA].hasPit)
                s += "I feel a draft.\n";
            if (dungeon[dungeon[r].roomB].hasPit)
                s += "I feel a draft.\n";
            if (dungeon[dungeon[r].roomC].hasPit)
                s += "I feel a draft.\n";
            if (dungeon[dungeon[r].roomA].hasBat)
                s += "Bats nearby.\n";
            if (dungeon[dungeon[r].roomB].hasBat)
                s += "Bats nearby.\n";
            if (dungeon[dungeon[r].roomC].hasBat)
                s += "Bats nearby.\n";
            if (dungeon[dungeon[r].roomA].hasWumpus)
                s += "I smell a Wumpus.\n";
            if (dungeon[dungeon[r].roomB].hasWumpus)
                s += "I smell a Wumpus.\n";
            if (dungeon[dungeon[r].roomC].hasWumpus)
                s += "I smell a Wumpus.\n";
            return s;
        }

        // for debugging only: prints the dodecahedron
        public string printMap()
        {
            string output = "";
            for (int i = 0; i < 20; i++)
            {
                output += "Room #" + (i + 1) + ":\n";
                output += " -A: " + dungeon[i].roomA + "\n";
                output += " -B: " + dungeon[i].roomB + "\n";
                output += " -C: " + dungeon[i].roomC + "\n";
            }
            return output;
        }

        public int sendPlayerRoomNumber()
        {
            for (int i = 0; i < 20; i++)
            {
                if (dungeon[i].hasYou == true)
                    return i;
            }
            return -1;
        }

        public int sendWumpusRoomNumber()
        {
            for (int i = 0; i < 20; i++)
            {
                if (dungeon[i].hasWumpus == true)
                    return i;
            }
            return -1;
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
