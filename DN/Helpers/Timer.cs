using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DN.Helpers
{
    public delegate void TickEventHandler();

    public delegate void UpdateEventHandler(float dt);

    public delegate void StartEventHandler();

    public class Timer
    {
        public bool Running
        {
            get { return _running; }
        }

        public bool Repeat = false;

        private float _elapsed;

        private bool _running;

        public float Duration;

        public TickEventHandler TickEvent;
        public UpdateEventHandler UpdateEvent;
        public StartEventHandler StartEvent;


        public Timer()
        {
        }

        public void Run(bool immediately = false)
        {
            if(_running)
                throw  new Exception("Already running");

            _running = true;

            _elapsed = immediately ? Duration : 0;

            if (StartEvent != null)
                StartEvent();
        }

        public void Finish()
        {
            _elapsed = Duration;
        }

        public void Stop()
        {
            _running = false;
            _elapsed = 0;
        }
        public void Pause()
        {
            _running = false;
        }
        public void Continue()
        {
            _running = true;
        }
        public void Restart()
        {
            _elapsed = 0;
        }



        public void Update(float dt)
        {
            if (_running)
            {
                _elapsed += dt;
                if (_elapsed >= Duration)
                {

                    if (Repeat)
                        Restart();
                    else
                        Stop();

                    if (TickEvent != null)
                        TickEvent();
                }
                else if (UpdateEvent != null)
                    UpdateEvent(dt);
            }
        }





    }
}
