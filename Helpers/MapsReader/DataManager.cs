using SwfDotNet.IO;
using SwfDotNet.IO.ByteCode;
using SwfDotNet.IO.ByteCode.Actions;
using SwfDotNet.IO.Tags;
using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;

namespace RetroCore.Helpers.MapsReader
{
    public static class DataManager
    {
        private static bool GamePathFound = false;
        private static string LangsVersion = "";
        private static string[] RequiredLangs = { "maps" };

        public static void Initialize()
        {
            GamePathFound = Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Ankama\\zaap\\retro\\resources\\app\\retroclient\\data");
            InitializeMaps();
            InitializeLangs();
        }

        public static void InitializeMaps()
        {
            if (!GamePathFound)
            {
                StringHelper.WriteLine("[Warning] Can't find retroclient, DataManager will download maps datas in maps folder.", ConsoleColor.Yellow);
                Constants.MapsPath = AppDomain.CurrentDomain.BaseDirectory + "\\maps";
                if (!Directory.Exists($"{Constants.MapsPath}\\"))
                    Directory.CreateDirectory($"{Constants.MapsPath}\\");
                return;
            }
            else
                Constants.MapsPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Ankama\\zaap\\retro\\resources\\app\\retroclient\\data\\maps";
            GamePathFound = true;
            string[] Files = Directory.GetFiles(Constants.MapsPath, "*.swf", SearchOption.TopDirectoryOnly);
            StringHelper.WriteLine($"[DataManager] {Files.Count()} maps found !", ConsoleColor.Cyan);
        }

        public static void InitializeLangs()
        {
            var web = new WebClient();
            var url = $"http://dofusretro.cdn.ankama.com/lang/versions_fr.txt";
            var responseString = web.DownloadString(url);
            string[] splitted = responseString.Replace("&f=", "").Split('|');
            LangsVersion = splitted[0].Split(',')[2];
            Constants.LangsPath = AppDomain.CurrentDomain.BaseDirectory + "\\langs";
            if (!Directory.Exists($"{Constants.LangsPath}\\"))
                Directory.CreateDirectory($"{Constants.LangsPath}\\");
            string[] Files = Directory.GetFiles(Constants.LangsPath, "*.swf", SearchOption.TopDirectoryOnly);
            StringHelper.WriteLine($"[DataManager] {Files.Count()} langs found !", ConsoleColor.Cyan);
            foreach (var file in Files)
            {
                if (!file.Contains(LangsVersion))
                {
                    StringHelper.WriteLine($"[DataManager] {file} is outdated, deleting it !", ConsoleColor.Red);
                    string langname = file.Split('_')[0];
                    File.Delete(file);
                }
            }

            foreach (var langname in RequiredLangs)
                DownloadLang(langname);
            
        }

        public static void DownloadMap(string id, string mapid)
        {
            if (File.Exists($"{Constants.MapsPath}\\{id}_{mapid}X.swf"))
                return;
            using (var client = new WebClient())
                client.DownloadFile($"https://dofusretro.cdn.ankama.com/maps/" + id + "_" + mapid + "X.swf", $"{Constants.MapsPath}\\{id}_{mapid}X.swf");
        }

        public static void DownloadLang(string langname)
        {
            if (File.Exists($"{Constants.LangsPath}\\{langname}_fr_{LangsVersion}.swf"))
                return;
            using (var client = new WebClient())
                client.DownloadFile($"https://dofusretro.cdn.ankama.com/lang/swf/" + langname + "_fr_" + LangsVersion + ".swf", $"{Constants.LangsPath}\\{langname}_fr_{LangsVersion}.swf");
        }

        public static SwfDecompiledMap GetSwfContent(string id, string mapid, string map_key)
        {
            if (!GamePathFound)
            {
                DownloadMap(id, mapid);
            }
            SwfDecompiledMap map = ReadSwfMap((Constants.MapsPath + $"\\{id}" + "_" + $"{mapid}X.swf"));
            //read map lang file here to get x,y coords values
            map.DecypheredMapData = DecypherData(map.MapData, map_key);
            return map;
        }

        public static SwfDecompiledMap ReadSwfMap(string path)
        {
            var map = new SwfDecompiledMap();
            SwfReader Reader = new SwfReader(path);
            Swf swf = Reader.ReadSwf();

            IEnumerator enumerator = swf.Tags.GetEnumerator();
            while (enumerator.MoveNext())
            {
                BaseTag current = (BaseTag)enumerator.Current;
                if (current.ActionRecCount != 0)
                {
                    string sb = "";
                    IEnumerator currentenumerator = current.GetEnumerator();
                    while (currentenumerator.MoveNext())
                    {
                        Decompiler decompiler = new Decompiler(swf.Version);
                        ArrayList actions = decompiler.Decompile((byte[])currentenumerator.Current);

                        foreach (BaseAction obj in actions)
                            sb += obj.ToString() + "\r\n";
                    }
                    map.MapData = Gettok(sb, "'", 30);
                    map.Id = int.Parse(Gettok(Gettok(sb, "push", 9), " ", 2));
                    map.Width = int.Parse(Gettok(Gettok(sb, "push", 11), " ", 2)); //x
                    map.Height = int.Parse(Gettok(Gettok(sb, "push", 13), " ", 2)); // y
                    Reader.Close();
                    swf = null;
                }
            }

            return map;
        }

        public static string Gettok(string GettokText, string GettokStr, int GettokNum)
        {
            string[] GettokTmp;

            GettokNum = GettokNum - 1;

            GettokTmp = GettokText.Split(GettokStr);

            return GettokTmp[GettokNum];
        }

        private static string DecypherData(string data, string decryptKey)
        {
            try
            {
                var result = string.Empty;

                if (decryptKey == "") return data;

                decryptKey = PrepareKey(decryptKey);
                var checkSum = CheckSum(decryptKey) * 2;

                for (int i = 0, k = 0; i < data.Length; i += 2)
                    result += (char)(int.Parse(data.Substring(i, 2), NumberStyles.HexNumber) ^ decryptKey[(k++ + checkSum) % decryptKey.Length]);

                return Uri.UnescapeDataString(result);
            }
            catch
            {
                return "error";
            }
        }

        private static string PrepareKey(string key)
        {
            var keyResult = string.Empty;
            for (var i = 0; i < key.Length; i += 2)
                keyResult += Convert.ToChar(int.Parse(key.Substring(i, 2), NumberStyles.HexNumber));
            return Uri.UnescapeDataString(keyResult);
        }

        private static int CheckSum(string data) => data.Sum(t => t % 16) % 16;
    }
}