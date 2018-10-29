using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MemoryProjectFull
{
    public class HighscorePanel : PanelBase
    {
        TextBlock scores;
        Button backButton;
        public HighscorePanel(int _width, int _height) : base(_width, _height)
        {


            string usersString = MemoryDatabase.database.GetDataFromTable("users", "name");
            string winsString = MemoryDatabase.database.GetDataFromTable("users", "wins");
            string lossesString = MemoryDatabase.database.GetDataFromTable("users", "loses");

            List<string> users = new List<string>(usersString.Split(','));
            List<string> wins = new List<string>(winsString.Split(','));
            List<string> losses = new List<string>(lossesString.Split(','));
            for (int i = 0; i < users.Count; i++)
            {
                users[i] = users[i].Replace(" ", "");
                users[i] = users[i].Replace(",", "");
                if (users[i] == "")
                {
                    users.RemoveAt(i);
                    wins.RemoveAt(i);
                    losses.RemoveAt(i);
                }
            }


            string highscores = "";

            for (int i = 0; i < users.Count; i++)
            {
                highscores += users[i] + " : " + wins[i] + " : " + losses[i] + "\n";
            }

            Console.WriteLine(highscores);
            scores = UIFactory.CreateTextBlock(highscores, new System.Windows.Thickness(), new System.Windows.Point(_width, Height), 16, System.Windows.TextAlignment.Center);

            this.addChild(scores);
            this.setBackground(Brushes.LightGray);

        }

        struct User
        {
            public int id;
            public int wins;
            public int losses;
        }
    }
}
