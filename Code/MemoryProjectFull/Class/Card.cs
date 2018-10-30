using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace MemoryProjectFull
{
    
    /// <summary>
    /// Card Control Class.
    /// </summary>
    public sealed class Card : Control
    {

        public delegate void OnClickCallback(Card card);

        /// <summary>
        /// Animator Class which handles the animation of the Card Control.
        /// </summary>
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

        /// <summary>
        /// Initializes a new instance of the Card class.
        /// </summary>
        /// <param name="id">The user assigned identifier of the Card.</param>
        /// <param name="size">The rendering size of the Card.</param>
        /// <param name="position">The position of the Card.</param>
        /// <param name="frontImage">The Front Image of the Card.</param>
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

        /// <summary>
        /// Causes the Card to flip sides or reverses the flipping motion if the card was already flipping.
        /// </summary>
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

        /// <summary>
        /// Determines whether the Card is currently Flipping.
        /// </summary>
        /// <returns>True if the Card is flipping, false otherwise.</returns>
        public bool IsFlipping()
        {
            return animator.IsAnimating();
        }

        /// <summary>
        /// Determines whether the Card is currently revealed.
        /// </summary>
        /// <returns>True if the Card is revealed, false otherwise.</returns>
        public bool IsRevealed()
        {
            return revealed;
        }

        /// <summary>
        /// Determines whether the Card is currently concealed.
        /// </summary>
        /// <returns>True if the Card is concealed, false otherwise.</returns>
        public bool IsConcealed()
        {
            return !revealed;
        }

        /// <summary>
        /// Reveals the Card, causing it to show the front image.
        /// </summary>
        private void Reveal()
        {
            revealed = true;
        }

        /// <summary>
        /// Conceals the Card, causing it to show the back image.
        /// </summary>
        private void Conceal()
        {
            revealed = false;
        }

        /// <summary>
        /// Reveals the Card if it was concealed, conceals it otherwise.
        /// </summary>
        private void SwapFace()
        {
            revealed = !revealed;
        }

        /// <summary>
        /// The Default Callback when the Card is clicked.
        /// </summary>
        /// <param name="card">The Card that was clicked.</param>
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

        /// <summary>
        /// Gets the user assigned identifier.
        /// </summary>
        public long ID
        {
            get { return cid; }
        }

        /// <summary>
        /// Gets or sets the size of the Card.
        /// </summary>
        public Size Size
        {
            get { return new Size(this.Width, this.Height); }
            set { this.Width = value.Width; this.Height = value.Height; }
        }

        /// <summary>
        /// Gets or sets the position of the Card.
        /// </summary>
        public Point Position
        {
            get { return new Point(this.Margin.Left, this.Margin.Top); }
            set { this.Margin = new Thickness(value.X, value.Y, 0, 0); }
        }

        /// <summary>
        /// Sets the Callback to be used when the Card is clicked.
        /// </summary>
        public static OnClickCallback Callback
        {
            set { callback = value; }
        }

        /// <summary>
        /// Sets the Back Image for all Cards.
        /// </summary>
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
