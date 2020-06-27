using Common.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPlayerCtl
{
    public class MPHelper
    {
        private Action<string> endHandle;
        public AxWMPLib.AxWindowsMediaPlayer player;
        private string path;

        public void Init(Action<string> endHandle)
        {
            this.endHandle = endHandle;
        }


        public void Play(string path)
        {
            this.path = path;
            player.URL = path;
            player.Ctlcontrols.play();
            player.PlayStateChange += new AxWMPLib._WMPOCXEvents_PlayStateChangeEventHandler(player_PlayStateChange);
            eventCount = 0;
        }

        private ulong eventCount = 0;

        private void player_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            eventCount++;
            // Test the current state of the player and display a message for each state.  
            switch (e.newState)
            {
                case 0: // Undefined  
                    LogAgent.Log("Undefined" + eventCount);
                    return;

                case 1: // Stopped  
                    LogAgent.Log("Stopped" + eventCount);
                    return;

                case 2: // Paused  
                    LogAgent.Log("Paused" + eventCount);
                    return;

                case 3: // Playing  
                    LogAgent.Log("Playing" + eventCount);
                    return;

                case 4: // ScanForward  
                    LogAgent.Log("ScanForward" + eventCount);
                    return;

                case 5: // ScanReverse  
                    LogAgent.Log("ScanReverse" + eventCount);
                    return;

                case 6: // Buffering  
                    LogAgent.Log("Buffering" + eventCount);
                    return;

                case 7: // Waiting  
                    LogAgent.Log("Waiting" + eventCount);
                    return;

                case 8: // MediaEnded  
                    LogAgent.Log("MediaEnded");
                    break;

                case 9: // Transitioning  
                    LogAgent.Log("Transitioning");
                    return;

                case 10: // Ready  
                    LogAgent.Log("Ready" + eventCount);
                    return;

                case 11: // Reconnecting  
                    LogAgent.Log("Reconnecting" + eventCount);
                    return;

                case 12: // Last  
                    LogAgent.Log("Last" + eventCount);
                    return;

                default:
                    LogAgent.Log("default" + eventCount);
                    return;
            }

            //player.Ctlcontrols.stop();
            if (endHandle != null)
            {
                endHandle(this.path);
            }
        }

    }
}
