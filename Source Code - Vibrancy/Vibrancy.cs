#region Namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
#endregion

namespace Vibrancy
{
    public class Vibrancy : Microsoft.Xna.Framework.Game
    {
        #region Variables
        #region System Variables
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GraphicsDevice gDevice;
        DisplayModeCollection resolutions;
        DisplayMode currResolution;
        GameTime _gameTime;

        KeyboardState currKeyState;
        MouseState currMouseState;

        int screenWidth;
        int screenHeight;
        #endregion

        #region ClassObj
        DefaultEffect effect;
        SoundProcessing soundProcess;
        SongList songsInLibrary;
        Block gameBlocks;
        #endregion

        #region GameVars
        Effect renderEffect;
        Effect gaussianBlur;
        Effect lightExtract;
        Effect Bloom;

        Vector3[] boxPositions;
        Vector3[] boxPositionsOuter;
        Vector3 minPointSource;

        SpriteFont textFont;
        SpriteFont ScoreFont;
        SpriteFont HUDText;
        SpriteFont MainMenuFont;
        SpriteFont InstructionFont;

        Texture2D boxTexture;
        Texture2D HUDtex;
        Texture2D vortexTexture;
        Texture2D dividerMenu;

        Video MainMenuVideo;
        Video GameVid;
        VideoPlayer videoPlayer;

        SongCollection totalSongs;
        List<string> songs;
        Song songToPlay;

        int height;
        int selectedDrive;
        int songCount;
        int blockCount;
        int blockCountOuter;
        int beatCountDud;
        int beatCount;
        int dudCount;
        int blockIndexOuter;
        int index;
        long score;
        long scoreIncrement;
        long HighScore;
        int multiplier;
        int negativeMultiplier;
        int songPixelLengthGlobal;

        bool songSelected;
        bool blockHit;
        bool InstructionsSwitch;

        float bassTransform;

        List<float> blockZTranslate;
        List<float> blockZTranslateOuter;
        List<float> DudBlockZ;
        List<Vector2> DudBlockCoords;
        List<Vector2> blockCoords;
        List<int> blockToDraw;
        List<int> blockToDrawOuter;
        List<int> dudblockToDraw;
        List<Vector3> worldToScreen;
        List<Vector4> InteractiveBlockColors;

        Random randomNumber;

        BoundingSphere[] bSphere;
        BoundingBox newBBox;
        BoundingBox[] newBBox1;
        BoundingBox mousePointer;
        BoundingBox NewGame;
        BoundingBox ExitGame;

        Color[] blockColors;
        #endregion

        #region Camera
        Vector3 cameraPos;
        Vector3 camTarget;
        Vector3 prevCam;

        Matrix View;
        Matrix Projection;

        BoundingFrustum frustumBounds;

        float leftEnd, rightEnd, topEnd, bottomEnd;
        #endregion

        #region GameState
        bool MainMenu;
        bool menu;
        bool loader;
        bool loadAction;
        bool game;
        #endregion

        #region Rendertarget
        RenderTarget2D NewGameButton;
        RenderTarget2D ExitGameButton;
        RenderTarget2D MainMenuScene;
        RenderTarget2D sceneTarget;
        RenderTarget2D bloomTarget;
        RenderTarget2D blurTarget;
        RenderTarget2D HUDTraget;
        RenderTarget2D finalHUDTarget;
        RenderTarget2D InstrucionTarget;
        #endregion
        #endregion

        #region Constructor
        public Vibrancy()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        #endregion

        #region Init
        protected override void Initialize()
        {
            #region SystemInit
            //Mouse Settings
            IsMouseVisible = true;
            //
            //Window Settings
            Window.Title = "Xna 3D test";
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += OnClientSizeChanged;
            //
            //Graphics Settings
            gDevice = graphics.GraphicsDevice;

            resolutions = gDevice.Adapter.SupportedDisplayModes;
            currResolution = resolutions.Last();

            screenWidth = graphics.PreferredBackBufferWidth = currResolution.Width;
            screenHeight = graphics.PreferredBackBufferHeight = currResolution.Height;

            graphics.ApplyChanges();
            //
            #endregion

            #region GameInit
            renderEffect = Content.Load<Effect>("Shaders\\baseShade");
            lightExtract = Content.Load<Effect>("Shaders\\brightnessExtract");
            gaussianBlur = Content.Load<Effect>("Shaders\\blur");
            Bloom        = Content.Load<Effect>("Shaders\\BloomCombine");

            boxPositions = new Vector3[12];
            boxPositionsOuter = new Vector3[24];

            songToPlay = null;

            height = 0;
            selectedDrive = 0;
            songCount = 0;
            blockCount = 0;
            beatCount = 0;
            beatCountDud = 0;
            blockCountOuter = 24;
            dudCount = 0;
            blockIndexOuter = 0;
            multiplier = 0;
            negativeMultiplier = 1;
            HighScore = 0;
            index = 0;

            songSelected = false;
            blockHit = false;
            InstructionsSwitch = false;

            songs = new List<string>();
            blockZTranslate = new List<float>();
            DudBlockZ = new List<float>();
            blockToDraw = new List<int>();
            blockZTranslateOuter = new List<float>();
            blockToDrawOuter = new List<int>();
            worldToScreen = new List<Vector3>();
            InteractiveBlockColors = new List<Vector4>();
            dudblockToDraw = new List<int>();
            DudBlockCoords = new List<Vector2>();
            blockCoords = new List<Vector2>();

            randomNumber = new Random();

            bSphere = new BoundingSphere[12];
            newBBox1 = new BoundingBox[1];

            blockColors = new Color[6];

            #endregion

            #region Camera Init
            cameraPos = new Vector3(0.0f, 0.0f, 150.0f);
            camTarget = new Vector3(0.0f, 0.0f, -1000.0f);
            #endregion

            #region ClassObj Init
            effect = new DefaultEffect(renderEffect);
            soundProcess = new SoundProcessing();
            gameBlocks = new Block(gDevice, effect);
            songsInLibrary = new SongList();
            DebugShapeRenderer.Initialize(gDevice);
            #endregion

            #region GameState Init
            MainMenu = true;
            menu = false;
            loader = false;
            loadAction = false;
            game = false;
            #endregion

            #region RenderTargets
            NewGameButton = new RenderTarget2D(gDevice, screenWidth, screenHeight);
            MainMenuScene = new RenderTarget2D(gDevice, screenWidth, screenHeight);
            ExitGameButton = new RenderTarget2D(gDevice, screenWidth, screenHeight);
            sceneTarget = new RenderTarget2D(gDevice, screenWidth, screenHeight);
            blurTarget  = new RenderTarget2D(gDevice, screenWidth, screenHeight);
            bloomTarget = new RenderTarget2D(gDevice, screenWidth, screenHeight);
            HUDTraget = new RenderTarget2D(gDevice, screenWidth, screenHeight);
            finalHUDTarget = new RenderTarget2D(gDevice, screenWidth, screenHeight);
            InstrucionTarget = new RenderTarget2D(gDevice, screenWidth, screenHeight);
            #endregion

            base.Initialize();
        }
        #endregion

