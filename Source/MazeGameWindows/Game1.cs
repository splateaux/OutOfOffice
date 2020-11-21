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

namespace MazeGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Maze _maze;
        MazeRunner _runner;
        MazeBoss _boss;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 1280;
            graphics.PreferredBackBufferWidth = 720;
            graphics.ApplyChanges();
            graphics.IsFullScreen = true;
            
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
            // TODO: Add your initialization logic here
            
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            MazeOptions options = new MazeOptions(
                new Vector2(1, 3),
                23,
                16,
                Content.Load<Texture2D>("Textures/Objects/SoftProDollar"));

            _maze = new Maze(graphics.GraphicsDevice, options);

            MazeRunnerOptions runnerOptions = new MazeRunnerOptions(
                _maze,
                Content.Load<Texture2D>("Textures/Players/John/CloseupSmall"),
                Content.Load<SoundEffect>("Audio/Effects/SoftProDollarCollected"),
                100);


            _runner = new MazeRunner(this, runnerOptions);

            _boss = new MazeBoss(
                Content.Load<Texture2D>("Textures/Objects/Boss"),
                new Vector2(0, 0),
                new Vector2(800, 0),
                new TimeSpan(0, 1, 0),
                Content.Load<Texture2D>("Textures/Objects/ConferenceRoom"));

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            _runner.Update(gameTime);
            _boss.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            _maze.Draw(spriteBatch);
            _runner.Draw(spriteBatch);
            _boss.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }


    }
}
