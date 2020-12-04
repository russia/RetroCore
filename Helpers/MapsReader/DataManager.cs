﻿using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;

namespace RetroCore.Helpers.MapsReader
{
    public static class DataManager
    {
        private static bool gamePathFound = false;

        public static void Initialize()
        {
           
            //bool gamePathFound = Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Ankama\\zaap\\retro\\resources\\app\\retroclient\\data\\maps");

            //if (!gamePathFound)
            //{
            //    StringHelper.WriteLine("[Warning], can't find retroclient, DataManager will download maps datas.", ConsoleColor.Yellow);
            //    return;
            //}
            //else
            //    Constants.MapsPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Ankama\\zaap\\retro\\resources\\app\\retroclient\\data\\maps";

            //string[] Files = Directory.GetFiles(Constants.MapsPath, "*.swf", SearchOption.TopDirectoryOnly);
            //StringHelper.WriteLine($"[DataManager] {Files.Count()} maps found !", ConsoleColor.Cyan);
        }

        public static void DownloadMap(string id, string mapid)
        {
            if (File.Exists($"./maps/{id}_{mapid}.swf"))
                return;
            if (!Directory.Exists("./maps/"))
                Directory.CreateDirectory("./maps/");
            using (var client = new WebClient())
                client.DownloadFile($"https://dofusretro.cdn.ankama.com/maps/" + id + "_" + mapid + ".swf", $"./maps/{id}_{mapid}.swf");
        }

