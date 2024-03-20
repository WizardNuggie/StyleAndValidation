using StyleAndValidation.Models;
using StyleAndValidation.Services;
using StyleAndValidation.Views;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StyleAndValidation.ViewModels
{
    public class RegisterPageViewModel:ViewModelBase
    {
        #region Service
       readonly AppServices appServices;
        #endregion

        #region Fields
        
        string username;
        string password;
        string fullName;
        string email;
        DateTime birthDate;
        #region validation messages
        bool showUserNameError;
        string userNameErrorMessage;
        //Task 3a
        bool showPasswordError;
        string passwordErrorMessage;
        #endregion
        #endregion

        #region Properties
        public string Username
        { get => username;
            set { if (username != value)
                    { 
                    username = value;
                    ValidateUserName();
                    OnPropertyChanged();
                    
                }
            }
        }


        public string Password { get=>password; set { password = value; OnPropertyChanged(); ValidatePassword(); } }
        public string FullName { get=>fullName; set { fullName = value; OnPropertyChanged(); } }
        public string Email { get=>email; set { email = value; OnPropertyChanged(); } }

        public DateTime BirthDate { get => birthDate; set { birthDate = value; OnPropertyChanged(); } }

        #region Validation Properties
        public bool ShowUserNameError { get=>showUserNameError;  set { showUserNameError = value; OnPropertyChanged(); } }
        public string UserNameErrorMEssage { get => userNameErrorMessage; set { userNameErrorMessage = value; OnPropertyChanged(); } }
        //Task 3a
        public bool ShowPasswordError { get=>showPasswordError; set { showPasswordError = value; OnPropertyChanged(); } }
        public string PasswordErrorMessage { get=>passwordErrorMessage;  set { passwordErrorMessage = value; OnPropertyChanged(); } }
        //Task 3c
        public DateTime ValidMaximumDate { get => DateTime.Today.AddYears(-13); }
        #endregion
        
    #endregion

        
        #region Commands
        public ICommand RegisterCommand { get; protected set; }

        #endregion
        public RegisterPageViewModel(AppServices service)
        {
            appServices = service;
            RegisterCommand = new Command(async () => await RegisterUser(),()=>ValidateAll()) ;
            Username = string.Empty;
            Password = string.Empty;
            //Task 3c
            BirthDate=new DateTime(2000,1,1);   

        }


        private async Task RegisterUser()
        {
           User registered=new () { BirthDate=BirthDate, Email=Email, FullName=FullName, Password=Password, Username=Username};
            #region מסך טעינה
            await AppShell.Current.GoToAsync("Loading");

            /*אם נרצה לעדכן את ההודעות שמוצגות במסך הפופאפ
            */

            var loading = Shell.Current.CurrentPage.Navigation.ModalStack.Last().BindingContext as LoadingPageViewModel;
            if (loading != null)
                loading.Message = "המתן בזמן שאנו רושמים אותך לאפליקציה...";
            #endregion
            bool ok = await appServices.RegisterUserAsync(registered);
            loading.Message = "Almost finished";
            await Task.Delay(3000);
            #region סגירת מסך טעינה
            await AppShell.Current.Navigation.PopModalAsync();
            //   await loading.Close();
            #endregion
            if (ok)
            {
                await AppShell.Current.DisplayAlert("Success", "You are being redirected to the log in page", "Ok");
                await AppShell.Current.Navigation.PopToRootAsync();
                await AppShell.Current.GoToAsync("Login");
            }
           else
            {
              
                await AppShell.Current.DisplayAlert("Oh no", "Something bad happened", "Ok");
            }
          
        }

        #region Validation

        //Task 3a
        private bool ValidatePassword()
        {
            #region שימוש בREGEX
            string pattern = @"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{4,16}$";

            if (Regex.IsMatch(Password, pattern))
            {
                ShowPasswordError = false;
                ValidateCommand();
                return true;
            }
            ShowPasswordError = true;
            PasswordErrorMessage = "A valid password must be 4-16 characters, and must contain at least 1 capital letter, 1 number and 1 special character (like '@'/'#' etc.)";
            ValidateCommand();
            return false;   
            #endregion
        }
        private void ValidateCommand()
        {
            //בדיקה האם הכפתור צריך להיות מנוטרל או פעיל
            var cmd = RegisterCommand as Command;
            cmd.ChangeCanExecute();
        }
        private bool ValidateUserName()
        {
            #region שימוש בREGEX
            /*string pattern = @"^[a-zA-Z](?=.*[0-9])(?=.*[a-z])[a-zA-Z0-9]{3,7}$";

            bool ok = Regex.IsMatch(Username, pattern);*/
            #endregion
            bool ok =(!string.IsNullOrEmpty(Username)) && (Username.Length > 3);
            switch (ok)
            {
                case false:
                //הצג הודעת שגיאה
                ShowUserNameError = true;
                UserNameErrorMEssage = "Username invalid";
                    break;
                case true:
                    //בטל הודעת שגיאה
                ShowUserNameError = false;
                UserNameErrorMEssage = string.Empty;
                    break;

            }

            ValidateCommand();
            return ok;
        }

        

        private bool ValidateAll()
        {
            return !ShowUserNameError&&!ShowPasswordError ;
        }

        #endregion
    }
}
