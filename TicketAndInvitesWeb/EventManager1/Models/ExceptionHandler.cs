using EventManager1.DBCon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web;

namespace EventManager1.Models
{
    public class ExceptionHandler
    {
        /// <summary>
        /// Add log Exception
        /// </summary>
        /// <param name="ex"></param>
        public static void LogException(Exception ex)
        {
            string FileName = string.Empty, MethodName = string.Empty, ErrText = string.Empty, ParentMethod = string.Empty, ParentMethodPath = string.Empty;
            //BusinessAccessLayer BL = new BusinessAccessLayer();

            try
            {
                StackTrace _stackTrace = new StackTrace();
                StackFrame _stackFrame = _stackTrace.GetFrame(2);
                MethodBase _methodBase = _stackFrame.GetMethod();
                ParentMethod = _methodBase.Name;
                if (_methodBase.ReflectedType != null)
                {
                    ParentMethodPath = _methodBase.ReflectedType.FullName;
                }

                StackTrace stackTrace = new StackTrace(ex, true);
                MethodBase methodBase;
                FileName = stackTrace.GetFrame((stackTrace.FrameCount - 1)).GetFileName();
                Int32 lineNumber = stackTrace.GetFrame((stackTrace.FrameCount - 1)).GetFileLineNumber();
                FileName = FileName + ":Line No. " + lineNumber.ToString();
                methodBase = stackTrace.GetFrame((stackTrace.FrameCount - 1)).GetMethod();
                //These two lines are respnsible to find out name of the method                
                MethodName = methodBase.Name;
                ErrText = ex.Message;
            }
            catch (Exception exIN)
            {
                ErrText = exIN.Message;
            }
            finally
            {
                // ErrorLog errorLog = new ErrorLog();
                Hashtable inparam = new Hashtable();
                Hashtable outParam = new Hashtable();
                //errorLog.ErrText = ErrText;
                //errorLog.ErrTime = System.DateTime.UtcNow;
                //errorLog.FilePath = FileName;
                //errorLog.Method = MethodName;
                //errorLog.ParentMethod = ParentMethod;
                //errorLog.ParentFilePath = ParentMethodPath;
                inparam.Add("@InErrText", ErrText);
                inparam.Add("@InErrTime", System.DateTime.UtcNow.ToString());
                inparam.Add("@InFilePath", FileName);
                inparam.Add("@InMethod", MethodName);
                inparam.Add("@InParentMethod", ParentMethod);
                inparam.Add("@InParentFilePath", ParentMethodPath);

                using (EventmanagerEntities dbconn = new EventmanagerEntities())
                {
                    try
                    {
                        int ReturnValue = dbconn.ErrorLog_Add(ErrText, System.DateTime.UtcNow, FileName, MethodName, ParentMethod, ParentMethodPath, null, null, null, null);
                    }
                    catch (Exception k)
                    {
                        Log4Net.Debug("Error in LogException Store procedure" + ex.StackTrace);
                    }
                    
                }

            }
        }


        public static void LogException(Exception ex, string ParentMethod, string ParentMethodPath)
        {
            string FileName = string.Empty, MethodName = string.Empty, ErrText = string.Empty;
            //BusinessAccessLayer BL = new BusinessAccessLayer();

            try
            {
                StackTrace stackTrace = new StackTrace(ex, true);
                MethodBase methodBase;
                FileName = stackTrace.GetFrame((stackTrace.FrameCount - 1)).GetFileName();
                Int32 lineNumber = stackTrace.GetFrame((stackTrace.FrameCount - 1)).GetFileLineNumber();
                FileName = FileName + ":Line No. " + lineNumber.ToString();
                methodBase = stackTrace.GetFrame((stackTrace.FrameCount - 1)).GetMethod();
                //These two lines are respnsible to find out name of the method                
                MethodName = methodBase.Name;
                ErrText = ex.Message;
            }
            catch (Exception exIN)
            {
                ErrText = exIN.Message;
            }
            finally
            {
                //ErrorLog errorLog = new ErrorLog();
                Hashtable inparam = new Hashtable();
                Hashtable outParam = new Hashtable();
                //errorLog.ErrText = ErrText;
                //errorLog.ErrTime = System.DateTime.UtcNow;
                //errorLog.FilePath = FileName;
                //errorLog.Method = MethodName;
                //errorLog.ParentMethod = ParentMethod;
                //errorLog.ParentFilePath = ParentMethodPath;
                inparam.Add("@InErrText", ErrText);
                inparam.Add("@InErrTime", System.DateTime.UtcNow.ToString());
                inparam.Add("@InFilePath", FileName);
                inparam.Add("@InMethod", MethodName);
                inparam.Add("@InParentMethod", ParentMethod);
                inparam.Add("@InParentFilePath", ParentMethodPath);
                using (EventmanagerEntities dbconn = new EventmanagerEntities())
                {
                    int ReturnValue = dbconn.ErrorLog_Add(ErrText, System.DateTime.UtcNow, FileName, MethodName, ParentMethod, ParentMethodPath, null, null, null, null);
                }

            }
        }

        public static void LogRespRequest(Exception ex, string RequestString = null, string ResponseString = null, string user_Id = null, string Company_Id = null)
        {
            string FileName = string.Empty, MethodName = string.Empty, ErrText = string.Empty, ParentMethod = string.Empty, ParentMethodPath = string.Empty;
            //BusinessAccessLayer BL = new BusinessAccessLayer();

            if(ManageSession.UserSession!=null)
            {
                user_Id = ManageSession.UserSession.Id.ToString();
            }
            else if(ManageSession.CompanySession!=null)
            {
                Company_Id = ManageSession.CompanySession.Id.ToString();
            }

            try
            {
                if (ex != null)
                {
                    StackTrace _stackTrace = new StackTrace();
                    StackFrame _stackFrame = _stackTrace.GetFrame(2);
                    MethodBase _methodBase = _stackFrame.GetMethod();
                    ParentMethod = _methodBase.Name;
                    if (_methodBase.ReflectedType != null)
                    {
                        ParentMethodPath = _methodBase.ReflectedType.FullName;
                    }

                    StackTrace stackTrace = new StackTrace(ex, true);
                    MethodBase methodBase;
                    FileName = stackTrace.GetFrame((stackTrace.FrameCount - 1)).GetFileName();
                    Int32 lineNumber = stackTrace.GetFrame((stackTrace.FrameCount - 1)).GetFileLineNumber();
                    FileName = FileName + ":Line No. " + lineNumber.ToString();
                    methodBase = stackTrace.GetFrame((stackTrace.FrameCount - 1)).GetMethod();
                    //These two lines are respnsible to find out name of the method                
                    MethodName = methodBase.Name;
                    ErrText = ex.Message;
                }
                else
                {
                    ErrText = "";
                }
            }
            catch (Exception exIN)
            {
                ErrText = exIN.Message;
            }
            finally
            {
                using (EventmanagerEntities dbconn = new EventmanagerEntities())
                {
                    int ReturnValue = dbconn.ErrorLog_Add(ErrText, System.DateTime.UtcNow, FileName, MethodName, ParentMethod, ParentMethodPath, RequestString, ResponseString, user_Id, Company_Id);
                }

            }
        }
    }
}
