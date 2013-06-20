using System.ComponentModel;
using System.Diagnostics;

namespace Next.Dtos
{
    public class LoggedInStatus
    {
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool logged_in { get; set; }

        public bool IsLoggedIn { get { return logged_in; } }
    }
}