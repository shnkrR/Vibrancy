#region Namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
#endregion

namespace Vibrancy
{
    class SoundProcessing
    {
        public Song songToPlay;
        VisualizationData visDataForSong;
        float[] result;
        public float[] _result;

        public float bass;
        public float cymbals;
        public float mids;

        public bool songPlayed;

        public SoundProcessing()
        {
            visDataForSong = new VisualizationData();
            MediaPlayer.Stop();
            MediaPlayer.IsVisualizationEnabled = true;
            result = new float[4];
            _result = new float[4];
            songPlayed = false;
            cymbals = 0;
        }

        public void LoadSong(string songName, string location)
        {
            Uri uri = new Uri(location);

            MediaPlayer.Volume = 1f;
            songToPlay = Song.FromUri(songName, new Uri(location));
        }

        public void LoadSong(Song _songToPlay)
        {
            songToPlay = _songToPlay;
        }

        public float BeginPlayer()
        {
            MediaPlayer.IsRepeating = false;
            if (songToPlay != null)
            {
                if (MediaPlayer.State == MediaState.Stopped || MediaPlayer.State == MediaState.Paused)
                {
                    MediaPlayer.Play(songToPlay); 
                }

                MediaPlayer.GetVisualizationData(visDataForSong);
                result[0] = ProcessVisualizationData(visDataForSong);

                TimeCheck();
            }

            result[0] *= 50;

            return result[0];
        }

        public void TimeCheck()
        {
            if (MediaPlayer.State == MediaState.Playing)
            {
                if (MediaPlayer.PlayPosition.Minutes >= songToPlay.Duration.Minutes) { }
                    //songPlayed = true;
            }
        }

        private float ProcessVisualizationData(VisualizationData visData)
        {
            ////////////////////////////////////////////////////////////////////////////////////////////////
            if (visData.Frequencies[0] > 0.4 && visData.Frequencies[0] < 0.42)
            {
                _result[0] = visData.Frequencies[0];
            }
            else
            {
                _result[0] = 0;
            }
            ////////////////////////////////////////////////////////////////////////////////////////////////
            if (visData.Frequencies[240] * 10 > 1.3 && cymbals == 0)
            {
                cymbals = (visData.Frequencies[240]);
            }
            else
            {
                cymbals = 0;
            }
            ////////////////////////////////////////////////////////////////////////////////////////////////
            bass = visData.Frequencies.Average();
            mids = visData.Frequencies[128];
            ////////////////////////////////////////////////////////////////////////////////////////////////
            if (_result[0] < 0.0f)
                _result[0] = 0;
            ////////////////////////////////////////////////////////////////////////////////////////////////
            return _result[0];
        }
    }
}
