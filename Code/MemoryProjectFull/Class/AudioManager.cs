using System;
using System.Collections.Generic;
using System.Media;
using System.Windows.Media;

namespace MemoryProjectFull
{

    /// <summary>
    /// Wrapper Class for WPF MediaPlayer.
    /// </summary>
    public class Audio
    {

        /// <summary>
        /// Initializes a new instance of the Audio class.
        /// </summary>
        /// <param name="source">The path of the Audio file to load.</param>
        public Audio(Uri source)
        {
            audio_internal = new MediaPlayer();

            audio_internal.Open(source);
            audio_internal.MediaEnded += MediaEndedCallback;

            IsLooping = false;
        }

        /// <summary>
        /// Starts audio playback.
        /// </summary>
        /// <param name="shouldLoop">Specifies whether the Audio should loop.</param>
        public void Play(bool shouldLoop)
        {
            audio_internal.Stop();
            IsLooping = shouldLoop;
            audio_internal.Play();
        }

        /// <summary>
        /// Pauses audio playback.
        /// </summary>
        public void Pause()
        {
            audio_internal.Pause();
        }

        /// <summary>
        /// Stops audio playback.
        /// </summary>
        public void Stop()
        {
            IsLooping = false;
            audio_internal.Stop();
        }

        /// <summary>
        /// Callback which runs after the Audio stops playing, causing the Audio to start playing again if looping is enabled.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void MediaEndedCallback(object o, EventArgs e)
        {
            if (IsLooping)
            {
                audio_internal.Position = TimeSpan.Zero;
                audio_internal.Play();
            }
        }

        /// <summary>
        /// Gets or sets the Audio's volume.
        /// </summary>
        public double Volume
        {
            get { return audio_internal.Volume; }
            set { audio_internal.Volume = value; }
        }

        /// <summary>
        /// Gets or sets whether the Audio is muted.
        /// </summary>
        public bool IsMuted
        {
            get { return audio_internal.IsMuted; }
            set { audio_internal.IsMuted = value; }
        }

        /// <summary>
        /// Gets or sets whether the audio is looping.
        /// </summary>
        private bool IsLooping
        {
            get;
            set;
        }

        private MediaPlayer audio_internal;

    }

    /// <summary>
    /// A Static Class for managing Audio.
    /// </summary>
    public static class AudioManager
    {

        static AudioManager()
        {
            registeredAudios = new Dictionary<string, Audio>();
        }

        /// <summary>
        /// Determines whether an Audio with the specified name has been registered.
        /// </summary>
        /// <param name="name">The name of the Audio to check for.</param>
        /// <returns>True if the Audio was registered, false otherwise.</returns>
        public static bool IsAudioRegistered(string name)
        {
            return registeredAudios.ContainsKey(name);
        }

        /// <summary>
        /// Loads a new Audio from the specified file path and registers it under the specified name.
        /// </summary>
        /// <param name="name">The name under which to register the Audio.</param>
        /// <param name="filePath">The path of the audio file.</param>
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

        /// <summary>
        /// Gets the Audio registered under the specified name.
        /// </summary>
        /// <param name="name">The name under which the Audio was registered.</param>
        /// <returns>The Audio if it was registered, null otherwise.</returns>
        public static Audio GetAudio(string name)
        {
            try
            { return registeredAudios[name]; }
            catch
            { return null; }
        }

        /// <summary>
        /// Unregisters the Audio that was registered under the specified name.
        /// </summary>
        /// <param name="name">The name of the Audio to unregister.</param>
        /// <returns>True if the Audio was registered, false otherwise.</returns>
        public static bool UnregisterAudio(string name)
        {
            return registeredAudios.Remove(name);
        }

        /// <summary>
        /// Stops all currently running Audio.
        /// </summary>
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

