namespace Common.Clock
{
    using System;
    using System.Runtime.CompilerServices;

    public class ClockEvent
    {
        public ClockEvent(float delayTime, Action handler)
        {
            this.DelayTime = delayTime;
            this.TimeHandler = handler;
        }

        public float DelayTime { get; private set; }

        public Action TimeHandler { get; private set; }
    }
}
