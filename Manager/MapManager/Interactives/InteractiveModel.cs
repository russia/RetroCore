using System;
using System.Collections.Generic;
using System.Linq;

namespace RetroCore.Manager.MapManager.Interactives
{
    public class InteractiveModel
    {
        public short[] gfxs { get; private set; }
        public bool Walkable { get; private set; }
        public short[] Habilities { get; private set; }
        public string Name { get; private set; }
        public bool Gatherable { get; private set; }

        private static List<InteractiveModel> LoadedInteractives = new List<InteractiveModel>();

        public InteractiveModel(string _name, string _gfx, bool _walkable, string _habilities, bool _gatherable)
        {
            Name = _name;

            if (!_gfx.Equals("-1") && !string.IsNullOrEmpty(_gfx))
            {
                string[] splitted = _gfx.Split(',');
                gfxs = new short[splitted.Length];

                for (byte i = 0; i < gfxs.Length; i++)
                    gfxs[i] = short.Parse(splitted[i]);
            }

            Walkable = _walkable;

            if (!_habilities.Equals("-1") && !string.IsNullOrEmpty(_habilities))
            {
                string[] splitted = _habilities.Split(',');
                Habilities = new short[splitted.Length];

                for (byte i = 0; i < Habilities.Length; ++i)
                    Habilities[i] = short.Parse(splitted[i]);
            }

            Gatherable = _gatherable;
            LoadedInteractives.Add(this);
        }

        public static InteractiveModel GetGfxModel(short gfx_id)
        {
            foreach (InteractiveModel interactive in LoadedInteractives)
            {
                if (interactive.gfxs.Contains(gfx_id))
                    return interactive;
            }
            return null;
        }

        public static InteractiveModel get_Modelo_Por_Habilidad(short habilityId)
        {
            IEnumerable<InteractiveModel> InteractivesList = LoadedInteractives.Where(i => i.Habilities != null);

            foreach (InteractiveModel interactive in InteractivesList)
            {
                if (interactive.Habilities.Contains(habilityId))
                    return interactive;
            }
            return null;
        }

        public static List<InteractiveModel> GetLoadedInteractives() => LoadedInteractives;
    }
}