using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.GamerServices;

namespace ProjectionMappingGame
{

    class TileGenerator
    {
        private const int MAX_WIDTH_OF_TILES = 4;
        private const int MIN_WIDTH_OF_TILES = 2;
        private const int MAX_AMOUNT_OF_TILES_TO_SPAWN = 4;
        private const int MIN_AMOUNT_OF_TILES_TO_SPAWN = 3;
        private const int DELTA_OF_TILE_Y_POSITION = 5;
        private const int TILE_WIDTH = 40;
        private int[] bitArray;
        private Random randGenerator = new Random();
        private ContentManager content;

        public Tile[] tileArray;
        public bool isReady;

        public TileGenerator(IServiceProvider serviceProvider)
        {
            //initializer for object
            content = new ContentManager(serviceProvider, "Content");
            tileArray = new Tile[40];
            bitArray = new int[40];
            isReady = false;
            setUpTilesInArray();
            
        }

        public Tile[] TileArray
        {
            get
            {
                return tileArray;
            }
        }

        public void checkToSpawnTiles(float elapsedTime)
        {
            //if first tile in array is at certain y then spawn a new tile

            //find index of first tile
            int index = 0;
            //bool flag = false;
            

            for (int i = 0; i < tileArray.Length; i++)
            {
                if (tileArray[i] != null)
                {
                    index = i;
                    break;
                }
            }

            //now check the tiles y to see if we can setup a new tile array
            if(tileArray[index].getPosition().Y >= 25)
            {
                //delete previous tiles and bitarray
                for (int j = 0; j < tileArray.Length; j++)
                {
                    tileArray[j] = null;
                    bitArray[j] = 0;
                }
                
                
                //setup new tile array
                setUpTilesInArray();
            }


            
        }
        public void setUpTilesInArray()
        {
            //random amount of tiles that should be placed
            int randAmountOfTiles = randGenerator.Next(MIN_AMOUNT_OF_TILES_TO_SPAWN, MAX_AMOUNT_OF_TILES_TO_SPAWN);

            //find positions to fill in array
            for (int i = 0; i < bitArray.Length; i++)
            {
                if (i >= 39)
                    break;
                int h;
                int chanceToSpawn = randGenerator.Next(0,100);
                //assign first tile
                if ((chanceToSpawn>= 75) && (randAmountOfTiles >= 0))
                {
                    bitArray[i] = 1;
                    h = i + 1;
                    //make sure we can check to add more onto the first tile
                    if (randAmountOfTiles >= 0)
                    {
                        //append tiles after first one depending on width selected
                        int randWidthOfTiles = randGenerator.Next(MIN_WIDTH_OF_TILES, MAX_WIDTH_OF_TILES);
                        for (int j = 0; j < randWidthOfTiles; j++)
                        {
                            if (h < 39)
                            {
                                bitArray[h] = 1;
                                h++;
                            }
                        }
                        i = h;
                    }
                }
                //no more tiles left so break
                else if (randAmountOfTiles <= 0)
                {
                    break;
                }
            }
            //now use array to setup tiles in correct positions
            for (int k = 0; k < bitArray.Length; k++)
            {
                if (bitArray[k] == 1)
                {
                    tileArray[k] = new Tile(content.Load<Texture2D>("Tiles/Platform"), TileCollision.Platform);
                    int randomDeltaY = randGenerator.Next(0, DELTA_OF_TILE_Y_POSITION);
                    tileArray[k].setPosition(new Vector2(k*40,-45-randomDeltaY));
                }
            }
            isReady = true;
        }
        

    }
    
}