        #region Window Management
        protected bool IsFullScreen
        {
            get
            {
                return graphics.IsFullScreen;
            }

            set
            {
                if (value != graphics.IsFullScreen)
                {
                    graphics.IsFullScreen = !(IsFullScreen);
                    IsMouseVisible = true;
                    graphics.ApplyChanges();
                }
            }
        }

        protected void OnClientSizeChanged(object sender, EventArgs e)
        {
            ResetProjection();
        }

        protected void ResetProjection()
        {
            Viewport viewport = graphics.GraphicsDevice.Viewport;

            // Set the Projection Matrix
            effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(30),
                (float)viewport.Width / viewport.Height,
                0.1f,
                100000.0f);
            Projection = effect.Projection;
        }
        #endregion

        #region Content
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            textFont = Content.Load<SpriteFont>("Fonts\\font");
            ScoreFont = Content.Load<SpriteFont>("Fonts\\scoreFont");
            HUDText = Content.Load<SpriteFont>("Fonts\\otherfont");
            MainMenuFont = Content.Load<SpriteFont>("Fonts\\MainMenuFont");
            InstructionFont = Content.Load<SpriteFont>("Fonts\\instructfont");

            boxTexture = Content.Load<Texture2D>("Textures\\1");
            HUDtex = Content.Load<Texture2D>("Textures\\crate129c");
            vortexTexture = Content.Load<Texture2D>("Textures\\VortexTex");
            dividerMenu = Content.Load<Texture2D>("Textures\\divider");

            MainMenuVideo = Content.Load<Video>("last_hope_WMV V9");
            videoPlayer = new VideoPlayer();
            videoPlayer.Stop();
            
            ResetProjection();

            InitEffect();

