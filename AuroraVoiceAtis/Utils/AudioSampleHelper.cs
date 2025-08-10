using AuroraVoiceAtis.ValueObjects;
using csharp_metar_decoder.entity;
using System;
using System.Collections.Generic;

namespace AuroraVoiceAtis.Utils
{
    public static class AudioSampleHelper
    {
        public enum Vocabulary
        {
            Cavok,
            Departure,
            DewPoint,
            GoodDay,
            Gust,
            IfrApproach,
            Information,
            RecordedAt,
            Runway,
            Temperature,
            Utc,
            Variable,
            Visibility,
            Wind,
        }

        public enum Units
        {
            Degrees,
            Feets,
            Kilometers,
            Knots,
        }

        private readonly static Dictionary<char, string> alphabetMapping = new Dictionary<char, string>
        {
            { 'A', "Alpha" },
            { 'B', "Bravo" },
            { 'C', "Charlie" },
            { 'D', "Delta" },
            { 'E', "Echo" },
            { 'F', "Foxtrot" },
            { 'G', "Golf" },
            { 'H', "Hotel" },
            { 'I', "India" },
            { 'J', "Juliett" },
            { 'K', "Kilo" },
            { 'L', "Lima" },
            { 'M', "Mike" },
            { 'N', "November" },
            { 'O', "Oscar" },
            { 'P', "Papa" },
            { 'Q', "Quebec" },
            { 'R', "Romeo" },
            { 'S', "Sierra" },
            { 'T', "Tango" },
            { 'U', "Uniform" },
            { 'V', "Victor" },
            { 'W', "Whiskey" },
            { 'X', "X-ray" },
            { 'Y', "Yankee" },
            { 'Z', "Zulu" }
        };

        private readonly static Dictionary<CloudLayer.CloudAmount, string> cloudMapping = new Dictionary<CloudLayer.CloudAmount, string>
        {
            { CloudLayer.CloudAmount.FEW, "Few" },
            { CloudLayer.CloudAmount.SCT, "Scatered" },
            { CloudLayer.CloudAmount.BKN, "Broken" },
            { CloudLayer.CloudAmount.OVC, "Overcast" }
        };

        private readonly static Dictionary<Vocabulary, string> vocabularyMapping = new Dictionary<Vocabulary, string>
        {
            { Vocabulary.Cavok, "Cavok" },
            { Vocabulary.Departure, "Departure" },
            { Vocabulary.DewPoint, "DewPoint" },
            { Vocabulary.GoodDay, "GoodDay" },
            { Vocabulary.Gust, "Gust" },
            { Vocabulary.IfrApproach, "IfrApproach" },
            { Vocabulary.Information, "Information" },
            { Vocabulary.RecordedAt, "RecordedAt" },
            { Vocabulary.Runway, "Runway" },
            { Vocabulary.Temperature, "Temperature" },
            { Vocabulary.Utc, "Utc" },
            { Vocabulary.Variable, "Variable" },
            { Vocabulary.Visibility, "Visibility" },
            { Vocabulary.Wind, "Wind" }
        };

        private static readonly Dictionary<ApproachKind, string> procedureMapping = new Dictionary<ApproachKind, string>
        {
            { ApproachKind.ILS, "Ils" },
            { ApproachKind.VOR, "Vor" },
            //{ ApproachKind.NDB, "Ndb" },
            //{ ApproachKind.Visual, "Visual" },
            { ApproachKind.RNP, "Rnp" }
        };

        private static readonly Dictionary<Units, string> unitsMapping = new Dictionary<Units, string>
        {
            { Units.Degrees, "Degrees" },
            { Units.Feets, "Feets" },
            { Units.Kilometers, "Kilometers" },
            { Units.Knots, "Knots" }
        };


        public static string GetAlphabet(char letter)
        {
            return $"Alphabet/{alphabetMapping[char.ToUpper(letter)]}.wav";
        }

        public static string GetCloud(CloudLayer.CloudAmount cloudAmound)
        {
            return $"Clouds/{cloudMapping[cloudAmound]}.wav";
        }

        public static string GetDigit(char digit)
        {
            if (!char.IsDigit(digit))
            {
                throw new ArgumentOutOfRangeException(nameof(digit), "Character must be a digit (0-9).");
            }
            return $"Digits/{digit}.wav";
        }

        public static string GetDigit(int digit)
        {
            if (digit < 0 || digit > 9)
                throw new ArgumentOutOfRangeException(nameof(digit), "Digit must be between 0 and 9.");
            return $"Digits/{digit}.wav";
        }

        public static string GetOrientation(char orientation)
        {
            var orientationLower = char.ToLower(orientation);
            if (orientationLower == 'l')
                return "Orientation/Left.wav";
            if (orientationLower == 'r')
                return "Orientation/Right.wav";
            throw new ArgumentOutOfRangeException(nameof(orientation), "Orientation must be 'L' or 'R'.");
        }

        public static string GetProcedure(ApproachKind approachKind)
        {
            return $"Procedures/{procedureMapping[approachKind]}.wav";
        }

        public static string GetVocabulary(Vocabulary vocabulary)
        {
            return $"Vocabulary/{vocabularyMapping[vocabulary]}.wav";
        }

        public static string GetUnit(Units unit)
        {
            return $"Units/{unitsMapping[unit]}.wav";
        }
    }
}
