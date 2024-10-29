using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OHC.EAMI.Common.FixedLengthFileGenerator
{
    public class FixedLengthFileFieldAttributes : Attribute
    {
        private int _digitsBeforeDecimal = -1;
        private int _digitsAfterDecimal = -1;
        private char _defaultFiler = (char)0x20;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is required.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is required; otherwise, <c>false</c>.
        /// </value>
        public bool IsRequired
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        public int Position
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the length of the byte.
        /// </summary>
        /// <value>
        /// The length of the byte.
        /// </value>
        public int ByteLength
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the digits before decimal.
        /// </summary>
        /// <value>
        /// The digits before decimal.
        /// </value>
        public int DigitsBeforeDecimal
        {
            get
            {
                return _digitsBeforeDecimal;
            }
            set
            {
                _digitsBeforeDecimal = value;
            }
        }
        /// <summary>
        /// Gets or sets the digits after decimal.
        /// </summary>
        /// <value>
        /// The digits after decimal.
        /// </value>
        public int DigitsAfterDecimal
        {
            get
            {
                return _digitsAfterDecimal;
            }
            set
            {
                _digitsAfterDecimal = value;
            }
        }
        /// <summary>
        /// Gets or sets the length of the following placeholder byte.
        /// </summary>
        /// <value>
        /// The length of the following placeholder byte.
        /// </value>
        public int FollowingPlaceholderByteLength //This is in addition to the byte length of the data member.
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the default filer.
        /// </summary>
        /// <value>
        /// The default filer.
        /// </value>
        public char DefaultFiler
        {
            get
            {
                return _defaultFiler;
            }
            set
            {
                _defaultFiler = value;
            }
        }

        /// <summary>
        /// Gets or sets the date format.
        /// </summary>
        /// <value>
        /// The date format.
        /// </value>
        public string DateFormat
        {
            get;
            set;
        }
    }
}
