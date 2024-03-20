using StyleAndValidation.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StyleAndValidation.ViewModels
{
    #region Task 2a הסבר לפתרון

    /*Task 2a:
     * אנחנו נכתוב את הקוד בפעולה המחוברת לכפתור ההתחברות
     * LOGINCOMMAND
     * המוגדרת בפעולה הבונה:
     * השלבים להוספת מסך טעינה:
     * 1. נזמן את הקריאה למסך
     * 2. נבצע את פעולת הלוגין
     * 3. בסיום הפעולה נסגור את מסך הטעינה
     * 
     */
    #endregion
    #region Task 2b הסבר לפתרון

    /*Task 2b:
     * אנחנו נכתוב את הקוד בפעולה המחוברת לכפתור ההתחברות
     * LOGINCOMMAND
     * המוגדרת בפעולה הבונה:
     * השלבים :
     * 1. נוסיף פרמטר נוסף שהינה הפעולה שמגדירה מתי ניתן הפעולה תהיה זמינה
     * 2. נכתוב את הפעולה
     * 3. בכל הנקודות (כלומר בעדכון שם משתמש וססמה) נקרא לבדיקה חוזרת של התנאי לזמינות הפעולה
     * 
     */
    #endregion
    public class LoginPageViewModel:ViewModelBase
    {
       readonly AppServices appServices;
        #region Fields

        string username;
        string password;
        bool showPassword;
        #endregion

        #region Properties
        public string Password
        { 
            get => password; 
            set 
            {
                password = value;
                OnPropertyChanged();
                //Task 2b
                ValidateCommands();
            } 
        }
        public bool ShowPassword { get => showPassword; set{ showPassword = value; OnPropertyChanged();  } }
        public string Username
        {
            get => username;
            set
            {
                if (username != value)
                {
                    username = value;

                    OnPropertyChanged();
                    //בדיקה האם הכפתור צריך להיות מנוטרל או פעיל
                    //Task 2b
                    ValidateCommands();
                    
                }
            }
        }



        #endregion

        #region Commands
        public ICommand LoginCommand { get; protected set; }
        public ICommand RegisterCommand { get; protected set; }
        public ICommand ForgotPasswordCommand { get; protected set; }

        public ICommand ShowPasswordCommand { get; protected set; }

        #endregion

        public LoginPageViewModel(AppServices service)
        {
            appServices = service;

            LoginCommand = new Command(async () =>
            {
                //הקפצת מסך הטעינה
                await AppShell.Current.GoToAsync("Loading");
               //נעדכן את ההודעה המוצגת למשתמש
                var loading = AppShell.Current.Navigation.ModalStack.Last().BindingContext as LoadingPageViewModel;
                if (loading != null) { loading.Message = "Please wait while we are trying to connect you to the app"; }
                bool success = await appServices.Login(Username, Password);
                //נסגור את מסך הטעינה
                await AppShell.Current.Navigation.PopAsync();
                if (success)
                {
                    //אם יש עוד חלונות שנמצעים מעל חלונות האחרים- נרוקן אותם
                    if (AppShell.Current.Navigation.ModalStack.Count > 0) { AppShell.Current.Navigation.PopToRootAsync(); }

                    await AppShell.Current.GoToAsync("///MyPage");
                }
                else
                   await AppShell.Current.DisplayAlert("Log in failed", "The username/password are incorrect", "Ok");

            }, ValidateLogin
            ) ;
            RegisterCommand = new Command(async () => 
            {
                if(AppShell.Current.Navigation.ModalStack.Count>0)
                    await AppShell.Current.Navigation.PopToRootAsync();
                await AppShell.Current.GoToAsync("Register");
            });
            ForgotPasswordCommand = new Command( () => { });
            ShowPasswordCommand = new Command(() => ShowPassword = !ShowPassword);
            ShowPassword = true;
        }


        //Task 2b
        /// <summary>
        /// מתי פעולת הLOGIN אפשרית
        /// </summary>
        /// <returns></returns>
        private bool ValidateLogin()
        {
            return !(string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password));
        }
        private void ValidateCommands()
        {
            var cmd = LoginCommand as Command;
            cmd.ChangeCanExecute();
        }
    }
}
