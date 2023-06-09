﻿using System;

namespace Newtonsoft_X.Json.Linq
{
    /// <summary>
    /// Specifies the settings used when loading JSON.
    /// </summary>
    public class JsonLoadSettings
    {
        /// <summary>
        /// Gets or sets how JSON comments are handled when loading JSON.
        /// The default value is <see cref="F:Newtonsoft.Json.Linq.CommentHandling.Ignore" />.
        /// </summary>
        /// <value>The JSON comment handling.</value>
        public CommentHandling CommentHandling
        {
            get
            {
                return _commentHandling;
            }
            set
            {
                if (value < CommentHandling.Ignore || value > CommentHandling.Load)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                _commentHandling = value;
            }
        }

        /// <summary>
        /// Gets or sets how JSON line info is handled when loading JSON.
        /// The default value is <see cref="F:Newtonsoft.Json.Linq.LineInfoHandling.Load" />.
        /// </summary>
        /// <value>The JSON line info handling.</value>
        public LineInfoHandling LineInfoHandling
        {
            get
            {
                return _lineInfoHandling;
            }
            set
            {
                if (value < LineInfoHandling.Ignore || value > LineInfoHandling.Load)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                _lineInfoHandling = value;
            }
        }

        private CommentHandling _commentHandling;

        private LineInfoHandling _lineInfoHandling;
    }
}
