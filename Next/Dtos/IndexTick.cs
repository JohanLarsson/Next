﻿using System;

namespace Next.Dtos
{
    public class IndexTick : ITick
    {
        public string I { get; set; }
        public string M { get; set; }
        public string T { get; set; }
        public DateTime TickTimestamp { get; set; }
        public decimal Last { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
    }
}