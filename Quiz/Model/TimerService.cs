using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Quiz.Model
{
    public class TimerService
    {
        private System.Timers.Timer _timer;
        public int SecondsElapsed { get; private set; }

        public event Action<int> Tick;

        public void Start()
        {
            _timer = new Timer(1000);
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