        public static void GetSwfContent(string id, string mapid)
        {
            if (!gamePathFound)
            {
                DownloadMap(id, mapid);
            }

            //open files



            //get values and mapdata 
            string map_key = "6a292e2c446752644236725b68355d733e3a446060303e7d476e7c63354e5e42505052657756526b32585753473b64782f6f2261545d47732a5e63253242772532427e4b636c663a6e33395031705c4461386348652d527f6059365d5154703a4c39416d342962707e653b214c2e7657573c4f5542223f5d285b66423a446c5d723c2f594a6d7820787826505b253242663d463150253235304b4e41312d7f784e7d23712c655a5230335f55382046202869795f6e71415463753c4f3840492c4e";
            string data = "205d3c125b5b250101517615260f1902542f3f23183833041237330a53391f3b265a01194e0e43001c3526124f3f024a164a3623020d035b0f52583179183d2504590229044c1a170138533c3035115b0451200c514803111f0473492d4f1336365d2e340a4a5e3c4d3a07235b252435135d4a382b0c1941301047313e4a075c2750184d512a2b20504c1e190615421049043b335152173d594123414908183e2619203506145d2e592101442f0b4c4f4d2506332c2a57133e09543c125f722c0101555f1c260f1d2b5d2f3f2731313304161e3a0a533d3632265a0530470e4304353c26124b160b4a164e1f2a020d07720652583550113d2500700b290448331e013857153935115f2d58200c55610a111f005a402d4f171f3f5d2e3023435e3c49130e235b210d3c135d4e11220c1945191947313a630e5c27543144512a2f09594c1e1d2f1c42104d2d323351563e34594127684008183a0f102035023d542e5925284d2f0b486644250637052357133a205d3c125b5b250101517615260f1902542f3f23183833041237330a53391f3b265a01194e0e43001c3526124f3f024a164a3623020d035b0f52583179183d2504590229044c1a170138533c3035115b0451200c514803111f0473492d4f1336365d2e340a4a5e3c4d3a07235b252435135d4a382b0c1941301047313e4a075c2750184d512a2b20504c1e190615421049043b335152173d594123414908183e2619203506145d2e592101442f0b4c4f4d2506332c2a57133e09543c125f722c0101555f1c260f1d2b5d2f3f2731313304161e3a0a533d3632265a0530470e4304353c26124b160b4a164e1f2a020d07720652583550113d2500700b290448331e013857153935115f2d58200c55610a111f005a402d4f171f3f5d2e3023435e3c49130e235b210d3c135d4e11220c1945191947313a630e5c27543144512a2f09594c1e1d2f1c42104d2d323351563e34594127684008183a0f102035023d542e5925284d2f0b486644250637052357133a205d3c125b5b250101517615260f1902542f3f23183833041237330a53391f3b265a01194e0e43001c3526124f3f024a164a3623020d035b0f52583179183d2504590229044c1a170138533c3035115b0451200c514803111f0473492d4f1336365d2e340a4a5e3c4d3a07235b252435135d4a382b0c1941301047313e4a075c2750184d512a2b20504c1e190615421049043b335152173d594123414908183e2619203506145d2e592101442f0b4c4f4d2506332c2a57133e09543c125f722c0101555f1c260f1d2b5d2f3f2731313304161e3a0a533d3632265a0530470e4304353c26124b160b4a164e1f2a020d07720652583550113d2500700b290448331e013857153935115f2d58200c55610a111f005a402d4f171f3f5d2e3023435e3c49130e235b210d3c135d4e11220c1945191947313a630e5c27543144512a2f09594c1e1d2f1c42104d2d323351563e34594127684008183a0f102035023d542e5925284d2f0b486644250637052357133a205d3c125b5b250101517615260f1902542f3f23183833041237330a53391f3b265a01194e0e43001c3526124f3f024a164a3623020d035b0f52583179183d2504590229044c1a170138533c3035115b0451200c514803111f0473492d4f1336365d2e340a4a5e3c4d3a07235b252435135d4a382b0c1941301047313e4a075c2750184d512a2b20504c1e190615421049043b335152173d594123414908183e2619203506145d2e592101442f0b4c4f4d2506332c2a57133e09543c125f722c0101555f1c260f1d2b5d2f3f2731313304161e3a0a533d3632265a0530470e4304353c26124b160b4a164e1f2a020d07720652583506243d2500700b290448331e013857153935115f2d58200c55610a111f005a402d4f171f3f5d2e3023435e3c49130e235b210d3c135d4e11220c1945191947313a630e5c27543144512a2f09594c1e1d2f1c42104d2d323351563e34594127684008183a0f102035023d542e5925284d2f0b486644250637052357133a205d3c125b5b250101517615260f1902542f3f23183833041237330a53391f3b265a01194e0e43001c3526124f3f024a164a3623020d035b0f52583179183d2504590229044c1a170138533c3035115b0451200c514803111f0473492d4f1336365d2e340a4a5e3c4d3a07235b252435135d4a382b0c1941301047313e4a075c2750184d512a2b20504c1e190615421049043b335152173d594123414908183e2619203506145d2e592101442f0b4c4f4d2506332c2a57133e09543c125f722c0101555f1c260f1d2b5d2f3f2731313304161e3a0a533d3632265a0530470e4304353c26124b160b4a164e1f2a020d07720652583550113d2500700b290448331e013857153935115f2d58200c55610a111f005a402d4f171f3f5d2e3023435e3c49130e235b210d3c135d4e11220c1945191947313a630e5c27543144512a2f09594c1e1d2f1c42104d2d323351563e34594127684008183a0f102035023d542e5925284d2f0b486644250637052357133a205d3c125b5b250101517615260f1902542f3f23183833041237330a53391f3b265a01194e0e43001c3526124f3f024a164a3623020d035b0f52583179183d2504590229044c1a170138533c3035115b0451200c514803111f0473492d4f1336365d2e340a4a5e3c4d3a07235b252435135d4a382b0c1941301047313e4a075c2750184d512a2b20504c1e190615421049043b335152173d594123414908183e2619203506145d2e592101442f0b4c4f4d2506332c2a57133e09543c125f722c0101555f1c260f1d2b5d2f3f2731313304161e3a0a533d3632265a053047284304353c26124b160b6c164e4b060a0d077d0652583550113d731b700b290448331e013857153935115f2d58200c55610a111f005a402d4f171f3f5d2e3023435e3c49130e235b210d3c135d4e11220c1945191947313a630e5c27543144512a2f09594c1e1d2f1c42104d2d323351563e34594127684008183a0f102035023d542e5925284d2f0b486644030637052357133a205d1a125b5b250101517615000f1902542f3f23183833041237330a6a6e1f3b265a01194e0e43001c3526124f3f024a164a3623020d035b0f52583179183d2504590229044c1a170138533c3035115b0451200c514803111f0473492d4f1336365d2e340a4a5e3c4d3a07235b252435135d4a382b0c1941301047313e4a075c2750184d772a2b20504c1e190615641049043b335152183d5941234149084c662619063506145d2e592101442f0b4c4f4d2506332c2a57133e09543c125f722c0101555f1c260f1d2b5d2f3f2731313304161e3a0a533d3632265a0530470e4304353c26124b160b4a164e1f2a020d07720652583550113d2500700b290448331e013857153935115f2d58200c55610a111f005a402d4f171f3f7b2e3023435e3c49130e055b210d3c135d4e11222a1945191947313a630e7a27543144512a2f09594c1e1d2f1c42104d2d323351563e34594127684008183a0f102035023d542e5925284d2f0b486644250637052357133a205d3c125b5b250101517615260f1954712f3f23183833041237330a53391f3b265a01194e0e43001c3500124f3f024a164a3623240d035b0f52583179181b2504590229044c1a172738533c3035115b0451060c514803111f0473490b4f1336365d2e340a4a5e3c4d3a07235b252435135d4a382b0c1941301047313e4a075c2750184d512a2b20504c1e190615421049043b335152173d594123414908183e2619203506145d2e592101442f0b4c4f4d2506332c2a57133e09543c125f722c2701555f1c260f1d2b5d093f2731313304161e3a2c533d3632265a053047284304353c26124b160b6c164e1f2a020d07720674583550113d2500700b290448331e013857153935115f2d58200c55610a111f005a402d4f171f3f5d2e3023435e3c49130e235b210d3c135d4e11220c1945191947313a630e5c27543144512a2f09594c1e1d2f1c42104d2d323351563e34594127684008183a0f102035023d54085925034b2f0b486644030637052357133a205d1a125b5b250101517615000f1902542f3f23183833041237330a07311f3b265a01194e0e43001c3526124f3f024a164a3623020d035b0f52583179183d2504590229044c1a170138533c3035115b0451200c514803111f0473492d4f1336365d2e340a4a5e3c4d3a07235b252435135d4a382b0c1941301047313e4a075c2750184d772a2b20504c1e190615641049043b335152173d7f4123414908183e2619063506145d2e59210144090b4c4f4d2506332c2a57133e09543c125f722c0101555f1c260f1d2b5d2f3f2731313304161e3a0a533d3632265a0530470e4304353c26124b160b4a164e1f2a020d07720652583550113d2500700b290448331e013857153935115f2d58200c55610a371f005a402d4f171f3f7b2e3023435e3c49130e055b210d3c135d4e11220c1945321f476532630e7a27543144512a2f09596a1e1d2f1c42104d2d323351563e34594127684008183a0f102035023d542e5925284d2f0b486644250637052357133a205d3c125b5b250101517615260f1902542f3f23183833041237330a05081f3b265a01194e0e43001c3526124f3f024a164a3623240d035b0f52583179181b2504590229044c1a170138533c3035115b0451060c514803111f0473490b4f1336365d2e340a4a783c4d3a07235b252435135d4a382b0c1941301061313e4a075c2750184d772a2b745e4c1e190615421049043b335152173d594123414908183e2619203506145d2e592101442f0b4c4f4d2506332c2a57133e09543c125f722c2701555f1c260f1d2b5d093f2731313304161e3a2c533d3632265a0530470e43041e3a26124b160b6c164e1f2a020d07720674583504253d2500700b0f0448331e013857153935115f2d58200c55610a371f005a402d4f171f3f7b2e3023435e3c49130e235b215930135d4e11220c1945191947313a630e5c27543144512a2f09594c1e1d2f1c42104d2d321551563e3459412768402e183a0f102035023d54085925284d2f0b486644030637052357133a205d3c125b70230101517615000f1902542f3f23183815041237330a53391f3b005a01194e0e43001c3526124f3f024a42413623240d035b0f52583176183d2504590229526c1a170138533c3035115b0451200c514803111f0473492d4f1336365d2e340a4a783c4d3a07235b252435355d4a382b0c1941301061313e4a075c1e06184d772a2b20504c1e190615421049043b335152173d594123414908183e2619063506145d2e59210144090b4c4f4d2506332c2a71133e09543c125f722c2701555f1c260f1d2b5d093f2731313304161e3a0a533d3632265a0530470e4304353c26124b160b4a164e1f2a020d07720652583550113d2500700b0f0448331e013857153913115f2d58200c55610a371f005a402d4f171f3f5d2e3023435e3c49130e235b210d3c135d4e11220c1945191947313a630e5c27543144512a2f09594c1e1d2f1c4244462d321551563e3459412768402e183a5b1c2035023d543e5925284d2f351d6644250637052357133a205d3c125b5b250101517615260f1902542f3f23183815041237330a53391f3b265a01194e0e43001c3526124f3f024a164a3623020d035b0f52583179183d2504590229044c1a170138533c3035115b0451200c511e26111f0473492d4f1336365d2e340a4a783c4d3a07235b252435355d4a382b0c1941301061313e4a075c2750184d512a2b20504c1e190615421049043b335152173d594123414908183e2619203506145d2e592101442f0b4c4f4d2506332c2a57133e09543c125f722c0101555f1c260f1d2b5d2f3f2765213b04161e3a0a533d3632260e3030470e4304353c26124b160b4a164e1f2a020d07720652583550113d2500700b0f0448331e013857153913115f2d58200c55610a371f005a402d1b291f3f5d2e3023435e3c49130e235b210d3c135d4e11220c1945191947313a630e5c27543144512a2f09594c1e1d2f1c42104d2d323351563e34594127684008183a0f102035023d542e5925284d2f0b486644250637052357133a205d3c125b5b250101517615260f1902542f3f23183815041237330a53391f3b005a01194e0e43001c3500124f3f024a41623623020d035b0f52583179183d2504590229044c1a170138533c3035115b0451200c514803111f0473492d4f1336365d2e340a4a5e3c4d3a07235b252435135d4a382b0c1941301047313e4a075c2750184d512a2b20504c1e190615421049043b335152173d594123414908183e2619203506145d2e60760144090b4c4f4d2506332c2a71133e09543c125f722c2701555f1c260f1d2b5d2f3f2731313304161e3a0a533d3632265a0530470e4304353c26124b160b4a164e1f2a020d07720652583550113d2500700b290448331e013857153935115f2d58200c55610a111f005a402d4f171f3f5d2e3023435e3c49130e235b210d3c135d4e11220c1945191947313a630e5c27543144512a2f09594c1e1d2f1c42104d2d323351563e34594127684008183a0f10203502";
            string value = DecypherData(data, map_key);
            Console.WriteLine(value);
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