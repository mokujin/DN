﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Blueberry
{
    class AnimationManager
    {
        private static AnimationManager instance = null;
        public Thread UpdateThread { get; private set; }

        public bool RunUpdates { get; set; }
        private List<IAnimation> animations;
        public List<IAnimation> Animations { get { return animations; } }

        public static AnimationManager Manager
        {
            get
            {
                if (instance == null)
                    instance = new AnimationManager();

                return instance;
            }
            set
            {
                instance = value;
            }
        }

        public AnimationManager(bool launchUpdateThread = true)
        {
            Init(launchUpdateThread);
        }

        private void Init(bool launchThread)
        {
            animations = new List<IAnimation>();
            RunUpdates = launchThread;

            instance = this;

            if (launchThread)
            {
                stopwatch = new Stopwatch();
                UpdateThread = new Thread(RunUpdateLoop);
                UpdateThread.IsBackground = true;
                UpdateThread.Start();
            }
            else
            {
                UpdateThread = null;
            }
        }


        public void Update(double dt)
        {
            lock (this)
            {
                for (int i = 0; i < animations.Count; i++)
                {
                    animations[i].Animate((float)dt);
                }
            }
        }
        Stopwatch stopwatch;
        double time;
        internal void RunUpdateLoop()
        {
            while (RunUpdates)
            {
                time = stopwatch.Elapsed.TotalSeconds;
                stopwatch.Restart();
                Update(time);
                Thread.Sleep(1);
            }
        }

        public void Dispose()
        {
            RunUpdates = false;
            UpdateThread.Join();

            foreach (IAnimation anim in animations)
            {
                anim.Dispose();
            }
        }
    }
}
