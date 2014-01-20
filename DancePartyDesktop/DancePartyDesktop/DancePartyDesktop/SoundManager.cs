using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace DanceParty
{
    public class SoundManager
    {
        private List<Song> _songs;

        private SoundEffect _scratchSound;
        private SoundEffect _popSound;

        private ContentManager _contentManager;
        private SoundManager(ContentManager contentManager)
        {
            _songs = new List<Song>();
            _contentManager = contentManager;
        }

        public void LoadAudio()
        {
            _songs.Add(_contentManager.Load<Song>("Audio\\Music0"));
            _songs.Add(_contentManager.Load<Song>("Audio\\Music1"));
            _songs.Add(_contentManager.Load<Song>("Audio\\Music2"));
            _songs.Add(_contentManager.Load<Song>("Audio\\Music3"));
            _songs.Add(_contentManager.Load<Song>("Audio\\Music4"));

            _scratchSound = _contentManager.Load<SoundEffect>("Audio\\RecordScratch");
            _popSound = _contentManager.Load<SoundEffect>("Audio\\Pop");
        }

        public Song GetRandomSong()
        {
            return _songs[Utilities.RandomHelper.GetRandomInt(0, _songs.Count)];
        }

        public SoundEffectInstance GetRecordScratchInstance()
        {
            return _scratchSound.CreateInstance();
        }

        public SoundEffectInstance GetPopSoundEffect()
        {
            return _popSound.CreateInstance();
        }

        private static SoundManager _instance;
        public static SoundManager Instance
        {
            get 
            {
                return _instance ?? (_instance = new SoundManager(DancePartyGame.Instance.Content));
            }
        }
    }
}

