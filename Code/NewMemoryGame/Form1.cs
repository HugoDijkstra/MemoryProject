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
    public partial class Form1 : Form
    {

        public List<Image> buttons;

        public Form1()
        {
            InitializeComponent();
            LoadButtons(5, 5);

        }

        public void LoadButtons(int x, int y)
        {
            buttons = new List<Image>();
            for(int i = 0; i < x; i++)
            {
                for(int j = 0;j < y; j++)
                {
                    Image image = Image.;
                }
            }
        }

        private void CheckButton(object sender, EventArgs e)
        {
            sender.ToString();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
