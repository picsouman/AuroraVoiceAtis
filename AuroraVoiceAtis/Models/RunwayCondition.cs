using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraVoiceAtis.Models
{
    public class RunwayCondition
    {
        public string RunwayDesignator { get; set; }
        public byte CodeTier1 { get; set; }
        public byte CodeTier2 { get; set; }
        public byte CodeTier3 { get; set; }
    }
}
