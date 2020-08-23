﻿using System;

namespace Selva.Core
{
    public class CaptchaFailedException : RedditException
    {
        public CaptchaFailedException()
        {

        }

        public CaptchaFailedException(string message)
            : base(message)
        {

        }

        public CaptchaFailedException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }
}