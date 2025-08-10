using AuroraVoiceAtis.Core;
using AuroraVoiceAtis.Models;
using AuroraVoiceAtis.Utils;
using csharp_metar_decoder;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AuroraVoiceAtis.ViewModels
{
    public class MainWindowViewModel : WindowViewModelBase
    {
        private string metar;
        public string Metar
        {
            get => metar;
            set { metar = value; OnPropertyChanged(); }
        }

        public ICommand GenerateAndPlayAudioCommand { get; private set; }

        public char AtisCode { get; set; } = 'A';

        public StringCollectionEditorViewModel DeparturesSuffix { get; set; } = new StringCollectionEditorViewModel();

        public StringCollectionEditorViewModel ArrivalsSuffix { get; set; } = new StringCollectionEditorViewModel();

        public StringCollectionEditorViewModel DepartureRunways { get; set; } = new StringCollectionEditorViewModel();

        public StringCollectionEditorViewModel ArrivalRunways { get; set; } = new StringCollectionEditorViewModel();

        public StringCollectionEditorViewModel ClosedRunways { get; set; } = new StringCollectionEditorViewModel();

        public ObservableCollection<ApproachProcedureViewModel> approachProcedures { get; set; } = new ObservableCollection<ApproachProcedureViewModel>();

        public MainWindowViewModel()
        {
            Title = "Aurora Voice Atis";

            GenerateAndPlayAudioCommand = new RelayCommand(ExecuteGenerateAndPlayAudioCommand);
        }

        private void ExecuteGenerateAndPlayAudioCommand(object obj)
        {
            try
            {
                var decodedMetar = MetarDecoder.ParseWithMode(Metar);

                AirportDataSnapshot atis = new AirportDataSnapshot
                {
                    AtisCode = AtisCode,
                    SnapshotTime = DateTime.UtcNow,
                    Metar = decodedMetar,
                    DeparturesSuffix = DeparturesSuffix.Items.ToArray(),
                    ArrivalsSuffix = ArrivalsSuffix.Items.ToArray(),
                    DepartureRunways = DepartureRunways.Items.ToArray(),
                    ArrivalRunways = ArrivalRunways.Items.ToArray(),
                    ClosedRunways = ClosedRunways.Items.ToArray(),
                };

                PromptBuilder prompt = new PromptBuilder();
                AppendAtisPrompt(prompt, "En", atis);
                prompt.AppendBreak(TimeSpan.FromSeconds(1));
                AppendAtisPrompt(prompt, "Fr", atis);

                using (SpeechSynthesizer synth = new SpeechSynthesizer())
                {
                    synth.Speak(prompt);

                    var outputPath = "test.wav";
                    using (var stream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                    {
                        synth.SetOutputToWaveStream(stream);
                        synth.Speak(prompt);
                    }
                }
            }
            catch (Exception)
            {
                var t = 0;
            }
        }

        private void AppendAtisPrompt(PromptBuilder prompt, string languageCode, AirportDataSnapshot atis)
        {
            var baseUri = $"Assets/Audio/{languageCode}/";

            void AppendRunwayPrompt(string[] runways)
            {
                foreach (var runway in runways)
                {
                    foreach (var runwayChar in runway)
                    {
                        if (char.IsDigit(runwayChar))
                        {
                            prompt.AppendAudio($"{baseUri}{AudioSampleHelper.GetDigit(runwayChar)}");
                        }
                        else
                        {
                            prompt.AppendAudio($"{baseUri}{AudioSampleHelper.GetOrientation(runwayChar)}");
                        }
                    }
                    prompt.AppendBreak(TimeSpan.FromMilliseconds(250));
                }
            }

            prompt.AppendAudio($"{baseUri}{AudioSampleHelper.GetVocabulary(AudioSampleHelper.Vocabulary.GoodDay)}");

            prompt.AppendAudio($"{baseUri}{AudioSampleHelper.GetVocabulary(AudioSampleHelper.Vocabulary.Information)}");
            prompt.AppendAudio($"{baseUri}{AudioSampleHelper.GetAlphabet(atis.AtisCode)}");

            prompt.AppendAudio($"{baseUri}{AudioSampleHelper.GetVocabulary(AudioSampleHelper.Vocabulary.RecordedAt)}");
            var hourString = atis.SnapshotTime.ToString("HH");
            var minuteString = atis.SnapshotTime.ToString("mm");
            prompt.AppendAudio($"{baseUri}{AudioSampleHelper.GetDigit(hourString[0])}");
            prompt.AppendAudio($"{baseUri}{AudioSampleHelper.GetDigit(hourString[1])}");
            prompt.AppendAudio($"{baseUri}{AudioSampleHelper.GetDigit(minuteString[0])}");
            prompt.AppendAudio($"{baseUri}{AudioSampleHelper.GetDigit(minuteString[1])}");
            prompt.AppendAudio($"{baseUri}{AudioSampleHelper.GetVocabulary(AudioSampleHelper.Vocabulary.Utc)}");

            if (atis.DepartureRunways.Any()) {
                prompt.AppendAudio($"{baseUri}{AudioSampleHelper.GetVocabulary(AudioSampleHelper.Vocabulary.Departure)}");
                prompt.AppendAudio($"{baseUri}{AudioSampleHelper.GetVocabulary(AudioSampleHelper.Vocabulary.Runway)}");

                AppendRunwayPrompt(atis.DepartureRunways);
            }

            //if (atis.ArrivalRunways.Any())
            //{
            //    prompt.AppendAudio($"{baseUri}{AudioSampleHelper.GetVocabulary(AudioSampleHelper.Vocabulary.Arrival)}");
            //    prompt.AppendAudio($"{baseUri}{AudioSampleHelper.GetVocabulary(AudioSampleHelper.Vocabulary.Runway)}");

            //    AppendRunwayPrompt(atis.DepartureRunways);
            //}

            // Wind
            prompt.AppendAudio($"{baseUri}{AudioSampleHelper.GetVocabulary(AudioSampleHelper.Vocabulary.Wind)}");
            var parsedWindDirection = atis.Metar.SurfaceWind.MeanDirection.ActualValue.ToString("###");
            foreach (var digit in parsedWindDirection)
            {
                prompt.AppendAudio($"{baseUri}{AudioSampleHelper.GetDigit(digit)}");
            }
            prompt.AppendAudio($"{baseUri}{AudioSampleHelper.GetUnit(AudioSampleHelper.Units.Degrees)}");

            var parsedWindSpeed = atis.Metar.SurfaceWind.MeanSpeed.ActualValue.ToString("###");
            foreach (var digit in parsedWindSpeed)
            {
                prompt.AppendAudio($"{baseUri}{AudioSampleHelper.GetDigit(digit)}");
            }
            prompt.AppendAudio($"{baseUri}{AudioSampleHelper.GetUnit(AudioSampleHelper.Units.Knots)}");

            
        }
    }
}
