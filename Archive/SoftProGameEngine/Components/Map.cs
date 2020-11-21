using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SoftProGameEngine.Framework;
using SoftProGameEngine.Helpers;
using SoftProGameEngine.Particles;
using SoftProGameEngine.Physics;
using System;
using System.Collections.Generic;

namespace SoftProGameEngine.Components
{
    public class Map : IComponent
    {
        //Properties
        private string _name;

        private Tile[,] _tiles;
        private int _width;
        private int _height;
        private int _viewWidth;

        //Dynamics
        private static int _scrollOffset;

        public static Player ActivePlayer;

        public const int RightMargin = 320;

        //Other shit
        private MapObjectCollection _mapObjects;

        private ParticleEffectCollection _particleEffects;

        private List<BackgroundTexture> _backgroundTextures;

        public Map(string name, int height, int width, int viewWidth)
        {
            _name = name;
            _width = width;
            _height = height;
            _viewWidth = viewWidth;

            _tiles = new Tile[Height, Width];
            _scrollOffset = 0;
            _mapObjects = new MapObjectCollection();
            _particleEffects = new ParticleEffectCollection(this);
            _backgroundTextures = new List<BackgroundTexture>();
        }

        public static Rectangle ScreenBounds
        {
            get
            {
                return new Rectangle(0, 0, 800, 640);
            }
        }

        public static BoundingBox ScreenBoundingBox
        {
            get
            {
                return ScreenBounds.MakeBoundingBox();
            }
        }

        public string Name
        {
            get { return _name; }
        }

        public Tile[,] Tiles
        {
            get { return _tiles; }
        }

        public MapObjectCollection MapObjects
        {
            get { return _mapObjects; }
        }

        public static int ScrollOffset
        {
            get { return _scrollOffset; }
        }

        public int Width
        {
            get { return _width; }
        }

        public int Height
        {
            get { return _height; }
        }

        public int ViewWidth
        {
            get { return _viewWidth; }
        }

        public List<BackgroundTexture> BackgroundTextures
        {
            get { return _backgroundTextures; }
        }

        public ParticleEffectCollection ParticleEffects
        {
            get { return _particleEffects; }
        }

        #region IComponent Members

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Draw backgrounds
            Rectangle screenRectangle = new Rectangle(0, 0, _width * Tile.DefaultWidth, _height * Tile.DefaultHeight);
            BoundingBox screenBox = screenRectangle.MakeBoundingBox();
            foreach (BackgroundTexture texture in _backgroundTextures)
            {
                Rectangle destination =
                    new Rectangle(texture.Point.X * Tile.DefaultWidth - ScrollOffset, texture.Point.Y * Tile.DefaultHeight, texture.Width, texture.Height);
                BoundingBox destinationBoundingBox = destination.MakeBoundingBox();

                if (destinationBoundingBox.Intersects(screenBox))
                {
                    //we can see it!
                    spriteBatch.Draw(texture.Texture, destination, Color.White);
                }
            }

            //Draw tiles
            for (int i = 0; i < ViewWidth + 1; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    if (i + ScrollOffset / Tile.DefaultWidth < 0)
                        continue;

                    Tile tile = _tiles[j, i + ScrollOffset / Tile.DefaultWidth];

                    if (tile == null)
                        continue;

                    if (!tile.IsAlive)
                        continue;

                    //Vector2 pos = new Vector2(Tile.DefaultWidth * i - (ScrollOffset % Tile.DefaultWidth), Tile.DefaultHeight * j);
                    //Rectangle dest = new Rectangle((int)pos.X, (int)pos.Y, Tile.DefaultWidth, Tile.DefaultHeight);
                    //spriteBatch.Draw(tile.Texture, dest, Color.White);
                    tile.Draw(gameTime, spriteBatch);
                }
            }

            //Draw player
            ActivePlayer.Draw(gameTime, spriteBatch);

            //Draw Objects
            MapObjects.Draw(gameTime, spriteBatch);

