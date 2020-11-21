using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftProGameWindows
{
    public interface IKeyboardSubscriber
    {
        void RecieveTextInput(char inputChar);
        void RecieveTextInput(string text);
        void RecieveCommandInput(char command);
        void RecieveSpecialInput(Keys key);

        bool Selected { get; set; } //or Focused
    }
}
