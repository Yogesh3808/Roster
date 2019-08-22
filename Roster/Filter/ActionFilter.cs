using RosterSystem.Repository;
using System.Web.Mvc;

namespace Roster.Filter
{
    public class ActionFilter : FilterAttribute, IExceptionFilter
    {
        public GenericUnitOfWork UnitOfWork = new GenericUnitOfWork();
        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext.Exception != null)
            {
                //ExceptionLogging logger = new ExceptionLogging()
                //{
                //    ExceptionMessage = filterContext.Exception.Message,
                //    ExceptionStackTrace = filterContext.Exception.StackTrace,
                //    ControllerName = filterContext.RouteData.Values["controller"].ToString(),
                //    ActionName = filterContext.RouteData.Values["action"].ToString(),
                //    UserIP = filterContext.HttpContext.Request.UserHostAddress,
                //    UserId = Convert.ToInt32(HttpContext.Current.Session["UserId"]),
                //    LogTime = DateTime.Now
                //};
                //UnitOfWork.GetRepositoryInstance<ExceptionLogging>().Add(logger);
                //filterContext.ExceptionHandled = true;
                //AuditLogFile.WriteAuditLog("ErrorMsg=" + filterContext.Exception.Message + " ControlerName=" + filterContext.RouteData.Values["controller"].ToString() + " .:" + DateTime.Now);

                //filterContext.HttpContext.Response.Clear();
                //filterContext.HttpContext.Response.Redirect("~/HttpErrorPages/HTTP400.html");

                //Redirect or return a view, but not both.
                filterContext.Result = new ViewResult()
                {
                    ViewName = "InnerErrorView",
                };

            }
        }
    }
}