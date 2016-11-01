using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.Audio;
namespace MoodMusic
{
    public class Audio_Manager : MonoBehaviour
    {

        public int NumberofTunes; // To know what to silecne when playing SFX

        public AudioMixer Mixer; // One Mixer to mix them all

        public string[] NameOfSongs; // Also set this in the mixer as Exposed Parameters

        public AudioMixerGroup[] TunesInMixer; //Take the mixer channels and put them in here

        public AudioSource[] TunesInSource;// Take the audio files and put them in here

        public float[] Volume;

        public float InterpolateMod = 100f; //Around 6 looks good for slow transition
        [Range(0, 100)]
        public int AdventureMeter = 0; // Can be any tringger for playing new music. Just change(make more rules) this when you want to change music

        //For SFX
        public bool SFXBool = false;
        private bool SFXIntBool = false;
       
        //For repetition of BGM
        private int[] CurrentPlayingSong;
        private int SelectedSong;


        void Start()
        {
            CurrentPlayingSong = new int[NumberofTunes];
            Volume = new float[NumberofTunes];
            //make sure that songs are not playing at the start of the scene
            for (int i = 0; i < NumberofTunes; i++)
            {
                CurrentPlayingSong[i] = i;
                Volume[i] = -80;
            }
            
        }

        
        void Update()
        {
            
            if (AdventureMeter > 50)
            {
                AmpsSong(1);
                SilenceSong(0);
                SelectedSong = 1;
            }
            else
            {
                AmpsSong(0);
                SilenceSong(1);
                SelectedSong = 0;
            }


            if (SFXBool == true)
            {
                PlaySFX(2, -20f);
            }

            PlayConstantly(SelectedSong);
        }
        /// <summary>
        /// Slowly silences a song given by Index
        /// </summary>
        /// <param name="SongIndex"></param> The number of the song that is going to be silenced[Starting form 0]
        void SilenceSong(int SongIndex)
        {
            TunesInMixer[CurrentPlayingSong[SongIndex]].audioMixer.SetFloat(NameOfSongs[SongIndex], Volume[CurrentPlayingSong[SongIndex]]);

            if (Volume[CurrentPlayingSong[SongIndex]] >= -80)
                Volume[CurrentPlayingSong[SongIndex]] = Volume[CurrentPlayingSong[SongIndex]] - InterpolateMod * Time.deltaTime;

            if ((int)Volume[CurrentPlayingSong[SongIndex]] == -80)
            {
                TunesInSource[CurrentPlayingSong[SongIndex]].Pause();
            }
        }
        /// <summary>
        /// Slowly increases the volume of a song till 0
        /// </summary>
        /// <param name="SongIndex"></param>The number of the song that volumes is increasing[Starting form 0]
        void AmpsSong(int SongIndex)
        {
            if ((int)Volume[CurrentPlayingSong[SongIndex]] == -80)
                TunesInSource[CurrentPlayingSong[SongIndex]].Play();

            if (Volume[CurrentPlayingSong[SongIndex]] < 0)
                Volume[CurrentPlayingSong[SongIndex]] = Volume[CurrentPlayingSong[SongIndex]] + InterpolateMod * Time.deltaTime;

            TunesInMixer[CurrentPlayingSong[SongIndex]].audioMixer.SetFloat(NameOfSongs[SongIndex], Volume[CurrentPlayingSong[SongIndex]]);
        }

        /// <summary>
        /// Silences other sounds while playing a SFX
        /// </summary>
        /// <param name="SongIndex"></param>[Starting form 0]
        /// <param name="SupressVol"></param> The volume of other sound while plaing the SFX min -80 max 0 
        public void PlaySFX(int SongIndex, float SupressVol)
        {            
            if (SFXIntBool == false)
            {
                TunesInSource[SongIndex].Play();
                SFXIntBool = true;           
            }

            if (TunesInSource[SongIndex].isPlaying == true)
            {
                for (int i = 0; i < NumberofTunes; i++)
                {
                    if (i != SongIndex)
                    {
                        TunesInMixer[i].audioMixer.SetFloat(NameOfSongs[i], SupressVol);
                    }
                }
            }
            
            if(TunesInSource[SongIndex].isPlaying == false)
            {
                SFXIntBool = false;
                SFXBool = false;
                return;
            }
        }
        /// <summary>
        /// Simple function to loop the songs; put it at the end of update()
        /// </summary>
        /// <param name="SongIndex"></param> Number of the song that is going to be looped.[Starting form 0]
        void PlayConstantly(int SongIndex)
        {
            if(TunesInSource[SongIndex].isPlaying == false)
            {
                TunesInSource[SongIndex].Play();
            }
        }
    }

}
