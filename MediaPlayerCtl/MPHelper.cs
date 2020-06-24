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
        private Action endHandle;
        public AxWMPLib.AxWindowsMediaPlayer player;

        public void Init(Action endHandle)
        {
            this.endHandle = endHandle;
        }


        public void Play(string path)
        {
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
                    break;

                case 1: // Stopped  
                    LogAgent.Log("Stopped" + eventCount);
                    break;

                case 2: // Paused  
                    LogAgent.Log("Paused" + eventCount);
                    break;

                case 3: // Playing  
                    LogAgent.Log("Playing" + eventCount);
                    return;

                case 4: // ScanForward  
                    LogAgent.Log("ScanForward" + eventCount);
                    break;

                case 5: // ScanReverse  
                    LogAgent.Log("ScanReverse" + eventCount);
                    break;

                case 6: // Buffering  
                    LogAgent.Log("Buffering" + eventCount);
                    break;

                case 7: // Waiting  
                    LogAgent.Log("Waiting" + eventCount);
                    break;

                case 8: // MediaEnded  
                    LogAgent.Log("MediaEnded");
                    break;

                case 9: // Transitioning  
                    LogAgent.Log("Transitioning");
                    break;

                case 10: // Ready  
                    LogAgent.Log("Ready" + eventCount);
                    return;

                case 11: // Reconnecting  
                    LogAgent.Log("Reconnecting" + eventCount);
                    break;

                case 12: // Last  
                    LogAgent.Log("Last" + eventCount);
                    break;

                default:
                    LogAgent.Log("default" + eventCount);
                    break;
            }

            player.Ctlcontrols.stop();
            if (endHandle != null)
            {
                endHandle();
            }
        }
    }
}
