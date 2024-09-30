using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Archive_Unpacker.Classes.BarViewModel.BarViewModel;

namespace Resource_Manager.Classes.sound
{
    [ObservableObject]
    public partial class PlaybackManager
    {
        private WaveOutEvent outputDevice;
        private WaveStream currentWaveStream;
        AudioSource currentSource;

        public AudioSource CurrentSource
        {
            get
            {
                return currentSource;
            }
            set
            {
                if (outputDevice != null)
                {
                    outputDevice.Dispose();
                    outputDevice = null;
                }
                if (currentWaveStream != null)
                {
                    currentWaveStream.Dispose();
                    currentWaveStream = null;
                }
                this.SetProperty(ref currentSource, value);

                if (currentSource != null)
                {
                    outputDevice = new WaveOutEvent();
                    currentWaveStream = currentSource.Audio;
                    outputDevice.Init(currentWaveStream);
                    Play();
                }
            }
        }

        public float NormalizedPosition
        {
            get => outputDevice != null ? (float)currentWaveStream.CurrentTime.TotalSeconds / (float)currentWaveStream.TotalTime.TotalSeconds : 0;
            set
            {
                if (outputDevice != null && currentWaveStream != null)
                {
                    if (currentWaveStream.CanSeek)
                    {
                        bool wasPlaying = CanPause;
                        outputDevice.Pause();
                        currentWaveStream.Position = (long)(value * currentWaveStream.Length / currentWaveStream.WaveFormat.BlockAlign) * currentWaveStream.WaveFormat.BlockAlign;
                        if (wasPlaying)
                        {
                            Play();
                        }
                    }
                }
            }
        }

        public string PositionText => outputDevice != null ? $"{currentWaveStream.CurrentTime:mm\\:ss} / {currentWaveStream.TotalTime:mm\\:ss}" : "00:00 / 00:00";

        [RelayCommand]
        public void Play()
        {
            if (outputDevice != null)
            {
                outputDevice.Play();
                OnPropertyChanged(nameof(CanPlay));
                OnPropertyChanged(nameof(CanPause));
                OnPropertyChanged(nameof(NormalizedPosition));
                OnPropertyChanged(nameof(PositionText));
                StartTimer();
            }
        }

        [RelayCommand]
        public void Pause()
        {
            if (outputDevice != null)
            {
                outputDevice.Pause();
                OnPropertyChanged(nameof(CanPlay));
                OnPropertyChanged(nameof(CanPause));
                OnPropertyChanged(nameof(NormalizedPosition));
                OnPropertyChanged(nameof(PositionText));
            }
        }

        [RelayCommand]
        public void Stop()
        {
            if (outputDevice != null)
            {
                outputDevice.Stop();
                OnPropertyChanged(nameof(CanPlay));
                OnPropertyChanged(nameof(CanPause));
                OnPropertyChanged(nameof(NormalizedPosition));
                OnPropertyChanged(nameof(PositionText));
            }
        }

        public async void StartTimer()
        {
            while (outputDevice != null && outputDevice.PlaybackState == PlaybackState.Playing)
            {
                await Task.Delay(100);
                OnPropertyChanged(nameof(NormalizedPosition));
                OnPropertyChanged(nameof(PositionText));
            }
            OnPropertyChanged(nameof(CanPlay));
            OnPropertyChanged(nameof(CanPause));
        }

        public bool CanPlay => outputDevice != null && outputDevice.PlaybackState != PlaybackState.Playing;
        public bool CanPause => outputDevice != null && outputDevice.PlaybackState == PlaybackState.Playing;
    }
}
