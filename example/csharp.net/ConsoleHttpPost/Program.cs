﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;

namespace ConsoleHttpPost {
    class Program {
            static void Main(string[] args) {
                byte[] xmlData = System.Text.Encoding.UTF8.GetBytes(File.ReadAllText("E:\\VA\\VLERDoc-UTF8.xml", Encoding.UTF8));
                //string strURL = "http://das.dynalias.org:8080/ecrud/v1/core/electronicCaseFiles/transform";
                //string strURL = "http://localhost:3001/ecrud/v1/core/electronicCaseFiles/transform";
                //for HTTPS
                //string strURL = "https://das.dynalias.org/ecrud/v1/core/electronicCaseFiles/transform";
                try {
	                HttpWebRequest POSTRequest = (HttpWebRequest)WebRequest.Create(strURL);
                    //For HTTPS two way cert, must be imported to "Trusted Root Certificate Authorities", Location: IE > Tools > Internet Options > Content > Certificates
                    //X509Certificate Cert = new X509Certificate(@"f:\downloads\das.dynalias.org.p12", "test123");
                    //X509Certificate Cert = new X509Certificate(@"F:\Downloads\ides-cftdeom.asmr.com.cer", "test123");
                    //POSTRequest.ClientCertificates.Add(Cert);
                    //End HTTPS
	                POSTRequest.Method = "POST";
	                POSTRequest.KeepAlive = false;
	                POSTRequest.Timeout = 5000;
	                //Content length of message body
	                POSTRequest.Accept = "application/xml";

                    var boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x", NumberFormatInfo.InvariantInfo);
                    POSTRequest.ContentType = "multipart/form-data; boundary=" + boundary;
                    boundary = "--" + boundary;

                    using (var requestStream = POSTRequest.GetRequestStream()) {
                        var buffer = Encoding.ASCII.GetBytes(boundary + Environment.NewLine);
                        requestStream.Write(buffer, 0, buffer.Length);
                        string fileName = "some_fileName.xml";
                        buffer = Encoding.UTF8.GetBytes(string.Format("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"{2}", "file", fileName, Environment.NewLine));
                        requestStream.Write(buffer, 0, buffer.Length);
                        buffer = Encoding.ASCII.GetBytes(string.Format("Content-Type: {0}{1}{1}", "application/xml", Environment.NewLine));
                        requestStream.Write(buffer, 0, buffer.Length);

                        requestStream.Write(xmlData, 0, xmlData.Length);
                    
                        buffer = Encoding.ASCII.GetBytes(Environment.NewLine);
                        requestStream.Write(buffer, 0, buffer.Length);

                        var boundaryBuffer = Encoding.ASCII.GetBytes(boundary + "--");
                        requestStream.Write(boundaryBuffer, 0, boundaryBuffer.Length);
                    }

                    HttpWebResponse POSTResponse = (HttpWebResponse)POSTRequest.GetResponse();
                    StreamReader reader =
                    new StreamReader(POSTResponse.GetResponseStream(), Encoding.UTF8);
                    string daLocation = POSTResponse.GetResponseHeader("Location");
                    HttpStatusCode daStatusCode = POSTResponse.StatusCode;

                    string ResponseFromPost = reader.ReadToEnd().ToString();
                    Console.WriteLine("StatusCode: " + daStatusCode);
                    System.Diagnostics.Debug.WriteLine("StatusCode: " + daStatusCode);

                    if (daStatusCode == HttpStatusCode.Accepted ||
                        daStatusCode == HttpStatusCode.Created ||
                        daStatusCode == HttpStatusCode.OK) {
                        Console.Write(ResponseFromPost);
                        System.Diagnostics.Debug.WriteLine(ResponseFromPost);
                    }
                } catch (WebException we) {
                    System.Diagnostics.Debug.WriteLine(new StreamReader(we.Response.GetResponseStream(), Encoding.UTF8).ReadToEnd().ToString());
                }
                Console.In.Read();
        }
    }
}
