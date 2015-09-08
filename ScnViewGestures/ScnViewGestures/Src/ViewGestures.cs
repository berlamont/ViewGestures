﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ScnViewGestures.Plugin.Forms
{
    public class ViewGestures : ContentView
    {
        public ViewGestures()
        {
            Tap += ViewGestures_AnimationTap;
            TouchBegan += PressBeganEffect;
            TouchBegan += PressEndedEffectWithDelay;

            TouchEnded += PressEndedEffect;

            BackgroundColor = Color.Transparent;
        }

        public ViewGestures(View content) : this()
        {
            Content = content;
        }

        [Flags]
        public enum GestureType
        {
            gtNone = 0,
            gtTap = 1,
            gtLongTap = 2,
            gtDrag = 4,
            gtSwipeLeft = 8,
            gtSwipeRight = 16,
            gtSwipeUp = 32,
            gtSwipeDown = 64,

            gtSwipeHorizontal = gtSwipeLeft | gtSwipeRight,
            gtSwipeVertical = gtSwipeUp | gtSwipeDown,
            gtSwipe = gtSwipeHorizontal | gtSwipeVertical,
        };
        public GestureType SupportGestures;

        private View _mainContent;
        public new View Content
        {
            get { return _mainContent; }
            set 
            {
                _mainContent = value;
                
                AbsoluteLayout contentLayout = new AbsoluteLayout();

                AbsoluteLayout.SetLayoutFlags(value, AbsoluteLayoutFlags.All);
                AbsoluteLayout.SetLayoutBounds(value, new Rectangle(0f, 0f, 1f, 1f));
                contentLayout.Children.Add(value);
                
                animateFlashBox = new BoxView 
                { 
                    BackgroundColor = _animationColor,
                };
                AbsoluteLayout.SetLayoutFlags(animateFlashBox, AbsoluteLayoutFlags.HeightProportional | AbsoluteLayoutFlags.XProportional);
                animateFlashBox.WidthRequest = 1;
                AbsoluteLayout.SetLayoutBounds(animateFlashBox,
                    new Rectangle(0.5f, 0f, AbsoluteLayout.AutoSize, 1f));
                contentLayout.Children.Add(animateFlashBox);
                animateFlashBox.IsVisible = false;

                base.Content = contentLayout;
            }
        }

        #region Animation
        public enum AnimationType
        {
            atNone = 0,
            atScaling = 1,
            atFlashing = 2,
            atFlashingTap = 3,
            atFading = 4
        }

        public AnimationType AnimationEffect = AnimationType.atNone;

        private Color _animationColor = Color.Transparent;
        public Color AnimationColor 
        { 
            get { return _animationColor; }
            set
            {
                _animationColor = value;
                if (animateFlashBox != null)
                    animateFlashBox.BackgroundColor = _animationColor;
            } 
        }

        private double _animatationScale = 0.0;
        public double AnimationScale
        {
            get { return _animatationScale; }
            set { _animatationScale = value; }
        }
        
        private BoxView animateFlashBox;

        void ViewGestures_AnimationTap(object sender, TapEventArgs e)
        {
            if (AnimationEffect == AnimationType.atFlashingTap)
            {
                animateFlashBox.TranslationX = e.PositionX - _mainContent.Width / 2;
                animateFlashBox.IsVisible = true;

                animateFlashBox.Animate<double>(
                    "AnimateFlashBox",
                    (t) => { return t * _mainContent.Width; },
                    (x) =>
                    {
                        animateFlashBox.WidthRequest = x;

                        var delta = _mainContent.Width / 2 - Math.Abs(animateFlashBox.TranslationX) - x / 2;
                        if (delta < 0)
                        {
                            if (animateFlashBox.TranslationX < 0)
                                animateFlashBox.TranslationX -= delta;
                            else
                                animateFlashBox.TranslationX += delta;
                        }
                    },
                    16, 500, Easing.SinOut,
                    (x, y) =>
                    {
                        animateFlashBox.WidthRequest = 1;
                        animateFlashBox.IsVisible = false;
                    });
            }
            else if (AnimationEffect == AnimationType.atFlashing)
            {
                animateFlashBox.TranslationX = 0;
                animateFlashBox.IsVisible = true;

                animateFlashBox.Animate<double>(
                    "AnimateFlashBox",
                    (t) => { return t * _mainContent.Width; },
                    (x) => { animateFlashBox.WidthRequest = x; },
                    16, 500, Easing.SinOut,
                    (x, y) =>
                    {
                        animateFlashBox.WidthRequest = 1;
                        animateFlashBox.IsVisible = false;
                    });
            }

        }
        #endregion

        #region TagProperty
        public static readonly BindableProperty TagProperty =
            BindableProperty.Create<ViewGestures, string>(p => p.Tag, "");

        public string Tag
        {
            get { return (string)GetValue(TagProperty); }
            set { SetValue(TagProperty, value); }
        }
        #endregion

        #region Main gesture
        public event EventHandler TouchBegan;
        public void OnTouchBegan()
        {
            if (TouchBegan != null) 
                TouchBegan(Content, EventArgs.Empty);
        }

        public event EventHandler Touch;
        public void OnTouch(GestureType gestureType)
        {
            if (Touch != null) 
                Touch(Content, EventArgs.Empty);
        }

        public event EventHandler TouchEnded;
        public void OnTouchEnded()
        {
            if (TouchEnded != null) 
                TouchEnded(Content, EventArgs.Empty);
        }
        #endregion

        #region Tap gesture
        static object tapLocker = new object();
        private bool _isTaped = false;

        private EventHandler<TapEventArgs> _tap;
        public event EventHandler<TapEventArgs> Tap
        {
            add
            {
                _tap += value;
                SupportGestures |= GestureType.gtTap;
            }
            remove
            {
                _tap -= value;
                if (_tap == null)
                    SupportGestures ^= GestureType.gtTap;
            }
        }

        public void OnTap(double positionX, double positionY)
        {
            lock (tapLocker) 
                if (!_isTaped)
                {
                    _isTaped = true;

                    if (_tap != null)
                        _tap(Content, new TapEventArgs(positionX, positionY));
                    OnTouch(GestureType.gtTap);
                }
        }
        #endregion

        #region LongTap gesture
        private EventHandler _longTap;
        public event EventHandler LongTap
        {
            add
            {
                _longTap += value;
                SupportGestures |= GestureType.gtLongTap;
            }
            remove
            {
                _longTap -= value;
                if (_longTap == null)
                    SupportGestures ^= GestureType.gtLongTap;
            }
        }
        public void OnLongTap()
        {
            if (_longTap != null)
                _longTap(Content, EventArgs.Empty);
            OnTouch(GestureType.gtLongTap);
        }
        #endregion

        #region Swipe gesture
        private EventHandler _swipeLeft;
        public event EventHandler SwipeLeft
        {
            add
            {
                _swipeLeft += value;
                SupportGestures |= GestureType.gtSwipeLeft;
            }
            remove
            {
                _swipeLeft -= value;
                if (_swipeLeft == null)
                    SupportGestures ^= GestureType.gtSwipeLeft;
            }
        }
        public void OnSwipeLeft()
        {
            if (_swipeLeft != null)
            {
                _swipeLeft(Content, EventArgs.Empty);
                OnTouch(GestureType.gtSwipeLeft);
            }
            else
                OnTouch(GestureType.gtSwipe);
        }

        private EventHandler _swipeRight;
        public event EventHandler SwipeRight
        {
            add
            {
                _swipeRight += value;
                SupportGestures |= GestureType.gtSwipeRight;
            }
            remove
            {
                _swipeRight -= value;
                if (_swipeRight == null)
                    SupportGestures ^= GestureType.gtSwipeRight;
            }
        }
        public void OnSwipeRight()
        {
            if (_swipeRight != null)
            {
                _swipeRight(Content, EventArgs.Empty);
                OnTouch(GestureType.gtSwipeRight);
            }
            else
                OnTouch(GestureType.gtSwipe);
        }

        private EventHandler _swipeUp;
        public event EventHandler SwipeUp
        {
            add
            {
                _swipeUp += value;
                SupportGestures |= GestureType.gtSwipeUp;
            }
            remove
            {
                _swipeUp -= value;
                if (_swipeUp == null)
                    SupportGestures ^= GestureType.gtSwipeUp;
            }
        }
        public void OnSwipeUp()
        {
            if (_swipeUp != null)
            {
                _swipeUp(Content, EventArgs.Empty);
                OnTouch(GestureType.gtSwipeUp);
            }
            else
                OnTouch(GestureType.gtSwipe);
        }

        private EventHandler _swipeDown;
        public event EventHandler SwipeDown
        {
            add
            {
                _swipeDown += value;
                SupportGestures |= GestureType.gtSwipeDown;
            }
            remove
            {
                _swipeDown -= value;
                if (_swipeDown == null)
                    SupportGestures ^= GestureType.gtSwipeDown;
            }
        }
        public void OnSwipeDown()
        {
            if (_swipeDown != null)
            {
                _swipeDown(Content, EventArgs.Empty);
                OnTouch(GestureType.gtSwipeDown);
            }
            else
                OnTouch(GestureType.gtSwipe);
        }
        #endregion

        #region Drag
        private EventHandler<DragEventArgs> _drag;
        public event EventHandler<DragEventArgs> Drag
        {
            add
            {
                _drag += value;
                SupportGestures |= GestureType.gtDrag;
            }
            remove
            {
                _drag -= value;
                if (_drag == null)
                    SupportGestures ^= GestureType.gtDrag;
            }
        }
        public void OnDrag(double distanceX, double distanceY)
        {
            if (_drag != null)
                _drag(Content, new DragEventArgs(distanceX, distanceY));
            OnTouch(GestureType.gtDrag);
        }
        #endregion

        async void PressBeganEffect(object sender, EventArgs e)
        {
            lock (tapLocker)
                _isTaped = false;

            if (AnimationEffect == AnimationType.atScaling)
                await this.ScaleTo(1 + (AnimationScale / 100), 100, Easing.CubicOut);
        }

        async void PressEndedEffect(object sender, EventArgs e)
        {
            if (AnimationEffect == AnimationType.atScaling)
                await this.ScaleTo(1, 100, Easing.CubicOut);
        }

        async void PressEndedEffectWithDelay(object sender, EventArgs e)
        {
            if (_drag != null)
                return;

            //OnTouchEnded isn't called if view is included in scroll panel
            await Task.Delay(2000);
            OnTouchEnded();
        }
    }

    public class DragEventArgs : EventArgs
    {
        public readonly double DistanceX;
        public readonly double DistanceY;

        public DragEventArgs(double distanceX, double distanceY)
        {
            DistanceX = distanceX;
            DistanceY = distanceY;
        }
    }

    public class TapEventArgs : EventArgs
    {
        public readonly double PositionX;
        public readonly double PositionY;

        public TapEventArgs(double positionX, double positionY)
        {
            PositionX = positionX;
            PositionY = positionY;
        }
    }
}
