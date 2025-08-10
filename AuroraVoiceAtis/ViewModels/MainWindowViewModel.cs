using AuroraVoiceAtis.Core;
using AuroraVoiceAtis.Models;
using AuroraVoiceAtis.Synthesizer;
using csharp_metar_decoder;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Media3D;

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
                    TransitionLevel = 60,
                    SnapshotTime = DateTime.UtcNow,
                    Metar = decodedMetar,
                    Approaches = new ApproachProcedure[]
                    {
                        new ApproachProcedure() { RunwayDesignator = "35R", ApproachKind = ValueObjects.ApproachKind.ILS, ComplementaryIdentifier = "" }
                    },
                    RunwayConditions = new RunwayCondition[] {
                        new RunwayCondition() { RunwayDesignator = "35L", CodeTier1 = 6, CodeTier2 = 6, CodeTier3 = 6 },
                        new RunwayCondition() { RunwayDesignator = "35R", CodeTier1 = 6, CodeTier2 = 6, CodeTier3 = 6 }
                    },
                    DeparturesSuffix = DeparturesSuffix.Items.ToArray(),
                    ArrivalsSuffix = ArrivalsSuffix.Items.ToArray(),
                    DepartureRunways = DepartureRunways.Items.ToArray(),
                    ArrivalRunways = ArrivalRunways.Items.ToArray(),
                    ClosedRunways = ClosedRunways.Items.ToArray(),
                };

                using (SpeechSynthesizer synth = new SpeechSynthesizer())
                {
                    PromptBuilder prompt = new PromptBuilder();
                    IAtisPromptBuilder[] atisPromptBuilders =
                    {
                        new FrenchAtisPromptBuilder(),
                    };

                    foreach (var atisPromptBuilder in atisPromptBuilders)
                    {
                        atisPromptBuilder.Initialize(synth, prompt);
                        atisPromptBuilder.SetVoice();

                        atisPromptBuilder.AppendIntroduction();
                        prompt.AppendBreak(TimeSpan.FromMilliseconds(250));

                        atisPromptBuilder.AppendInformationKeyword();
                        atisPromptBuilder.AppendOaciAlphabet(atis.AtisCode);
                        prompt.AppendBreak(TimeSpan.FromMilliseconds(250));

                        atisPromptBuilder.AppendRecordDatetime(atis.SnapshotTime);
                        prompt.AppendBreak(TimeSpan.FromMilliseconds(250));

                        // procedures
                        if (atis.ArrivalsSuffix.Any())
                        {
                            atisPromptBuilder.AppendArrivalProcedures();
                            prompt.AppendBreak(TimeSpan.FromMilliseconds(50));
                            foreach(var arrivalSuffix in atis.ArrivalsSuffix)
                            {
                                AppendCharsOneByOneAndConvertLetterToOaciAlphabet(prompt, atisPromptBuilder, arrivalSuffix);
                            }
                        }

                        if (atis.DeparturesSuffix.Any())
                        {
                            atisPromptBuilder.AppendDepartureProcedures();
                            prompt.AppendBreak(TimeSpan.FromMilliseconds(50));
                            foreach (var departureSuffix in atis.DeparturesSuffix)
                            {
                                AppendCharsOneByOneAndConvertLetterToOaciAlphabet(prompt, atisPromptBuilder, departureSuffix);
                            }
                            prompt.AppendBreak(TimeSpan.FromMilliseconds(250));
                        }

                        // approaches
                        if (atis.Approaches.Any())
                        {
                            atisPromptBuilder.AppendApproachProcedures();
                            foreach (var approach in atis.Approaches)
                            {
                                prompt.AppendBreak(TimeSpan.FromMilliseconds(50));
                                switch (approach.ApproachKind)
                                {
                                    case ValueObjects.ApproachKind.ILS:
                                        prompt.AppendText("I L S");
                                        break;
                                    case ValueObjects.ApproachKind.RNP:
                                        prompt.AppendText("R N P");
                                        break;
                                    case ValueObjects.ApproachKind.VOR:
                                        prompt.AppendText("VOR");
                                        break;
                                }
                                if (!string.IsNullOrEmpty(approach.ComplementaryIdentifier))
                                {
                                    prompt.AppendBreak(TimeSpan.FromMilliseconds(10));
                                    AppendCharsOneByOneAndConvertLetterToOaciAlphabet(prompt, atisPromptBuilder, approach.ComplementaryIdentifier);
                                }

                                prompt.AppendBreak(TimeSpan.FromMilliseconds(10));
                                atisPromptBuilder.AppendRunwayKeyword();
                                prompt.AppendBreak(TimeSpan.FromMilliseconds(10));
                                atisPromptBuilder.AppendRunwayDesignator(approach.RunwayDesignator);
                            }
                            prompt.AppendBreak(TimeSpan.FromMilliseconds(250));
                        }

                        // arrival runways
                        if (atis.ArrivalRunways.Any())
                        {
                            atisPromptBuilder.AppendArrivalRunways();
                            prompt.AppendBreak(TimeSpan.FromMilliseconds(50));
                            var firstArrivalRunway = true;
                            foreach (var runway in atis.ArrivalRunways)
                            {
                                if (!firstArrivalRunway)
                                {
                                    atisPromptBuilder.AppendAndKeyword();
                                }
                                atisPromptBuilder.AppendRunwayDesignator(runway);
                                firstArrivalRunway = false;
                            }
                            prompt.AppendBreak(TimeSpan.FromMilliseconds(250));
                        }

                        // departure runways
                        if (atis.DepartureRunways.Any())
                        {
                            atisPromptBuilder.AppendDepartureRunways();
                            prompt.AppendBreak(TimeSpan.FromMilliseconds(50));
                            var firstDepartureRunway = true;
                            foreach (var runway in atis.DepartureRunways)
                            {
                                if (!firstDepartureRunway)
                                {
                                    atisPromptBuilder.AppendAndKeyword();
                                }
                                atisPromptBuilder.AppendRunwayDesignator(runway);
                                firstDepartureRunway = false;
                            }
                            prompt.AppendBreak(TimeSpan.FromMilliseconds(250));
                        }

                        // RWYCC
                        if (atis.RunwayConditions.Any())
                        {
                            foreach(var runwayCondition in atis.RunwayConditions)
                            {
                                prompt.AppendBreak(TimeSpan.FromMilliseconds(50));
                                atisPromptBuilder.AppendRunwayConditionCode();
                                prompt.AppendBreak(TimeSpan.FromMilliseconds(50));
                                atisPromptBuilder.AppendRunwayDesignator(runwayCondition.RunwayDesignator);
                                prompt.AppendBreak(TimeSpan.FromMilliseconds(50));
                                prompt.AppendText($"{runwayCondition.CodeTier1};{runwayCondition.CodeTier2};{runwayCondition.CodeTier3};");
                            }
                            prompt.AppendBreak(TimeSpan.FromMilliseconds(250));
                        }

                        // transition level
                        atisPromptBuilder.AppendTransitionLevel();
                        AppendNumberOneByOne(prompt, atisPromptBuilder, atis.TransitionLevel);

                        prompt.AppendBreak(TimeSpan.FromMilliseconds(250));

                        //weather
                        if (atis.Metar.Cavok)
                        {
                            atisPromptBuilder.AppendCavok();
                        }
                        else
                        {
                            atisPromptBuilder.AppendCloudKeyword();
                            foreach (var cloud in atis.Metar.Clouds)
                            {
                                prompt.AppendBreak(TimeSpan.FromMilliseconds(100));
                                atisPromptBuilder.AppendCloud(cloud.Amount);
                                prompt.AppendBreak(TimeSpan.FromMilliseconds(20));
                                prompt.AppendText(cloud.BaseHeight.ActualValue.ToString("0"));
                                atisPromptBuilder.AppendUnit(ValueObjects.Units.Feets);
                            }

                            prompt.AppendBreak(TimeSpan.FromMilliseconds(100));

                            atisPromptBuilder.AppendVisibilityKeyword();
                            var visibility = atis.Metar.Visibility.MinimumVisibility.ActualValue;
                            if (visibility % 1000 == 0)
                            {
                                prompt.AppendText((visibility / 1000).ToString("0"));
                                atisPromptBuilder.AppendUnit(ValueObjects.Units.Kilometers);
                            }
                            else
                            {
                                prompt.AppendText(visibility.ToString("0"));
                                atisPromptBuilder.AppendUnit(ValueObjects.Units.Meters);
                            }
                        }

                        prompt.AppendBreak(TimeSpan.FromMilliseconds(250));

                        atisPromptBuilder.AppendTemperatureDewPointQnh(
                            temperature: (int)atis.Metar.AirTemperature.ActualValue,
                            dewPoint: (int)atis.Metar.DewPointTemperature.ActualValue,
                            qnh: (int)atis.Metar.Pressure.ActualValue
                        );

                        // conclusion



                        prompt.EndVoice();
                    }

                    synth.Speak(prompt);

                    //var outputPath = "test.wav";
                    //using (var stream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                    //{
                    //    synth.SetOutputToWaveStream(stream);
                    //    synth.Speak(prompt);
                    //}
                }
            }
            catch (Exception)
            {
                var t = 0;
            }
        }

        private void AppendNumberOneByOne(PromptBuilder prompt, IAtisPromptBuilder atisPromptBuilder, int number)
        {
            var digitsCount = (int)Math.Log10(number) + 1;
            for (int i = digitsCount - 1; i >= 0; i--)
            {
                var digit = (number / (int)Math.Pow(10, i)) % 10;
                atisPromptBuilder.AppendDigit(digit);
                prompt.AppendBreak(TimeSpan.FromMilliseconds(50));
            }
        }

        private void AppendCharsOneByOne(PromptBuilder prompt, string text)
        {
            prompt.AppendText(string.Join(";", text));
        }

        private void AppendCharsOneByOneAndConvertLetterToOaciAlphabet(PromptBuilder prompt, IAtisPromptBuilder atisPromptBuilder, string text)
        {
            foreach (var c in text)
            {
                if (char.IsDigit(c))
                {
                    prompt.AppendText($"{c};");
                }
                else
                {
                    atisPromptBuilder.AppendOaciAlphabet(c);
                    prompt.AppendText(";");
                }
            }
        }
    }
}
