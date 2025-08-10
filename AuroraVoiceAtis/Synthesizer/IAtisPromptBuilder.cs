using AuroraVoiceAtis.Models;
using AuroraVoiceAtis.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using static csharp_metar_decoder.entity.CloudLayer;

namespace AuroraVoiceAtis.Synthesizer
{
    internal interface IAtisPromptBuilder
    {
        void Initialize(SpeechSynthesizer synthesizer, PromptBuilder promptBuilder);
        void SetVoice();
        void AppendIntroduction();
        void AppendRecordDatetime(DateTime dateTime);
        void AppendArrivalProcedures();
        void AppendApproachProcedures();
        void AppendArrivalRunways();
        void AppendDepartureRunways();
        void AppendDepartureProcedures();
        void AppendRunwayKeyword();
        void AppendRunwayDesignator(string runwayDesignator);
        void AppendRunwayConditionCode();
        void AppendTransitionLevel();
        void AppendCavok();
        void AppendVisibilityKeyword();
        void AppendAndKeyword();
        void AppendInformationKeyword();
        void AppendOaciAlphabet(char letter);
        void AppendCloudKeyword();
        void AppendCloud(CloudAmount amount);
        void AppendUnit(Units unit);
        void AppendTemperatureDewPointQnh(int temperature, int dewPoint, int qnh);

        void AppendDigit(int digit);
    }
}
