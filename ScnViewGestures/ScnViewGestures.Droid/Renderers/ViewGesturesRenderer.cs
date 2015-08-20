using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using ScnViewGestures.Plugin.Forms;
using ScnViewGestures.Plugin.Forms.Droid.Renderers;

[assembly: ExportRenderer(typeof(ViewGestures), typeof(ViewGesturesRenderer))]

namespace ScnViewGestures.Plugin.Forms.Droid.Renderers
{
    public class ViewGesturesRenderer : ViewRenderer
    {
        // Used for registration with dependency service
        public async static void Init()
        {
            var temp = DateTime.Now;
        }

        private readonly GestureListener _listener;
        private readonly GestureDetector _detector;

        public ViewGesturesRenderer()
        {
            _listener = new GestureListener();
            _detector = new GestureDetector(_listener);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.View> e)
        {
            base.OnElementChanged(e);

            var viewGestures = (ViewGestures)Element;

            if (e.NewElement == null)
            {
                this.GenericMotion -= HandleGenericMotion;
                this.Touch -= HandleTouch;

                _listener.OnTap = null;
                _listener.OnTouchBegan = null;
                _listener.OnTouchEnded = null;
                _listener.OnLongTap = null;

                _listener.OnSwipeLeft = null;
                _listener.OnSwipeRight = null;
                _listener.OnSwipeUp = null;
                _listener.OnSwipeDown = null;
            }

            if (e.OldElement == null)
            {
                this.GenericMotion += HandleGenericMotion;
                this.Touch += HandleTouch;

                _listener.OnTap = (() => viewGestures.OnTap());
                _listener.OnLongTap = (() => viewGestures.OnLongTap());

                _listener.OnTouchBegan = (() => viewGestures.OnTouchBegan());
                _listener.OnTouchEnded = (() => viewGestures.OnTouchEnded());

                _listener.OnSwipeLeft = (() => viewGestures.OnSwipeLeft());
                _listener.OnSwipeRight = (() => viewGestures.OnSwipeRight());
                _listener.OnSwipeUp = (() => viewGestures.OnSwipeUp());
                _listener.OnSwipeDown = (() => viewGestures.OnSwipeDown());

                _listener.OnDrag = ((x, y) => viewGestures.OnDrag(x, y));
            }
        }

        void HandleTouch(object sender, TouchEventArgs e)
        {
            _detector.OnTouchEvent(e.Event);
        }

        void HandleGenericMotion(object sender, GenericMotionEventArgs e)
        {
            _detector.OnTouchEvent(e.Event);
        }


        public class GestureListener : GestureDetector.SimpleOnGestureListener
        {
            public Action OnTap;
            public Action OnLongTap;

            public Action OnTouchBegan;
            public Action OnTouchEnded;

            public Action OnSwipeLeft;
            public Action OnSwipeRight;
            public Action OnSwipeUp;
            public Action OnSwipeDown;

            public Action<float, float> OnDrag;

            public override bool OnDown(MotionEvent e)
            {
                #if DEBUG
                Console.WriteLine("OnDown");
                #endif

                if (OnTouchBegan != null)
                    OnTouchBegan();

                return base.OnDown(e);
            }

            public override void OnShowPress(MotionEvent e)
            {
                #if DEBUG
                Console.WriteLine("OnShowPress");
                #endif

                if (OnTouchBegan != null)
                    OnTouchBegan();

                base.OnShowPress(e);
            }

            public override void OnLongPress(MotionEvent e)
            {
                #if DEBUG
                Console.WriteLine("OnLongPress");
                #endif

                if (OnLongTap != null)
                    OnLongTap();

                base.OnLongPress(e);
            }

            public override bool OnDoubleTap(MotionEvent e)
            {
                #if DEBUG
                Console.WriteLine("OnDoubleTap");
                #endif

                return base.OnDoubleTap(e);
            }

            public override bool OnDoubleTapEvent(MotionEvent e)
            {
                #if DEBUG
                Console.WriteLine("OnDoubleTapEvent");
                #endif

                return base.OnDoubleTapEvent(e);
            }

            public override bool OnSingleTapUp(MotionEvent e)
            {
                #if DEBUG
                Console.WriteLine("OnSingleTapUp");
                #endif

                if (OnTap != null)
                    OnTap();

                return base.OnSingleTapUp(e);
            }

            public override bool OnSingleTapConfirmed(MotionEvent e)
            {
                #if DEBUG
                Console.WriteLine("OnSingleTapConfirmed");
                #endif

                if (OnTouchEnded != null)
                    OnTouchEnded();

                return base.OnSingleTapConfirmed(e);
            }

            private static int SWIPE_THRESHOLD = 100;
            private static int SWIPE_VELOCITY_THRESHOLD = 100;
            private static int TAP_THRESHOLD = 20;

            public override bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
            {
                #if DEBUG
                Console.WriteLine("OnFling");
                #endif

                float diffY = e2.GetY() - e1.GetY();
                float diffX = e2.GetX() - e1.GetX();

                if (Math.Abs(diffX) < TAP_THRESHOLD && Math.Abs(diffX) < TAP_THRESHOLD)
                {
                    if (OnTap != null)
                        OnTap();
                }
                else
                if (Math.Abs(diffX) > Math.Abs(diffY))
                {
                    if (Math.Abs(diffX) > SWIPE_THRESHOLD && Math.Abs(velocityX) > SWIPE_VELOCITY_THRESHOLD)
                    {
                        if (diffX > 0)
                        {
                            if (OnSwipeRight != null)
                                OnSwipeRight();
                        }
                        else
                        {
                            if (OnSwipeLeft != null)
                                OnSwipeLeft();
                        }
                    }
                }
                else if (Math.Abs(diffY) > SWIPE_THRESHOLD && Math.Abs(velocityY) > SWIPE_VELOCITY_THRESHOLD)
                {
                    if (diffY > 0)
                    {
                        if (OnSwipeDown != null)
                            OnSwipeDown();
                    }
                    else
                    {
                        if (OnSwipeUp != null)
                            OnSwipeUp();
                    }
                }

                if (OnTouchEnded != null)
                    OnTouchEnded();

                return true;
            }

            private static int SCROLL_THRESHOLD = 2;

            public override bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
            {
                #if DEBUG
                Console.WriteLine("OnScroll");
                #endif

                float diffX = e2.GetX() - e1.GetX();
                float diffY = e2.GetY() - e1.GetY();

                if (OnDrag != null)
                    OnDrag(diffX / SCROLL_THRESHOLD, diffY / SCROLL_THRESHOLD);
                else
                    if (OnTouchEnded != null)
                        OnTouchEnded();

                return base.OnScroll(e1, e2, distanceX, distanceY);
            }
        }
     }
}