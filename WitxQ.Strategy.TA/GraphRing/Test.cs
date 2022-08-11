using System;
using System.Collections.Generic;

namespace WitxQ.Strategy.TA.GraphRing
{
    class Test
    {
        static void Main(string[] args)
        {


            //Stack<char> st = new Stack<char>();

            //st.Push('A');
            //st.Push('M');
            //st.Push('G');
            //st.Push('W');

            //Console.WriteLine("Current stack: ");
            //foreach (char c in st)
            //{
            //    Console.Write(c + " ");
            //}
            //Console.WriteLine();


            DateTime dtStart = DateTime.Now;
            //List<string> pairs = new List<string>()
            //{
            //    "LRC-ETH","ETH-USDT","LRC-USDT","LINK-ETH","LINK-USDT"
            //};
            List<string> pairs = new List<string>()
            {
                "ETH-BTC","ETC-BTC","ETC-ETH","ZEC-BTC","DASH-BTC","LTC-BTC","BCC-BTC","QTUM-BTC",
                "QTUM-ETH","XRP-BTC","ZRX-BTC","ZRX-ETH","DNT-ETH","DPY-ETH","OAX-ETH","LRC-ETH",
                "LRC-BTC","PST-ETH","TNT-ETH","SNT-ETH","SNT-BTC","OMG-ETH","OMG-BTC","PAY-ETH",
                "PAY-BTC","BAT-ETH","CVC-ETH","STORJ-ETH","STORJ-BTC","EOS-ETH","EOS-BTC"
            };

            //List<string> pairs = new List<string>()
            //{
            //    "1-2","1-6","2-3","2-5","3-4","3-5","4-5","5-6","6-7","6-9","7-8","8-9","9-10"
            //};



            UDGraphRing graphRing = new UDGraphRing(pairs);
            graphRing.DFSTraverse();
            var rings = graphRing.GetRings("ETH",3);

            Console.WriteLine("\n----------------------------------------------\n");
            // Dictionary<string, Dictionary<string,List<Tuple<string, bool>>>>
            if(rings!=null && rings.Count>0)
            {
                foreach(var kv in rings)
                {
                    Console.WriteLine($"Target:{kv.Key}");

                    foreach(var ringInfos in kv.Value)
                    {
                        string str = $"    Loop Ring Name:{ringInfos.Key}   ";
                        for (int i = 0; i < ringInfos.Value.Count; i++)
                        {
                            str += i + $":{ringInfos.Value[i].Item1}  {ringInfos.Value[i].Item2}   ";
                        }
                        Console.WriteLine(str);
                    }

                }
            }
            DateTime dtEnd = DateTime.Now;

            Console.WriteLine((dtEnd-dtStart).TotalMilliseconds+"ms");

            Console.ReadKey();
            Console.WriteLine("Hello World!");
        }
    }
}
