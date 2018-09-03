using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NewMemoryGame
{
    public partial class Game : Form
    {

        public List<Card> cards;

        public Game()
        {
            InitializeComponent();
            LoadButtons(5, 5);
        }

        public void LoadButtons(int x, int y)
        {
            cards = new List<Card>();
            for(int i = 0; i < x; i++)
            {
                for(int j = 0;j < y; j++)
                {
                   
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
