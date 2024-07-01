using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
using System.Globalization;
using FrostweepGames.Plugins.Core;
using TMPro;

namespace FrostweepGames.Plugins.GoogleCloud.TextToSpeech
{
    public class GC_TextToSpeech_TutorialExample : MonoBehaviour
    {
        public static float Voice = 7;
        public static GCTextToSpeech _gcTextToSpeech;
        public UnityEvent SynthesizeEvents, FinalResponseEvent, TTSVoiceStartEvent;
        public UnityEvent[] TTS_CompleteEvent;
        public bool AutoSendText, do_Nothing;
        private bool lastmessage;

        private Voice[] _voices;
        public Voice _currentVoice;
        public string Voices;

        public static CultureInfo _provider;

        

        private string UserInput;
        public InputField pitchInputField;
        public InputField speakingRateInputField;


        public Toggle ssmlToggle;

        public Dropdown languageCodesDropdown;
        public Dropdown voiceTypesDropdown;
        public Dropdown voicesDropdown;

        public AudioSource audioSource;

        private void Start()
        {
            lastmessage = false;

            _gcTextToSpeech = GCTextToSpeech.Instance;

            _gcTextToSpeech.GetVoicesSuccessEvent += _gcTextToSpeech_GetVoicesSuccessEvent;
            _gcTextToSpeech.SynthesizeSuccessEvent += _gcTextToSpeech_SynthesizeSuccessEvent;

            _gcTextToSpeech.GetVoicesFailedEvent += _gcTextToSpeech_GetVoicesFailedEvent;
            _gcTextToSpeech.SynthesizeFailedEvent += _gcTextToSpeech_SynthesizeFailedEvent;
            
            voicesDropdown.onValueChanged.AddListener(VoiceSelectedDropdownOnChangedHandler);
            voiceTypesDropdown.onValueChanged.AddListener(VoiceTypeSelectedDropdownOnChangedHandler);

            _provider = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            _provider.NumberFormat.NumberDecimalSeparator = ".";

            voicesDropdown.ClearOptions();
            string newOption = Voices;
            voicesDropdown.options.Add(new Dropdown.OptionData(newOption));
            voicesDropdown.RefreshShownValue();
            voicesDropdown.value = 0;



            GetVoicesButtonOnClickHandler();
        }

        public void SynthesizeButtonOnClickHandler(string Input)
        {
            UserInput = Input;
            string content = UserInput;
            

            if (string.IsNullOrEmpty(content) || _currentVoice == null)
                return;

            if (GeneralConfig.Config.betaAPI)
            {
                _gcTextToSpeech.Synthesize(content, new VoiceConfig()
                {
                    gender = _currentVoice.ssmlGender,
                    languageCode = _currentVoice.languageCodes[0],
                    name = _currentVoice.name
                },
                ssmlToggle.isOn,
                double.Parse(pitchInputField.text, _provider),
                double.Parse(speakingRateInputField.text, _provider),
                _currentVoice.naturalSampleRateHertz, new string[0] { },
                new Enumerators.TimepointType[] { Enumerators.TimepointType.TIMEPOINT_TYPE_UNSPECIFIED });
            }
            else
            {
                _gcTextToSpeech.Synthesize(content, new VoiceConfig()
                {
                    gender = _currentVoice.ssmlGender,
                    languageCode = _currentVoice.languageCodes[0],
                    name = _currentVoice.name
                },
                ssmlToggle.isOn,
                double.Parse(pitchInputField.text, _provider),
                double.Parse(speakingRateInputField.text, _provider),
                _currentVoice.naturalSampleRateHertz, new string[0] { });
            }
        }

        private void GetVoicesButtonOnClickHandler()
        {
            _gcTextToSpeech.GetVoices(new GetVoicesRequest()
            {
                //languageCode = LanguageCode
                //languageCode = _gcTextToSpeech.PrepareLanguage((Enumerators.LanguageCode)languageCodesDropdown.value)
            });
        }


        private void FillVoicesList()
        {            
            VoiceSelectedDropdownOnChangedHandler(0);            
        }

        private void VoiceSelectedDropdownOnChangedHandler(int index)
        {
            var voice = _voices.ToList().Find(item => item.name.Contains(voicesDropdown.options[0].text));
            _currentVoice = voice;
        }

        private void VoiceTypeSelectedDropdownOnChangedHandler(int index)
        {
            FillVoicesList();
        }

        #region failed handlers

        private void _gcTextToSpeech_SynthesizeFailedEvent(string error, long requestId)
        {
            Debug.Log(error);
        }

        private void _gcTextToSpeech_GetVoicesFailedEvent(string error, long requestId)
        {
            Debug.Log(error);
        }

        #endregion failed handlers

        #region sucess handlers

        private void _gcTextToSpeech_SynthesizeSuccessEvent(PostSynthesizeResponse response, long requestId)
        {
            audioSource.clip = _gcTextToSpeech.GetAudioClipFromBase64(response.audioContent, Constants.DEFAULT_AUDIO_ENCODING);
            audioSource.Play();
            TTSVoiceStartEvent.Invoke();
            Invoke("StartRecord", audioSource.clip.length);            
        }
        public void StartRecord()
        {
            if (AutoSendText)
            {
                if (lastmessage)
                {
                    FinalResponseEvent.Invoke();
                    lastmessage = false;
                }
                else
                {
                    SynthesizeEvents.Invoke();
                }
            }            
            else
            {                
                TTS_CompleteEvent[PlayerPrefs.GetInt("EventNumber")].Invoke();
            }            
            
        }
        public void VoiceRateChange0_7()
        {
            speakingRateInputField.text = "0.7";
        }
        public void VoiceRateChange0_8()
        {
            speakingRateInputField.text = "0.8";
        }
        public void VoiceRateChange0_9()
        {
            speakingRateInputField.text = "0.9";
        }
        public void VoiceRateChange1_0()
        {
            speakingRateInputField.text = "1.0";
        }

        private void _gcTextToSpeech_GetVoicesSuccessEvent(GetVoicesResponse response, long requestId)
        {
            _voices = response.voices;

            FillVoicesList();
        }
        public void LastMessage(string Input)
        {
            lastmessage = true;
            SynthesizeButtonOnClickHandler(Input);
        }


        #endregion sucess handlers
    }
    
}