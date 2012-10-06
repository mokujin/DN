using System;
using System.Collections.Generic;
using System.Text;

using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using NVorbis;
using System.IO;

namespace Blueberry.Audio
{
    /// <summary>
    /// An object for buffering and playing data on a single channel. 
    /// </summary>
    public class AudioChannel : IDisposable
    {
        public float[] SegmentBuffer { get; private set; }
        public short[] CastBuffer { get; private set; }
        public int[] Buffers { get; private set; }
        public int BufferCount { get; private set; }
        public int BufferSize { get; private set; }
        public int Source { get; private set; }
        public VorbisReader Reader { get; private set; }
        public AudioClip CurrentClip { get; private set; }
        public bool Ready { get; private set; }

        public ALFormat CurrentFormat { get; private set; }
        public int CurrentRate { get; private set; }

        private bool eof;

        private long lastStreamPosition;

        public bool IsFree
        {
            get
            {
                return Reader == null || Stoped;
            }
        }

        public bool Paused { get { ALSourceState st = AL.GetSourceState(Source); return st == ALSourceState.Paused; } }
        public bool Stoped { get { ALSourceState st = AL.GetSourceState(Source); return st == ALSourceState.Stopped; } }
        public bool Playing { get { ALSourceState st = AL.GetSourceState(Source); return st == ALSourceState.Playing; } }

        /// <summary>
        /// Constructs an empty channel, ready to play sound.
        /// </summary>
        /// <param name="bufferCount">The number of audio buffers to size.</param>
        /// <param name="bufferSize">The size, in bytes, of each audio buffer.</param>
        internal AudioChannel(int bufferCount, int bufferSize)
        {
            Buffers = new int[bufferCount];
            BufferCount = bufferCount;
            BufferSize = bufferSize;
            Reader = null;
            CurrentClip = null;
            Ready = false;

            SegmentBuffer = new float[bufferSize];
            CastBuffer = new short[bufferSize];

            lastStreamPosition = 0;

            // Make the source
            Source = AL.GenSource();

            // Make the buffers
            for(int i = 0; i < BufferCount; i++)
                Buffers[i] = AL.GenBuffer();
        }

        /// <summary>
        /// Deconstructs the channel, freeing its hardware resources.
        /// </summary>
        ~AudioChannel()
        {
            Dispose(); 
        }

        /// <summary>
        /// Disposes the channel, freeing its hardware resources.
        /// </summary>
        public void Dispose()
        {
            AL.SourceStop(Source);
            if(Buffers != null)
                AL.DeleteBuffers(Buffers);

            Buffers = null;
            Close();
        }

        /// <summary>
        /// Determines the format of the given audio clip.
        /// </summary>
        /// <param name="clip">The clip to determine the format of.</param>
        /// <returns>The audio format.</returns>
        public static ALFormat DetermineFormat(VorbisReader reader)
        { 
            // The number of channels is determined by the clip.  The bit depth
            // however is the choice of the player.  If desired, 8-bit audio
            // could be supported here.
            if (reader.Channels == 1)
            {
                return ALFormat.Mono16;
            }
            else if (reader.Channels == 2)
            {
                return ALFormat.Stereo16;
            }
            else
            {
                throw new NotImplementedException("Only mono and stereo are implemented.  Audio has too many channels.");
            }
        }

        /// <summary>
        /// Determines the rate of the given audio clip.
        /// </summary>
        /// <param name="clip">The clip to determine the rate of.</param>
        /// <returns>The audio rate.</returns>
        public int DetermineRate(VorbisReader reader)
        {
            return reader.SampleRate;
        }
        public static void Check()
        {
            ALError error;
            if ((error = AL.GetError()) != ALError.NoError)
                throw new InvalidOperationException(AL.GetErrorString(error));

        }
        void Empty()
        {
            int queued;
            AL.GetSource(Source, ALGetSourcei.BuffersQueued, out queued);
            if (queued > 0)
            {
                try
                {
                    AL.SourceUnqueueBuffers(Source, queued);
                    Check();
                }
                catch (InvalidOperationException)
                {
                    // This is a bug in the OpenAL implementation
                    // Salvage what we can
                    int processed;
                    AL.GetSource(Source, ALGetSourcei.BuffersProcessed, out processed);
                    var salvaged = new int[processed];
                    if (processed > 0)
                    {
                        AL.SourceUnqueueBuffers(Source, processed, salvaged);
                        Check();
                    }

                    // Try turning it off again?
                    AL.SourceStop(Source);
                    Check();

                    Empty();
                }
            }
        }
        /// <summary>
        /// Begins playing the given clip.
        /// </summary>
        /// <param name="clip">The clip to play.</param>
        public void Play(AudioClip clip)
        {
            var state = AL.GetSourceState(Source);

            switch (state)
            {
                case ALSourceState.Playing:
                    if (clip == CurrentClip)
                        return;
                    else
                    {
                        Stop();
                        Ready = false;
                        Reader.DecodedTime = TimeSpan.Zero;
                        Empty();
                        break;
                    }
                case ALSourceState.Paused:
                    Resume();
                    return;
                case ALSourceState.Stopped:
                    Ready = false;
                    Reader.DecodedTime = TimeSpan.Zero;
                    Empty();
                    break;
            }

            Prepare(clip);

            // Start playing the clip
            AL.SourcePlay(Source);
        }
        private void PreCashe()
        {
            DequeuUsedBuffers();
            // Buffer initial audio
            int usedBuffers = 0;
            for (int i = 0; i < BufferCount; i++)
            {
                int samples = Reader.ReadSamples(SegmentBuffer, 0, SegmentBuffer.Length);
                CastBuffers();

                if (samples > 0)
                {
                    // Buffer the segment
                    AL.BufferData(Buffers[i], CurrentFormat, CastBuffer, samples * sizeof(short), CurrentRate);

                    usedBuffers++;
                }
                else if (samples == 0)
                {
                    // Clip is too small to fill the initial buffer, so stop
                    // buffering.
                    break;
                }
                else
                {
                    // TODO: There was an error reading the file
                    throw new System.IO.IOException("Error reading or processing OGG file");
                }
            }
            AL.SourceQueueBuffers(Source, usedBuffers, Buffers);
        }
        
