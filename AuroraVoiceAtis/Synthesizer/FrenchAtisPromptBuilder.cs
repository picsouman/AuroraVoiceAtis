using AuroraVoiceAtis.Models;
using AuroraVoiceAtis.ValueObjects;
using csharp_metar_decoder.entity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AuroraVoiceAtis.Synthesizer
{
    internal class FrenchAtisPromptBuilder : IAtisPromptBuilder
    {
        private PromptBuilder promptBuilder;
        private SpeechSynthesizer synthesizer;
        public void Initialize(SpeechSynthesizer synthesizer, PromptBuilder promptBuilder)
        {
            this.promptBuilder = promptBuilder;
            this.synthesizer = synthesizer;
        }

        public void SetVoice()
        {
            var voiceToPlay = synthesizer
                .GetInstalledVoices()
                .Where(voice => voice.Enabled && voice.VoiceInfo.Culture.TwoLetterISOLanguageName == "fr")
                .FirstOrDefault();

            if (voiceToPlay is null)
            {
                throw new CultureNotFoundException("fr");
            }
            promptBuilder.StartVoice(voiceToPlay.VoiceInfo);
        }

        public void AppendDepartureProcedures()
        {
            promptBuilder.AppendText("Procédures de départ :");
        }

        public void AppendApproachProcedures()
        {
            promptBuilder.AppendText("Approches I F R :");
        }

        public void AppendArrivalProcedures()
        {
            promptBuilder.AppendText("Procédures d'arrivée :");
        }

        public void AppendArrivalRunways()
        {
            promptBuilder.AppendText("Pistes à l'arrivée :");
        }

        public void AppendCavok()
        {
            promptBuilder.AppendText("Cave-okay");
        }

        public void AppendDepartureRunways(IEnumerable<string> departureRunways)
        {
            promptBuilder.AppendText("Pistes au départ :");
            var runways = departureRunways.ToList();
            var firstRunway = true;
            foreach (var runway in runways)
            {
                if (!firstRunway)
                {
                    promptBuilder.AppendText(" et ");
                }
                firstRunway = false;
                promptBuilder.AppendText(runway);
            }
        }


        public void AppendIntroduction()
        {
            promptBuilder.AppendText("Bonjour, ici Lyon saint ex");
        }

        public void AppendRecordDatetime(DateTime dateTime)
        {
            var minutes = dateTime.Minute == 0
                ? string.Empty
                : dateTime.Minute.ToString();
            promptBuilder.AppendText($"Enregistré à {dateTime:HH} Heures {minutes} UTC");
        }

        public void AppendRunwayConditionCode()
        {
            promptBuilder.AppendText("Code état surface");
        }

        public void AppendRunwayDesignator(string runwayDesignator)
        {
            string runwayNumber = string.Empty;
            char? runwaySide = null;
            foreach (var letter in runwayDesignator)
            {
                if (char.IsDigit(letter))
                {
                    runwayNumber += letter;
                }
                else if (char.IsLetter(letter))
                {
                    runwaySide = letter;
                    break;
                }
            }
            promptBuilder.AppendText($"{runwayNumber}");

            if (runwaySide.HasValue)
            {
                switch (char.ToLower(runwaySide.Value))
                {
                    case 'l':
                        promptBuilder.AppendText(" gauche");
                        break;
                    case 'r':
                        promptBuilder.AppendText(" droite");
                        break;
                    case 'c':
                        promptBuilder.AppendText(" centre");
                        break;
                }
            }
        }

        public void AppendTransitionLevel()
        {
            promptBuilder.AppendText($"Niveau de transition");
        }

        public void AppendVisibilityKeyword()
        {
            promptBuilder.AppendText($"Visibilité");
        }

        public void AppendDepartureRunways()
        {
            promptBuilder.AppendText("Pistes au départ");
        }

        public void AppendRunwayKeyword()
        {
            promptBuilder.AppendText("Piste");
        }

        public void AppendAndKeyword()
        {
            promptBuilder.AppendText(" et ");
        }

        public void AppendInformationKeyword()
        {
            promptBuilder.AppendText("Information");
        }

        public void AppendOaciAlphabet(char letter)
        {
            var letterUpper = char.ToUpper(letter, CultureInfo.InvariantCulture);
            var oaciAlphabet = new Dictionary<char, string>
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
                { 'N', "Novembre" },
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
            if (oaciAlphabet.TryGetValue(letterUpper, out var word))
            {
                promptBuilder.AppendText(word);
            }
            else
            {
                promptBuilder.AppendText(letterUpper.ToString());
            }
        }

        public void AppendCloudKeyword()
        {
            promptBuilder.AppendText("Nuages");
        }

        public void AppendCloud(CloudLayer.CloudAmount amount)
        {
            switch (amount)
            {
                case CloudLayer.CloudAmount.FEW:
                    promptBuilder.AppendText("Léger");
                    break;
                case CloudLayer.CloudAmount.SCT:
                    promptBuilder.AppendText("épars");
                    break;
                case CloudLayer.CloudAmount.BKN:
                    promptBuilder.AppendText("fragmentés");
                    break;
                case CloudLayer.CloudAmount.OVC:
                    promptBuilder.AppendText("couvert");
                    break;
            }
        }

        public void AppendUnit(Units unit)
        {
            switch(unit)
            {
                case Units.Feets:
                    promptBuilder.AppendText("pieds");
                    break;
                case Units.Kilometers:
                    promptBuilder.AppendText("kilomètres");
                    break;
                case Units.Meters:
                    promptBuilder.AppendText("Mètres");
                    break;
                case Units.Knots:
                    promptBuilder.AppendText("noeuds");
                    break;
            }
        }

        public void AppendTemperatureDewPointQnh(int temperature, int dewPoint, int qnh)
        {
            promptBuilder.AppendText($"Température {temperature}; point de rosée {dewPoint};");
            promptBuilder.AppendText("QNH");
            AppendNumberOneByOne(qnh);
        }

        private void AppendNumberOneByOne(int number)
        {
            var digitsCount = (int)Math.Log10(number) + 1;
            for (int i = digitsCount - 1; i >= 0; i--)
            {
                var digit = (number / (int)Math.Pow(10, i)) % 10;
                AppendDigit(digit);
                promptBuilder.AppendBreak(TimeSpan.FromMilliseconds(50));
            }
        }

        public void AppendDigit(int digit)
        {
            if (digit == 1)
            {
                promptBuilder.AppendText("unitée");
            }
            else
            {
                promptBuilder.AppendText(digit.ToString());
            }
        }
    }
}
