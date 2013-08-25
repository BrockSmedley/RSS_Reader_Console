//RSS Reader
//Brock Smedley
//Strongly based off a tutorial by Andre Pociu
//Tutorial at http://www.geekpedia.com/tutorial147_Creating-an-RSS-feed-reader-in-Csharp.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace RSS_Reader
{
    class Clipboard
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetClipboardData(uint uFormat);
        [DllImport("user32.dll")]
        static extern bool IsClipboardFormatAvailable(uint format);
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool OpenClipboard(IntPtr hWndNewOwner);
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool CloseClipboard();
        [DllImport("kernel32.dll")]
        static extern IntPtr GlobalLock(IntPtr hMem);
        [DllImport("kernel32.dll")]
        static extern bool GlobalUnlock(IntPtr hMem);

        const uint CF_UNICODETEXT = 13;

        public static string GetText()
        {
            if (!IsClipboardFormatAvailable(CF_UNICODETEXT))
                return null;
            if (!OpenClipboard(IntPtr.Zero))
                return null;

            string data = null;
            var hGlobal = GetClipboardData(CF_UNICODETEXT);
            if (hGlobal != IntPtr.Zero)
            {
                var lpwcstr = GlobalLock(hGlobal);
                if (lpwcstr != IntPtr.Zero)
                {
                    data = Marshal.PtrToStringUni(lpwcstr);
                    GlobalUnlock(lpwcstr);
                }
            }
            CloseClipboard();

            return data;
        }
    }

    class story
    {
        public string title, link, description;
    }

    class Program
    {
        static XmlTextReader rssReader;
        static XmlDocument rssDoc;
        static XmlNode nodeRss;
        static XmlNode nodeChannel;
        static XmlNode nodeItem;
        static string url = "http://rss.cnn.com/rss/cnn_topstories.rss";
        static List<story> stories = new List<story>();
        static int consoleBuffer = Console.BufferHeight+150;

        static void Main(string[] args)
        {
            readRSS();
        }

        static void readRSS()
        {
            ///sorry.
            Console.WriteLine("Enter an RSS URL to be read. Enter 'P' to paste from clipboard.");

            url = Console.ReadLine();
            Debug.WriteLine("url: " + url);

            int n = 1;
            switch (n)
            {
                case 3://shit works
                    //create new XMLTextReader from specified URL (RSS FEED)
                    rssReader = new XmlTextReader(url);
                    rssDoc = new XmlDocument();

                    //load the XML content in to an XMLDocument
                    try
                    {
                        rssDoc.Load(rssReader);
                    }
                    catch
                    {
                        Console.WriteLine("\nNot a valid RSS page\n");
                        readRSS();
                    }

                    //loop for the <rss> tag
                    for (int i = 0; i < rssDoc.ChildNodes.Count; i++)
                    {
                        //if it's the rss tag
                        if (rssDoc.ChildNodes[i].Name == "rss")
                        {
                            //rss tag found
                            nodeRss = rssDoc.ChildNodes[i];
                        }
                    }

                    //loop for the channel tag
                    for (int i = 0; i < nodeRss.ChildNodes.Count; i++)
                    {
                        if (nodeRss.ChildNodes[i].Name == "channel")
                        {
                            //channel tag found
                            nodeChannel = nodeRss.ChildNodes[i];
                        }
                    }

                    //set the labels with information from inside the nodes
                    string title = "Title: " + nodeChannel["title"].InnerText;
                    string language = "Language: " + nodeChannel["language"].InnerText;
                    string link = "Link: " + nodeChannel["link"].InnerText;
                    string description = "Description: " + nodeChannel["description"].InnerText;

                    //loop for the title, link, description, and all the other tags
                    for (int i = 0; i < nodeChannel.ChildNodes.Count; i++)
                    {
                        if (nodeChannel.ChildNodes[i].Name == "item")
                        {
                            nodeItem = nodeChannel.ChildNodes[i];
                            story thisStory = new story();
                            thisStory.title = nodeItem["title"].InnerText;
                            thisStory.link = nodeItem["link"].InnerText;
                            thisStory.description = nodeItem["description"].InnerText.Split(new string[] { "&lt", "<div", "FULL STORY" }, StringSplitOptions.None)[0];

                            stories.Add(thisStory);
                        }
                    }

                    Console.Clear();
                    //URL Title
                    Console.WriteLine(title + "\n" + language + "\n" + link + "\n" + description + "\n\n\n");

                    foreach (story s in stories)
                    {
                        Console.SetBufferSize(Console.WindowWidth, consoleBuffer + 5);
                        Console.WriteLine("Title: " + s.title + "\nLink: " + s.link + "\n" + "Description: " + s.description + "\n");
                    }

                    Console.Read();
                    break;

                case 2://url is copied from clipboard
                    //get text from clipboard
                    url = Clipboard.GetText();
                    Console.WriteLine("Is this URL correct? (y/n): " + url);
                    string correct = Console.ReadLine();
                    if (correct.ToLower() != "y" || !correct.ToLower().Contains('y'))
                        readRSS();
                    goto case 3;

                case 1://url is typed
                    if (url.EndsWith(".rss") && url.ToString().StartsWith("http"))
                    {
                        goto case 3;
                    }
                    else if (url.ToUpper() == "P")
                        goto case 2;
                    
                    break;

                default:
                    Console.WriteLine("Invalid URL");
                    break;

            }
        }
    }
}
