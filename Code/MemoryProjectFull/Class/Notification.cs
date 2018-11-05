using MemoryProjectFull;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace NotificationsWPF
{

    public class NotificationHandler : Canvas {

        private Notification n;

        public NotificationHandler() {
            NotificationManager.OnRequestNotification += OnRequestNotification;
        }

        public void OnRequestNotification(string _message) {
            if (n != null) {
                this.Children.Remove(n);
            }

            n = new Notification(_message);
            this.Children.Add(n);
            n.FadeIn();
        }

    }

    public static class NotificationManager {

        public static Action<string> OnRequestNotification;

        public static void RequestNotification(string _message) {
            OnRequestNotification?.Invoke(_message);
        }

    }

    public class Notification : Control
    {
        private const int OFFSET_FROM_TOP = 32;

        private const int CONTENT_X_OFFSET  = 32;
        private const int CONTENT_Y_OFFSET  = 20;

        private const string FONTFAMILY_PATH = "Verdana";

        private static readonly Color OUTLINE_COLOR    = Color.FromRgb(255, 255, 255);
        private static readonly Color BACKGROUND_COLOR = Color.FromRgb(238, 238, 238);
        private static readonly Color FOREGROUND_COLOR = Color.FromRgb( 96,  96,  96);

        private class Animator
        {

            public enum FadeType { FADE_IN, FADE_OUT }

            private const double MAX_OPACITY = 1.0;
            private const double MIN_OPACITY = 0.0;

            private const double FRAMES = 60.0;
            private const double STEP = (MAX_OPACITY - MIN_OPACITY) / FRAMES;

            private const double DURATION_IN_MS = 1000.0;

            private static readonly TimeSpan INTERVAL = TimeSpan.FromMilliseconds(DURATION_IN_MS / FRAMES);

            public Animator(Notification notification)
            {
                this.notification = notification;
                this.timer = new DispatcherTimer(INTERVAL, DispatcherPriority.Send, UpdateAnimation, Dispatcher.CurrentDispatcher);
                this.timer.Stop(); //Prevent the timer from starting automagically.
            }

            public void Start(FadeType fadeType)
            {
                if (!IsAnimating)
                {
                    this.fadeType = fadeType;
                    this.timer.Start();
                }
            }

            private void UpdateAnimation(object o, EventArgs e)
            {
                switch (fadeType)
                {
                    case FadeType.FADE_IN:

                        if ((notification.Opacity += STEP) >= MAX_OPACITY)
                        {
                            this.Stop();
                            notification.Opacity = MAX_OPACITY;
                            
                            this.fadeType = FadeType.FADE_OUT;
                            this.timer.Start();
                        }
                        break;

                    case FadeType.FADE_OUT:

                        if ((notification.Opacity -= STEP) <= MIN_OPACITY)
                        {
                            this.Stop();
                            notification.Opacity = MIN_OPACITY;
                        }
                        break;
                }

                notification.InvalidateVisual();
            }

            public void Stop()
            {
                if (IsAnimating) this.timer.Stop();
            }

            public bool IsAnimating
            {
                get { return timer.IsEnabled; }
            }

            private Notification notification;
            private DispatcherTimer timer;
            private FadeType fadeType;

        }

        public Notification(string message)
        {
            this.FontFamily = new FontFamily(FONTFAMILY_PATH);
            this.FontWeight = FontWeights.Light;

            this.Foreground = new SolidColorBrush(FOREGROUND_COLOR);

            this.message = new FormattedText(   message,
                                                CultureInfo.CurrentCulture,
                                                FlowDirection.LeftToRight,
                                                new Typeface(this.FontFamily, this.FontStyle, this.FontWeight, this.FontStretch),
                                                this.FontSize,
                                                this.Foreground                                                                              );

            this.message.Trimming = TextTrimming.CharacterEllipsis;

            this.HorizontalAlignment = HorizontalAlignment.Center;

            this.Width  = this.message.Width  + CONTENT_X_OFFSET;
            this.Height = this.message.Height + CONTENT_Y_OFFSET;

            this.Margin = new Thickness((MainWindow.SCREEN_WIDTH - this.Width) / 2, OFFSET_FROM_TOP, 0, 0);

            this.Opacity = 0.0;

            this.animator = new Animator(this);
        }

        public void FadeIn()
        {
            animator.Stop();
            animator.Start(Animator.FadeType.FADE_IN);
        }

        public void FadeOut()
        {
            animator.Stop();
            animator.Start(Animator.FadeType.FADE_OUT);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            var brush = new SolidColorBrush(BACKGROUND_COLOR);
            var pen   = new Pen(new SolidColorBrush(OUTLINE_COLOR), 1.0);
            var rect  = new Rect(new Size(this.Width, this.Height));

            drawingContext.DrawRoundedRectangle(brush, pen, rect, 16.0, 16.0);

            var textX = (this.Width  - message.Width ) / 2.0;
            var textY = (this.Height - message.Height) / 2.0;

            drawingContext.DrawText(message, new Point(textX, textY));
        }

        public bool IsShown
        {
            get { return (!animator.IsAnimating) && (Opacity == 1.0); }
        }

        private FormattedText message;

        private Animator animator;

    }
}
