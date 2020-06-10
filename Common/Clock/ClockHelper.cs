namespace Common.Clock
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public class ClockHelper
    {
        private object clockLock = new object();
        private float clockSecond;
        private List<string> errorQueue = new List<string>();
        private List<ClockEvent> eventList = new List<ClockEvent>();
        private List<float> eventTimeList = new List<float>();
        public static readonly ClockHelper Instance = new ClockHelper();
        private int specialCount;
        private float startUpSecond;
        private bool timeSwitch;

        private ClockHelper()
        {
            this.StartClock();
        }

        public void AddNewEvent(ClockEvent eventClock)
        {
            object clockLock = this.clockLock;
            lock (clockLock)
            {
                int insertInTimeListIndex = this.GetInsertInTimeListIndex(eventClock.DelayTime + this.clockSecond);
                if (insertInTimeListIndex > -1)
                {
                    this.eventList.Insert(insertInTimeListIndex, eventClock);
                }
                else
                {
                    this.eventList.Add(eventClock);
                }
                this.specialCount++;
            }
        }

        public void ClearClock()
        {
            object clockLock = this.clockLock;
            lock (clockLock)
            {
                this.timeSwitch = false;
                this.eventTimeList.Clear();
                this.eventList.Clear();
                this.clockSecond = 0f;
            }
        }

        ~ClockHelper()
        {
            this.ClearClock();
            this.clockLock = null;
        }

        public string[] GetErrorList()
        {
            List<string> errorQueue = this.errorQueue;
            lock (errorQueue)
            {
                return this.errorQueue.ToArray();
            }
        }

        private int GetInsertInTimeListIndex(float time)
        {
            for (int i = 0; i < this.eventTimeList.Count; i++)
            {
                if (this.eventTimeList[i] >= time)
                {
                    this.eventTimeList.Insert(i, time);
                    return i;
                }
            }
            this.eventTimeList.Add(time);
            return -1;
        }

        public float GetTimeSinceProjectStart() => 
            this.startUpSecond;

        public void PauseClock()
        {
            object clockLock = this.clockLock;
            lock (clockLock)
            {
                this.timeSwitch = false;
            }
        }

        public void RemvoveEvent(ClockEvent eventClock)
        {
            object clockLock = this.clockLock;
            lock (clockLock)
            {
                if (this.eventList.Contains(eventClock))
                {
                    int index = this.eventList.FindIndex(tmpEvent => tmpEvent == eventClock);
                    if (index > -1)
                    {
                        this.eventList.RemoveAt(index);
                        this.eventTimeList.RemoveAt(index);
                    }
                }
            }
        }

        public void StartClock()
        {
            object clockLock = this.clockLock;
            lock (clockLock)
            {
                this.timeSwitch = true;
            }
        }

        public void Update(float timeSpan)
        {
            this.startUpSecond += timeSpan;
            if (!this.timeSwitch)
            {
                return;
            }
            this.clockSecond += timeSpan;
        Label_0025:
            Thread.Sleep(0);
            Action timeHandler = null;
            bool flag = false;
            object clockLock = this.clockLock;
            lock (clockLock)
            {
                if (this.eventTimeList.Count <= 0)
                {
                    return;
                }
                if (this.eventTimeList[0] <= this.clockSecond)
                {
                    flag = true;
                    timeHandler = this.eventList[0].TimeHandler;
                    this.eventList.RemoveAt(0);
                    this.eventTimeList.RemoveAt(0);
                }
            }
            if (flag)
            {
                try
                {
                    if (timeHandler != null)
                    {
                        timeHandler.Invoke();
                    }
                    goto Label_0025;
                }
                catch (Exception exception)
                {
                    List<string> errorQueue = this.errorQueue;
                    lock (errorQueue)
                    {
                        if (this.errorQueue.Count > this.ErrorSaveMaxCount)
                        {
                            this.errorQueue.RemoveAt(0);
                        }
                        this.errorQueue.Add(exception.ToString());
                    }
                    goto Label_0025;
                }
            }
        }

        public int ErrorSaveMaxCount { get; set; }
    }
}
