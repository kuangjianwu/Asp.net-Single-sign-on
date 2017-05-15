# Asp.net-Single-sign-
在登录的时候调用一下LoginRegister方法

Global.asax中：

protected void Session_End(object sender, EventArgs e)
        {
            ltGameStore.Common.SSOHelper.GlobalSessionEnd();
        }
剩下的就是在每次客户端对服务器有请求的时候验证当前会话ID是否被注销掉了（被其他用户挤掉）

我用的是一个继承Controller的基类，重写里面的OnAuhorization方法：

复制代码
/// <summary>
        /// 在进行授权时调用
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            //不能应用在子方法上
            if (filterContext.IsChildAction)
                return;

            //如果没有登录，则跳转到登录视图
            if (WorkContext.Action != "login")
            {
                if (!TicketHelper.IsLogin())
                {
                    filterContext.Result = new RedirectResult("/Account/login");
                }
            }

            if (!SSOHelper.CheckOnline())
            {
                filterContext.Result = PromptView("您的账号已在别处登录");
            }


            //如果当前用户不是管理员
            //if (WorkContext.AdminGid == 1)
            //{
            //    if (WorkContext.IsHttpAjax)
            //        filterContext.Result = new ContentResult { Content = "404" };
            //    else
            //        filterContext.Result = new RedirectResult("/");
            //    return;
            //}
        }
复制代码
注意，这样写的话会有个问题，每次客户端请求的SessionID都不一样，这样就无法校验了，搜了一下解决方法，在重写的Initialize方法(继承Controller的基类中)中不断的注册SessionId：

复制代码
/// <summary>
        /// 初始化调用构造函数后可能不可用的数据
        /// </summary>
        /// <param name="requestContext"></param>
        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            Session["SessionId"] = Session.SessionID;
        }
