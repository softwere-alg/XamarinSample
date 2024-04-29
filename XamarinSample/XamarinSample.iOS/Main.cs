using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

namespace XamarinSample.iOS
{
    public class Application
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            UIApplication.Main(args, null, typeof(AppDelegate));
        }

        /// <summary>
        /// リソースを取得します。
        /// </summary>
        /// <param name="name">リソース名</param>
        /// <param name="type">リソース型</param>
        /// <returns></returns>
        public static string LoadResource(string name, string type)
        {
            string path = NSBundle.MainBundle.PathForResource(name, type);
            return System.IO.File.ReadAllText(path);
        }
    }
}
