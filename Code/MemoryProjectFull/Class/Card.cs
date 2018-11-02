using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace MemoryProjectFull
{
    [Serializable]
    public sealed class Card : Control
    {

        public delegate void OnClickCallback(Card card);

        private class Animator
        {

            private enum Motion { COUNTERCLOCKWISE, CLOCKWISE }

            private const uint MAX_ROTATION = 180;
            private const uint MID_ROTATION = 90;
            private const uint MIN_ROTATION = 0;

            private const double DURATION = 500.0;
            private const uint FRAMES = 60;

            private const uint STEP = MAX_ROTATION / FRAMES;

            public Animator(Card card)
            {
                this.card = card;
                this.timer = new DispatcherTimer(interval, DispatcherPriority.Send, UpdateAnimation, Dispatcher.CurrentDispatcher);
                this.timer.Stop(); //prevent timer from running automagically.
                this.rotation = MIN_ROTATION;
                this.motion = Motion.COUNTERCLOCKWISE;
            }

            public void Start()
            {
                if (!IsAnimating())
                {
                    card.IsEnabled = false;
                    timer.Start();
                }
            }

            public bool IsAnimating()
            {
                return timer.IsEnabled;
            }

            public void ReverseMotion()
            {
                this.motion = (this.motion == Motion.COUNTERCLOCKWISE) ? Motion.CLOCKWISE : Motion.COUNTERCLOCKWISE;

                if (rotation == MID_ROTATION)
                {
                    card.SwapFace();
                }
            }

            private void RotateCounterclockwise()
            {
                if (rotation >= MAX_ROTATION)
                {
                    this.Stop();
                    motion = Motion.CLOCKWISE;

                    return;
                }

                rotation += STEP;

                if (rotation == MID_ROTATION)
                {
                    card.SwapFace();
                }
            }

            private void RotateClockwise()
            {
                if (rotation == MIN_ROTATION)
                {
                    this.Stop();
                    motion = Motion.COUNTERCLOCKWISE;

                    return;
                }

                rotation -= STEP;

                if (rotation == MID_ROTATION)
                {
                    card.SwapFace();
                }
            }

            private void UpdateAnimation(object o, EventArgs e)
            {
                switch (motion)
                {
                    case Motion.COUNTERCLOCKWISE:
                        RotateCounterclockwise();
                        break;
                    case Motion.CLOCKWISE:
                        RotateClockwise();
                        break;
                }

                card.InvalidateVisual();
            }

            public void DrawNextFrame(DrawingContext context)
            {
                ImageSource img = card.IsRevealed() ? card.front : Card.back;
                int frame_width = (int)(card.Width * cosines[rotation]);
                Rect rect = new Rect(new Point(((card.Width - frame_width) / 2), 0), (new Size(frame_width, card.Height)));

                context.DrawImage(img, rect);
            }

            public void Stop()
            {
                if (IsAnimating())
                {
                    timer.Stop();
                    card.IsEnabled = true;
                }
            }

            private Card card;

            private DispatcherTimer timer;
            private static readonly TimeSpan interval = new TimeSpan(0, 0, 0, 0, (int)(DURATION / FRAMES));

            private uint rotation;
            private Motion motion;

            private static readonly double[] cosines = ProduceCosines();
            private static double[] ProduceCosines()
            {
                double[] cosines = new double[MAX_ROTATION + 1];

                for (uint angle = MIN_ROTATION; angle <= MID_ROTATION; angle++)
                {
                    cosines[angle] = Math.Cos(angle * (Math.PI / 180));
                    cosines[MAX_ROTATION - angle] = cosines[angle]; //Mirror angle
                }
                return cosines;
            }
        }

        public Card(long id, Size size, Point position, ImageSource frontImage)
        {
            if (back == null) throw new InvalidOperationException("BackImage not initialized. The BackImage must be initialized BEFORE creating any Card");

            this.Margin = new Thickness(position.X, position.Y, 0, 0);
            this.Width = size.Width;
            this.Height = size.Height;
            this.IsTabStop = false;
            this.Cursor = Cursors.Hand;

            this.cid = id;
            this.front = frontImage.Clone();

            this.revealed = false;
            this.animator = new Animator(this);
        }

        public void Flip()
        {
            if (!IsFlipping())
            {
                animator.Start();
            }
            else
            {
                animator.ReverseMotion();
            }
        }

        public bool IsFlipping()
        {
            return animator.IsAnimating();
        }

        public bool IsRevealed()
        {
            return revealed;
        }

        public bool IsConcealed()
        {
            return !revealed;
        }

        private void Reveal()
        {
            revealed = true;
        }

        private void Conceal()
        {
            revealed = false;
        }

        private void SwapFace()
        {
            revealed = !revealed;
        }

        private static void DefaultCallback(Card card)
        {
            card.Flip();
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            callback(this);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            animator.DrawNextFrame(drawingContext);
        }

        public long ID
        {
            get { return cid; }
        }

        public Size Size
        {
            get { return new Size(this.Width, this.Height); }
            set { this.Width = value.Width; this.Height = value.Height; }
        }

        public Point Position
        {
            get { return new Point(this.Margin.Left, this.Margin.Top); }
            set { this.Margin = new Thickness(value.X, value.Y, 0, 0); }
        }

        public static OnClickCallback Callback
        {
            set { callback = value; }
        }

        public static BitmapImage BackImage
        {
            set { back = value.Clone(); }
        }

        private readonly long cid;

        private ImageSource front;
        private static ImageSource back = null;

        private bool revealed;
        private Animator animator;

        [Obsolete("This field was made public by mistake. Please use the public property 'Callback' (note: uppercase 'C') instead.", false)]
        public static OnClickCallback callback = DefaultCallback;
    }
}
