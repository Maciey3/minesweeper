using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Channels;
using System.Windows.Forms;

namespace minesweeperWinForms
{
    public partial class Form1 : Form
    {
        private readonly int n = 6;
        private readonly int m = 6;
        public readonly int tileSize = 70;

        public Board board;
        public List<Button> buttons = new List<Button>();
        public bool gameOver = false;
        public bool gameWin = false;
        public int points = 0;

        public Form1() {
            this.board = new Board(n, m);
            InitializeComponent(board.n, board.m, tileSize);
            Shown += CreateMainButton;
            Shown += CreateButtonDelegate;
        }

        private void CreateMainButton(object sender, EventArgs e) {
            Button button = new Button();
            string src = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName, "images", "smiley.png");
            Image orginalImage = Image.FromFile(src);
            Image image = new Bitmap(orginalImage, new Size(this.tileSize - 25, this.tileSize - 25));
            button.Image = image;
            button.ImageAlign = ContentAlignment.MiddleCenter;
            button.Location = new Point(((board.n * tileSize + 150) - tileSize) / 2, 30);
            button.Size = new Size(tileSize, tileSize);
            button.Visible = true;
            button.BringToFront();
            Controls.Add(button);
            button.MouseDown += button_Reset;

        }
        private void CreateButtonDelegate(object sender, EventArgs e) {
            for (int i = 0; i < board.n; i++)
            {
                for (int j = 0; j < board.m; j++)
                {
                    Button button = new Button();
                    
                    button.Tag = new Tile(board.board[i, j], i, j, false);

                    button.BackColor = Color.FromArgb(230, 230, 230);
                    button.Location = new Point(70 + i * tileSize, 120 + j * tileSize);
                    button.Size = new Size(tileSize, tileSize);
                    button.Visible = true;
                    button.BringToFront();
                    Controls.Add(button);
                    button.MouseDown += button_MouseDown;
                    buttons.Insert(0, button);
                }

            }
        }

        private void deleteButtons() {
            int count = this.buttons.Count;
            for (int i = 0;i < count; i++) {
                Button buttonToRemove = this.buttons[0];
                this.buttons.Remove(buttonToRemove);
                this.Controls.Remove(buttonToRemove);
            }
        }

        private void button_Reset(object sender, MouseEventArgs e) {
            //MessageBox.Show("Reset!");
            this.board = new Board(n, m);
            this.deleteButtons();
            this.CreateButtonDelegate(null, null);
            this.gameOver = false;
            this.gameWin = false;
        }
        private void button_MouseDown(object sender, MouseEventArgs e) {
            if (this.gameWin || this.gameOver) { return; }

            switch (e.Button)
            {
                case MouseButtons.Left:
                    this.leftClick(sender);
                    break;

                case MouseButtons.Right:
                    this.rightClick(sender);
                    break;

                case MouseButtons.Middle:
                    this.log(sender);
                    //this.markFlags();
                    break;
            }

            this.checkWin();

            if (this.gameWin)
            {
                this.markFlags();
                MessageBox.Show("You won!");
            }

            if (this.gameOver) {
                this.uncoverAll();
                MessageBox.Show("You lost!");
            }
        }

        public void log(object sender) {
            Button button = (Button)sender;
            Tile tile = (Tile)button.Tag;
            Console.WriteLine(tile.Value);
        }

        public void leftClick(object sender) {
            Control[,] board = this.initBoard();

            Button button = (Button)sender;
            Tile tile = (Tile)button.Tag;

            if (this.gameOver || this.gameWin) {
                return;
            }

            if (tile.Clicked == true || tile.Marked) {
                return;
            }

            if (tile.Value == 0)
            {
                this.markZeroes(tile.PositionX, tile.PositionY, board);
            }
            else
            {
                this.uncoverTile(button);
            }
        }

