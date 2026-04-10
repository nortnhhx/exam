using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;
using University.Models;

namespace University
{
    class SessionManager
    {
        private static SessionManager instance;
        public static SessionManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new SessionManager();
                return instance;
            }
        }

        public Authorization CurrentUser { get; private set; }
        public bool IsGuest { get; private set; }

        public void StartSession(Authorization user)
        {
            CurrentUser = user;
            IsGuest = false;
        }

        public void StartGuestSession()
        {
            CurrentUser = null;
            IsGuest = true;
        }

        public void EndSession()
        {
            CurrentUser = null;
            IsGuest = false;
        }

        public bool IsAuthenticated => CurrentUser != null || IsGuest;
        public bool CanEdit => CurrentUser != null;
    }
}
