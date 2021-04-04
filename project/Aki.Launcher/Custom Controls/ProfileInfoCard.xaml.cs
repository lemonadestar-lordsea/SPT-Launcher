﻿using System;
using System.Windows;
using System.Windows.Controls;

namespace Aki.Launcher.Custom_Controls
{
    /// <summary>
    /// Interaction logic for ProfileInfoCard.xaml
    /// </summary>
    public partial class ProfileInfoCard : UserControl
    {
        public int XPProgressPercentage { get; set; }

        public ProfileInfoCard()
        {
            InitializeComponent();
        }

        //nickname
        public static readonly DependencyProperty NickNameProperty =
            DependencyProperty.Register("NickName", typeof(string), typeof(ProfileInfoCard), new PropertyMetadata(string.Empty));

        public string NickName
        {
            get => (string)GetValue(NickNameProperty);
            set => SetValue(NickNameProperty, value);
        }

        //side image
        public static readonly DependencyProperty SideImagePathProperty =
            DependencyProperty.Register("SideImagePath", typeof(string), typeof(ProfileInfoCard), new PropertyMetadata(string.Empty));

        public string SideImagePath
        {
            get => (string)GetValue(SideImagePathProperty);
            set => SetValue(SideImagePathProperty, value);
        }

        //side
        public static readonly DependencyProperty SideProperty =
            DependencyProperty.Register("Side", typeof(string), typeof(ProfileInfoCard), new PropertyMetadata(string.Empty));

        public string Side
        {
            get => (string)GetValue(SideProperty);
            set => SetValue(SideProperty, value);
        }

        //level
        public static readonly DependencyProperty LevelProperty =
            DependencyProperty.Register("Level", typeof(string), typeof(ProfileInfoCard), new PropertyMetadata(string.Empty));

        public string Level
        {
            get => (string)GetValue(LevelProperty);
            set => SetValue(LevelProperty, value);
        }

        //xp
        public static readonly DependencyProperty CurrentXPProperty =
            DependencyProperty.Register("CurrentXP", typeof(string), typeof(ProfileInfoCard), new PropertyMetadata(string.Empty));

        public string CurrentXP
        {
            get => (string)GetValue(CurrentXPProperty);
            set => SetValue(CurrentXPProperty, value);
        }

        //xp_total
        public static readonly DependencyProperty TotalXPProperty =
            DependencyProperty.Register("TotalXP", typeof(string), typeof(ProfileInfoCard), new PropertyMetadata(string.Empty));

        public string TotalXP
        {
            get => (string)GetValue(TotalXPProperty);
            set => SetValue(TotalXPProperty, value);
        }

        //remainingXP
        public static readonly DependencyProperty RemainingXPProperty =
            DependencyProperty.Register("RemainingXP", typeof(string), typeof(ProfileInfoCard), new PropertyMetadata(string.Empty));

        public string RemainingXP
        {
            get => (string)GetValue(RemainingXPProperty);
            set => SetValue(RemainingXPProperty, value);
        }

        //levelprogressvalue
        public static readonly DependencyProperty LevelProgressValueProperty =
            DependencyProperty.Register("LevelProgressValue", typeof(int), typeof(ProfileInfoCard), new PropertyMetadata(0));

        public int LevelProgressValue
        {
            get => (int)GetValue(LevelProgressValueProperty);
            set => SetValue(LevelProgressValueProperty, value);
        }
    }
}