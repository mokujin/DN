﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


namespace Blueberry.Audio
{
    /// <summary>
    /// A container for audio.  Represents a single piece of audio that can
    /// be repeatedly played.
    /// </summary>
    public class AudioClip
    {
        //VorbisFile rawClip;

        internal Stream underlyingStream;

        public AudioChannel StaticChanel { get; private set; }
        /// <summary>
        /// Constructs an audio clip from the given file.
        /// </summary>
        /// <param name="fileName">The file which to read from.</param>
        public AudioClip(string fileName)
        {
            underlyingStream = File.OpenRead(fileName);
            StaticChanel = new AudioChannel(AudioManager.Instance.BuffersPerChannel, AudioManager.Instance.BytesPerBuffer);
            StaticChanel.Init(this);
            //StaticChanel.Prepare();
            AudioManager.Instance.StaticClips.Add(this);
            //rawClip = new VorbisFile(fileName);

            //Cache(64 * 1024);
        }

        /// <summary>
        /// Reads an audio clip from the given stream.
        /// </summary>
        /// <param name="inputStream">The stream to read from.</param>
        public AudioClip(Stream inputStream)
        {
            underlyingStream = inputStream;
            StaticChanel = new AudioChannel(AudioManager.Instance.BuffersPerChannel, AudioManager.Instance.BytesPerBuffer);
            StaticChanel.Init(this);
            //StaticChanel.Prepare();
            AudioManager.Instance.StaticClips.Add(this);
            //rawClip = new VorbisFile(inputStream);
            //Cache(64 * 1024);
        }

        /// <summary>
        /// Caches the given number of bytes by reading them in and discarding
        /// them.  This is useful so that when the sound if first played,
        /// there's not a delay.
        /// </summary>
        /// <param name="bytes">Then number of PCM bytes to read.</param>
        /*
        protected void Cache(int bytes)
        {
            VorbisFileInstance instance = rawClip.makeInstance();

            int totalBytes = 0;
            byte[] buffer = new byte[4096];

            while (totalBytes < bytes)
            {
                int bytesRead = instance.read(buffer, buffer.Length, 0, 2, 1, null);

                if (bytesRead <= 0)
                    break;

                totalBytes += bytesRead;
            }
        }
        */
        /// <summary>
        /// Plays the audio clip.
        /// </summary>
        public void PlayDynamic()
        {
            lock (AudioManager.Instance)
            {
                AudioManager.Instance.PlayClip(this);
            }
        }
        public void Play()
        {
            StaticChanel.Play();
        }
        public void Pause()
        {
            StaticChanel.Pause();
        }
        public void Stop()
        {
            StaticChanel.Stop();
        }
        public void Resume()
        {
            StaticChanel.Resume();
        }

    }
}
