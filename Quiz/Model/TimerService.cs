﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quiz.Model
{
    public class TimerService
    {
        private System.Timers.Timer _timer; // Fully qualified name to avoid ambiguity
        public int SecondsElapsed { get; private set; }

        public event Action<int> Tick;

        public void Start()
        {
            _timer = new System.Timers.Timer(1000); // Fully qualified name here as well
            _timer.Elapsed += (_, _) =>
            {
                SecondsElapsed++;
                Tick?.Invoke(SecondsElapsed);
            };
            _timer.Start();
        }

        public void Stop() => _timer?.Stop();
    }
}
