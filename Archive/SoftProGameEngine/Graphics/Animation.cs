using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SoftProGameEngine.Framework;
using System.Collections.Generic;

namespace SoftProGameEngine.Graphics
{
    public class Animation : IComponent
    {
        private List<Texture2D> _frames;
        private int _framesPerSecond;
        private Queue<Texture2D> _queue;
        private bool _isPlaying;
        private Texture2D _stopTexture;

        private bool LoopPlayback { get; set; }

        private float _frameTime;
        private Texture2D _currentFrame;

        public List<Texture2D> Frames
        {
            get { return _frames; }
        }

        public int FramesPerSecond
        {
            get { return _framesPerSecond; }
        }

        public bool IsPlaying
        {
            get { return _isPlaying; }
        }

        public Texture2D CurrentFrame
        {
            get { return _currentFrame; }
        }

        private bool _hasFinished;

        public bool HasFinished
        {
            get
            {
                return _hasFinished;
            }
        }

        //public void Add(Texture2D texture2D)
        //{
        //    _frames.Add(texture2D);
        //}

        //public void Add(Texture2D texture2D, int n)
        //{
        //    for (int i = 0; i < n; i++)
        //        _frames.Add(texture2D);
        //}

        public Animation(List<Texture2D> frames, int framesPerSecond, bool loopPlayback, Texture2D stopTexture)
        {
            _frames = frames;
            _framesPerSecond = framesPerSecond;
            LoopPlayback = loopPlayback;

            _queue = null;
            _isPlaying = false;

            _frameTime = 99;//force update imediately to first texture
            _currentFrame = _frames[0];

            _stopTexture = stopTexture;

            _hasFinished = false;
        }

        public void Play()
        {
            _queue = new Queue<Texture2D>(_frames);
            _isPlaying = true;
        }

        public void Stop()
        {
            _isPlaying = false;
            if (_stopTexture != null)
                _currentFrame = _stopTexture;
        }

        //public void Stop(bool useStopTexture)
        //{
        //    if (useStopTexture)
        //    {
        //        Animation.Stop(_stopTexture);
        //    }
        //    else
        //    {
        //        Animation.Stop();
        //    }
        //}

        //public void Stop(Texture2D texture)
        //{
        //    _isPlaying = false;
        //    _currentFrame = texture;
        //}

        #region IComponent Members

        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_isPlaying)
            {
                if (_frameTime > (1.0f) / _framesPerSecond)
                {
                    _frameTime = 0;
                    _currentFrame = _queue.Dequeue();

                    if (_queue.Count == 0)
                    {
                        if (LoopPlayback)
                        {
                            _queue = new Queue<Texture2D>(_frames);
                        }
                        else
                        {
                            _isPlaying = false;
                            _hasFinished = true;
                        }
                    }
                }
                else
                {
                    _frameTime += elapsed;
                }
                //spriteBatch.Draw(CurrentFrame, _position, Color.White);
            }
            else
            {
                //spriteBatch.Draw(_frames[0], _position, Color.White);
                //_currentFrame = _frames[0];
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            return;

            //float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            //if (_isPlaying)
            //{
            //    if (_frameTime > (1.0f) / _framesPerSecond)
            //    {
            //        _frameTime = 0;
            //        _currentFrame = _queue.Dequeue();

            //        if (_queue.Count == 0)
            //            _isPlaying = false;
            //    }
            //    else
            //    {
            //        _frameTime += elapsed;
            //    }
            //    spriteBatch.Draw(CurrentFrame, _position, Color.White);
            //}
            //else
            //{
            //    spriteBatch.Draw(_frames[0], _position, Color.White);
            //}
        }

        #endregion IComponent Members

        public void Reset()
        {
            _queue = null;
            _isPlaying = false;

            _frameTime = 99;//force update imediately to first texture
            _currentFrame = _frames[0];
        }
    }
}