using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AuroraVoiceAtis.ViewModels;

namespace AuroraVoiceAtis.Views
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        //private void TestButton_Click(object sender, RoutedEventArgs e)
        //{
        //    Console.WriteLine("Entrez le metar :");
        //    var metar = "METAR LFLL 190930Z 35006KT 9999 -TSRA SCT033 SCT043CB BKN048 18/16 Q1013 BECMG NSW FEW050CB";

        //    PromptBuilder promptFr = GeneratePrompt(metar);

        //    using (SpeechSynthesizer synth = new SpeechSynthesizer())
        //    {
        //        synth.SpeakAsync(promptFr);

        //        //var outputPath = "test.wav";
        //        //using (var stream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
        //        //{
        //        //    synth.SetOutputToWaveStream(stream);
        //        //    synth.Speak(promptFr);
        //        //}
        //    }
        //}

        //private PromptBuilder GeneratePrompt(string metar)
        //{
        //    var pb = new PromptBuilder();

        //    var baseUri = "Assets/Audio/Fr/";
        //    pb.AppendAudio($"{baseUri}/Vocabulary/GoodDay.wav");
        //    pb.AppendAudio($"{baseUri}/Vocabulary/Information.wav");
        //    pb.AppendAudio($"{baseUri}/Alphabet/Foxtrot.wav");
        //    pb.AppendAudio($"{baseUri}/Vocabulary/RecordedAt.wav");

        //    var utcNow = DateTime.UtcNow;
        //    var hour = utcNow.ToString("HH");
        //    var minutes = utcNow.ToString("mm");
        //    pb.AppendAudio($"{baseUri}/Digits/{hour[0]}.wav");
        //    pb.AppendAudio($"{baseUri}/Digits/{hour[1]}.wav");
        //    pb.AppendAudio($"{baseUri}/Digits/{minutes[0]}.wav");
        //    pb.AppendAudio($"{baseUri}/Digits/{minutes[1]}.wav");
        //    pb.AppendAudio($"{baseUri}/Alphabet/Zulu.wav");

        //    return pb;
        //}
    }
}
