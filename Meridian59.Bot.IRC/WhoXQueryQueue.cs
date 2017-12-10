﻿/*
 Copyright (c) 2012-2013 Clint Banzhaf
 This file is part of "Meridian59 .NET".

 "Meridian59 .NET" is free software: 
 You can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, 
 either version 3 of the License, or (at your option) any later version.

 "Meridian59 .NET" is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
 without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 See the GNU General Public License for more details.

 You should have received a copy of the GNU General Public License along with "Meridian59 .NET".
 If not, see http://www.gnu.org/licenses/.
*/

using Meridian59.Common;
using System;

namespace Meridian59.Bot.IRC
{
    public class WhoXQueryQueue
    {
        public LockingQueue<Tuple<DateTime, string>> Queue { get; set; }
        private DateTime LastQueryTime;

        public WhoXQueryQueue()
        {
            Queue = new LockingQueue<Tuple<DateTime, string>>();
            LastQueryTime = DateTime.UtcNow;
        }

        public void Enqueue(string Name)
        {
            Queue.Enqueue(Tuple.Create(DateTime.UtcNow, Name));
        }

        public bool TryDequeue(out string Name)
        {
            Name = null;

            // Only send one query per second.
            if ((DateTime.UtcNow - LastQueryTime).TotalSeconds > 1
                && Queue.TryPeek(out Tuple<DateTime, string> whoXCommand))
            {
                DateTime now = DateTime.UtcNow;
                // Query must be at least 5 seconds old.
                // This allows for channel joiners who don't register immediately.
                if ((DateTime.UtcNow - whoXCommand.Item1).TotalSeconds > 5
                    && Queue.TryDequeue(out whoXCommand))
                {
                    Name = whoXCommand.Item2;
                    LastQueryTime = DateTime.UtcNow;

                    return true;
                }
            }

            return false;
        }
    }
}
