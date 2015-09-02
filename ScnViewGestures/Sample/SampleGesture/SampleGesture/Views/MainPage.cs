﻿using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using ScnViewGestures.Plugin.Forms;

namespace SampleGesture.Views
{
    public class MainPage : ContentPage
    {
        public MainPage()
        {
            #region Gesture 1
            Label titleTap = new Label
            {
                Text = "Press label:",
            };

            Label pressLabel = new Label
            {
                Text = "Tap me",
                FontSize = 30,
            };

            var tapViewGestures = new ViewGestures
            {
                BackgroundColor = Color.Transparent,
                Content = pressLabel,
                DeformationValue = -5,
                HorizontalOptions = LayoutOptions.Center,
            };
            tapViewGestures.Tap += (s, e) => { this.DisplayAlert("Tap", "Gesture finished", "OK"); };
            #endregion

            #region Gesture 2
            Label titleSwipe = new Label
            {
                Text = "Swipe or touch this box:",
            };
            
            BoxView boxSwipe = new BoxView
            {
                BackgroundColor = Color.Yellow,
                WidthRequest = 150,
                HeightRequest = 150,
            };

            var swipeViewGestures = new ViewGestures
            {
                BackgroundColor = Color.Transparent,
                Content = boxSwipe,
                HorizontalOptions = LayoutOptions.Center,
            };
            swipeViewGestures.SwipeUp += (s, e) => { this.DisplayAlert("Swipe", "UP", "OK"); };
            swipeViewGestures.SwipeDown += (s, e) => { this.DisplayAlert("Swipe", "DOWN", "OK"); };
            swipeViewGestures.SwipeLeft += (s, e) => { this.DisplayAlert("Swipe", "LEFT", "OK"); };
            swipeViewGestures.SwipeRight += (s, e) => { this.DisplayAlert("Swipe", "RIGHT", "OK"); };

            swipeViewGestures.TouchBegan += (s, e) => { boxSwipe.BackgroundColor = Color.Red; };
            swipeViewGestures.TouchEnded += (s, e) => { boxSwipe.BackgroundColor = Color.Yellow; };

            #endregion

            #region Gesture 3
            Label titleDrag = new Label
            {
                Text = "Drag box:",
            };

            BoxView boxDrag = new BoxView
            {
                BackgroundColor = Color.Green,
                WidthRequest = 100,
                HeightRequest = 100,
            };

            var dragViewGestures = new ViewGestures
            {
                BackgroundColor = Color.Transparent,
                Content = boxDrag,
                HorizontalOptions = LayoutOptions.Center,
            };
            dragViewGestures.Drag += (s, e) =>
            {
                dragViewGestures.TranslationX += e.DistanceX;
                dragViewGestures.TranslationY += e.DistanceY;
            };
            #endregion

            Content = new StackLayout
            {
                Padding = new Thickness(20),
                Spacing = 20,
                Children = 
                {
                    titleTap,
                    tapViewGestures,

                    titleSwipe,
                    swipeViewGestures,

                    titleDrag,
                    dragViewGestures,
                }
            };
        }
    }
}