<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="airfare.aspx.cs" Inherits="neo4jtest.neo4j.airfare" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <link href="../CSS/neo4j.css" type="text/css" rel="stylesheet" />
    <title>机票</title>
</head>
<body>
    <form id="form1" runat="server">
    <div class="result">
        <div class="resultTop">【去程】<%=Session["origincode"] %>-><%=Session["destcode"] %> 【时间】<%=Session["origindate"] %>-><%=Session["destdate"] %>【时间值】<%=Session["times"] %></div>
            <%
                if (originlist.Count < 1)
                {
                    %><div class="noresultdata">
                        对不起，没有此时段航班！
                        <img src="../image/nofinddata.png" />
                      </div> <%
                }
                else
                {
                    for (int i = 0; i < originlist.Count; i++) {
                    if (airportlist2.Count > 0)
                    {
                        %>
                            <div class="resultTop">
                                <div class="resultAirport"><%=originlist[i] %></div>
                                <img src="../image/airlines.png" style="float:left;padding-top:1px;" />
                                <div class="resultAirport"><%=airportlist[i] %></div>
                                <img src="../image/airlines.png" style="float:left;padding-top:1px;" />
                                <div class="resultAirport"><%=airportlist2[i] %></div>
                                <img src="../image/airlines.png" style="float:left;padding-top:1px;" />
                                <div class="resultAirport"><%=destlist[i] %></div>
                            </div><br />
                            <div class="resultContent">
                                <br />
                                <div class="resultRoute">
                                    <label style="font-weight:bold"><%=originlist[i] %></label>
                                    <label style="color:blue"><%=routelist[i].flightno %></label>
                                    <label style="font-weight:bold"><%=routelist[i].departuretime %></label>
                                    <label style="font-weight:bold"><%=routelist[i].arrivaltime %></label>
                                    <label style="color:red"><%=routelist[i].price %></label>
                                    <label style="font-weight:bold"><%=airportlist[i] %></label>
                                </div>
                                <div class="resultRoute">
                                    <label style="font-weight:bold"><%=airportlist[i] %></label>
                                    <label style="color:blue"><%=routelist2[i].flightno %></label>
                                    <label style="font-weight:bold"><%=routelist2[i].departuretime %></label>
                                    <label style="font-weight:bold"><%=routelist2[i].arrivaltime %></label>
                                    <label style="color:red"><%=routelist2[i].price %></label>
                                    <label style="font-weight:bold"><%=airportlist2[i] %></label>
                                </div>
                                <div class="resultRoute">
                                    <label style="font-weight:bold"><%=airportlist2[i] %></label>
                                    <label style="color:blue"><%=routelist3[i].flightno %></label>
                                    <label style="font-weight:bold"><%=routelist3[i].departuretime %></label>
                                    <label style="font-weight:bold"><%=routelist3[i].arrivaltime %></label>
                                    <label style="color:red"><%=routelist3[i].price %></label>
                                    <label style="font-weight:bold"><%=destlist[i] %></label>
                                </div>
                            </div>
                        <%
                        }
                        else if (airportlist.Count > 0)
                        {
                            %>
                                <div class="resultTop">
                                    <div class="resultAirport"><%=originlist[i] %></div>
                                    <img src="../image/airlines.png" style="float:left;padding-top:1px;" />
                                    <div class="resultAirport"><%=airportlist[i] %></div>
                                    <img src="../image/airlines.png" style="float:left;padding-top:1px;" />
                                    <div class="resultAirport"><%=destlist[i] %></div>
                                </div><br />
                                <div class="resultContent">
                                    <br />
                                    <div class="resultRoute">
                                        <label style="font-weight:bold"><%=originlist[i] %></label>
                                        <label style="color:blue"><%=routelist[i].flightno %></label>
                                        <label style="font-weight:bold"><%=routelist[i].departuretime %></label>
                                        <label style="font-weight:bold"><%=routelist[i].arrivaltime %></label>
                                        <label style="color:red"><%=routelist[i].price %></label>
                                        <label style="font-weight:bold"><%=airportlist[i] %></label>
                                    </div>
                                    <div class="resultRoute">
                                        <label style="font-weight:bold"><%=airportlist[i] %></label>
                                        <label style="color:blue"><%=routelist2[i].flightno %></label>
                                        <label style="font-weight:bold"><%=routelist2[i].departuretime %></label>
                                        <label style="font-weight:bold"><%=routelist2[i].arrivaltime %></label>
                                        <label style="color:red"><%=routelist2[i].price %></label>
                                        <label style="font-weight:bold"><%=destlist[i] %></label>
                                    </div>
                                </div>
                            <%
                        }
                        else
                        {
                            %>
                                <div class="resultTop">
                                    <div class="resultAirport"><%=originlist[i] %></div>
                                    <img src="../image/oneline.png" style="float:left;padding-top:1px;" />
                                    <div class="resultAirport"><%=destlist[i] %></div>
                                </div><br />
                                <div class="resultContent">
                                    <br />
                                    <div class="resultRoute">
                                        <label style="font-weight:bold"><%=originlist[i] %></label>
                                        <label style="color:blue"><%=routelist[i].flightno %></label>
                                        <label style="font-weight:bold"><%=routelist[i].departuretime %></label>
                                        <label style="font-weight:bold"><%=routelist[i].arrivaltime %></label>
                                        <label style="color:red"><%=routelist[i].price %></label>
                                        <label style="font-weight:bold"><%=destlist[i] %></label>
                                    </div>
                                </div>
                            <%
                        }
                    } 
                }
           %>
        <div class="resultEnd"></div>
    </div>
    </form>
</body>
</html>
