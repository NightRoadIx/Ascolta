using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ascolta.Services
{
    public class DummySpeechToTextService : ISpeechToTextService
    {
        public event EventHandler<string>? TranscriptionUpdated;
        public bool IsListening { get; private set; }

        public Task StartListeningAsync()
        {
            IsListening = true;
            TranscriptionUpdated?.Invoke(this, "[SpeechToText no implementado en esta plataforma]");
            return Task.CompletedTask;
        }

        public Task StopListeningAsync()
        {
            IsListening = false;
            return Task.CompletedTask;
        }
    }
}
