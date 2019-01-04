using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;

namespace ShapeShop.GameEngine
{
    /// Component that manages audio playback for all cues.
    public class AudioManager : GameComponent
    {
        // Singleton
        /// The singleton for this type.
        private static AudioManager audioManager = null;

        // Audio Data
        /// The audio engine used to play all cues.
        private AudioEngine audioEngine;

        /// The soundbank that contains all cues.
        private SoundBank soundBank;

        /// The wavebank with all wave files for this game.
        private WaveBank waveBank;

        // Initialization Methods
        /// Constructs the manager for audio playback of all cues.
        /// <param name="game">The game that this component will be attached to.</param>
        /// <param name="settingsFile">The filename of the XACT settings file.</param>
        /// <param name="waveBankFile">The filename of the XACT wavebank file.</param>
        /// <param name="soundBankFile">The filename of the XACT soundbank file.</param>
        private AudioManager(Game game, string settingsFile, string waveBankFile, string soundBankFile)
            : base(game)
        {
            try
            {
                audioEngine = new AudioEngine(settingsFile);
                waveBank = new WaveBank(audioEngine, waveBankFile);
                soundBank = new SoundBank(audioEngine, soundBankFile);
            }
            catch (NoAudioHardwareException)
            {
                // silently fall back to silence
                audioEngine = null;
                waveBank = null;
                soundBank = null;
            }
        }

        /// Initialize the static AudioManager functionality.
        /// <param name="game">The game that this component will be attached to.</param>
        /// <param name="settingsFile">The filename of the XACT settings file.</param>
        /// <param name="waveBankFile">The filename of the XACT wavebank file.</param>
        /// <param name="soundBankFile">The filename of the XACT soundbank file.</param>
        public static void Initialize(Game game, string settingsFile, string waveBankFile, string soundBankFile)
        {
            audioManager = new AudioManager(game, settingsFile, waveBankFile, soundBankFile);
            if (game != null)
            {
                game.Components.Add(audioManager);
            }
        }

        // Cue Methods
        /// Retrieve a cue by name.
        /// <param name="cueName">The name of the cue requested.</param>
        /// <returns>The cue corresponding to the name provided.</returns>
        public static Cue GetCue(string cueName)
        {
            if (String.IsNullOrEmpty(cueName) ||
                (audioManager == null) || (audioManager.audioEngine == null) ||
                (audioManager.soundBank == null) || (audioManager.waveBank == null))
            {
                return null;
            }
            return audioManager.soundBank.GetCue(cueName);
        }

        /// Plays a cue by name.
        /// <param name="cueName">The name of the cue to play.</param>
        public static void PlayCue(string cueName)
        {
            if ((audioManager != null) && (audioManager.audioEngine != null) &&
                (audioManager.soundBank != null) && (audioManager.waveBank != null))
            {
                audioManager.soundBank.PlayCue(cueName);
            }
        }

        // Music
        /// The cue for the music currently playing, if any.
        private Cue musicCue;

        /// Stack of music cue names, for layered music playback.
        private Stack<string> musicCueNameStack = new Stack<string>();

        /// Plays the desired music, clearing the stack of music cues.
        /// <param name="cueName">The name of the music cue to play.</param>
        public static void PlayMusic(string cueName)
        {
            // start the new music cue
            if (audioManager != null)
            {
                audioManager.musicCueNameStack.Clear();
                PushMusic(cueName);
            }
        }

        /// Plays the music for this game, adding it to the music stack.
        /// <param name="cueName">The name of the music cue to play.</param>
        public static void PushMusic(string cueName)
        {
            // start the new music cue
            if ((audioManager != null) && (audioManager.audioEngine != null) &&
                (audioManager.soundBank != null) && (audioManager.waveBank != null))
            {
                audioManager.musicCueNameStack.Push(cueName);
                if ((audioManager.musicCue == null) ||
                    (audioManager.musicCue.Name != cueName))
                {
                    if (audioManager.musicCue != null)
                    {
                        audioManager.musicCue.Stop(AudioStopOptions.AsAuthored);
                        audioManager.musicCue.Dispose();
                        audioManager.musicCue = null;
                    }
                    audioManager.musicCue = GetCue(cueName);
                    if (audioManager.musicCue != null)
                    {
                        audioManager.musicCue.Play();
                    }
                }
            }
        }

        /// Stops the current music and plays the previous music on the stack.
        public static void PopMusic()
        {
            // start the new music cue
            if ((audioManager != null) && (audioManager.audioEngine != null) &&
                (audioManager.soundBank != null) && (audioManager.waveBank != null))
            {
                string cueName = null;
                if (audioManager.musicCueNameStack.Count > 0)
                {
                    audioManager.musicCueNameStack.Pop();
                    if (audioManager.musicCueNameStack.Count > 0)
                    {
                        cueName = audioManager.musicCueNameStack.Peek();
                    }
                }
                if ((audioManager.musicCue == null) ||
                    (audioManager.musicCue.Name != cueName))
                {
                    if (audioManager.musicCue != null)
                    {
                        audioManager.musicCue.Stop(AudioStopOptions.AsAuthored);
                        audioManager.musicCue.Dispose();
                        audioManager.musicCue = null;
                    }
                    if (!String.IsNullOrEmpty(cueName))
                    {
                        audioManager.musicCue = GetCue(cueName);
                        if (audioManager.musicCue != null)
                        {
                            audioManager.musicCue.Play();
                        }
                    }
                }
            }
        }

        /// Stop music playback, clearing the cue.
        public static void StopMusic()
        {
            if (audioManager != null)
            {
                audioManager.musicCueNameStack.Clear();
                if (audioManager.musicCue != null)
                {
                    audioManager.musicCue.Stop(AudioStopOptions.AsAuthored);
                    audioManager.musicCue.Dispose();
                    audioManager.musicCue = null;
                }
            }
        }

        // Updating Methods
        /// Update the audio manager, particularly the engine.
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // update the audio engine
            if (audioEngine != null)
            {
                audioEngine.Update();
            }

            //if ((musicCue != null) && musicCue.IsStopped)
            //{
            //    AudioManager.PopMusic();
            //}

            base.Update(gameTime);
        }

        // Instance Disposal Methods
        /// Clean up the component when it is disposing.
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    StopMusic();
                    if (soundBank != null)
                    {
                        soundBank.Dispose();
                        soundBank = null;
                    }
                    if (waveBank != null)
                    {
                        waveBank.Dispose();
                        waveBank = null;
                    }
                    if (audioEngine != null)
                    {
                        audioEngine.Dispose();
                        audioEngine = null;
                    }
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

    }
}
