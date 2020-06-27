using Common.Log;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MediaPlayerCtl
{
    public partial class MainForm : Form
    {

        public MainForm()
        {
            InitializeComponent();
            axWindowsMediaPlayer1.PlayStateChange += player_PlayStateChange;
        }

        private int playlistIndex = 0;

        public void AddList(List<string> pathes)
        {
            for (int i = 0; i < pathes.Count; i++)
            {
                axWindowsMediaPlayer1.currentPlaylist.appendItem(axWindowsMediaPlayer1.newMedia(pathes[i]));
            }
        }

        public void PlayVedio()
        {
            if (axWindowsMediaPlayer1.currentPlaylist.count > 0)
                axWindowsMediaPlayer1.Ctlcontrols.playItem(axWindowsMediaPlayer1.currentPlaylist.get_Item(playlistIndex));
            else
                HidePlayer();
        }

        public void HidePlayer()
        {
            axWindowsMediaPlayer1.Hide();
            this.BackColor = Color.Black;
        }


        private void player_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            // Test the current state of the player and display a message for each state.  
            switch (e.newState)
            {
                case 0: // Undefined  
                    LogAgent.Log("Undefined");
                    break;

                case 1: // Stopped  
                    LogAgent.Log("Stopped");
                    break;

                case 2: // Paused  
                    LogAgent.Log("Paused");
                    break;

                case 3: // Playing  
                    LogAgent.Log("Playing");
                    return;

                case 4: // ScanForward  
                    LogAgent.Log("ScanForward");
                    break;

                case 5: // ScanReverse  
                    LogAgent.Log("ScanReverse");
                    break;

                case 6: // Buffering  
                    LogAgent.Log("Buffering");
                    break;

                case 7: // Waiting  
                    LogAgent.Log("Waiting");
                    break;

                case 8: // MediaEnded  
                    LogAgent.Log("MediaEnded");
                    AddIndex();
                    break;

                case 9: // Transitioning  
                    LogAgent.Log("Transitioning");
                    break;

                case 10: // Ready  
                    LogAgent.Log("Ready");
                    return;

                case 11: // Reconnecting  
                    LogAgent.Log("Reconnecting");
                    break;

                case 12: // Last  
                    LogAgent.Log("Last");
                    break;

                default:
                    LogAgent.Log("default");
                    break;
            }
        }

        private void AddIndex()
        {
            playlistIndex++;
            if (playlistIndex >= (axWindowsMediaPlayer1.currentPlaylist).count)
            {
                playlistIndex = 0;
            }
        }

    }
}
