using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace walterhcain.CainCustomBullets
{
    public class RBulletsConfig : IRocketPluginConfiguration
    {
        public List<ushort> rubberList;
        public List<ushort> tranqList;
        public List<ushort> trainingList;
        public List<ushort> zombieList;
        public List<ushort> trackList;
        public bool rubberEnabled;
        public bool tranqEnabled;
        public bool trainingEnabled;
        public bool zombieEnabled;
        public bool trackingEnabled;

        public float tracktime;


        public void LoadDefaults()
        {
            rubberList = new List<ushort>
            {
                485
            };
            tranqList = new List<ushort>
            {
                108
            };
            trainingList = new List<ushort>
            {
                1384
            };
            zombieList = new List<ushort>
            {
                20
            };

            rubberEnabled = true;
            tranqEnabled = true;
            trainingEnabled = true;
            zombieEnabled = true;
            trackingEnabled = true;
            tracktime = 60;
        }
    }
}
