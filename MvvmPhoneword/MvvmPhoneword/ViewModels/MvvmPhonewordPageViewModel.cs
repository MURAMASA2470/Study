﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MvvmPhoneword.Models;
using Xamarin.Forms;

namespace MvvmPhoneword.ViewModels
{
    public class MvvmPhonewordPageViewModel : INotifyPropertyChanged
    {
        private string phoneNumber;
        public string PhoneNumber
        {
            get { return phoneNumber; }
            set
            {
                if (phoneNumber != value)
                {
                    phoneNumber = value;
                    // ModelのPhoneNumberをSetします。
                    Numbers.Instance.PhoneNumber = phoneNumber;
                    OnPropertyChanged();
                }
            }
        }

        private string translatedNumber;
        public string TranslatedNumber
        {
            get { return translatedNumber; }
            set
            {
                if (translatedNumber != value)
                {
                    translatedNumber = value;
                    OnPropertyChanged();
                    // TranslateCommandのcanExecuteを再評価させます。
                    CallCommand.ChangeCanExecute();
                }
            }
        }


        private bool CanCall()
        {
            return !string.IsNullOrEmpty(Numbers.Instance.TranslatedNumber);
        }

        private bool CanShowHistory()
        {
            return Numbers.Instance.PhoneNumbers.Count > 0;
        }


        /// <summary>
        /// ViewModel constructor
        /// </summary>
        public MvvmPhonewordPageViewModel()
        {
            // Initialize.
            Numbers.Instance.PropertyChanged += Instance_PropertyChanged;

            // public Command (Action execute, Func<bool> canExecute);
            // 第2引数でbool値を受け取り、ボタンの実行可否が判断されます。
            this.CallCommand = new Command(() =>
            {
                System.Diagnostics.Debug.WriteLine("【CallCommand】");

                // ModelのDialメソッドを呼び出します。
                Numbers.Instance.Dial();
                // CallHistoryCommandの
                CallHistoryCommand.ChangeCanExecute();
            }, CanCall);


            this.CallHistoryCommand = new Command(() =>
            {
                System.Diagnostics.Debug.WriteLine("【CallHistoryCommand】");

                // MessagingCenterを使用してViewModelからのページ遷移を行う。
                MessagingCenter.Send(this, "ShowCallHistoryPage");
            }, CanShowHistory);

        }

        public Command TranslateCommand { get; private set; }
        public Command CallCommand { get; private set; }
        public Command CallHistoryCommand { get; private set; }


        /// <summary>
        /// Do something depending on the PropertyChanged events you catched.
        /// </summary>
        void Instance_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"【PropertyName】: {e.PropertyName}");

            switch (e.PropertyName)
            {
                case nameof(Numbers.Instance.PhoneNumber):
                    this.PhoneNumber = Numbers.Instance.PhoneNumber;
                    break;
                case nameof(Numbers.Instance.TranslatedNumber):
                    this.TranslatedNumber = Numbers.Instance.TranslatedNumber;
                    break;
                default:
                    break;
            }
        }




        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

