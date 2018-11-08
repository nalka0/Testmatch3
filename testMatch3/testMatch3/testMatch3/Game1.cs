using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Text;
using Discord.WebSocket;
using System.Threading.Tasks;
using testMatch3DLL;

namespace testMatch3
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        #region variables
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D gemmeRouge;
        Texture2D gemmeBleue;
        Texture2D gemmeJaune;
        Texture2D gemmeVerte;
        Texture2D gemmeBlanche;
        Texture2D gemmeNoire;
        Joueur joueurLocal;
        Joueur joueurDistant;
        ButtonState lastLeftState = ButtonState.Released;
        Vector2? selectedGemPos;
        SpriteFont font;
        KeyboardState keysState;
        DiscordSocketClient client = new DiscordSocketClient();
        Plateau board = new Plateau();
        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            client = new DiscordSocketClient();
            client.LoginAsync(Discord.TokenType.Bot, "MzYyMjE4NjE3NDg1NjU2MDY0.DKwjzA.jqBAQLN1Cd5lq1Vi1GitoN81qKk").GetAwaiter().GetResult();
            client.StartAsync().GetAwaiter().GetResult();
            client.MessageReceived += Client_MessageReceived;
            joueurLocal = new Joueur("Moi");
            joueurDistant = new Joueur("Toi");
            IsMouseVisible = true;
            Window.IsBorderless = true;
            //graphics.IsFullScreen = true;
            graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
            graphics.ApplyChanges();
            base.Initialize();
        }

        private Task Client_MessageReceived(SocketMessage arg)
        {
            string[] infos = arg.Content.Split('=');
            if (infos[0] == "Move" && joueurDistant.IsActive)
            {
                selectedGemPos = new Vector2(int.Parse(infos[1]), int.Parse(infos[2]));
                Directions direction = (Directions)int.Parse(infos[3]);
                Move(direction);
            }
            else if (infos[0] == "Init")
            {
                joueurLocal.IsActive = infos[1] == "true";
            }
            return new Task(() => { });
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            gemmeBlanche = Content.Load<Texture2D>("gemme blanche");
            gemmeNoire = Content.Load<Texture2D>("gemme noire");
            gemmeRouge = Content.Load<Texture2D>("gemme rouge");
            gemmeVerte = Content.Load<Texture2D>("gemme verte");
            gemmeJaune = Content.Load<Texture2D>("gemme jaune");
            gemmeBleue = Content.Load<Texture2D>("gemme bleue");
            font = Content.Load<SpriteFont>("font");
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            keysState = Keyboard.GetState();
            if (joueurLocal.IsActive)
            {
                if (Mouse.GetState().LeftButton == ButtonState.Pressed && lastLeftState == ButtonState.Released)
                    selectedGemPos = GetSelectedGemPos(Mouse.GetState().Position);
                if (keysState.IsKeyDown(Keys.Left))
                    Move(Directions.Gauche);
                else if (keysState.IsKeyDown(Keys.Right))
                    Move(Directions.Droite);
                else if (keysState.IsKeyDown(Keys.Up))
                    Move(Directions.Haut);
                else if (keysState.IsKeyDown(Keys.Down))
                    Move(Directions.Bas);
            }
            lastLeftState = Mouse.GetState().LeftButton;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            DrawPlateau();
            DrawScores();
            spriteBatch.DrawString(font, GetSelectedGemInfo(), new Vector2(GraphicsDevice.DisplayMode.Width - 50, GraphicsDevice.DisplayMode.Height - 50) - font.MeasureString(GetSelectedGemInfo()), Color.Black);
            spriteBatch.DrawString(font, joueurLocal.IsActive.ToString(), new Vector2(0, GraphicsDevice.DisplayMode.Height / 2 - 10), Color.Black);
            spriteBatch.End();
            base.Draw(gameTime);
        }

        #region Methodes

        #region obsolete
        private string convertMessage(byte[] message)
        {
            string ret = "";
            foreach (char element in Encoding.ASCII.GetChars(message))
            {
                ret += element;
            }
            return ret;
        }

        private byte[] convertMessage(string message)
        {
            return Encoding.ASCII.GetBytes(message);
        }
        #endregion

        private void DrawScores()
        {
            spriteBatch.DrawString(font, joueurLocal.Pseudo + " : " + joueurLocal.Score, new Vector2(50, 50), Color.Black);
            spriteBatch.DrawString(font, joueurDistant.Pseudo + " : " + joueurDistant.Score, new Vector2(GraphicsDevice.DisplayMode.Width - 50 - font.MeasureString(joueurDistant.Pseudo + " : " + joueurDistant.Score).X, 50), Color.Black);
        }

        private void DrawPlateau()
        {
            int ligne = 0;
            while (ligne < Plateau.rowCount * Plateau.columnCount)
            {
                foreach (Gemme item in board.gemmes[ligne / Plateau.rowCount])
                {
                    switch (item.Couleur)
                    {
                        case Couleurs.Blanc:
                            spriteBatch.Draw(gemmeBlanche, getGemPos(ligne), Color.White);
                            break;
                        case Couleurs.Bleu:
                            spriteBatch.Draw(gemmeBleue, getGemPos(ligne), Color.White);
                            break;
                        case Couleurs.Jaune:
                            spriteBatch.Draw(gemmeJaune, getGemPos(ligne), Color.White);
                            break;
                        case Couleurs.Noir:
                            spriteBatch.Draw(gemmeNoire, getGemPos(ligne), Color.White);
                            break;
                        case Couleurs.Rouge:
                            spriteBatch.Draw(gemmeRouge, getGemPos(ligne), Color.White);
                            break;
                        case Couleurs.Vert:
                            spriteBatch.Draw(gemmeVerte, getGemPos(ligne), Color.White);
                            break;
                    }
                    ligne++;
                }
            }
        }

        private void Move(Directions direction)
        {
            Swap(direction);
            try
            {
                board.faireMatch(joueurLocal.IsActive ? joueurLocal : (joueurDistant.IsActive ? joueurDistant : null));
            }
            catch (customException)
            {
                changeSelectedGem(direction);
                Swap(getOppositeDirection(direction));
                selectedGemPos = null;
                return;
            }
            //client.Client.Send(convertMessage("Move=" + selectedGemPos.Value.X + "=" + selectedGemPos.Value.Y + "=" + (int)direction));
            endTurn();
        }

        private void changeSelectedGem(Directions direction)
        {
            if (selectedGemPos != null)
            {
                switch (direction)
                {
                    case Directions.Droite:
                        selectedGemPos = new Vector2(selectedGemPos.Value.X, selectedGemPos.Value.Y + 1);
                        break;
                    case Directions.Gauche:
                        selectedGemPos = new Vector2(selectedGemPos.Value.X, selectedGemPos.Value.Y - 1);
                        break;
                    case Directions.Haut:
                        selectedGemPos = new Vector2(selectedGemPos.Value.X - 1, selectedGemPos.Value.Y);
                        break;
                    case Directions.Bas:
                        selectedGemPos = new Vector2(selectedGemPos.Value.X + 1, selectedGemPos.Value.Y);
                        break;
                    default:
                        throw new Exception();
                }
            }
        }

        private Directions getOppositeDirection(Directions direction)
        {
            switch (direction)
            {
                case Directions.Droite:
                    return Directions.Gauche;
                case Directions.Gauche:
                    return Directions.Droite;
                case Directions.Haut:
                    return Directions.Bas;
                case Directions.Bas:
                    return Directions.Haut;
                default:
                    throw new Exception();
            }
        }

        private Rectangle getGemPos(int gemPos)
        {
            int boardWidth = (88 * (Plateau.columnCount - 1) + 80);
            int boardHeight = (88 * (Plateau.rowCount - 1) + 80);
            return new Rectangle((GraphicsDevice.DisplayMode.Width - boardWidth) / 2 + (88 * (gemPos % Plateau.columnCount)), (GraphicsDevice.DisplayMode.Height - boardHeight) / 2 + (88 * (gemPos / Plateau.rowCount)), 80, 80);
        }

        private Vector2? GetSelectedGemPos(Point mousePosition) 
        {
            Rectangle mouseRectangle = new Rectangle(mousePosition.X, mousePosition.Y, 1, 1);
            int i = 0;
            while (i < Plateau.columnCount * Plateau.rowCount)
            {
                if (getGemPos(i).Intersects(mouseRectangle))
                {
                    return new Vector2(i / Plateau.rowCount, i % Plateau.columnCount);
                }
                i++;
            }
            return null;
        }

        private string GetSelectedGemInfo()
        {
            string ret = "Selected gem : ";
            ret += selectedGemPos == null ? "none" : board.gemmes[(int)selectedGemPos.Value.X][(int)selectedGemPos.Value.Y].Couleur.ToString();
            ret += "\nLigne : ";
            ret += selectedGemPos == null ? "" : selectedGemPos.Value.X.ToString();
            ret += "\nColonne : ";
            ret += selectedGemPos == null ? "" : selectedGemPos.Value.Y.ToString();
            return ret;
        }

        private void Swap(Directions direction)
        {
            if (selectedGemPos != null && selectedGemPos.Value.X < 8 && selectedGemPos.Value.X > -1 && selectedGemPos.Value.Y < 8 && selectedGemPos.Value.Y > -1)
            {
                Gemme temp = new Gemme(board.gemmes[(int)selectedGemPos.Value.X][(int)selectedGemPos.Value.Y]);
                switch (direction)
                {
                    case Directions.Droite:
                        if (selectedGemPos.Value.Y < 7)
                        {
                            board.gemmes[(int)selectedGemPos.Value.X][(int)selectedGemPos.Value.Y] = board.gemmes[(int)selectedGemPos.Value.X][(int)selectedGemPos.Value.Y + 1];
                            board.gemmes[(int)selectedGemPos.Value.X][(int)selectedGemPos.Value.Y + 1] = temp;
                        }
                        else
                            return;
                        break;
                    case Directions.Gauche:
                        if (selectedGemPos.Value.Y > 0)
                        {
                            board.gemmes[(int)selectedGemPos.Value.X][(int)selectedGemPos.Value.Y] = board.gemmes[(int)selectedGemPos.Value.X][(int)selectedGemPos.Value.Y - 1];
                            board.gemmes[(int)selectedGemPos.Value.X][(int)selectedGemPos.Value.Y - 1] = temp;
                        }
                        else
                            return;
                        break;
                    case Directions.Haut:
                        if (selectedGemPos.Value.X > 0)
                        {
                            board.gemmes[(int)selectedGemPos.Value.X][(int)selectedGemPos.Value.Y] = board.gemmes[(int)selectedGemPos.Value.X - 1][(int)selectedGemPos.Value.Y];
                            board.gemmes[(int)selectedGemPos.Value.X - 1][(int)selectedGemPos.Value.Y] = temp;
                        }
                        else
                            return;
                        break;
                    case Directions.Bas:
                        if (selectedGemPos.Value.X < 7)
                        {
                            board.gemmes[(int)selectedGemPos.Value.X][(int)selectedGemPos.Value.Y] = board.gemmes[(int)selectedGemPos.Value.X + 1][(int)selectedGemPos.Value.Y];
                            board.gemmes[(int)selectedGemPos.Value.X + 1][(int)selectedGemPos.Value.Y] = temp;
                        }
                        else
                            return;
                        break;
                    default:
                        throw new customException("Fatal error, please contact the developers of the game");
                }
            }
        }

        private void endTurn()
        {
            joueurLocal.IsActive = !joueurLocal.IsActive;
            joueurDistant.IsActive = !joueurDistant.IsActive;
            selectedGemPos = null;
        }
        #endregion
    }
}