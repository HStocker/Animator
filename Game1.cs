using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.IO;

namespace animator
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont output;
        Vector2 ZERO = new Vector2(0, 0);
        Texture2D animator;
        Texture2D texture;

        //Rectangle Sprite = new Rectangle(0, 0, 119, 378); //knight
        Rectangle Sprite = new Rectangle(131, 1, 175, 275); //monk
        //Rectangle Sprite = new Rectangle(0,0,184,178);
        Vector2 Vector = new Vector2(0, 0);
        float rotation = 0f;
        int frameTime = 0;


        int captureTimer = 0;
        int playbackCounter = 0;
        Animation animation;
        bool playback = false;
        int frameIndex = 0;

        Vector2 animLocation;
        float animRotation;
        int timeSinceLastFrame = 1;

        bool loop = false;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1350; //26
            graphics.PreferredBackBufferHeight = 768; //16
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
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
            animator = this.Content.Load<Texture2D>("NPCs");
            output = this.Content.Load<SpriteFont>("Output18pt");
            texture = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            texture.SetData<Color>(new Color[] { Color.White });

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
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

            KeyboardState state = Keyboard.GetState();


            if (captureTimer < 320) { captureTimer += gameTime.ElapsedGameTime.Milliseconds; }
            if (captureTimer >= 320 && state.IsKeyDown(Keys.T)) { animation = new Animation(Vector, rotation); captureTimer = 0; }
            if (captureTimer >= 320 && state.IsKeyDown(Keys.Y) && animation != null) { animation.addFrame(Vector, rotation, frameTime); captureTimer = 0; }
            frameTime = (Mouse.GetState().ScrollWheelValue / 6) + 80;
            if (!playback && !loop && state.IsKeyDown(Keys.P)) { playback = true; }
            if (!playback && !loop && state.IsKeyDown(Keys.L)) { loop = true; }

            if (captureTimer >= 320 && state.IsKeyDown(Keys.M)) { this.export(); }
            if (state.IsKeyDown(Keys.Escape))
            {
                this.playback = false;
                this.loop = false;
                this.animation = null;
            }
            int shiftOn = 0;
            if (state.IsKeyDown(Keys.LeftShift)) { shiftOn = 4; }
            if (state.IsKeyDown(Keys.W)) { Vector.Y -= 1 + shiftOn; }
            if (state.IsKeyDown(Keys.A)) { Vector.X -= 1 + shiftOn; }
            if (state.IsKeyDown(Keys.D)) { Vector.X += 1 + shiftOn; }
            if (state.IsKeyDown(Keys.S)) { Vector.Y += 1 + shiftOn; }
            if (state.IsKeyDown(Keys.Q)) { rotation -= .02f; }
            if (state.IsKeyDown(Keys.E)) { rotation += .02f; }

            if (playback || loop)
            {
                if (this.frameIndex >= animation.Count() - 1)
                {
                    if (!loop)
                    {
                        this.playback = false;
                    }
                    this.playbackCounter = 0;
                    this.frameIndex = 0;
                }
                if (animation.getLocation(timeSinceLastFrame, frameIndex, out animLocation, out animRotation))
                {
                    timeSinceLastFrame = 1;
                    frameIndex++;
                }
                else { timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds; }
                
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Magenta);

            spriteBatch.Begin();

            spriteBatch.Draw(texture, new Vector2(0, 384), new Rectangle(0, 0, 1350, 768), Color.Black, 0f, ZERO, new Vector2(1, 1), SpriteEffects.None, 0f);

            if (playback || loop)
            {
                spriteBatch.Draw(animator, animLocation, Sprite, Color.White, animRotation, new Vector2(Sprite.Width / 2, Sprite.Height / 2), new Vector2(1, 1), SpriteEffects.None, 0f);
                spriteBatch.DrawString(output, string.Format("{0}", frameIndex), new Vector2(Sprite.Width / 2, Sprite.Height / 2), Color.Black);
            }
            else
            {
                if(animation != null)
                foreach (Frame frame in animation.frames)
                {
                    spriteBatch.Draw(animator, frame.location, Sprite, new Color(255, 255, 255, 60), frame.rotation, new Vector2(Sprite.Width/2,Sprite.Height/2), new Vector2(1, 1), SpriteEffects.None, 0f);
                }

                spriteBatch.Draw(animator, Vector, Sprite, Color.White, rotation, new Vector2(Sprite.Width / 2, Sprite.Height / 2), new Vector2(1, 1), SpriteEffects.None, 0f);
                spriteBatch.DrawString(output, string.Format("{0}", frameTime), ZERO, Color.Black);
                if (animation != null)
                {
                    spriteBatch.DrawString(output, string.Format("ANIMATING: {0}", animation.Count()), new Vector2(0, 27), Color.Black);
                }
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }


        public void export()
        {

            StreamWriter streamWriter = File.AppendText("Animation.dat");
            streamWriter.WriteLine("<animation>");

            float xNormal = animation.frames[0].location.X;
            float yNormal = animation.frames[0].location.Y;
            foreach (Frame frame in animation.frames)
            {
                streamWriter.WriteLine("<frame X=" + Convert.ToString(frame.location.X - xNormal) +
                    " Y=" + Convert.ToString(frame.location.Y - yNormal) +
                    " Rotation ="+ Convert.ToString(frame.rotation) +
                    " Time =" + Convert.ToString(frame.time) + "/>");
            }

            streamWriter.WriteLine("</animation>");
            streamWriter.Flush();
        }
    }



    class Animation
    {
        public List<Frame> frames = new List<Frame>();

        Vector2 startVector;
        float startRotation;

        public Animation(Vector2 startVector, float startRotation)
        {
            frames.Add(new Frame(startVector, startRotation, 80));
            this.startVector = new Vector2(startVector.X, startVector.Y);
            this.startRotation = startRotation;
        }
        public void addFrame(Vector2 location, float rotation, int time)
        {
            frames.Add(new Frame(location, rotation, time));
        }
        public Frame getFrame(int index) { return this.frames[index]; }
        public int frameTime(int index)
        {
            return frames[index].time;
        }
        public int Count() { return frames.Count; }

        public bool getLocation(int timeSinceLastFrame, int frameindex, out Vector2 location, out float rotation)
        {

            if (timeSinceLastFrame > frames[frameindex].time)
            {
                //account for last frame
                location = frames[frameindex + 1].location;
                rotation = frames[frameindex + 1].rotation;
                return true;
            }

            float percentThrough = (float)timeSinceLastFrame / (frames[frameindex + 1].time);

            float newX = (frames[frameindex + 1].location.X - frames[frameindex].location.X) * percentThrough;
            float newY = (frames[frameindex + 1].location.Y - frames[frameindex].location.Y) * percentThrough;
            location = new Vector2(newX + frames[frameindex].location.X, newY + frames[frameindex].location.Y);
            rotation = ((frames[frameindex + 1].rotation - frames[frameindex].rotation) * percentThrough) + frames[frameindex].rotation;

            return false;

        }
        public static Vector2 RotateAboutOrigin(Vector2 point, Vector2 origin, float rotation)
        {
            return Vector2.Transform(point - origin, Matrix.CreateRotationZ(rotation)) + origin;
        }
    }
    class Frame
    {
        public Vector2 location;
        public float rotation;
        public int time;

        public Frame(Vector2 location, float rotation, int time)
        {
            this.location = location;
            this.rotation = rotation;
            this.time = time;
        }
    }
}
