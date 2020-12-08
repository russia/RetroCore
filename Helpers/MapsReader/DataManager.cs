using RetroCore.Helpers.MapsReader.Types;
using SwfDotNet.IO;
using SwfDotNet.IO.ByteCode;
using SwfDotNet.IO.ByteCode.Actions;
using SwfDotNet.IO.Tags;
using System;
using System.Collections;
using System.Collections.Generic;
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
                Constants.MapsPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Ankama\\zaap\\retro\\resources\\app\\retroclient\\data\\maps\\";
            GamePathFound = true;
            string[] Files = Directory.GetFiles(Constants.MapsPath, "*.swf", SearchOption.TopDirectoryOnly);
            StringHelper.WriteLine($"[DataManager] {Files.Count()} maps found !", ConsoleColor.Cyan);
        }

        public static void InitializeLangs()
        {
            WebClient web = new WebClient();
            string url = $"http://dofusretro.cdn.ankama.com/lang/versions_fr.txt";
            var responseString = web.DownloadString(url);
            string[] splitted = responseString.Replace("&f=", "").Split('|');
            LangsVersion = splitted[0].Split(',')[2];
            Constants.LangsPath = AppDomain.CurrentDomain.BaseDirectory + "langs";
            if (!Directory.Exists($"{Constants.LangsPath}\\"))
                Directory.CreateDirectory($"{Constants.LangsPath}\\");
            string[] Files = Directory.GetFiles(Constants.LangsPath, "*.swf", SearchOption.TopDirectoryOnly);
            StringHelper.WriteLine($"[DataManager] {Files.Count()} lang(s) found !", ConsoleColor.Cyan);
            foreach (string file in Files)
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
            if (File.Exists($"{Constants.MapsPath}{id}_{mapid}X.swf"))
                return;
            using (WebClient client = new WebClient())
                client.DownloadFile($"https://dofusretro.cdn.ankama.com/maps/" + id + "_" + mapid + "X.swf", $"{Constants.MapsPath}{id}_{mapid}X.swf");
        }

        public static void DownloadLang(string langname)
        {
            if (File.Exists($"{Constants.LangsPath}\\{langname}_fr_{LangsVersion}.swf"))
                return;
            using (WebClient client = new WebClient())
                client.DownloadFile($"https://dofusretro.cdn.ankama.com/lang/swf/" + langname + "_fr_" + LangsVersion + ".swf", $"{Constants.LangsPath}\\{langname}_fr_{LangsVersion}.swf");
        }

        public static MapDatas GetMapContent(string id, string mapid, string map_key = "")
        {
            if (!GamePathFound)
            {
                DownloadMap(id, mapid);
            }
            SwfDecompiledMap map = ReadSwfMap(id, mapid);
            GlobalMapsInfos.First(x => x.Id == map.Id).SwfDatas = map;
            if (map_key != "")
                map.DecypheredMapData = Hash.DecypherData(map.CypheredMapData, map_key);
            return GlobalMapsInfos.First(x => x.Id == map.Id);
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
                        string regex = @"getMemberpush ([0-9]*?) as int push (-?[0-9]*?) as var push (-?[0-9]*?) as int push (-?[0-9]*?) as var push (-?[0-9]*?) as int push (-?[0-9]*?) as var push (-?[0-9]*?) as int";
                        MatchCollection matches = Regex.Matches(sb, regex);
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
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        sb = "";
                        Console.Write($"\r{DateTime.Now.ToString("[HH:mm:ss:fff]")} [DataManager] {GlobalMapsInfos.Count()} maps loaded..");
                    }
                }
            }
            Reader.Close();
            swf = null;

            Console.Write("\n");
            StringHelper.WriteLine($"[DataManager] {GlobalMapsInfos.Count()} maps added to list !", ConsoleColor.Cyan);
            StringHelper.WriteLine("[DataManager] Map with undefinied AreaId : " + GlobalMapsInfos.Count(x => x.AreaId == -1), ConsoleColor.Blue);
        }

        public static List<MapDatas> ReadListSwfMap(string id)
        {
            List<MapDatas> MapList = new List<MapDatas>();
            List<string> Files = Directory.GetFiles(Constants.MapsPath, "*.swf").ToList();
            Files = Files.Where(x => x.Contains("\\" + id)).ToList();
            SwfReader Reader;
            foreach (var mapFile in Files)
            {
                string path = mapFile;

                Reader = new SwfReader(path);
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
                        SwfDecompiledMap content = ParseSwfMapDatas(sb);
                        content.DecypheredMapData = Hash.DecypherData(content.CypheredMapData, MapKeyCracker.MapCrackKey(content.CypheredMapData));
                        GlobalMapsInfos.First(x => x.Id == content.Id).SwfDatas.MapId = path.Substring(path.IndexOf(id) + id.Length + 1, path.IndexOf(".swf") - (path.IndexOf(id) + id.Length + 1));
                        GlobalMapsInfos.First(x => x.Id == content.Id).SwfDatas = content;
                        MapList.Add(GlobalMapsInfos.First(x => x.SwfDatas == content));
                        sb = "";
                    }
                }
                Reader.Close();
                swf = null;
            }
            string firstValue = MapList.First().SwfDatas.DecypheredMapData;
            if (MapList.All(x => x.SwfDatas.DecypheredMapData == firstValue))
                return new List<MapDatas>() { MapList.First() };
            return MapList;
        }

        public static SwfDecompiledMap ReadSwfMap(string id, string mapid)
        {
            string path = (Constants.MapsPath + $"{id}" + "_" + $"{mapid}X.swf");
            SwfReader Reader = new SwfReader(path);
            Swf swf = Reader.ReadSwf();
            SwfDecompiledMap mapDatas = null;
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
                    mapDatas = ParseSwfMapDatas(sb);
                    sb = "";
                }
            }
            Reader.Close();
            swf = null;
            return mapDatas;
        }

        public static SwfDecompiledMap ParseSwfMapDatas(string sb)
        {
            SwfDecompiledMap map = new SwfDecompiledMap();
            map.CypheredMapData = sb.Substring(sb.IndexOf("mapData','") + "mapData','".Length, sb.IndexOf("push") - (sb.IndexOf("mapData','") + "mapData','".Length)).Replace("'", "");
            map.OutDoor = sb.Contains("True") ? true : false;
            sb = sb.Replace(StringHelper.UppercaseFirst(map.OutDoor.ToString()), "");
            sb = sb.Replace("(1 args)", "");

            string regex = @"push ([0-9]*?) as var getVariablepush ([0-9]*?) as var getMemberpush ([0-9]*?) as int push (-?[0-9]*?) as var getVariablepush ([0-9]*?) as var getMemberpush ([0-9]*?) as var callMethod poppush ([0-9]*?) as var push ([0-9]*?) as int setVariablepush ([0-9]*?) as var push ([0-9]*?) as int setVariablepush ([0-9]*?) as var push ([0-9]*?) as int setVariablepush ([0-9]*?) as var push ([0-9]*?) as int setVariablepush ([0-9]*?) as var push ([0-9]*?) as int setVariablepush ([0-9]*?) as var push ([0-9]*?) as int setVariablepush ([0-9]*?) as var push  as bool setVariablepush ([0-9]*?) as var push ([0-9]*?) as int setVariablepush ([0-9]*?) as var push ([0-9]*?) as var";
            MatchCollection matches = Regex.Matches(sb, regex);

            map.Id = int.Parse(matches.First().Groups[8].Value);
            map.Width = int.Parse(matches.First().Groups[10].Value);
            map.Height = int.Parse(matches.First().Groups[12].Value);
            GlobalMapsInfos.First(x => x.Id == map.Id).SwfDatas = map;

            return map;
        }
    }
}