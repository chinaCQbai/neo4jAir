using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Neo4jClient;
using Neo4jClient.Cypher;

namespace neo4jtest.neo4j
{
    public partial class airfare : System.Web.UI.Page
    {
        public List<string> originlist = new List<string>();
        public List<Route> routelist = new List<Route>();
        public List<string> airportlist = new List<string>();
        public List<Route> routelist2 = new List<Route>();
        public List<string> airportlist2 = new List<string>();
        public List<Route> routelist3 = new List<Route>();
        public List<string> destlist = new List<string>();
        protected void Page_Load(object sender, EventArgs e)
        {
            //这里记录一个bug就是没有换算这里的天数 a2.day_long > 16805
            if (!IsPostBack)
            {
                if (Session["middle"] == "1")
                {
                    if (Session["mids"].ToString() != "")
                    {
                        OneWay1(Session["origincode"].ToString(), Session["origindate"].ToString(), Session["destcode"].ToString(), Session["destdate"].ToString(), Session["mids"].ToString());
                    }
                    else
                    {
                        OneWay1(Session["origincode"].ToString(), Session["origindate"].ToString(), Session["destcode"].ToString(), Session["destdate"].ToString());
                    }
                }
                else if (Session["middle"] == "2")
                {
                    if (Session["mids"].ToString() != "")
                    {
                        OneWay2(Session["origincode"].ToString(), Session["origindate"].ToString(), Session["destcode"].ToString(), Session["destdate"].ToString(), Session["mids"].ToString());
                    }
                    else
                    {
                        OneWay2(Session["origincode"].ToString(), Session["origindate"].ToString(), Session["destcode"].ToString(), Session["destdate"].ToString());
                    }   
                }
                else
                {
                    OneWay(Session["origincode"].ToString(), Session["origindate"].ToString(), Session["destcode"].ToString(), Session["destdate"].ToString());
                }
            }
        }
        //机场对象
        public class Airport
        {
            public string airportcode { get; set; }
            public string day { get; set; }
            public int day_long { get; set; }
        }
        //航班对象
        public class Route
        {
            public string flightno { get; set; }
            public string sourceairportcode { get; set; }
            public string destinationairportcode { get; set; }
            public string price { get; set; }
            public string arrivaltime { get; set; }
            public string departuretime { get; set; }
            public int departuretime_long { get; set; }
            public int arrivaltime_long { get; set; }
        }
        /// <summary>
        /// 返回具体的天数 由于数据库里面的日期是存储的1608这样的方式所以要转化成具体的天数
        /// </summary>
        /// <param name="times">需要转化为天数的字符</param>
        /// <returns></returns>
        public int getTimeDays(string times)
        {
            DateTime t1 = Convert.ToDateTime("1970-01-01");
            DateTime t2 = Convert.ToDateTime(times);
            TimeSpan ts1 = new TimeSpan(t1.Ticks);
            TimeSpan ts2 = new TimeSpan(t2.Ticks);
            
            return ts1.Subtract(ts2).Duration().Days;
        }
        /// <summary>
        /// 返回单程无中转限制的路线
        /// </summary>
        /// <param name="origincode">起点</param>
        /// <param name="origindate">起点时间</param>
        /// <param name="destcode">终点</param>
        /// <param name="destdate">终点时间</param>
        public void OneWay(string origincode, string origindate, string destcode, string destdate)
        {
            var client = new GraphClient(new Uri("http://120.25.59.147:7474/db/data"), "neo4j", "gaoxia");
            client.Connect();
            var query = client.Cypher
                .Match("(origin:Airportday)-[r1:Route]->(dest:Airportday)")
                .Where((Airport origin) => origin.airportcode == origincode)
                .AndWhere((Airport origin) => origin.day == origindate)
                .AndWhere((Airport dest) => dest.airportcode == destcode)
                .AndWhere((Airport dest) => dest.day == destdate)
                .Return((origin, r1, dest) => new
                {
                    origin = origin.As<Airport>(),
                    r1 = r1.As<Route>(),
                    dest = dest.As<Airport>()
                });
            foreach (var result in query.Results)
            {
                Route r = new Route();
                r.flightno = result.r1.flightno;
                r.price=result.r1.price;
                r.arrivaltime=result.r1.arrivaltime;
                r.departuretime=result.r1.departuretime;

                originlist.Add(result.origin.airportcode);
                routelist.Add(r);
                destlist.Add(result.dest.airportcode);
            }
            client.Dispose();
        }
        /// <summary>
        /// 返回单程有一个中转站 且不指定中转站
        /// </summary>
        /// <param name="origincode">起点</param>
        /// <param name="origindate">起点时间</param>
        /// <param name="destcode">终点</param>
        /// <param name="destdate">终点时间</param>
        public void OneWay1(string origincode, string origindate, string destcode, string destdate)
        {
            int stimes = getTimeDays(Session["origintime"].ToString());
            int etimes = getTimeDays(Session["desttime"].ToString());
            var client = new GraphClient(new Uri("http://120.25.59.147:7474/db/data"), "neo4j", "gaoxia");
            client.Connect();
            var query = client.Cypher
                .Match("(origin:Airportday)-[r1:Route]->(a1:Airportday)")
                .Match("(a2:Airportday{airportcode:a1.airportcode})-[r2:Route]->(dest:Airportday)")
                .Where((Airport origin) => origin.airportcode == origincode)
                .AndWhere((Airport origin) => origin.day == origindate)
                .AndWhere((Airport dest) => dest.airportcode == destcode)
                .AndWhere((Airport dest) => dest.day == destdate)
                .AndWhere((Airport a2) => a2.day_long > stimes)
                .AndWhere((Airport a2) => a2.day_long < etimes)
                .AndWhere((Airport a2, Airport a1) => a2.day_long >= a1.day_long)
                //.AndWhere((Route r2, Route r1) => r2.departuretime_long-r1.arrivaltime_long>3 * 3600)
                .Return((origin, r1, a1, r2, dest) => new
                {
                    origin = origin.As<Airport>(),
                    r1 = r1.As<Route>(),
                    a1 = a1.As<Airport>(),
                    r2 = r2.As<Route>(),
                    dest = dest.As<Airport>()
                }).Limit(100);
            foreach (var result in query.Results)
            {
                Route r = new Route();
                r.flightno = result.r1.flightno;
                r.price = result.r1.price;
                r.arrivaltime = result.r1.arrivaltime;
                r.departuretime = result.r1.departuretime;

                Route r2 = new Route();
                r2.flightno = result.r2.flightno;
                r2.price = result.r2.price;
                r2.arrivaltime = result.r2.arrivaltime;
                r2.departuretime = result.r2.departuretime;

                originlist.Add(result.origin.airportcode);
                routelist.Add(r);
                airportlist.Add(result.a1.airportcode);
                routelist2.Add(r2);
                destlist.Add(result.dest.airportcode);
            }
            client.Dispose();
        }
        /// <summary>
        /// 返回单程有一个中转站 且指定中转站
        /// </summary>
        /// <param name="origincode">起点</param>
        /// <param name="origindate">起点时间</param>
        /// <param name="destcode">终点</param>
        /// <param name="destdate">终点时间</param>
        public void OneWay1(string origincode, string origindate, string destcode, string destdate,string mid)
        {
            int stimes = getTimeDays(Session["origintime"].ToString());
            int etimes = getTimeDays(Session["desttime"].ToString());
            var client = new GraphClient(new Uri("http://120.25.59.147:7474/db/data"), "neo4j", "gaoxia");
            client.Connect();
            var query = client.Cypher
                .Match("(origin:Airportday)-[r1:Route]->(a1:Airportday)")
                .Match("(a2:Airportday{airportcode:a1.airportcode})-[r2:Route]->(dest:Airportday)")
                .Where((Airport origin) => origin.airportcode == origincode)
                .AndWhere((Airport origin) => origin.day == origindate)
                .AndWhere((Airport dest) => dest.airportcode == destcode)
                .AndWhere((Airport dest) => dest.day == destdate)
                .AndWhere((Airport a2) => a2.day_long > stimes)
                .AndWhere((Airport a2) => a2.day_long < etimes)
                .AndWhere((Airport a2)=> a2.airportcode==mid)
                .AndWhere((Airport a2, Airport a1) => a2.day_long >= a1.day_long)
                //.AndWhere((Route r2, Route r1) => r2.departuretime_long-r1.arrivaltime_long>3 * 3600)
                .Return((origin, r1, a1, r2, dest) => new
                {
                    origin = origin.As<Airport>(),
                    r1 = r1.As<Route>(),
                    a1 = a1.As<Airport>(),
                    r2 = r2.As<Route>(),
                    dest = dest.As<Airport>()
                }).Limit(100);
            foreach (var result in query.Results)
            {
                Route r = new Route();
                r.flightno = result.r1.flightno;
                r.price = result.r1.price;
                r.arrivaltime = result.r1.arrivaltime;
                r.departuretime = result.r1.departuretime;

                Route r2 = new Route();
                r2.flightno = result.r2.flightno;
                r2.price = result.r2.price;
                r2.arrivaltime = result.r2.arrivaltime;
                r2.departuretime = result.r2.departuretime;

                originlist.Add(result.origin.airportcode);
                routelist.Add(r);
                airportlist.Add(result.a1.airportcode);
                routelist2.Add(r2);
                destlist.Add(result.dest.airportcode);
            }
            client.Dispose();
        }
        /// <summary>
        /// 返回单程有两个个中转站 且不指定中转站
        /// </summary>
        /// <param name="origincode">起点</param>
        /// <param name="origindate">起点时间</param>
        /// <param name="destcode">终点</param>
        /// <param name="destdate">终点时间</param>
        public void OneWay2(string origincode, string origindate, string destcode, string destdate)
        {
            int stimes = getTimeDays(Session["origintime"].ToString());
            int etimes = getTimeDays(Session["desttime"].ToString());
            var client = new GraphClient(new Uri("http://120.25.59.147:7474/db/data"), "neo4j", "gaoxia");
            client.Connect();
            var query = client.Cypher
                .Match("(origin:Airportday)-[r1:Route]->(a1:Airportday)")
                .Match("(a4:Airportday)-[r3:Route]->(dest:Airportday)")
                .Match("(a2:Airportday{airportcode:a1.airportcode})-[r2:Route]->(a3:Airportday{airportcode:a4.airportcode})")
                .Where((Airport origin) => origin.airportcode == origincode)
                .AndWhere((Airport origin) => origin.day == origindate)
                .AndWhere((Airport dest) => dest.airportcode == destcode)
                .AndWhere((Airport dest) => dest.day == destdate)
                .AndWhere((Airport a2) => a2.day_long > stimes)
                .AndWhere((Airport a2) => a2.day_long < etimes)
                .AndWhere((Airport a2, Airport a1) => a2.day_long >= a1.day_long)
                .AndWhere((Airport a3, Airport a2) => a3.day_long >= a2.day_long)
                .AndWhere((Airport a3) => a3.airportcode!=origincode)
                .AndWhere((Airport a4, Airport a3) => a4.day_long >= a3.day_long)
                //.AndWhere((Route r2, Route r1) => r2.departuretime_long-r1.arrivaltime_long>3 * 3600)
                .Return((origin, r1, a1, r2,a3,r3, dest) => new
                {
                    origin = origin.As<Airport>(),
                    r1 = r1.As<Route>(),
                    a1 = a1.As<Airport>(),
                    r2 = r2.As<Route>(),
                    a3 = a3.As<Airport>(),
                    r3 = r3.As<Route>(),
                    dest = dest.As<Airport>()
                }).Limit(100);
            foreach (var result in query.Results)
            {
                Route r = new Route();
                r.flightno = result.r1.flightno;
                r.price = result.r1.price;
                r.arrivaltime = result.r1.arrivaltime;
                r.departuretime = result.r1.departuretime;

                Route r2 = new Route();
                r2.flightno = result.r2.flightno;
                r2.price = result.r2.price;
                r2.arrivaltime = result.r2.arrivaltime;
                r2.departuretime = result.r2.departuretime;

                Route r3 = new Route();
                r3.flightno = result.r3.flightno;
                r3.price = result.r3.price;
                r3.arrivaltime = result.r3.arrivaltime;
                r3.departuretime = result.r3.departuretime;

                originlist.Add(result.origin.airportcode);
                routelist.Add(r);
                airportlist.Add(result.a1.airportcode);
                routelist2.Add(r2);
                airportlist2.Add(result.a3.airportcode);
                routelist3.Add(r3);
                destlist.Add(result.dest.airportcode);
            }
            client.Dispose();
        }
        /// <summary>
        /// 返回单程有两个个中转站 且指定中转站
        /// </summary>
        /// <param name="origincode">起点</param>
        /// <param name="origindate">起点时间</param>
        /// <param name="destcode">终点</param>
        /// <param name="destdate">终点时间</param>
        public void OneWay2(string origincode, string origindate, string destcode, string destdate,string mid)
        {
            int stimes = getTimeDays(Session["origintime"].ToString());
            int etimes = getTimeDays(Session["desttime"].ToString());
            var client = new GraphClient(new Uri("http://120.25.59.147:7474/db/data"), "neo4j", "gaoxia");
            client.Connect();
            var query = client.Cypher
                .Match("(origin:Airportday)-[r1:Route]->(a1:Airportday)")
                .Match("(a4:Airportday)-[r3:Route]->(dest:Airportday)")
                .Match("(a2:Airportday{airportcode:a1.airportcode})-[r2:Route]->(a3:Airportday{airportcode:a4.airportcode})")
                .Where((Airport origin) => origin.airportcode == origincode)
                .AndWhere((Airport origin) => origin.day == origindate)
                .AndWhere((Airport dest) => dest.airportcode == destcode)
                .AndWhere((Airport dest) => dest.day == destdate)
                .AndWhere((Airport a2) => a2.day_long > stimes)
                .AndWhere((Airport a2) => a2.day_long < etimes)
                .AndWhere((Airport a1) => a1.airportcode == mid)
                .AndWhere((Airport a2, Airport a1) => a2.day_long >= a1.day_long)
                .AndWhere((Airport a3, Airport a2) => a3.day_long >= a2.day_long)
                .AndWhere((Airport a3) => a3.airportcode != origincode)
                .AndWhere((Airport a4, Airport a3) => a4.day_long >= a3.day_long)
                //.AndWhere((Route r2, Route r1) => r2.departuretime_long-r1.arrivaltime_long>3 * 3600)
                .Return((origin, r1, a1, r2, a3, r3, dest) => new
                {
                    origin = origin.As<Airport>(),
                    r1 = r1.As<Route>(),
                    a1 = a1.As<Airport>(),
                    r2 = r2.As<Route>(),
                    a3 = a3.As<Airport>(),
                    r3 = r3.As<Route>(),
                    dest = dest.As<Airport>()
                }).Limit(100);
            foreach (var result in query.Results)
            {
                Route r = new Route();
                r.flightno = result.r1.flightno;
                r.price = result.r1.price;
                r.arrivaltime = result.r1.arrivaltime;
                r.departuretime = result.r1.departuretime;

                Route r2 = new Route();
                r2.flightno = result.r2.flightno;
                r2.price = result.r2.price;
                r2.arrivaltime = result.r2.arrivaltime;
                r2.departuretime = result.r2.departuretime;

                Route r3 = new Route();
                r3.flightno = result.r3.flightno;
                r3.price = result.r3.price;
                r3.arrivaltime = result.r3.arrivaltime;
                r3.departuretime = result.r3.departuretime;

                originlist.Add(result.origin.airportcode);
                routelist.Add(r);
                airportlist.Add(result.a1.airportcode);
                routelist2.Add(r2);
                airportlist2.Add(result.a3.airportcode);
                routelist3.Add(r3);
                destlist.Add(result.dest.airportcode);
            }
            query = client.Cypher
                .Match("(origin:Airportday)-[r1:Route]->(a1:Airportday)")
                .Match("(a4:Airportday)-[r3:Route]->(dest:Airportday)")
                .Match("(a2:Airportday{airportcode:a1.airportcode})-[r2:Route]->(a3:Airportday{airportcode:a4.airportcode})")
                .Where((Airport origin) => origin.airportcode == origincode)
                .AndWhere((Airport origin) => origin.day == origindate)
                .AndWhere((Airport dest) => dest.airportcode == destcode)
                .AndWhere((Airport dest) => dest.day == destdate)
                .AndWhere((Airport a2) => a2.day_long > stimes)
                .AndWhere((Airport a2) => a2.day_long < etimes)
                .AndWhere((Airport a4) => a4.airportcode == mid)
                .AndWhere((Airport a2, Airport a1) => a2.day_long >= a1.day_long)
                .AndWhere((Airport a3, Airport a2) => a3.day_long >= a2.day_long)
                .AndWhere((Airport a3) => a3.airportcode != origincode)
                .AndWhere((Airport a4, Airport a3) => a4.day_long >= a3.day_long)
                //.AndWhere((Route r2, Route r1) => r2.departuretime_long-r1.arrivaltime_long>3 * 3600)
                .Return((origin, r1, a1, r2, a3, r3, dest) => new
                {
                    origin = origin.As<Airport>(),
                    r1 = r1.As<Route>(),
                    a1 = a1.As<Airport>(),
                    r2 = r2.As<Route>(),
                    a3 = a3.As<Airport>(),
                    r3 = r3.As<Route>(),
                    dest = dest.As<Airport>()
                }).Limit(100);
            foreach (var result in query.Results)
            {
                Route r = new Route();
                r.flightno = result.r1.flightno;
                r.price = result.r1.price;
                r.arrivaltime = result.r1.arrivaltime;
                r.departuretime = result.r1.departuretime;

                Route r2 = new Route();
                r2.flightno = result.r2.flightno;
                r2.price = result.r2.price;
                r2.arrivaltime = result.r2.arrivaltime;
                r2.departuretime = result.r2.departuretime;

                Route r3 = new Route();
                r3.flightno = result.r3.flightno;
                r3.price = result.r3.price;
                r3.arrivaltime = result.r3.arrivaltime;
                r3.departuretime = result.r3.departuretime;

                originlist.Add(result.origin.airportcode);
                routelist.Add(r);
                airportlist.Add(result.a1.airportcode);
                routelist2.Add(r2);
                airportlist2.Add(result.a3.airportcode);
                routelist3.Add(r3);
                destlist.Add(result.dest.airportcode);
            }
            client.Dispose();
        }
    }
}