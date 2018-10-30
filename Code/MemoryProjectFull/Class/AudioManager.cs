using System;
using System.Collections.Generic;
using System.Media;
using System.Windows.Media;

namespace MemoryProjectFull
{

    public class Audio
    {

        public Audio(Uri source)
        {
            audio_internal = new MediaPlayer();

            audio_internal.Open(source);
            audio_internal.MediaEnded += MediaEndedCallback;

            IsLooping = false;
        }

        public void Play(bool shouldLoop)
        {
            audio_internal.Stop();
            IsLooping = shouldLoop;
            audio_internal.Play();
        }

        public void Pause()
        {
            audio_internal.Pause();
        }

        public void Stop()
        {
            IsLooping = false;
            audio_internal.Stop();
        }

        private void MediaEndedCallback(object o, EventArgs e)
        {
            if (IsLooping)
            {
                audio_internal.Position = TimeSpan.Zero;
                audio_internal.Play();
            }
        }

        private bool IsLooping
        {
            get;
            set;
        }

        private MediaPlayer audio_internal;

    }

    public static class AudioManager
    {

        static AudioManager()
        {
            registeredAudios = new Dictionary<string, Audio>();
        }

        public static bool IsAudioRegistered(string name)
        {
            return registeredAudios.ContainsKey(name);
        }

        public static void RegisterAudio(string name, string filePath)
        {
            try
            {
                var src = new Uri(filePath, UriKind.RelativeOrAbsolute);
                var audio = new Audio(src);
                
                registeredAudios.Add(name, audio);
            }
            catch
            {
                throw;
            }
        }

        public static Audio GetAudio(string name)
        {
            try
            { return registeredAudios[name]; }
            catch
            { return null; }
        }

        public static bool UnregisterAudio(string name)
        {
            return registeredAudios.Remove(name);
        }

        public static void StopAll()
        {
            foreach (var entry in registeredAudios)
            {
                (entry.Value).Stop();
            }
        }
            
        private static Dictionary<string, Audio> registeredAudios;

    }

}

