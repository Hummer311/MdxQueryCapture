using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace QueryCapture
{
    public class RequestParameter
    {
        public RequestParameter(XmlNode requestParameters)
        {
            foreach (XmlNode n in requestParameters.ChildNodes)
            {
                switch (n.Name)
                {
                    case "Name":
                        mParameterName = n.InnerText;
                        break;
                    case "Value":
                        mValue = n.InnerText;
                        break;
                }
            }
        }

        private string mParameterName;

        public string ParameterName
        {
          get { return mParameterName; }
          //set { mParameterName = value; }
        }
        private string mValue;

        public string Value
        {
          get { return "\"" + mValue + "\""; }
          //set { mValue = value; }
        }

        
    }
}
