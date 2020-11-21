using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace QuizGameDataTypes
{
    /// <summary>
    /// A quiz question
    /// </summary>
    public class Question
    {
        private String _questionText;
        private Answer[] _answers;

        /// <summary>
        /// Gets or sets the answers.
        /// </summary>
        /// <value>
        /// The answers.
        /// </value>
        public Answer[] Answers
        {
            get { return _answers; }
            set { _answers = value; }
        }

        /// <summary>
        /// Gets or sets the question text.
        /// </summary>
        /// <value>
        /// The question text.
        /// </value>
        public string QuestionText
        {
            get { return _questionText; }
            set { _questionText = value; }
        }

    }
}
