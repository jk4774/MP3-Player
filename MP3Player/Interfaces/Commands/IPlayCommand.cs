﻿using System;

namespace MP3Player.Interfaces.Commands
{
    public interface IPlayCommand
    {
        event EventHandler CanExecuteChanged;
        bool CanExecute(object parameter);
        void Execute(object parameter);
    }
}