using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace QuizGameDataTypes
{
    /// <summary>
    /// A quiz answer
    /// </summary>
    public class Answer
    {
        private string _answerText;
        private bool _isCorrect;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is correct.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is correct; otherwise, <c>false</c>.
        /// </value>
        public bool isCorrect
        {
            get { return _isCorrect; }
            set { _isCorrect = value; }
        }

        /// <summary>
        /// Gets or sets the answer text.
        /// </summary>
        /// <value>
        /// The answer text.
        /// </value>
        public string AnswerText
        {
            get { return _answerText; }
            set { _answerText = value; }
        }
    }
}
