using AuroraVoiceAtis.ValueObjects;
using csharp_metar_decoder.entity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;

namespace AuroraVoiceAtis.Synthesizer
{
    public class EnglishAtisPromptBuilder : IAtisPromptBuilder
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
                .Where(voice => voice.Enabled && voice.VoiceInfo.Culture.TwoLetterISOLanguageName == "en")
                .FirstOrDefault();

            if (voiceToPlay is null)
            {
                throw new CultureNotFoundException("en");
            }
            promptBuilder.StartVoice(voiceToPlay.VoiceInfo);
        }

        public void AppendDepartureProcedures()
        {
            promptBuilder.AppendText("Departure procedure :");
        }

        public void AppendApproachProcedures()
        {
            promptBuilder.AppendText("I F R Approach :");
        }

        public void AppendArrivalProcedures()
        {
            promptBuilder.AppendText("Arrival procedure :");
        }

        public void AppendArrivalRunways()
        {
            promptBuilder.AppendText("Landing runway :");
        }

        public void AppendCavok()
        {
            promptBuilder.AppendText("Cave OK");
        }

        public void AppendIntroduction()
        {
            promptBuilder.AppendText("Good day, this is Lyon saint ex");
        }

        public void AppendRecordDatetime(DateTime dateTime)
        {
            promptBuilder.AppendText($"Recorded at ");
            foreach (var digit in dateTime.ToString("HHmm"))
            {
                AppendDigit(digit);
            }
            promptBuilder.AppendText($" UTC");
        }

        public void AppendRunwayConditionCode()
        {
            promptBuilder.AppendText("Runway condition code");
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

            foreach (var digit in runwayNumber)
            {
                AppendDigit(digit);
            }

            if (runwaySide.HasValue)
            {
                switch (char.ToLower(runwaySide.Value))
                {
                    case 'l':
                        promptBuilder.AppendText(" left");
                        break;
                    case 'r':
                        promptBuilder.AppendText(" right");
                        break;
                    case 'c':
                        promptBuilder.AppendText(" center");
                        break;
                }
            }
        }

        public void AppendTransitionLevel()
        {
            promptBuilder.AppendText($"Transition level");
        }

        public void AppendVisibilityKeyword()
        {
            promptBuilder.AppendText($"Visibility");
        }

        public void AppendDepartureRunways()
        {
            promptBuilder.AppendText("Departing runway");
        }

        public void AppendRunwayKeyword()
        {
            promptBuilder.AppendText("Runway");
        }

        public void AppendAndKeyword()
        {
            promptBuilder.AppendText(" and ");
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
            promptBuilder.AppendText("clouds");
        }

        public void AppendCloud(CloudLayer.CloudAmount amount)
        {
            switch (amount)
            {
                case CloudLayer.CloudAmount.FEW:
                    promptBuilder.AppendText("few");
                    break;
                case CloudLayer.CloudAmount.SCT:
                    promptBuilder.AppendText("scatered");
                    break;
                case CloudLayer.CloudAmount.BKN:
                    promptBuilder.AppendText("broken");
                    break;
                case CloudLayer.CloudAmount.OVC:
                    promptBuilder.AppendText("overcast");
                    break;
            }
        }

        public void AppendUnit(Units unit)
        {
            switch (unit)
            {
                case Units.Feets:
                    promptBuilder.AppendText("feets");
                    break;
                case Units.Kilometers:
                    promptBuilder.AppendText("kilometers");
                    break;
                case Units.Meters:
                    promptBuilder.AppendText("meters");
                    break;
                case Units.Knots:
                    promptBuilder.AppendText("knots");
                    break;
                case Units.Degrees:
                    promptBuilder.AppendText("degrees");
                    break;
            }
        }

        public void AppendTemperatureDewPointQnh(int temperature, int dewPoint, int qnh)
        {
            promptBuilder.AppendText($"temperature ");
            AppendNumberOneByOne(temperature);
            promptBuilder.AppendText("; dew point ");
            AppendNumberOneByOne(dewPoint);
            promptBuilder.AppendText("; Q N H");
            AppendNumberOneByOne(qnh);
        }

        private void AppendNumberOneByOne(int number)
        {
            var digitsCount = (int)Math.Log10(number) + 1;
            for (int i = digitsCount - 1; i >= 0; i--)
            {
                var digit = (number / (int)Math.Pow(10, i)) % 10;
                AppendDigit(digit);
            }
        }

        public void AppendDigit(int digit)
        {
            if (digit < 0 || digit > 9)
            {
                throw new ArgumentOutOfRangeException(nameof(digit), "Digit must be between 0 and 9.");
            }
            AppendDigit((char)(digit + 48));
        }

        public void AppendDigit(char digit)
        {
            promptBuilder.AppendBreak(TimeSpan.FromMilliseconds(5));
            promptBuilder.AppendText(digit.ToString());
            promptBuilder.AppendBreak(TimeSpan.FromMilliseconds(5));
        }

        public void AppendNumber(int number)
        {
            if (number < 0)
            {
                AppendMinusKeyword();
                promptBuilder.AppendBreak(TimeSpan.FromMilliseconds(50));
            }
            AppendNumberOneByOne(number);
        }

        public void AppendWindKeyword()
        {
            promptBuilder.AppendText("wind");
        }

        public void AppendGustKeyword()
        {
            promptBuilder.AppendText("gust");
        }

        public void AppendConclusion(char atisInformation)
        {
            promptBuilder.AppendText("Inform Saint-ex you have received information ");
            AppendOaciAlphabet(atisInformation);
            promptBuilder.AppendText(" on initial contact.");
        }

        private void AppendMinusKeyword()
        {
            promptBuilder.AppendText("minus");
        }
    }
}