            //Draw Particle Effects
            _particleEffects.Draw(gameTime, spriteBatch);
        }

        private const int CollisionTolerance = 0;

        public void UpdateMapObjects(GameTime gameTime)
        {
            for (int i = 0; i < _mapObjects.Count; i++)
            {
                MapObject mapObject = _mapObjects[i];

                if (!mapObject.IsAlive)
                    continue;

                Vector2 previousPosition = new Vector2(mapObject.Position.X, mapObject.Position.Y);

                mapObject.Update(gameTime);

                if (!mapObject.CollisionsEnabled)
                    continue;

                if (mapObject is Player)
                {
                    //Update scroll offset
                    if ((mapObject.Position.X < Width * Tile.DefaultWidth - RightMargin) && (mapObject.Position.X > _viewWidth * Tile.DefaultWidth - RightMargin))
                    {
                        int newOffset = (int)mapObject.Position.X - (_viewWidth * Tile.DefaultWidth - RightMargin);
                        if (_scrollOffset < newOffset)
                        {
                            _scrollOffset = newOffset;
                        }
                    }

                    //Keep player in field of view
                    if (mapObject.Position.X < _scrollOffset)
                        mapObject.Position = new Vector2(_scrollOffset, mapObject.Position.Y);
                }

                //collisions
                BoundingBox mapObjectBoundingBox = mapObject.GetBoundingBox();

                List<MapObject> tiles = GetTiles(mapObject);
                tiles.AddRange(_mapObjects);

                foreach (MapObject item in tiles)
                {
                    if (mapObject == item)
                        continue;//Same object

                    if (item == null)//for null tiles
                        continue;

                    if (!item.HitEnabled)
                        continue;

                    if (!item.IsAlive)
                        continue;

                    //make bounding box
                    BoundingBox box = item.GetBoundingBox();

                    if (mapObjectBoundingBox.Intersects(box))
                    {
                        //we are now hitting a tile. Change position to not hit tile, and call appropriate collider function
                        if (previousPosition.Y + mapObject.Height < item.Position.Y + CollisionTolerance)
                        {
                            //Handle collisions
                            mapObject.OnBottomCollision(item);
                            item.OnTopCollision(mapObject);

                            //Special cases - these are shitty but necessary
                            if (mapObject is IPhysicsObject)
                            {
                                IPhysicsObject physicsObject = mapObject as IPhysicsObject;
                                if (item is Tile)
                                {
                                    physicsObject.Velocity = new Vector2(physicsObject.Velocity.X, 0);

                                    if (mapObject is Player)
                                        (mapObject as Player).HasLanded = true;
                                }
                            }

                            if (mapObject is Player)
                                (mapObject as Player).IsJumping = false;

                            //Correct position
                            mapObject.Position = new Vector2(mapObject.Position.X, item.Position.Y - mapObject.Height - 1);
                        }
                        else if (previousPosition.Y > item.Position.Y + item.Height - CollisionTolerance)
                        {
                            //Handle collisions
                            mapObject.OnTopCollision(item);
                            item.OnBottomCollision(mapObject);

                            if (!mapObject.HandlesCollisions)
                            {
                                //Correct position
                                mapObject.Position = new Vector2(mapObject.Position.X, item.Position.Y + item.Height + 1);
                                if (mapObject is IPhysicsObject)
                                {
                                    IPhysicsObject physicsObject = mapObject as IPhysicsObject;
                                    physicsObject.Velocity = new Vector2(physicsObject.Velocity.X, 0);//let gravity bring us down
                                }
                            }
                        }
                        else if (previousPosition.X + mapObject.Width < item.Position.X - CollisionTolerance)
                        {
                            //Handle collisions
                            mapObject.OnRightCollision(item);
                            item.OnLeftCollision(mapObject);

                            if (!mapObject.HandlesCollisions)
                                //Correct position
                                mapObject.Position = new Vector2(item.Position.X - mapObject.Width - 1, mapObject.Position.Y);
                        }
                        else if (previousPosition.X > item.Position.X + item.Width + CollisionTolerance)
                        {
                            //Handle collisions
                            mapObject.OnLeftCollision(item);
                            item.OnRightCollision(mapObject);

                            if (!mapObject.HandlesCollisions)
                                //Correct position
                                mapObject.Position = new Vector2(item.Position.X + item.Width + 1, mapObject.Position.Y);
                        }
                        //if (mapObject is Player)
                        //{
                        //    Player player = mapObject as Player;
                        //    player.HasLanded = false;
                        //}
                    }
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Update tiles
            for (int i = 0; i < ViewWidth + 1; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    Tile tile = _tiles[j, i + ScrollOffset / Tile.DefaultWidth];
                    if (tile == null)
                        continue;

                    if (!tile.IsAlive)
                        continue;

                    tile.Update(gameTime);
                }
            }

            UpdateMapObjects(gameTime);

            _particleEffects.Update(gameTime);
        }

        #endregion IComponent Members

        public void Reset()
        {
            _scrollOffset = 0;
            foreach (MapObject mapObject in _mapObjects)
            {
                if (mapObject is PhysicsMapObject)
                    (mapObject as PhysicsMapObject).Reset();
            }
            foreach (Tile tile in _tiles)
            {
                if (tile != null)
                    tile.Reset();
            }
        }

        private List<MapObject> GetTiles(MapObject mapObject)
        {
            Vector2 center = mapObject.GetCenter();
            Point coordinate = GetCoordinates(center);

            List<MapObject> ret = new List<MapObject>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    Point mod = new Point(coordinate.X + x, coordinate.Y + y);
                    Tile tile = TryGet(mod);
                    if (tile != null)
                        ret.Add(tile);
                }
            }

            return ret;
        }

        #region Xml Loading

        public static Map ReadFromXml(string fileName, ICreator creator, out MapExport outMap)
        {
            MapExport mapExport = null;
            bool status = MapExport.Load(ref mapExport, fileName);

            Map map = new Map(mapExport.Name, mapExport.Height, mapExport.Width, mapExport.ViewWidth);

            for (int i = 0; i < mapExport.Data.Count; i++)
            {
                string line = mapExport.Data[i];
                for (int j = 0; j < line.Length; j++)
                {
                    int n = int.Parse(line[j].ToString());
                    Tile tile = null;
                    if (n != 0)
                    {
                        string tileName = mapExport.GetTileExport(n).Name;
                        tile = creator.CreateInstance<Tile>(tileName);
                        tile.Map = map;
                        tile.IsAlive = true;
                        tile.Position = new Vector2(j * Tile.DefaultWidth, i * Tile.DefaultHeight);
                    }
                    map.Tiles[i, j] = tile;
                }
            }
            outMap = mapExport;
            return map;
        }

        #endregion Xml Loading

        #region MapApis

        public Vector2 ShiftVector(Vector2 original)
        {
            return new Vector2(original.X - ScrollOffset, original.Y);
        }

        public Tile TryGet(Point point)
        {
            try
            {
                return _tiles[point.Y, (int)point.X];
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Vector2 IndexToPosition(Vector2 index)
        {
            return new Vector2(index.X * Tile.DefaultWidth, index.Y * Tile.DefaultHeight);
        }

        public Tile[] VerticalTilesAtOffset(int offset)
        {
            Tile[] tiles = new Tile[Height];
            for (int i = 0; i < Height; i++)
            {
                tiles[i] = _tiles[i, offset];
            }
            return tiles;
        }

        public Point GetCoordinates(Vector2 raw)
        {
            int indexX = (int)(raw.X / Tile.DefaultWidth);
            int indexY = (int)(raw.Y / Tile.DefaultHeight);

            return new Point(indexX, indexY);
        }

        public Vector2 GetTileCoordinate(Tile tile)
        {
            for (int i = 0; i < ViewWidth + 1; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    if (i + ScrollOffset / Tile.DefaultWidth < 0)
                        continue;

                    if (_tiles[j, i + ScrollOffset / Tile.DefaultWidth] == tile)
                    {
                        Vector2 pos = new Vector2((i + (ScrollOffset / Tile.DefaultWidth)) * Tile.DefaultWidth, Tile.DefaultHeight * j);// - (ScrollOffset % Tile.Width)
                        return pos;
                    }
                }
            }
            return new Vector2(-1);
        }

        #endregion MapApis
    }
}
