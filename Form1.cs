using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace minesweeperWinForms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Shown += CreateButtonDelegate;
        }

        private void CreateButtonDelegate(object sender, EventArgs e)
        {
            Board board = new Board(5, 6);

            for (int i = 0; i < board.n; i++)
            {
                for (int j = 0; j < board.m; j++)
                {
                    Button button = new Button();
                    if (board.board[i, j] == -1)
                    {
                        button.Tag = $"bomb|{i}|{j}";
                    }
                    else {
                        button.Tag = $"{board.board[i, j]}|{i}|{j}";
                    }
                    
                    button.Text = "button " + (j + 1) + " " + (i + 1);
                    button.Location = new Point(70 + i * 50, 70 + j * 50);
                    button.Size = new Size(50, 50);
                    button.Visible = true;
                    button.BringToFront();
                    Controls.Add(button);
                    button.Click += button_Click;
                }

            }
        }

        private void button_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            string[] split = button.Tag.ToString().Split('|');
            string label = split[0];
            int n = Int32.Parse(split[1]);
            int m = Int32.Parse(split[2]);

            //button.Text = label;
            button.Text = $"{label}";
            button.Enabled = false;
            if (label == "bomb")
            {
                button.BackColor = Color.FromArgb(255, 128, 128);
            }
            else
            {
                button.BackColor = Color.FromArgb(255, 255, 179);
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void button10_Click(object sender, EventArgs e)
        {

        }
    }
}
