using csharp_metar_decoder.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraVoiceAtis.Models
{
    public class AirportDataSnapshot
    {
        public char AtisCode { get; set; }

        public DecodedMetar Metar { get; set; }
        public int TransitionLevel { get; set; }

        public DateTime SnapshotTime { get; set; }

        public string[] DeparturesSuffix { get; set; }

        public string[] ArrivalsSuffix { get; set; }

        public string[] DepartureRunways { get; set; }

        public string[] ClosedRunways { get; set; }

        public string[] ArrivalRunways { get; set; }

        public ApproachProcedure[] Approaches { get; set; }

        public RunwayCondition[] RunwayConditions { get; set; }
    }
}
