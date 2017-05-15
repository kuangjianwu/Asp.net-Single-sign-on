/// <summary>
    /// 单点登录帮助类
    /// </summary>
    public class SSOHelper
    {
        /// <summary>
        /// 登录后执行
        /// </summary>
        /// <param name="UserID">用户标识</param>
        public void LoginRegister(string UserID)
        {
            Hashtable hOnline = (Hashtable)System.Web.HttpContext.Current.Application["Online"];
            if (hOnline != null)
            {
                IDictionaryEnumerator idE = hOnline.GetEnumerator();
                string strKey = "";
                while (idE.MoveNext())
                {
                    if (idE.Value != null && idE.Value.ToString().Equals(UserID))
                    {
                        //already login 
                        strKey = idE.Key.ToString();
                        hOnline[strKey] = "XXXXXX";
                        break;
                    }
                }
            }
            else
            {
                hOnline = new Hashtable();
            }

            hOnline[System.Web.HttpContext.Current.Session.SessionID] = UserID;
            System.Web.HttpContext.Current.Application.Lock();
            System.Web.HttpContext.Current.Application["Online"] = hOnline;
            System.Web.HttpContext.Current.Application.UnLock();
        }

        /// <summary>
        /// 检查是否唯一登录
        /// </summary>
        /// <returns></returns>
        public static bool CheckOnline()
        {
            Hashtable hOnline = (Hashtable)System.Web.HttpContext.Current.Application["Online"];
            if (hOnline != null)
            {
                IDictionaryEnumerator idE = hOnline.GetEnumerator();
                while (idE.MoveNext())
                {
                    if (idE.Key != null && idE.Key.ToString().Equals(System.Web.HttpContext.Current.Session.SessionID))
                    {
                        //already login
                        if (idE.Value != null && "XXXXXX".Equals(idE.Value.ToString()))
                        {
                            hOnline.Remove(System.Web.HttpContext.Current.Session.SessionID);
                            System.Web.HttpContext.Current.Application.Lock();
                            System.Web.HttpContext.Current.Application["Online"] = hOnline;
                            System.Web.HttpContext.Current.Application.UnLock();
                            return false;
                        }
                        break;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Global文件的SessionEnd事件中增加此代码
        /// </summary>
        public static void GlobalSessionEnd()
        {
            Hashtable hOnline = (Hashtable)System.Web.HttpContext.Current.Application["Online"];
            if (hOnline[System.Web.HttpContext.Current.Session.SessionID] != null)
            {
                hOnline.Remove(System.Web.HttpContext.Current.Session.SessionID);
                System.Web.HttpContext.Current.Application.Lock();
                System.Web.HttpContext.Current.Application["Online"] = hOnline;
                System.Web.HttpContext.Current.Application.UnLock();
            }
        }

    }