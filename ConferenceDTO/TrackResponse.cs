﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ConferenceDTO
{
    public class TrackResponse : Track
    {
        public ICollection<Track> tracks { get; set; } = new List<Track>();
    }
}
