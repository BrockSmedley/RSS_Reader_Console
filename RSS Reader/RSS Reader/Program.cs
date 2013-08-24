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

namespace RSS_Reader
{
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

        static void Main(string[] args)
        {
            readRSS();
        }

        static void readRSS()
        {
            Console.WriteLine("Enter an RSS URL to be read.");

            url = Console.ReadLine();
            if (!url.EndsWith(".rss"))
            {
                Console.WriteLine("\nNot a valid URL\n\n");
                readRSS();
            }
            else
            {
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
                        //come back to this
                        story thisStory = new story();
                        thisStory.title = nodeItem["title"].InnerText;
                        thisStory.link = nodeItem["link"].InnerText;
                        thisStory.description = nodeItem["description"].InnerText.Split(new string[] { "&lt", "<div", "FULL STORY" }, StringSplitOptions.None)[0];

                        stories.Add(thisStory);
                    }
                }

                //URL Title
                Console.WriteLine(title + "\n" + language + "\n" + link + "\n" + description + "\n\n\n");

                foreach (story s in stories)
                {
                    Console.WriteLine("Title: " + s.title + "\nLink: " + s.link + "\n" + "Description: " + s.description + "\n");
                }

                Console.Read();
            }
        }
    }
}
