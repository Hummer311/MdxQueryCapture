using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AnalysisServices;
using System.IO;
using System.Xml;

namespace QueryCapture
{
    public delegate void QueryCapturedDelegate();
    

    public class QueryTraceProcessor
    {
        public event QueryCapturedDelegate QueryCaptured;

        const string COMMENT_PREFIX = "// ";
        Server amoServer = null;
        string traceFileName = "";
        Trace queryTrc;
        FileStream traceFs;
        StreamWriter sw;

        public QueryTraceProcessor()
        {
        }

        public void Start(string serverName, string traceFile)
        {
            amoServer = new Server();
            amoServer.Connect(serverName);
            traceFileName = traceFile;

            queryTrc = amoServer.Traces.Add();
            TraceEvent qryEndEvnt = queryTrc.Events.Add(TraceEventClass.QueryBegin);
            qryEndEvnt.Columns.Add(TraceColumn.DatabaseName);
            //qryEndEvnt.Columns.Add(TraceColumn.Duration);
            qryEndEvnt.Columns.Add(TraceColumn.NTCanonicalUserName);
            qryEndEvnt.Columns.Add(TraceColumn.RequestParameters);
            //qryEndEvnt.Columns.Add(TraceColumn.RequestProperties);
            qryEndEvnt.Columns.Add(TraceColumn.StartTime);
            qryEndEvnt.Columns.Add(TraceColumn.TextData);
            queryTrc.AutoRestart = false;
            queryTrc.Description = "QueryCapture tool trace";
            //queryTrc.ID = "QueryTraceTool";
            //queryTrc.Name = "QueryTraceTool";
            //amoServer.Traces.Add(queryTrc);
            queryTrc.Update();
            queryTrc.OnEvent += new TraceEventHandler(queryTrc_OnEvent);
            queryTrc.Start();
// open stream
            traceFs = new FileStream(traceFileName, FileMode.Create);
            sw = new StreamWriter(traceFs);
            
        }

        void queryTrc_OnEvent(object sender, TraceEventArgs e)
        {
            sw.WriteLine("/*");
            sw.WriteLine("Database     : " + e.DatabaseName);
            sw.WriteLine("User Name    : " + e.NTCanonicalUserName);
            //sw.WriteLine("Parameters   : " + e.RequestParameters);
            //sw.WriteLine("Properties   : " + e.RequestProperties);
            sw.WriteLine("Start Time   : " + e.StartTime.ToString("dd MMM yyyy hh:mm:ss"));
            sw.WriteLine("*/");
            sw.WriteLine("");
            sw.WriteLine(ReplaceQueryParameters( e.TextData,e.RequestParameters) + ";" );
            sw.WriteLine("GO");
            sw.WriteLine("");
            QueryCaptured();
        }

        public void Stop()
        {
            queryTrc.Stop();
            amoServer.Traces.Remove(queryTrc);
            queryTrc = null;
            //close stream
            sw.Close();
            traceFs.Close();
        }

        private string ReplaceQueryParameters(string query, string parameters)
        {

            if (parameters == null || parameters.Length == 0)
            {
                return query;
            }

            
            StringBuilder sbQry = new StringBuilder(query);
            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(parameters);
            XmlNodeList nodeList = xDoc.GetElementsByTagName("Parameter");
            RequestParameter param;
            int iCnt = 0;
            foreach (XmlNode n in nodeList)
            {
                param = new RequestParameter(n);
                sbQry.Replace("@" + param.ParameterName, param.Value);
            }
            return sbQry.ToString();
        }
    }
}
