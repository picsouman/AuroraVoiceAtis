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
                    AtisCode = 'A',
                    SnapshotTime = DateTime.UtcNow,
                    Metar = decodedMetar,
                    DeparturesSuffix = DeparturesSuffix.Items.ToArray(),
                    ArrivalsSuffix = ArrivalsSuffix.Items.ToArray(),
                    DepartureRunways = DepartureRunways.Items.ToArray(),
                    ArrivalRunways = ArrivalRunways.Items.ToArray(),
                    ClosedRunways = ClosedRunways.Items.ToArray(),
                };

                PromptBuilder prompt = new PromptBuilder();
                AppendAtisPromp(prompt, "En", atis);
                prompt.AppendBreak(TimeSpan.FromSeconds(2));
                AppendAtisPromp(prompt, "Fr", atis);

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

        private void AppendAtisPromp(PromptBuilder prompt, string languageCode, AirportDataSnapshot atis)
        {
            var baseUri = $"Assets/Audio/{languageCode}/";
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
        }
    }
}
