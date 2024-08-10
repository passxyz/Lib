using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using User = PassXYZLib.User;
using KeePassLib;
using KeePassLib.Keys;
using KeePassLib.Serialization;
using KeePassLib.Utility;
using PassXYZLib;

namespace UITest
{
    public partial class MainViewModel : ObservableObject
    {
        const string TEST_DB = "test1";
        const string TEST_DB_KEY = "12345";

        public string RunTest(int number)
        {
            switch (number) 
            {
                case 1:
                    return TestCase1();
                case 2:
                    return TestCase2GetInfo();
                case 3:
                    return TestCase3GetInfo();
                default:
                    Debug.WriteLine("");
                    break;
            }

            return $"Test case {number} is not implemented.";
        }

        private string TestCase1()
        {
            return Users();
        }

        private static string TestCase2GetInfo() 
        {
            PxDatabase db = new PxDatabase();
            User user = new()
            {
                Username = "test1",
                Password = "12345"
            };
            if (db != null) 
            {
                db.Open(user);
                if(db.RootGroup != null)
                    return $"Test case 2, read DB, RootGroup={db.RootGroup.Name}";
            }
            return $"Cannot open database {user.Username}.";
        }

        private static string TestCase3GetInfo()
        {
            PxDatabase db = new PxDatabase();
            User user = new()
            {
                Username = "MixedKeyData",
                Password = "123123"
            };
            if (db != null)
            {
                db.Open(user);
                if (db.RootGroup != null)
                    return $"Test case 3, read DB with device lock, RootGroup={db.RootGroup.Name}";
            }
            return $"Cannot open database {user.Username}.";
        }

        public static string Users()
        {
            string userList = "";
            User.GetUsersList().ForEach(u => {
                User user = new User();
                user.Username = u;
                userList = userList + $"<b>{u}</b>: {user.Path}" + "\n";
            });
            return userList;
        }
    }
}