        public void rightClick(object sender) {
            Color LIGHT_YELLOW = Color.FromArgb(255, 255, 179);
            Color PURPULE = Color.FromArgb(204, 51, 153);
            Color WHITE = Color.FromArgb(230, 230, 230);

            Control[,] board = this.initBoard();
            Button button = (Button)sender;
            Tile tile = (Tile)button.Tag;

            if (tile.Clicked == true) { return; }

            if (tile.Marked == false) {
                string src = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName, "images", "flag.png");
                Image orginalImage = Image.FromFile(src);
                Image image = new Bitmap(orginalImage, new Size(this.tileSize - 25, this.tileSize - 25));
                button.Image = image;
                button.ImageAlign = ContentAlignment.MiddleCenter;

                board[tile.PositionX, tile.PositionY].Tag = new Tile(tile.Value, tile.PositionX, tile.PositionY, tile.Clicked, true);
            }
            else{
                button.Image = null;
                board[tile.PositionX, tile.PositionY].Tag = new Tile(tile.Value, tile.PositionX, tile.PositionY, false); ;
            }
            
        }

        public void markZeroes(int n, int m, Control[,] board) {
            
            if (n < 0 || m < 0 || n >= this.board.n || m >= this.board.m) { return; }

            Tile tile = (Tile)board[n, m].Tag;

            if (tile.Clicked == true) { return; }

            if (tile.Value == 0)
            {
                board[n, m].Text = " ";
                board[n, m].Tag = new Tile(null, tile.PositionX, tile.PositionY, true);
                this.uncoverTile(board[n, m]);
                for (int i = n - 1; i <= n + 1; i++)
                {
                    for (int j = m - 1; j <= m + 1; j++)
                    {
                        markZeroes(i, j, board);
                    }
                }
            }
            else
            {
                board[n, m].Tag = new Tile(tile.Value, tile.PositionX, tile.PositionY, true);
                this.uncoverTile(board[n, m]);
            }

        }

        public void uncoverTile(Control c) {
            Color RED = Color.FromArgb(255, 51, 51);
            Color BLUE = Color.FromArgb(51, 51, 255);
            Color LIGHT_YELLOW = Color.FromArgb(255, 255, 179);
            Color ORANGE = Color.FromArgb(255, 117, 26);

            Button button = (Button)c;
            //Console.WriteLine(button.Tag);
            Tile tile = (Tile)button.Tag;

            
            
            button.Tag = new Tile(tile.Value, tile.PositionX, tile.PositionY, true);
            button.Font = new Font(button.Font.FontFamily, 11, FontStyle.Bold);
            button.BackColor = LIGHT_YELLOW;


            if (tile.Value == -1)
            {
                button.BackColor = RED;
                string src = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName, "images", "bomb.png");
                Image orginalImage = Image.FromFile(src);
                Image image = new Bitmap(orginalImage, new Size(this.tileSize - 25, this.tileSize - 25));
                button.Image = image;
                button.ImageAlign = ContentAlignment.MiddleCenter;

                this.gameOver = true;
            }
            else if (tile.Value == 0) {
                button.Text = " ";
                
            }
            else
            {
                button.Text = Convert.ToString(tile.Value);
                switch (tile.Value)
                {
                    case 1:
                        {
                            button.ForeColor = BLUE;
                            break;   
                        }
                    case 2:
                        {
                            button.ForeColor = ORANGE;
                            break;
                        }
                    default:
                        {
                            button.ForeColor = RED;
                            break;
                        }
                }
            }
        }

        public Control[,] initBoard() {
            Control[,] board;
            board = new Control[this.board.n, this.board.m];
            int i = 0;
            foreach (Control c in this.Controls)
            {
                i++;
                if (i == 1) { continue; }
                Tile tile = (Tile)c.Tag;
                //Console.WriteLine(tile.Value);

                board[tile.PositionX, tile.PositionY] = c;
            }

            return board;
        }

        public void checkWin() {
            int i = 0;
            foreach (Control c in this.Controls)
            {
                i++;
                if (i == 1) { continue; }
                Tile tile = (Tile)c.Tag;
                if (tile.Value != -1 && tile.Clicked != true) {
                    return;
                }
            }
            this.gameWin = true;
        }

        public void uncoverAll() {
            int i = 0;
            foreach (Control c in this.Controls) {
                i++;
                if (i == 1) { continue; }
                Tile tile = (Tile)c.Tag;
                if (tile.Marked == true) {
                    this.rightClick(c);
                }
                this.uncoverTile(c);
            }
        }

        public void markFlags() {
            int i = 0;
            foreach (Control c in this.Controls) {
                i++;
                if (i == 1) { continue; }
                Tile tile = (Tile)c.Tag;
                if (tile.Value == -1 && tile.Marked == false) {
                    this.rightClick(c);
                }
            }
        }
        private void Form1_Load(object sender, EventArgs e) { }
    }
}
