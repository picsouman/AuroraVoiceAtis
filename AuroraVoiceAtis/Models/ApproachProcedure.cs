using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuroraVoiceAtis.ValueObjects;

namespace AuroraVoiceAtis.Models
{
    public class ApproachProcedure
    {
        public string RunwayDesignator { get; set; }
        public ApproachKind ApproachKind { get; set; }
        public string ComplementaryIdentifier { get; set; }
    }
}
