﻿using System;

namespace GsmAtWrapper
{
    public class GsmException : Exception
    {
        public GsmException()
        {

        }
        public GsmException(string message) : base(message)
        {

        }
    }
}