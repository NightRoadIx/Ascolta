using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ascolta.Services
{
    public interface ISpeechToTextService
    {
        /// <summary>
        /// Se dispara cuando hay nuevo texto reconocido (parcial o final).
        /// </summary>
        event EventHandler<string>? TranscriptionUpdated;

        /// <summary>
        /// Indica si actualmente está escuchando.
        /// </summary>
        bool IsListening { get; }

        /// <summary>
        /// Comienza la escucha del micrófono.
        /// </summary>
        Task StartListeningAsync();

        /// <summary>
        /// Detiene la escucha del micrófono.
        /// </summary>
        Task StopListeningAsync();
    }
}
