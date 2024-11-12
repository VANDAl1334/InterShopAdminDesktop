using System;
using ReactiveUI;
using InterShopAdminDesktop.Models;

namespace InterShopAdminDesktop.ViewModels
{

    public class UserViewModel : ReactiveObject
    {
        private User user;

        public UserViewModel(User user) => this.user = user;
        
        public string Login
        {
            get { return user.Login; }
            set
            {
                user.Login = value;
                this.RaisePropertyChanged("Login");
            }
        }
        public string Password
        {
            get { return user.Password; }
            set
            {
                user.Password = value;
                this.RaisePropertyChanged("Password");
            }
        }

        public UserViewModel()
        {
            
        }
    }
}