using System;
using System.Linq;

namespace RetroCore.Helpers.MapsReader
{
    public class SwfMapStructure
    {
        private int id;
        private int width;
        private int height;
        private int backgroundNum;
        private int ambianceId;
        private int musicId;
        private bool outdoor;
        private int capabilities;
        private string mapData;
        private string version;

        public int Id()
        {
            return id;
        }

        public void setId(int id)
        {
            this.id = id;
        }

        public int Width()
        {
            return width;
        }

        public void setWidth(int width)
        {
            this.width = width;
        }

        public int Height()
        {
            return height;
        }

        public void setHeight(int height)
        {
            this.height = height;
        }

        public int BackgroundNum()
        {
            return backgroundNum;
        }

        public void setBackgroundNum(int backgroundNum)
        {
            this.backgroundNum = backgroundNum;
        }

        public int AmbianceId()
        {
            return ambianceId;
        }

        public void setAmbianceId(int ambianceId)
        {
            this.ambianceId = ambianceId;
        }

        public int MusicId()
        {
            return musicId;
        }

        public void setMusicId(int musicId)
        {
            this.musicId = musicId;
        }

        public bool isOutdoor()
        {
            return outdoor;
        }

        public void setOutdoor(bool outdoor)
        {
            this.outdoor = outdoor;
        }

        public int Capabilities()
        {
            return capabilities;
        }

        public void setCapabilities(int capabilities)
        {
            this.capabilities = capabilities;
        }

        public string MapData()
        {
            return mapData;
        }

        public void setMapData(String mapData)
        {
            this.mapData = mapData;
        }

        public string Version()
        {
            return version;
        }

        public void setVersion(string version)
        {
            this.version = version;
        }

        public bool valid()
        {
            return id > 0 && width > 0 && height > 0 && mapData != null && !mapData.Any();
        }

        public string toString()
        {
            return "MapStructure{" +
                "id=" + id +
                ", width=" + width +
                ", height=" + height +
                ", backgroundNum=" + backgroundNum +
                ", ambianceId=" + ambianceId +
                ", musicId=" + musicId +
                ", outdoor=" + outdoor +
                ", capabilities=" + capabilities +
                ", mapData='" + mapData + '\'' +
                '}'
            ;
        }
    }
}