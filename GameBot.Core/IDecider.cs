﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBot.Core
{
    public interface IDecider
    {
    }

    public interface IDecider<T> : IDecider where T : IGameState, new()
    {
        ICommand Decide(T gameState);
    }
}
