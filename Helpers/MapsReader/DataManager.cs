using RetroCore.Helpers.MapsReader.Types;
using SwfDotNet.IO;
using SwfDotNet.IO.ByteCode;
using SwfDotNet.IO.ByteCode.Actions;
using SwfDotNet.IO.Tags;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace RetroCore.Helpers.MapsReader
{
    public static class DataManager
    {
        public static List<MapDatas> GlobalMapsInfos = new List<MapDatas>();

        public static bool GamePathFound = false;
        private static string LangsVersion = "";
        private static string[] RequiredLangs = { "maps" };

        public static void Initialize()
        {
            GamePathFound = Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Ankama\\zaap\\retro\\resources\\app\\retroclient\\data");
            InitializeMaps();
            InitializeLangs();
            ReadSwfLang($"{Constants.LangsPath}\\{RequiredLangs[0]}_fr_{LangsVersion}.swf"); //on trouvera bien une utilité à RequiredLangs
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
            Constants.LangsPath = AppDomain.CurrentDomain.BaseDirectory + "langs";
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

        public static SwfDecompiledMap GetSwfContent(string id, string mapid, string map_key = "")
        {
            Console.WriteLine(map_key);
            if (!GamePathFound)
            {
                DownloadMap(id, mapid);
            }
            SwfDecompiledMap map = ReadSwfMap(id, mapid);
            //GlobalMapsInfos.First(x => x.MapId == map.Id).OutDoor = map.OutDoor;
            if (map_key != "")
                map.DecypheredMapData = DecypherData(map.MapData, map_key);
            return map;
        }

        public static void ReadSwfLang(string path)
        {
            //This part is dirty but there arent so much ressources about how to decompile langs, decompilation & reading takes 12 secondes
            StringHelper.WriteLine($"[DataManager] Reading maps lang ..", ConsoleColor.Cyan);
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
                        {
                            sb += obj.ToString();
                        }

                        //maps coords & subarea id
                        var regex = @"getMemberpush ([0-9]*?) as int push (-?[0-9]*?) as var push (-?[0-9]*?) as int push (-?[0-9]*?) as var push (-?[0-9]*?) as int push (-?[0-9]*?) as var push (-?[0-9]*?) as int";
                        var matches = Regex.Matches(sb, regex);
                        foreach (Match match in matches)
                        {
                            GlobalMapsInfos.Add(new MapDatas(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[3].Value), int.Parse(match.Groups[5].Value), int.Parse(match.Groups[7].Value)));
                        }
                        //area id
                        foreach (var map in GlobalMapsInfos.Where(x => x.AreaId == -1))
                        {
                            var regex2 = @"getMemberpush " + map.SubAreaId + " as int push (-?[0-9]*?) as var push (-?[0-9]*?) as var push (-?[0-9]*?) as var push (-?[0-9]*?) ";
                            var matches2 = Regex.Matches(sb, regex2);
                            foreach (Match match2 in matches2)
                                map.AreaId = int.Parse(match2.Groups[4].Value);
                        }
                        Console.Write("\r{0} maps..", GlobalMapsInfos.Count());
                        sb = "";
                        GC.Collect();
                    }
                }
            }

            Reader.Close();
            swf = null;
            Console.Write("\n");
            StringHelper.WriteLine($"[DataManager] {GlobalMapsInfos.Count()} maps added to list !", ConsoleColor.Cyan);
            StringHelper.WriteLine("[DataManager] Map with undefinied AreaId : " + GlobalMapsInfos.Count(x => x.AreaId == -1), ConsoleColor.Blue);
        }

        public static List<SwfDecompiledMap> ReadListSwfMap(string id)
        {
            List<SwfDecompiledMap> MapList = new List<SwfDecompiledMap>();

            var Files = Directory.GetFiles(Constants.MapsPath, "*.swf", SearchOption.TopDirectoryOnly).Where(x => x.Contains(id));

            foreach (var mapFile in Files)
            {
                string path = mapFile;
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
                                sb += obj.ToString();
                        }

                        //mapData
                        map.MapData = sb.Substring(sb.IndexOf("mapData','") + "mapData','".Length, sb.IndexOf("push") - (sb.IndexOf("mapData','") + "mapData','".Length));
                        map.OutDoor = sb.Contains("True") ? true : false;
                        sb = sb.Replace(StringHelper.UppercaseFirst(map.OutDoor.ToString()), "");
                        sb = sb.Replace("(1 args)", "");

                        var regex = @"push ([0-9]*?) as var getVariablepush ([0-9]*?) as var getMemberpush ([0-9]*?) as int push (-?[0-9]*?) as var getVariablepush ([0-9]*?) as var getMemberpush ([0-9]*?) as var callMethod poppush ([0-9]*?) as var push ([0-9]*?) as int setVariablepush ([0-9]*?) as var push ([0-9]*?) as int setVariablepush ([0-9]*?) as var push ([0-9]*?) as int setVariablepush ([0-9]*?) as var push ([0-9]*?) as int setVariablepush ([0-9]*?) as var push ([0-9]*?) as int setVariablepush ([0-9]*?) as var push ([0-9]*?) as int setVariablepush ([0-9]*?) as var push  as bool setVariablepush ([0-9]*?) as var push ([0-9]*?) as int setVariablepush ([0-9]*?) as var push ([0-9]*?) as var";
                        var matches = Regex.Matches(sb, regex);

                        map.Id = int.Parse(matches.First().Groups[8].Value);
                        map.Width = int.Parse(matches.First().Groups[10].Value);
                        map.Height = int.Parse(matches.First().Groups[12].Value);
                        map.XValue = DataManager.GlobalMapsInfos.First(x => x.MapId == map.Id).xPos;
                        map.YValue = DataManager.GlobalMapsInfos.First(x => x.MapId == map.Id).yPos;
                        MapList.Add(map);
                        sb = "";
                    }
                }
                Reader.Close();
                swf = null;
            }
            return MapList;
        }

        public static SwfDecompiledMap ReadSwfMap(string id, string mapid)
        {
            string path = (Constants.MapsPath + $"\\{id}" + "_" + $"{mapid}X.swf");
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
                            sb += obj.ToString();
                    }

                    //mapData
                    map.MapData = sb.Substring(sb.IndexOf("mapData','") + "mapData','".Length, sb.IndexOf("push") - (sb.IndexOf("mapData','") + "mapData','".Length));
                    map.OutDoor = sb.Contains("True") ? true : false;
                    sb = sb.Replace(StringHelper.UppercaseFirst(map.OutDoor.ToString()), "");
                    sb = sb.Replace("(1 args)", "");

                    var regex = @"push ([0-9]*?) as var getVariablepush ([0-9]*?) as var getMemberpush ([0-9]*?) as int push (-?[0-9]*?) as var getVariablepush ([0-9]*?) as var getMemberpush ([0-9]*?) as var callMethod poppush ([0-9]*?) as var push ([0-9]*?) as int setVariablepush ([0-9]*?) as var push ([0-9]*?) as int setVariablepush ([0-9]*?) as var push ([0-9]*?) as int setVariablepush ([0-9]*?) as var push ([0-9]*?) as int setVariablepush ([0-9]*?) as var push ([0-9]*?) as int setVariablepush ([0-9]*?) as var push ([0-9]*?) as int setVariablepush ([0-9]*?) as var push  as bool setVariablepush ([0-9]*?) as var push ([0-9]*?) as int setVariablepush ([0-9]*?) as var push ([0-9]*?) as var";
                    var matches = Regex.Matches(sb, regex);

                    map.Id = int.Parse(matches.First().Groups[8].Value);
                    map.Width = int.Parse(matches.First().Groups[10].Value);
                    map.Height = int.Parse(matches.First().Groups[12].Value);
                    map.XValue = DataManager.GlobalMapsInfos.First(x => x.MapId == map.Id).xPos;
                    map.YValue = DataManager.GlobalMapsInfos.First(x => x.MapId == map.Id).yPos;
                    sb = "";
                }
            }
            Reader.Close();
            swf = null;
            return map;
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
                return "";
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