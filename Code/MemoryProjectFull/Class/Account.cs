using MemoryProjectFull;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public class Account{

    private static bool isActivate;

    public static int id;
    public static string name;
    public static Score score;

    public static void Load() {
        string autologin = MainConfig.GetGroup("account").GetEntry("autologin").GetValue<string>();

        if (autologin == null || autologin == "0"){
            createDummyAccount();
        }
        else{
            string name = MainConfig.GetGroup("account").GetEntry("name").GetValue<string>();
            string password = MainConfig.GetGroup("account").GetEntry("password").GetValue<string>();

            login(name, password, true, null);
        }
    }

    public static void save() {
        if (!isActivate)
            return;

        // save new wins/losses data when user is active account
        SortedList<string, string> userData = new SortedList<string, string>();
        userData.Add("wins", score.wins.ToString());
        userData.Add("losses", score.wins.ToString());

        MemoryDatabase.database.UpdateDataToTableFilter("users", "id = '" + id.ToString() + "'", userData);
    }

    public static void login(string _name, string _password, bool _autologin, Action _callback) {
        if (MemoryDatabase.database.CheckTableExistence("users")) {
            if (_autologin) {
                MainConfig.GetGroup("account").GetEntry("autologin").SetValue("1");
                MainConfig.GetGroup("account").GetEntry("name").SetValue(_name);
                MainConfig.GetGroup("account").GetEntry("password").SetValue(_password);
                MainConfig.Save();
            }

            string compactData = MemoryDatabase.database.GetDataFromTableFilter("users", "name='" + _name + "' && password='" + _password + "'");
            string[] data = compactData.Split(',');

            id = int.Parse(data[0]);
            name = Regex.Replace(data[1], @"\s+", "");
            score = new Score() { wins = int.Parse(data[3]), losses = int.Parse(data[4]) };

            isActivate = true;
            _callback?.Invoke();
        }
    }

    public static void logout() {
        MainConfig.GetGroup("account").GetEntry("autologin").SetValue("0");
        MainConfig.Save();
        createDummyAccount();
    }

    private static void createDummyAccount() {
        id = -1;
        name = createDummyName(5);
        score = new Score() { wins = 0, losses = 0 };

        isActivate = false;
    }

    private static string createDummyName(int _idSize) {
        Random r = new Random();
        string name = "Dummy#";
        for (int i = 0; i < _idSize; i++){
            name += r.Next(0, 9).ToString();
        }
        return name;
    }

    public static bool isActivateAccount() {
        return isActivate;
    }
}

public struct Score {
    public int wins;
    public int losses;

    public void Won() {
        wins++;
    }
    public void Lost() {
        losses++;
    }
}

