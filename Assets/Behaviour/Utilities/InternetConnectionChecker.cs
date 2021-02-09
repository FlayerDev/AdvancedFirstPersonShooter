using System.Net;
using System.IO;
public class InternetConnectionChecker
{
    public string Endpoint = "http://google.com";

    public InternetConnectionChecker() {}    
    public InternetConnectionChecker(string endpoint)
    {
        Endpoint = endpoint;
    }

    public bool Check()
    {
        string HtmlText = GetHtmlFromUri(Endpoint);
        if (HtmlText == "")
        {
            //No connection
            return false;
        }
        else if (!HtmlText.Contains("schema.org/WebPage"))
        {
            //Redirecting since the beginning of googles html contains that 
            //phrase and it was not found
            return true;
        }
        else
        {
            //success
            return true;
        }
    }


    public string GetHtmlFromUri(string resource)
    {
        string html = string.Empty;
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(resource);
        try
        {
            using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
            {
                bool isSuccess = (int)resp.StatusCode < 299 && (int)resp.StatusCode >= 200;
                if (isSuccess)
                {
                    using (StreamReader reader = new StreamReader(resp.GetResponseStream()))
                    {
                        //We are limiting the array to 80 so we don't have
                        //to parse the entire html document feel free to 
                        //adjust (probably stay under 300)
                        char[] cs = new char[80];
                        reader.Read(cs, 0, cs.Length);
                        foreach (char ch in cs)
                        {
                            html += ch;
                        }
                    }
                }
            }
        }
        catch
        {
            return "";
        }
        return html;
    }
}