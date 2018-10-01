using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace MemoryProjectFull
{

    public sealed class Card : Control
    {

        public delegate void OnClickCallback(Card card);

        private class Animator
        {

            private const uint max_rotation = 180;
            private const uint mid_rotation = 90;
            private const uint min_rotation = 0;

            private const double duration = 500.0;
            private const uint frames = 60;

            private const uint step = max_rotation / frames;

            public Animator(Card card)
            {
                this.card = card;
                this.timer = new DispatcherTimer(DispatcherPriority.Send);
                this.updatePending = false;
                this.rotation = min_rotation;

                timer.Interval = new TimeSpan(0, 0, 0, 0, (int) Math.Round(duration / frames));
                timer.Tick += new EventHandler(UpdateAnimation);
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

            public void Stop()
            {
                if (IsAnimating())
                {
                    timer.Stop();
                    card.IsEnabled = true;
                    updatePending = false;
                }
            }

            public void DrawNextFrame(DrawingContext context)
            {
                updatePending = false;

                ImageSource img = card.IsRevealed() ? card.front : Card.back;
                int frame_width = (int)(card.Width * cosines[rotation]);
                Rect rect = new Rect(new Point(((card.Width - frame_width) / 2), 0), (new Size(frame_width, card.Height)));

                context.DrawImage(img, rect);
            }

            private void UpdateAnimation(object o, EventArgs e)
            {
                rotation += step;

                if (rotation == mid_rotation)
                {
                    card.Swap();
                }
                else if (rotation >= max_rotation)
                {
                    rotation = min_rotation;
                    this.Stop();
                }

                if (!updatePending)
                {
                    updatePending = true;
                    card.InvalidateVisual();
                }
            }

            private Card card;

            private DispatcherTimer timer;

            private bool updatePending;
            private uint rotation;

            private static readonly double[] cosines = ProduceCosines();
            private static double[] ProduceCosines()
            {
                double[] cosines = new double[max_rotation + 1];

                for (uint angle = min_rotation; angle <= max_rotation; angle++)
                {
                    cosines[angle] = Math.Cos(angle * (Math.PI / 180));
                    if (cosines[angle] < 0) cosines[angle] *= -1;
                }

                return cosines;
            }

            public bool disposed = false;

        }

        public Card(long id, Size size, Point position, ImageSource frontImage)
        {
            this.Margin = new Thickness(position.X, position.Y, 0, 0);
            this.Width = size.Width;
            this.Height = size.Height;
            this.IsTabStop = false;
            this.Cursor = Cursors.Hand;

            this.cid = id;
            this.front = frontImage.Clone();
            this.front.Freeze();

            this.revealed = false;
            this.animator = new Animator(this);
        }

        public void Flip()
        {
            animator.Start();
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

        private void Swap()
        {
            revealed = !revealed;
        }

        private void Reveal()
        {
            revealed = true;
        }

        private void Conceal()
        {
            revealed = false;
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
            set
            {
                back = value.Clone();
                back.Freeze();
            }
        }

        private readonly long cid;

        private ImageSource front;
        private static ImageSource back;

        private bool revealed;
        private Animator animator;

        private static OnClickCallback callback = DefaultCallback;

    }
}
