﻿using MP3Player.Enums;
using MP3Player.Interfaces.Helpers;
using MP3Player.Interfaces.Models;
using MP3Player.Models;
using NAudio.Wave;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MP3Player.Helpers
{
    public class SongHelpers : ISongHelpers
    {
        private ISong Song;
        public SongHelpers() { }
        public SongHelpers(ISong song)
        {
            Song = song;
        }

        public bool CanPlay(IPlaylist playlist)
        {
            if (Song == null || string.IsNullOrWhiteSpace(playlist?.SelectedSong))
                return false;
            return true;
        }

        public bool CanPause()
        {
            if (Song == null || !Song.IsPlaying)
                return false;
            return true;
        }

        public void Play(IPlaylist playlist, WaveOut waveOut)
        {
            if (!Song.IsPlaying && Song.IsPausing && playlist.SelectedSong == Song.Path)
            {
                waveOut.Play();
                Song.IsPlaying = true;
                Song.IsPausing = false;
                return;
            }
            else if (Song.IsPlaying)
            {
                waveOut.Pause();
            }

            PlayerHelper(playlist, waveOut);
        }

        public void Pause(WaveOut waveOut)
        {
            waveOut.Pause();
            Song.IsPlaying = false;
            Song.IsPausing = true;
        }

        public void UniversalPlay(IPlaylist playlist, PlayType playType, WaveOut waveOut)
        {
            if (Song.IsPlaying)
            {
                waveOut.Pause();
                Song.IsPausing = false;
            }

            playlist.SelectedSong = GetNewSongPath(playlist.SongsList, playType);
            PlayerHelper(playlist, waveOut);
        }

        private void PlayerHelper(IPlaylist playlist, WaveOut waveOut)
        {
            if (!File.Exists(playlist.SelectedSong))
                playlist.SongsList.Remove(playlist.SelectedSong);

            if (!string.IsNullOrWhiteSpace(playlist.SelectedSong) && File.Exists(playlist.SelectedSong))
            {
                Song = new Song(playlist.SelectedSong, Song.Volume) { IsPlaying = true };
                Song.PositionMax = Song.MP3.TotalTime.TotalSeconds;

                Song.CountTime((obj, e) =>
                {
                    if (Song.MP3.CurrentTime == Song.MP3.TotalTime && playlist.SongsList.FirstOrDefault() != null)
                    {
                        playlist.SelectedSong = GetNewSongPath(playlist.SongsList, PlayType.Next);
                        PlayerHelper(playlist, waveOut);
                    }

                    Song.TimeText = string.Format("{0} {1}",
                        Song.MP3.CurrentTime.ToString(@"hh\:mm\:ss"),
                        Song.MP3.TotalTime.ToString().Split('.')[0]);

                    if (Song.IsPlaying)
                        Song.PositionValue = Song.MP3.CurrentTime.TotalSeconds;
                });

                Song.ChangePosition();
                Song.Name = Path.GetFileName(Song.MP3.FileName);
                Song.MP3.Volume = Song.Volume / 100;
                waveOut.Init(Song.MP3);
                waveOut.Play();
            }
        }

        private string GetNewSongPath(IList<string> songsList, PlayType playType)
        {
            var songsListWithIDs = songsList.Select((x, i) => new { Index = i, Value = x });
            var currentlyID = songsListWithIDs.First(x => x.Value == Song.Path).Index;
            var ID = currentlyID == 0 ? songsList.Count - 1 : currentlyID - 1;

            if (playType == PlayType.Next)
                ID = currentlyID == songsList.Count - 1 ? 0 : currentlyID + 1;

            return songsListWithIDs.First(x => x.Index == ID).Value;
        }
    }
}