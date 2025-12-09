using System.Windows.Input;
using Ascolta.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Ascolta.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly ISpeechToTextService _speechService;

        [ObservableProperty]
        private string transcription = string.Empty;

        [ObservableProperty]
        private bool isListening;

        public string ToggleButtonText => IsListening ? "Detener escucha" : "Empezar a escuchar";

        public IRelayCommand ToggleListeningCommand { get; }

        public MainViewModel(ISpeechToTextService speechService)
        {
            _speechService = speechService;

            _speechService.TranscriptionUpdated += OnTranscriptionUpdated;

            ToggleListeningCommand = new RelayCommand(async () => await ToggleListeningAsync());
        }

        private void OnTranscriptionUpdated(object? sender, string text)
        {
            // Aquí decides si reemplazas o concatenas texto.
            // Ejemplo: mostrar la última frase:
            Transcription = text;

            // Si quieres que se vaya pegando:
            // Transcription += (string.IsNullOrWhiteSpace(Transcription) ? "" : " ") + text;
        }

        private async Task ToggleListeningAsync()
        {
            if (IsListening)
            {
                await _speechService.StopListeningAsync();
                IsListening = false;
            }
            else
            {
                try
                {
                    await _speechService.StartListeningAsync();
                    IsListening = true;
                }
                catch (Exception ex)
                {
                    // Manejo simple de error
                    Transcription = $"Error: {ex.Message}";
                    IsListening = false;
                }
            }

            OnPropertyChanged(nameof(ToggleButtonText));
        }
    }
}