            #region BlockColorFill
            blockColors[0] = new Color(0, 100, 255, 100);
            blockColors[1] = new Color(0, 255, 120, 100);
            blockColors[2] = new Color(252, 255, 0, 100);
            blockColors[3] = new Color(255, 133, 2, 100);
            blockColors[4] = new Color(255, 2, 2, 100);
            blockColors[5] = Color.Gold;
            #endregion
        }

        protected override void UnloadContent()
        {
        }
        #endregion

        #region Loop
        #region Input
        private void Input()
        {
            KeyboardState KeyState = Keyboard.GetState();

            KeyState = Keyboard.GetState();
            currMouseState = Mouse.GetState();

            if (KeyState.IsKeyDown(Keys.F12))
                IsFullScreen = !(IsFullScreen);

            #region Main Menu
            if (MainMenu)
            {
                IsMouseVisible = false;
            }
            #endregion

            #region Menu
            if (menu)
            {
                IsMouseVisible = true;

                if (KeyState.IsKeyDown(Keys.Enter) && songSelected == true)
                {
                    loader = true;
                    menu = false;
                }
                if (KeyState.IsKeyDown(Keys.Escape))
                    if (!currKeyState.IsKeyDown(Keys.Escape))
                    {
                        this.Exit();
                    }

                if (KeyState.IsKeyDown(Keys.Up))
                    index-=3;
                else
                if (KeyState.IsKeyDown(Keys.Down))
                    index+=3;
            }
            #endregion

            #region game
            if (game)
            {
                IsMouseVisible = false;

                if (KeyState.IsKeyDown(Keys.Escape))
                {
                    MediaPlayer.Stop();
                    songSelected = false;
                    songToPlay = null;
                    songs.Clear();
                    totalSongs = null;
                    MainMenu = true;
                    game = false;
                }
            }
            #endregion

            currKeyState = KeyState;
        }
        #endregion

        #region GameUpdate
        protected override void Update(GameTime gameTime)
        {
            Input();

            #region Shader Cam Params
            //////////////////////////
            minPointSource = gDevice.Viewport.Unproject(new Vector3(currMouseState.X, currMouseState.Y, 0),
                                                        Projection,
                                                        Matrix.CreateLookAt(cameraPos, camTarget, Vector3.Up),
                                                        Matrix.Identity);
            prevCam = new Vector3(minPointSource.X * 500, minPointSource.Y * 500, 150.0f);
            //////////////////////////
            effect.View = Matrix.CreateLookAt(prevCam, camTarget, new Vector3(0.0f, 1.0f, 0.0f));
            View = effect.View;
            effect.ViewxProjection = View * Projection;
            //////////////////////////
            //Frustum Bounds update
            if (!(Projection == null && View == null))
            {
                Matrix FrustumMatrix = View * Projection;
                frustumBounds = new BoundingFrustum(FrustumMatrix);

                leftEnd = (frustumBounds.Left.D);
                rightEnd = -(frustumBounds.Right.D);
                topEnd = -(frustumBounds.Top.D);
                bottomEnd = (frustumBounds.Bottom.D);
            }
            //////////////////////////
            #endregion

            #region Main Menu Upd
            if (MainMenu)
            {
                if (videoPlayer.State == MediaState.Stopped)
                {
                    videoPlayer.IsLooped = true;
                    videoPlayer.Play(MainMenuVideo);
                }
            }
            #endregion

            #region menuUpd
            if (menu)
            {
                videoPlayer.Stop();
                MediaPlayer.Stop();
                getSongs();
                songSelect();

                if (index >= 0)
                    index = 0;

                if (songCount > 0)
                {
                    if (index <= (-(songCount * 25) + screenHeight))
                    {
                        index = (-(songCount * 25) + screenHeight);
                    }
                }
            }
            #endregion

            #region loadUpd
            if (loadAction)
            {
                IsMouseVisible = false;
                score = 0;
                scoreIncrement = 0;
                MediaPlayer.Stop();
                if (songToPlay == null)
                {
                    //songToPlay = fileSystem.CopyToGameFolder(songs[selectedDrive]);

                    if (totalSongs != null)
                    songToPlay = totalSongs[selectedDrive];
                }

                if (songToPlay != null)
                {
                    soundProcess.LoadSong(songToPlay);
                    game = true;
                    loader = false;
                    loadAction = false;
                }

                #region GameInit
                blockZTranslate.Clear();
                blockToDraw.Clear();
                InteractiveBlockColors.Clear();
                blockCoords.Clear();
                blockCount = 0;

                DudBlockCoords.Clear();
                DudBlockZ.Clear();
                dudblockToDraw.Clear();
                dudCount = 0;

                blockZTranslateOuter.Clear();
                blockToDrawOuter.Clear();
                blockCountOuter = 0;
                #endregion
            }
            #endregion

            #region ingameUpd
            if (game)
            {
                bassTransform = soundProcess.BeginPlayer();

                if (bassTransform > 0 || soundProcess.cymbals > 0)
                {
                    beatCount++;
                }

                if (soundProcess.bass > 0)
                    blockCountOuter++;

                if (soundProcess.mids > 0)
                {
                    beatCountDud++;
                }

                if (beatCount >= 7)
                {
                    blockCount++;
                    beatCount = 0;
                }

                if (beatCountDud >= 15)
                {
                    dudCount++;
                    beatCountDud = 0;
                }

                if (scoreIncrement < score)
                {
                    if (score - scoreIncrement < 500)
                        scoreIncrement += 50;
                    else if (score - scoreIncrement < 2000)
                        scoreIncrement += 200;
                    else if (score - scoreIncrement < 4000)
                        scoreIncrement += 400;
                    else if (score - scoreIncrement < 7000)
                        scoreIncrement += 700;
                    else if (score - scoreIncrement < 10000)
                        scoreIncrement += 1000;
                    else
                        scoreIncrement += 9000;
                }

                if (scoreIncrement >= score)
                {
                    //if (scoreIncrement - score  < 500)
                    //    scoreIncrement -= 50;
                    //else if (scoreIncrement - score < 2000)
                    //    scoreIncrement -= 750;
                    //else if (scoreIncrement - score < 4000)
                    //    scoreIncrement -= 2500;
                    //else if (scoreIncrement - score  < 7000)
                    //    scoreIncrement -= 5000;
                    //else if (scoreIncrement - score  < 1000)
                    //    scoreIncrement -= 8500;
                    //else
                    //    scoreIncrement -= 9000;
                    scoreIncrement = score;
                }

                if (score > HighScore)
                {
                    HighScore = score;
                }

                if (MediaPlayer.PlayPosition >= soundProcess.songToPlay.Duration || MediaPlayer.State == MediaState.Stopped)
                {
                    songSelected = false;
                    songToPlay = null;
                    songs.Clear();
                    totalSongs = null;
                    MainMenu = true;
                    game = false;
                }

                if (score < 0)
                    score = 0;

                if (multiplier >= 4)
                    multiplier = 4;

                if (negativeMultiplier >= 3)
                    negativeMultiplier = 3;
            }
            #endregion

            base.Update(gameTime);
        }
        #endregion

        #region GameDraw
        protected override void Draw(GameTime gameTime)
        {
            _gameTime = gameTime;
            gDevice.BlendState = BlendState.Opaque;
            gDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.Clear(Color.Black);
            #region Main Menu
            if (MainMenu)
            {
                IsMouseVisible = false;
                effect.ColorFromSource = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);

                #region Main Menu Buttons to Render Target
                gDevice.SetRenderTarget(NewGameButton);
                gDevice.Clear(Color.Transparent);
                
                spriteBatch.Begin();
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                Vector2 NewGamePixel = MainMenuFont.MeasureString("New Game");
                spriteBatch.DrawString(MainMenuFont, "New Game",
                                       new Vector2(screenWidth - NewGamePixel.X, screenHeight - NewGamePixel.Y),
                                       Color.White);
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                spriteBatch.DrawString(MainMenuFont, "Vibrancy", new Vector2(screenWidth - NewGamePixel.X, screenHeight - NewGamePixel.Y * 4), Color.White);

                spriteBatch.End();

                gDevice.SetRenderTarget(null);

                gDevice.SetRenderTarget(ExitGameButton);
                gDevice.Clear(Color.Transparent);

                spriteBatch.Begin();
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                Vector2 ExitGamePixel = MainMenuFont.MeasureString("Exit");
                spriteBatch.DrawString(MainMenuFont, "Exit",
                                       new Vector2(0, 0),
                                       Color.White);
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                spriteBatch.End();

                gDevice.SetRenderTarget(null);
                #endregion

                #region Main menu Full Scene to RT
                gDevice.SetRenderTarget(MainMenuScene);
                gDevice.Clear(Color.Transparent);

                #region New Game Button
                effect.Texture = NewGameButton;
                gameBlocks.DrawBox(new Vector3(-15.0f, 45.0f, -100.0f), new Vector3(0.0f, 20.0f, 0.0f), new Vector3(100, 200, 0.0001f));
                ///////////////////////////////////////////////////////////////
                Vector3[] newGameBB = gameBlocks.bBox.GetCorners();

                Matrix NewGameWorld = effect.World;
                Vector3.Transform(newGameBB, ref NewGameWorld, newGameBB);

                NewGame = BoundingBox.CreateFromPoints(newGameBB);
                ///////////////////////////////////////////////////////////////
                #endregion

                #region Exit Game Button
                effect.Texture = ExitGameButton; 
                gameBlocks.DrawBox(new Vector3(125.0f, -65.0f, -100.0f), new Vector3(0.0f, 20.0f, 0.0f), new Vector3(100.0f, 200.0f, 0.0001f));
                ///////////////////////////////////////////////////////////////
                Vector3[] exitGameBB = gameBlocks.bBox.GetCorners();

                Matrix ExitGameWorld = effect.World;
                Vector3.Transform(exitGameBB, ref ExitGameWorld, exitGameBB);

                ExitGame = BoundingBox.CreateFromPoints(exitGameBB);
                ///////////////////////////////////////////////////////////////
                #endregion

                #region pointer
                effect.Texture = boxTexture;
                Vector3 pointerMenu = new Vector3(minPointSource.X * 3000, minPointSource.Y * 3000, -100.0f);

                gameBlocks.DrawBox(pointerMenu, Vector3.Zero, new Vector3(1.0f, 1.0f, 10.0f));

                ///////////////////////////////////////////////////////////////
                Vector3[] mousePointerBB = gameBlocks.bBox.GetCorners();

                Matrix mouseWorld = effect.World;
                Vector3.Transform(mousePointerBB, ref mouseWorld, mousePointerBB);

                mousePointer = BoundingBox.CreateFromPoints(mousePointerBB);
                ///////////////////////////////////////////////////////////////.
                #endregion

                #region Menu coll check
                if (mousePointer.Intersects(NewGame) && currMouseState.LeftButton == ButtonState.Pressed)
                {
                    menu = true;
                    MainMenu = false;
                }

                if (mousePointer.Intersects(ExitGame) && currMouseState.LeftButton == ButtonState.Pressed)
                {
                    this.Exit();
                }
                #endregion

                gDevice.SetRenderTarget(null);
                #endregion

                #region Effect Processing
                //////Extract Excess Light
                DrawScreenQuad(bloomTarget, MainMenuScene, lightExtract, screenWidth, screenHeight);
                //////Blur X&Y
                DrawScreenQuad(blurTarget, bloomTarget, gaussianBlur, screenWidth, screenHeight);
                //////Combine Scene and Blur(Additive Combine)
                gDevice.Textures[1] = MainMenuScene;
                DrawScreenQuad(bloomTarget, blurTarget, Bloom, screenWidth, screenHeight);
                #endregion

                #region SpriteBatchforMenu
                gDevice.Clear(Color.Black);
                spriteBatch.Begin(/*SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.AnisotropicClamp, DepthStencilState.Default, null*/);

                if (videoPlayer.State != MediaState.Stopped)
                {
                    videoPlayer.IsMuted = true;
                    spriteBatch.Draw(videoPlayer.GetTexture(), new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
                }

                spriteBatch.Draw(bloomTarget, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);      

                spriteBatch.End();
                #endregion
            }
            #endregion

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, DepthStencilState.Default, null);
            #region MenuDrawing
            if (menu)
            {
                //Song Selection screen
                if (songs != null)
                {
                    for (int i = 0; i < songs.Count; i++)
                    {
                        if (songs != null)
                        {
                            Vector2 songPixelLength = textFont.MeasureString(songs[i]);
                            spriteBatch.DrawString(textFont, songs[i], new Vector2(10, height), Color.White);
                            spriteBatch.Draw(dividerMenu, new Rectangle(0, height, (int)songPixelLength.X + 10, 25), Color.White);
                            height += (int)songPixelLength.Y;
                            songPixelLengthGlobal = (int)songPixelLength.Y;
                        }        
                    }
                    height = index;
                    if (songSelected)
                    {
                        spriteBatch.DrawString(textFont, "Selected Song: " + Path.GetFileNameWithoutExtension(songs[selectedDrive]), new Vector2(screenWidth / 2, 20), Color.White);
                        spriteBatch.DrawString(textFont, "Hit Enter to play...", new Vector2(screenWidth / 2, 40), Color.White);
                    }
                    else
                    {
                        spriteBatch.DrawString(textFont, "Selected Song: ", new Vector2(screenWidth / 2, 20), Color.White);
                    }
                }
            }
            #endregion

            #region Pre-Loader Drawing
            if (loader)
            {
                spriteBatch.DrawString(textFont, "Loading", new Vector2(screenWidth / 2, screenHeight / 2), Color.White);
                loadAction = true;
            }
            #endregion
            spriteBatch.End();

            #region Game Drawing
            if (game)
            {
                #region ToBeDeleted??
                //Color blockColor =  Color.White;
                //float Red = 0, Green = 0, Blue = 0;

                //if (soundProcess.bass <= 0.3f)
                //{
                //    Blue = (0.6f - soundProcess.bass);
                //    Green = soundProcess.bass;
                //    Red = 0;
                //}
                //else if (soundProcess.bass > 0.3f && soundProcess.bass <= 0.6f)
                //{
                //    Blue = (0.6f - soundProcess.bass);
                //    Green = soundProcess.bass;
                //    Red = -(0.3333f - soundProcess.bass);
                //}
                //else if (soundProcess.bass > 0.6f)
                //{
                //    Red = -(0.3333f - soundProcess.bass);
                //    Green = 0.9999f - soundProcess.bass;
                //    Blue = 0;
                //}

                //blockColor = new Color(Red * 2, Green * 2, Blue * 2);
                #endregion

                Color blockColor = Color.Black;

                if (soundProcess.bass < 0.35f)
                    blockColor = blockColors[0];
                else if (soundProcess.bass < (0.35f) && soundProcess.bass >= (0.32f))
                    blockColor = blockColors[1];
                else if (soundProcess.bass < (0.38f) && soundProcess.bass >= (0.35f))
                    blockColor = blockColors[2];
                else if (soundProcess.bass < (0.40f) && soundProcess.bass >= (0.38f))
                    blockColor = blockColors[3];
                else if (soundProcess.bass < (0.49f) && soundProcess.bass >= (0.40f))
                    blockColor = blockColors[4];
                else if (soundProcess.bass < (0.60f) && soundProcess.bass >= (0.49f))
                    blockColor = blockColors[5];
                else if (soundProcess.bass >= (0.60f))
                    blockColor = blockColors[5];

                effect.ColorFromSource = new Vector4(1.0f, 1.0f, 1.0f, 0.2f);

                #region ObjectGrouping

                #region HUD
                #region ScoreArea
                gDevice.SetRenderTarget(HUDTraget);
                gDevice.Clear(Color.Transparent);

                Vector2 scoreTextPixelSize = HUDText.MeasureString("Score: ");
                Vector2 ScorePixelSize = ScoreFont.MeasureString(score.ToString());

                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.AnisotropicClamp, DepthStencilState.Default, null);

                spriteBatch.DrawString(HUDText, "Score ", new Vector2((200), (screenHeight / 2 - ScorePixelSize.Y / 2) - 20), Color.White);
                spriteBatch.DrawString(ScoreFont, scoreIncrement.ToString(), new Vector2((0 + 200), (screenHeight / 2 - scoreTextPixelSize.Y / 2)), Color.White);

                Vector2 multiplierTextPixelSize = HUDText.MeasureString("Multiplier: ");
                if (multiplier > 1)
                {
                    if (multiplier == 2)
                        spriteBatch.DrawString(HUDText, " x " + multiplier, new Vector2((multiplierTextPixelSize.X + 60), (screenHeight / 2- ScorePixelSize.Y / 2) - 20), blockColors[1]);
                    if (multiplier == 3)
                        spriteBatch.DrawString(HUDText, " x " + multiplier, new Vector2((multiplierTextPixelSize.X + 60), (screenHeight / 2 - ScorePixelSize.Y / 2) - 20), blockColors[2]);
                    if (multiplier == 4)
                        spriteBatch.DrawString(HUDText, " x " + multiplier, new Vector2((multiplierTextPixelSize.X + 60), (screenHeight / 2 - ScorePixelSize.Y / 2) - 20), blockColors[3]);
                }
                else if (negativeMultiplier > 1)
                    spriteBatch.DrawString(HUDText, " x -" + negativeMultiplier, new Vector2((multiplierTextPixelSize.X + 60), (screenHeight / 2 - ScorePixelSize.Y / 2) - 20), blockColors[0]);
                else
                    spriteBatch.DrawString(HUDText, "x 1 ", new Vector2((multiplierTextPixelSize.X + 50), (screenHeight / 2 - ScorePixelSize.Y / 2) - 20), blockColors[0]);

                if (HighScore > 0)
                {
                    spriteBatch.DrawString(HUDText, "Highest: " + HighScore.ToString(), new Vector2((0 + 200), (screenHeight / 2 + scoreTextPixelSize.Y / 2) + 75), Color.White);
                }

                spriteBatch.End();

                gDevice.SetRenderTarget(null);
                #endregion

                #region Instructions
                gDevice.SetRenderTarget(InstrucionTarget);
                gDevice.Clear(Color.Transparent);
                spriteBatch.Begin();

                if (gameTime.TotalGameTime.Ticks % 300 == 0)
                {
                    InstructionsSwitch = !(InstructionsSwitch);
                }

                if (InstructionsSwitch)
                {
                    spriteBatch.DrawString(InstructionFont, "Collect the Green,\ndon't forget the Red,\nbeware of the White,\nthey'll eat you're Head!", new Vector2(screenWidth - 550, screenHeight / 2 - 50), Color.White);
                }

                if (!InstructionsSwitch)
                {
                    spriteBatch.DrawString(InstructionFont, "Move the mouse,\nor you're finger\n(if you're on touch),\ncollect the blocks,\nand make you're score,\nBIGGER!", new Vector2(screenWidth - 550, screenHeight / 2 - 50), Color.White);
                }

                spriteBatch.End();
                gDevice.SetRenderTarget(null);
                #endregion 
                #endregion

                #region FinalHUD
                gDevice.SetRenderTarget(finalHUDTarget);
                gDevice.Clear(Color.Transparent);

                effect.Texture = HUDTraget;
                gameBlocks.DrawBox(new Vector3(leftEnd - 50, 0.0f, -500.0f), new Vector3(0.0f, 90.0f, 0.0f), new Vector3(500.0f, 600.0f, 0.00001f));

                effect.Texture = InstrucionTarget;
                gameBlocks.DrawBox(new Vector3(rightEnd + 50, 0.0f, -500.0f), new Vector3(0.0f, 90.0f, 0.0f), new Vector3(500.0f, 600.0f, 0.00001f));

                gDevice.SetRenderTarget(null);
                #endregion

                #region Game Scene
                gDevice.SetRenderTarget(sceneTarget);
                gDevice.Clear(Color.Transparent);

                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.AnisotropicClamp, DepthStencilState.Default, null);

                spriteBatch.Draw(finalHUDTarget, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);

                effect.Texture = boxTexture;
                DrawLines();
                DrawModel(blockColor);

                spriteBatch.End();
                gDevice.SetRenderTarget(null);
                #endregion

                #endregion

                #region Effect Processing
                /////For Game Scene////////////////////////////////////////////////////////
                //////Extract Excess Light
                DrawScreenQuad(bloomTarget, sceneTarget, lightExtract, screenWidth, screenHeight);
                //////Blur X&Y
                gaussianBlur.Parameters["BlurStrength"].SetValue(7.0f);
                DrawScreenQuad(blurTarget, bloomTarget, gaussianBlur, screenWidth, screenHeight);
                //////Combine Scene and Blur(Additive Combine)
                gDevice.Textures[1] = sceneTarget;
                DrawScreenQuad(bloomTarget, blurTarget, Bloom, screenWidth, screenHeight);
                #endregion
                
                #region SpriteBatchforGame
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.AnisotropicClamp, DepthStencilState.Default, null);
                gDevice.Clear(Color.Black);

                Vector2 pauseText = HUDText.MeasureString("Move the pointer up here and keep it here for a few seconds to return to the menu");
                
                spriteBatch.Draw(bloomTarget, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
                if (currMouseState.X >= screenWidth || currMouseState.X <= 0)
                    spriteBatch.DrawString(HUDText, "Move the pointer up here and keep it here for a few seconds to return to the menu", new Vector2(screenWidth / 2 - pauseText.X / 2, 10), Color.White);

                spriteBatch.End();
                #endregion
            }
            #endregion

            ResetProjection();
            base.Draw(gameTime);
        }
        #endregion
        #endregion

        #region Game Elements
        #region Song Selection Portions
        private void getSongs()
        {
            if (menu)
            {
                if (totalSongs == null)
                    totalSongs = songsInLibrary.FetchSongsFromLibrary();
            

                if (totalSongs != null)
                {
                    if (songs.Count < totalSongs.Count)
                    {
                        foreach (Song song in totalSongs)
                        {
                            songs.Add(song.Name);
                        }
                    }
                }

                if (songs != null)
                    songCount = songs.Count;
            }
        }

        private void songSelect()
        {
            if (songPixelLengthGlobal > 0)
            {
                if (currMouseState.Y % songPixelLengthGlobal > height && currMouseState.LeftButton == ButtonState.Pressed)
                {
                    selectedDrive = Math.Abs((currMouseState.Y - height) / songPixelLengthGlobal);

                    if (selectedDrive >= songCount)
                        selectedDrive = songCount - 1;

                    songSelected = true;
                }
            }
        }
        #endregion

        #region Draw Portions
        private void DrawModel(Color _blockColor)
        {
            effect.Texture = boxTexture;

            #region Static back
            Vector3 basePosition = new Vector3(((leftEnd + 5) + (rightEnd - 5))/2, bottomEnd + 10 , -500);
            Vector3 basePositionOuter = new Vector3(((leftEnd + rightEnd)/2), bottomEnd - 15, -2000);
            Vector3 collBox = new Vector3(minPointSource.X * 1500, minPointSource.Y * 1500, 40.0f);
            
            for (int i = 0; i < boxPositions.Length; i++)
            {
                ///////////////////////////////////////////////////////////////
                boxPositions[i] = Vector3.Transform(basePosition, Matrix.CreateRotationZ(MathHelper.ToRadians(30 * i)));
                ///////////////////////////////////////////////////////////////
            }

            for (int i = 0; i < boxPositionsOuter.Length; i++)
            {
                boxPositionsOuter[i] = Vector3.Transform(basePositionOuter, Matrix.CreateRotationZ(MathHelper.ToRadians(15 * i)));
            }

            gameBlocks.DrawBox(collBox, Vector3.Zero, new Vector3(1.0f, 1.0f, 7.0f));
            ///////////////////////////////////////////////////////////////
            Vector3[] otherObjectBB = gameBlocks.bBox.GetCorners();

            Matrix World = effect.World;
            Vector3.Transform(otherObjectBB, ref World, otherObjectBB);

            newBBox1[0] = BoundingBox.CreateFromPoints(otherObjectBB);
            ///////////////////////////////////////////////////////////////
            #endregion

            #region Interactive
                for (int a = 0; a <= blockCount && blockCount < 30000; a++)
                {
                    blockZTranslate.Add(-601.0f);
                    blockToDraw.Add(randomNumber.Next(0, boxPositions.Length));

                    #region Block Coords
                    if ((int)boxPositions[blockToDraw[a]].X >= 0 && (int)boxPositions[blockToDraw[a]].Y >= 0)
                        blockCoords.Add(new Vector2(randomNumber.Next(0, (int)boxPositions[blockToDraw[a]].X), 
                                                    randomNumber.Next(0, (int)boxPositions[blockToDraw[a]].Y)));
                    else
                     if ((int)boxPositions[blockToDraw[a]].X < 0 && (int)boxPositions[blockToDraw[a]].Y >= 0)
                        blockCoords.Add(new Vector2(randomNumber.Next((int)boxPositions[blockToDraw[a]].X, 0), 
                                                    randomNumber.Next(0, (int)boxPositions[blockToDraw[a]].Y)));
                    else
                     if ((int)boxPositions[blockToDraw[a]].X >= 0 && (int)boxPositions[blockToDraw[a]].Y < 0)
                        blockCoords.Add(new Vector2(randomNumber.Next(0, (int)boxPositions[blockToDraw[a]].X), 
                                                    randomNumber.Next((int)boxPositions[blockToDraw[a]].Y, 0)));
                    else
                     if ((int)boxPositions[blockToDraw[a]].X < 0 && (int)boxPositions[blockToDraw[a]].Y < 0)
                        blockCoords.Add(new Vector2(randomNumber.Next((int)boxPositions[blockToDraw[a]].X, 0), 
                                                    randomNumber.Next((int)boxPositions[blockToDraw[a]].Y, 0)));
                    #endregion

                    InteractiveBlockColors.Add(new Vector4(_blockColor.ToVector4().X, _blockColor.ToVector4().Y, _blockColor.ToVector4().Z, 1.0f));
                    blockHit = false;
                    if (blockZTranslate[a] < 160.0f)
                    {
                        ///////////////////////////////////////////////////////////////
                        blockZTranslate[a]++;
                        blockZTranslate[a] += (soundProcess.bass * 10);
                        effect.ColorFromSource = InteractiveBlockColors[a];
                        gameBlocks.DrawBox(new Vector3(blockCoords[a], blockZTranslate[a]), new Vector3(0.0f, 0.0f, 0.0f), new Vector3(1.0f, 4.0f, 20.0f) * 2);
                        ///////////////////////////////////////////////////////////////
                        Vector3[] ObjectBB = gameBlocks.bBox.GetCorners();
                        Matrix rotationTransform = Matrix.CreateRotationZ(blockToDraw[a] * 30);

                        World = effect.World;
                        Vector3.Transform(ObjectBB, ref World, ObjectBB);

                        newBBox = BoundingBox.CreateFromPoints(ObjectBB);
                        ///////////////////////////////////////////////////////////////
                        for (int k = 0; k < 1; k++)
                        {
                            if (newBBox.Intersects(newBBox1[k]) == true)
                            {
                                if (_blockColor == blockColors[0])
                                    score += 1000;
                                if (_blockColor == blockColors[1])
                                    score += 1500;
                                if (_blockColor == blockColors[2])
                                    score += 2000;
                                if (_blockColor == blockColors[3])
                                    score += 3000;
                                if (_blockColor == blockColors[4])
                                    score += 4000;
                                if (_blockColor == blockColors[5])
                                    score += 5000;
                                blockHit = true;
                            }
                        }
                        ///////////////////////////////////////////////////////////////
                    }
                    else if ((blockZTranslate[a] >= 160.0f && blockCount > 1))
                    {
                        blockZTranslate.RemoveAt(a);
                        blockToDraw.RemoveAt(a);
                        InteractiveBlockColors.RemoveAt(a);
                        blockCoords.RemoveAt(a);
                        multiplier = 0;
                        blockCount--;
                    }

                    if (blockHit)
                    {
                        multiplier++;
                        blockZTranslate.RemoveAt(a);
                        blockToDraw.RemoveAt(a);
                        InteractiveBlockColors.RemoveAt(a);
                        blockCoords.RemoveAt(a);
                        blockCount--;

                        if (multiplier > 1)
                        {
                            if (_blockColor == blockColors[0])
                                score += (500 * multiplier);
                            if (_blockColor == blockColors[1])
                                score += (750 * multiplier);
                            if (_blockColor == blockColors[2])
                                score += (1000 * multiplier);
                            if (_blockColor == blockColors[3])
                                score += (1500 * multiplier);
                            if (_blockColor == blockColors[4])
                                score += (2000 * multiplier);
                            if (_blockColor == blockColors[5])
                                score += (2100 * multiplier);
                        }
                    }
                }
            #endregion

            #region Duds
            for (int b = 0; b <= dudCount && dudCount <= 30000; b++)
            {
                DudBlockZ.Add(-601.0f);
                dudblockToDraw.Add(randomNumber.Next(0, boxPositions.Length));
                #region Dud Coords
                if ((int)boxPositions[dudblockToDraw[b]].X >= 0 && (int)boxPositions[dudblockToDraw[b]].Y >= 0)
                    DudBlockCoords.Add(new Vector2(randomNumber.Next(0, (int)boxPositions[dudblockToDraw[b]].X), randomNumber.Next(0, (int)boxPositions[dudblockToDraw[b]].Y)));
                else if ((int)boxPositions[dudblockToDraw[b]].X < 0 && (int)boxPositions[dudblockToDraw[b]].Y >= 0)
                    DudBlockCoords.Add(new Vector2(randomNumber.Next((int)boxPositions[dudblockToDraw[b]].X, 0), randomNumber.Next(0, (int)boxPositions[dudblockToDraw[b]].Y)));
                else if ((int)boxPositions[dudblockToDraw[b]].X >= 0 && (int)boxPositions[dudblockToDraw[b]].Y < 0)
                    DudBlockCoords.Add(new Vector2(randomNumber.Next(0, (int)boxPositions[dudblockToDraw[b]].X), randomNumber.Next((int)boxPositions[dudblockToDraw[b]].Y, 0)));
                else if ((int)boxPositions[dudblockToDraw[b]].X < 0 && (int)boxPositions[dudblockToDraw[b]].Y < 0)
                    DudBlockCoords.Add(new Vector2(randomNumber.Next((int)boxPositions[dudblockToDraw[b]].X, 0), randomNumber.Next((int)boxPositions[dudblockToDraw[b]].Y, 0)));
                #endregion
                blockHit = false;
                if (DudBlockZ[b] <= 160.0f)
                {
                    ///////////////////////////////////////////////////////////////
                    DudBlockZ[b]++;
                    DudBlockZ[b] += (soundProcess.bass * 10);
                    effect.ColorFromSource = new Vector4(0.7f, 0.7f, 0.7f, 1.0f);
                    gameBlocks.DrawBox(new Vector3(DudBlockCoords[b], DudBlockZ[b]), new Vector3(0.0f, 0.0f, 0.0f), new Vector3(1.0f, 4.0f, 20.0f) * 2);
                    ///////////////////////////////////////////////////////////////
                    Vector3[] ObjectBB = gameBlocks.bBox.GetCorners();
                    Matrix rotationTransform = Matrix.CreateRotationZ(dudblockToDraw[b] * 30);

                    Matrix DudWorld = effect.World;
                    Vector3.Transform(ObjectBB, ref DudWorld, ObjectBB);

                    newBBox = BoundingBox.CreateFromPoints(ObjectBB);
                    ///////////////////////////////////////////////////////////////
                    for (int k = 0; k < 1; k++)
                    {
                        if (newBBox.Intersects(newBBox1[k]) == true)
                        {
                            score -= 1000;
                            blockHit = true;
                        }
                    }
                    ///////////////////////////////////////////////////////////////
                }
                else if (DudBlockZ[b] > 160.0f && dudCount > 1)
                {
                    DudBlockCoords.RemoveAt(b);
                    DudBlockZ.RemoveAt(b);
                    dudblockToDraw.RemoveAt(b);
                    negativeMultiplier = 0;
                    dudCount--;
                }

                if (blockHit)
                {
                    negativeMultiplier++;
                    DudBlockCoords.RemoveAt(b);
                    DudBlockZ.RemoveAt(b);
                    dudblockToDraw.RemoveAt(b);
                    dudCount--;

                    if (negativeMultiplier > 1)
                        score -= (negativeMultiplier * 2000);
                }
            }
            #endregion
        }

        private void DrawLines()
        {
            if (soundProcess.bass < 0.3)
                effect.ColorFromSource = new Vector4(soundProcess.bass, 0.5f, 1 - soundProcess.bass, 1.0f); 
            else
                effect.ColorFromSource = new Vector4(soundProcess.bass, 0.5f, soundProcess.bass / 2, 1.0f); 
            #region Lines
            Vector3 baseLine = new Vector3((leftEnd + rightEnd) / 2, bottomEnd - 15, 0.0f);

            effect.Texture = vortexTexture;

            for (int i = 0; i < 24; i++)
            {
                gameBlocks.DrawBox(Vector3.Transform(baseLine, Matrix.CreateRotationZ(MathHelper.ToRadians(15) * i)),
                                   new Vector3(0.0f, 0.0f, 0.0f),
                                   new Vector3(0.1f, 0.2f, 1000.0f));
            }

            

            for (int j = 1; j < blockCountOuter && blockCountOuter <= 30000; j++)
            {
                #region sdfaas
                blockZTranslateOuter.Add(-1000.0f);
                blockToDrawOuter.Add(blockIndexOuter);
                blockIndexOuter++;

                if (blockIndexOuter >= boxPositionsOuter.Length)
                    blockIndexOuter = 0;
                #region Step1
                if (blockZTranslateOuter[j - 1] < 500.0f)
                {
                    blockZTranslateOuter[j - 1] += 2;
                    blockZTranslateOuter[j - 1] += (soundProcess.bass * 40);

                    if ((j - 1) % 20 == 0)
                    {
                        for (int i = 0; i < boxPositionsOuter.Length; i++)
                        {
                            gameBlocks.DrawBox(new Vector3(boxPositionsOuter[i].X, boxPositionsOuter[i].Y, blockZTranslateOuter[j - 1]), new Vector3(0.0f, 0.0f, 15 * i), new Vector3(7.0f, 0.5f, 0.5f));
                        }
                    }
                }
                #endregion

                #endregion
            }
            #endregion

            #region Block Cache Check
            if (blockCountOuter > 30000)
                blockCountOuter = 2000;

            if (dudCount > 30000)
                dudCount = 2000;

            if (blockToDrawOuter.Count > 500000)
                blockToDrawOuter.RemoveRange(100000, 300000);

            if (blockZTranslate.Count > 500000)
                blockZTranslate.RemoveRange(100000, 300000);

            if (blockZTranslateOuter.Count > 500000)
                blockZTranslateOuter.RemoveRange(100000, 300000);

            if (InteractiveBlockColors.Count > 500000)
                InteractiveBlockColors.Clear();

            if (DudBlockZ.Count > 500000)
                DudBlockZ.RemoveRange(100000, 300000);

            if (dudblockToDraw.Count > 500000)
                dudblockToDraw.RemoveRange(100000, 300000);

            if (DudBlockCoords.Count > 500000)
                DudBlockCoords.RemoveRange(100000, 300000);
            #endregion
        }
        #endregion

        #region Effect Params
        private void InitEffect()
        {
            #region Shader params
            //Shader Params
            effect.IsLightingEnabled = true;
            effect.AmbientLightColor = new Vector4(0.5f, 0.5f, 0.5f, 1.0f);
            effect.AmbientLightIntensity = 0.2f;

            effect.LightDirection = new Vector3(0.0f, 0.0f, 5.0f);
            effect.DiffuseColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            effect.DiffuseLightIntensity = 0.2f;

            effect.Shine = 500.0f;
            effect.SpecularColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            effect.SpecularIntensity = 1.0f;

            effect.ViewVector = new Vector3(0, 0, 1);

            effect.Texture = boxTexture;
            #endregion
        }
        #endregion

        #region Scene texture
        protected Texture2D ObtainSceneTexture(RenderTarget2D _renderTarget)
        {
            gDevice.SetRenderTarget(_renderTarget);
            gDevice.Clear(Color.Transparent);

            //Draw pre-loaded mesh...
            //DrawLines();

            gDevice.SetRenderTarget(null);

            return _renderTarget;
        }
        #endregion

        #region DrawScreenQuad
        public void DrawScreenQuad(RenderTarget2D _renderTarget, Texture2D texture, Effect effect, int width, int height)
        {
            gDevice.SetRenderTarget(_renderTarget);
            gDevice.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.AnisotropicClamp, DepthStencilState.Default, null, effect);

            spriteBatch.Draw(texture, new Rectangle(0, 0, width, height), Color.White);

            spriteBatch.End();
            gDevice.SetRenderTarget(null);
        }
        #endregion
        #endregion
    }
}
