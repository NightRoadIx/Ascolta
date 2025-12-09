#if ANDROID
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Speech;
using Ascolta.Services;
using Microsoft.Maui.ApplicationModel;
using Application = Android.App.Application;

namespace Ascolta.Platforms.Android.Services
{
    public class SpeechToTextService : Java.Lang.Object, ISpeechToTextService, IRecognitionListener
    {
        private SpeechRecognizer? _speechRecognizer;
        private Intent? _speechIntent;

        public event EventHandler<string>? TranscriptionUpdated;

        public bool IsListening { get; private set; }

        public async Task StartListeningAsync()
        {
            if (IsListening)
                return;

            // 1. Verificar permiso de micrófono
            var status = await Permissions.CheckStatusAsync<Permissions.Microphone>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.Microphone>();
                if (status != PermissionStatus.Granted)
                    throw new Exception("Permiso de micrófono denegado.");
            }

            // 2. Crear SpeechRecognizer (una sola vez)
            if (_speechRecognizer == null)
            {
                _speechRecognizer = SpeechRecognizer.CreateSpeechRecognizer(Application.Context);
                _speechRecognizer.SetRecognitionListener(this);
            }

            // 3. Crear Intent
            _speechIntent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
            _speechIntent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
            _speechIntent.PutExtra(RecognizerIntent.ExtraLanguage, Java.Util.Locale.Default);
            _speechIntent.PutExtra(RecognizerIntent.ExtraPartialResults, true);

            // 4. Empezar a escuchar
            _speechRecognizer.StartListening(_speechIntent);
            IsListening = true;
        }

        public Task StopListeningAsync()
        {
            if (!IsListening)
                return Task.CompletedTask;

            _speechRecognizer?.StopListening();
            IsListening = false;
            return Task.CompletedTask;
        }

        // IRecognitionListener implementación

        public void OnBeginningOfSpeech()
        {
            // Opcional: podrías notificar algo
        }

        public void OnBufferReceived(byte[] buffer) { }

        public void OnEndOfSpeech()
        {
            // Para hacerlo "continuo", cuando termina volvemos a iniciar
            if (IsListening && _speechRecognizer != null && _speechIntent != null)
            {
                _speechRecognizer.StartListening(_speechIntent);
            }
        }

        public void OnError([GeneratedEnum] SpeechRecognizerError error)
        {
            // Si hay error y seguimos en modo "escuchando", reintentar
            if (IsListening && _speechRecognizer != null && _speechIntent != null)
            {
                _speechRecognizer.StartListening(_speechIntent);
            }
        }

        public void OnEvent(int eventType, Bundle? @params) { }

        public void OnPartialResults(Bundle? partialResults)
        {
            var texts = partialResults?.GetStringArrayList(SpeechRecognizer.ResultsRecognition);
            if (texts != null && texts.Count > 0)
            {
                var text = texts[0];
                TranscriptionUpdated?.Invoke(this, text);
            }
        }

        public void OnReadyForSpeech(Bundle? @params) { }

        public void OnResults(Bundle? results)
        {
            var texts = results?.GetStringArrayList(SpeechRecognizer.ResultsRecognition);
            if (texts != null && texts.Count > 0)
            {
                var text = texts[0];
                TranscriptionUpdated?.Invoke(this, text);
            }
        }

        public void OnRmsChanged(float rmsdB) { }
    }
}
#endif
