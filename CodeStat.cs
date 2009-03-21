using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Streamlet.CodeStat
{
    public class Stat
    {
        static public StatModel StatFile(string filename)
        {
            StatModel sm = new StatModel(filename);

            FileInfo fi = new FileInfo(filename);

            if (fi.Exists)
            {
                string ext = filename.Substring(filename.LastIndexOf('.') + 1).ToLower();

                switch (ext)
                { 
                    case "c":
                    case "cpp":
                    case "h":
                    case "cs":
                    case "php":
                    case "js":
                    case "java":
                        StatCSeries(ref sm);
                        break;
                    case "asp":
                    case "vb":
                    case "bas":
                    case "frm":
                    case "vbs":
                        StatVBSeries(ref sm);
                        break;
                    case "asm":
                        StatASMSeries(ref sm);
                        break;
                    default:
                        break;
                }
            }

            return sm;
        }

        static private void StatCSeries(ref StatModel sm)
        {
            try
            {
                using (StreamReader sr = new StreamReader(sm.FileName))
                {
                    string s;
                    bool inComment = false;

                    while ((s = sr.ReadLine()) != null)
                    {
                        s = s.Trim();

                        if (inComment)
                        {
                            if (s.Length >= 2)
                            {
                                if (s.Substring(s.Length - 2) == "*/")
                                {
                                    inComment = false;
                                }
                            }

                            sm.CommentLines++;
                        }
                        else
                        {
                            if (s.Length == 0)
                            {
                                sm.BlankLines++;
                            }
                            else if (s.Length >= 2)
                            {
                                if (s.Substring(0, 2) == "//")
                                {
                                    sm.CommentLines++;
                                }
                                else if (s.Substring(0, 2) == "/*")
                                {
                                    inComment = true;
                                    sm.CommentLines++;
                                }
                                else
                                {
                                    sm.CodeLines++;
                                }
                            }
                            else
                            {
                                sm.CodeLines++;
                            }
                        }
                    }
                    sr.Close();
                }
            }
            catch { }
        }

        static private void StatVBSeries(ref StatModel sm)
        {
            try
            {
                using (StreamReader sr = new StreamReader(sm.FileName))
                {
                    string s;

                    while ((s = sr.ReadLine()) != null)
                    {
                        s = s.Trim();

                        if (s.Length == 0)
                        {
                            sm.BlankLines++;
                        }
                        else if (s.Substring(0, 1) == "'")
                        {
                            sm.CommentLines++;
                        }
                        else
                        {
                            sm.CodeLines++;
                        }
                    }
                    sr.Close();
                }
            }
            catch { }
        }

        static private void StatASMSeries(ref StatModel sm)
        {
            try
            {
                using (StreamReader sr = new StreamReader(sm.FileName))
                {
                    string s;

                    while ((s = sr.ReadLine()) != null)
                    {
                        s = s.Trim();

                        if (s.Length == 0)
                        {
                            sm.BlankLines++;
                        }
                        else if (s.Substring(0, 1) == ";")
                        {
                            sm.CommentLines++;
                        }
                        else
                        {
                            sm.CodeLines++;
                        }
                    }
                    sr.Close();
                }
            }
            catch { }
        }
    }
}
