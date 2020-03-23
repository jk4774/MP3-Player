﻿using MP3Player.Interfaces;
using System;
using System.Windows.Input;
using static MP3Player.Extensions.PlaylistExtension;

namespace MP3Player.Commands
{
    public class ClearPlaylistCommand : ICommand, IClearPlaylistCommand
    {
        private readonly IPlaylist Playlist;

        public ClearPlaylistCommand(IPlaylist playlist)
        {
            Playlist = playlist;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return Playlist.CanDeleteOrClear();
        }

        public void Execute(object parameter)
        {
            Playlist.ClearPlaylist();
        }
    }
}