        internal void Prepare(AudioClip clip)
        {
            if (Ready && clip == CurrentClip) return;
            CurrentClip = clip;

            Stop();
            
            lastStreamPosition = 0;
            CurrentClip.underlyingStream.Seek(lastStreamPosition, SeekOrigin.Begin);
            Reader = new VorbisReader(CurrentClip.underlyingStream, false);
            Ready = true;

            CurrentFormat = DetermineFormat(Reader);
            CurrentRate = DetermineRate(Reader);

            eof = false;

            PreCashe();
        }

        public void Pause()
        {
            if (AL.GetSourceState(Source) != ALSourceState.Playing)
                return;
            AL.SourcePause(Source);
        }

        public void Resume()
        {
            if (AL.GetSourceState(Source) != ALSourceState.Paused)
                return;
            AL.SourcePlay(Source);
        }

        public void Stop()
        {
            var state = AL.GetSourceState(Source);
            if (state == ALSourceState.Playing || state == ALSourceState.Paused)
            {
                AL.SourceStop(Source);
            }
        }

        internal void Close()
        {
            if (Reader != null)
            {
                Reader.Dispose();
                Reader = null;
            }
            Ready = false;
        }

        /// <summary>
        /// Removes all empty buffers from the audio queue.
        /// </summary>
        protected void DequeuUsedBuffers()
        {
            int processedBuffers;
            AL.GetSource(Source, ALGetSourcei.BuffersProcessed, out processedBuffers);

            int[] removedBuffers = new int[processedBuffers];
            AL.SourceUnqueueBuffers(Source, processedBuffers, removedBuffers);
        }

        /// <summary>
        /// Updates the channel, buffer addition audio if needed.  This method
        /// needs to be called frequently to maintain real-time performance.
        /// </summary>
        public void Update()
        {
            if (!Playing) return;
            if (Reader != null)
            {
                CurrentClip.underlyingStream.Position = lastStreamPosition;
                int buffersQueued;
                AL.GetSource(Source, ALGetSourcei.BuffersQueued, out buffersQueued);

                int processedBuffers;
                AL.GetSource(Source, ALGetSourcei.BuffersProcessed, out processedBuffers);

                if (eof)
                {
                    // Clip is done being buffered
                    if (buffersQueued <= processedBuffers)
                    {
                        // Clip has finished
                        Stop();
                        DequeuUsedBuffers();

                        return;
                    }
                }
                else
                {
                    // Still some buffering to do
                    if (buffersQueued - processedBuffers > 0 && AL.GetError() == ALError.NoError)
                    {
                        // Make sure we're playing (not sure why we would've stopped)
                        if (AL.GetSourceState(Source) != ALSourceState.Playing)
                        {
                            AL.SourcePlay(Source);
                        }
                    }

                    // Detect buffer under-runs
                    bool underRun = (processedBuffers >= BufferCount);

                    // Remove processed buffers
                    while (processedBuffers > 0)
                    {
                        int removedBuffer = 0;

                        // TODO: Can remove more than one buffer here.  Can also
                        // add the buffers back to the queue.
                        AL.SourceUnqueueBuffers(Source, 1, ref removedBuffer);

                        // Just remove the buffer and don't do anything else if
                        // we're at the end of the clip.
                        if (eof)
                        {
                            processedBuffers--;
                            continue;
                        }

                        // Buffer the next chunk
                        int readSamples = Reader.ReadSamples(SegmentBuffer, 0, SegmentBuffer.Length);
                        CastBuffers();

                        if (readSamples > 0)
                        {
                            // TOOD: Queue multiple buffers here
                            AL.BufferData(removedBuffer, CurrentFormat, CastBuffer, readSamples * sizeof(short),
                                CurrentRate);
                            AL.SourceQueueBuffer(Source, removedBuffer);
                        }
                        else if (readSamples == 0)
                        {
                            // Reached the end of the file
                            eof = true;
                        }
                        else
                        {
                            // A file read error has occurred, stop playing
                            Stop();
                            break;
                        }

                        // Check for OpenAL errors
                        if (AL.GetError() != ALError.NoError)
                        {
                            Stop();
                            break;
                        }

                        processedBuffers--;
                    }
                }

                lastStreamPosition = CurrentClip.underlyingStream.Position;
            }
        }
        private void CastBuffers()
        {
            for (int i = 0; i < BufferSize; i++)
            {
                var temp = (int)(32767f * SegmentBuffer[i]);
                if (temp > short.MaxValue) temp = short.MaxValue;
                else if (temp < short.MinValue) temp = short.MinValue;
                CastBuffer[i] = (short)temp;
            }
        }
    }
}
