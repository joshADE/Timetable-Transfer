using mshtml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace TTTest
{
    /// <summary>
    ///  This class represents a single is used for getting the html content from a webpage without the markup.
    /// </summary>
    public class HTMLParser
    {
        
        public static string RetrieveContenet(string url)
        {
            string urlAddress = url;

            if (File.Exists(url))
            {
                using (StreamReader reader = new StreamReader(url))
                {
                    return reader.ReadToEnd();
                }
            }

            HttpWebRequest request;
            HttpWebResponse response;
           
           
                request = (HttpWebRequest)WebRequest.Create(urlAddress);
                 
                response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream receiveStream = response.GetResponseStream();
                    StreamReader readStream = null;

                    if (String.IsNullOrWhiteSpace(response.CharacterSet))
                        readStream = new StreamReader(receiveStream);
                    else
                        readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));

                    string data = readStream.ReadToEnd();

                    response.Close();
                    readStream.Close();
                    return data;
                }
            
            
                

            return "";

        }

        public static string RetrieveInnerTextContent(string htmlContent) 
        {

            object[] oPageText = { htmlContent };
            HTMLDocument doc = new HTMLDocumentClass();
            IHTMLDocument2 document = (IHTMLDocument2)doc;
            document.write(oPageText);
            document.close();
            //Console.WriteLine(document.body.innerHTML);  // whole content of body
            Console.WriteLine(document.body.innerText);  // all plain text in body
            return document.body.innerText;
        }

    }
}
