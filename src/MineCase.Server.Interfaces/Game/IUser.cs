﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Orleans;

namespace MineCase.Server.Interfaces.Game
{
    public interface IUser : IGrainWithStringKey, IGrain
    {
        Task<string> GetName();

        Task Login(Guid sessionId);

        Task Logout();
    }
}
