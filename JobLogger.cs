using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class JobLogger
{
    /// <summary>
    /// Used to log messages
    /// </summary>
    
    public String LogMessage(string message, bool isMessage, bool isWarning, bool isError, bool logToFile, bool logToConsole, bool logToDatabase) 
    {
        try
        {

            String ReturnValue = String.Empty;
            String LogFilepath = System.Configuration.ConfigurationManager.AppSettings["LogFileDirectory"] + "LogFile" + DateTime.Now.ToShortDateString().Replace("/","-") + ".txt";
            String LogFileContent = String.Empty;
            String sParam = String.Empty;
            int msgType  = 0;

            message.Trim();

            if (message == null)
                ReturnValue = "Invalid parameter [message]";
            else
            {
                if (message.Length == 0)
                    ReturnValue = "Invalid parameter [message]";
                else 
                {
                    if (!logToConsole && !logToFile && !logToDatabase)
                        ReturnValue = "Invalid configuration";
                    else
                    {
                        if (!isMessage && !isError && !isWarning)
                            ReturnValue = "Error or Warning or Message must be specified";
                    }
                }
            }

            if (String.IsNullOrEmpty(ReturnValue))
            {
                //Log to Data Base
                if (logToDatabase)
                {
                    System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.AppSettings["ConnectionString"]);

                    connection.Open();                    

                    if (isMessage)
                        msgType = 1;

                    if (isError)
                        msgType = 2;

                    if (isWarning)
                        msgType = 3;

                    sParam = "@Message='" + message + "',";
                    sParam += "@MessageType=" + msgType.ToString();

                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SaveLog " + sParam, connection);

                    command.ExecuteNonQuery();

                    command.Dispose();
                }

                //Log to File
                if (logToFile)
                {
                    if (System.IO.File.Exists(LogFilepath))
                        LogFileContent = System.IO.File.ReadAllText(LogFilepath);

                    LogFileContent += "\r\n";

                    if (isError)
                        LogFileContent += String.Format("ERROR: {0} {1}",DateTime.Now.ToShortDateString(),message);

                    if (isWarning)
                        LogFileContent += String.Format("WARNING: {0} {1}", DateTime.Now.ToShortDateString(), message);

                    if (isMessage)
                        LogFileContent += String.Format("MESSAGE: {0} {1}", DateTime.Now.ToShortDateString(), message);

                    System.IO.File.WriteAllText(LogFilepath, LogFileContent);
                }

                //Log to Console
                if (logToConsole)
                {
                    if (isError)
                        Console.ForegroundColor = ConsoleColor.Red;

                    if (isWarning)
                        Console.ForegroundColor = ConsoleColor.Yellow;

                    if (isMessage)
                        Console.ForegroundColor = ConsoleColor.White;

                    Console.WriteLine(String.Format("{0} {1}", DateTime.Now.ToShortDateString(), message));
                }
            }

            return ReturnValue;

        }
        catch (Exception error)
        {
            return String.Format("ERROR: {0} {1}",error.Message, error.StackTrace);
        }
    } 
}

