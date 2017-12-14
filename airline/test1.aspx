<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="test1.aspx.cs" Inherits="neo4jtest.neo4j.test1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <link href="../CSS/neo4j.css" type="text/css" rel="stylesheet" />
    <script language="javascript" type="text/javascript" src="../My97DatePicker/WdatePicker.js"></script>
    <title>机票选择</title>
</head>
<body>
    <form id="form1" runat="server">
        <div class="main">
            <div class="maintitletext">发现 精彩世界</div>
            <div class="change">
                <img src="../image/start.png" style="float:left" />
                <asp:TextBox runat="server" ID="txtOrigin" Text="起点" Height="35px" style="float:left;width:76px"></asp:TextBox>
                <input runat="server" id="txtOrigindate" style="width:150px;float:left;height:35px;" type="text" onClick="WdatePicker()">
                <img src="../image/end.png" style="float:left"/>
                <asp:TextBox runat="server" ID="txtDest" Height="35px" Text="终点" style="float:left;width:76px"></asp:TextBox>
                <input runat="server" id="txtDestdate" style="width:150px;float:left;height:35px;" type="text" onClick="WdatePicker()">
                <img src="../image/mid.png" style="float:left" />
                <asp:TextBox runat="server" ID="txtMid" Height="35px" style="float:left;width:78px"></asp:TextBox>
                <asp:DropDownList runat="server" ID="dropmid" Height="37px" style="float:left;width:90px">
                    <asp:ListItem Selected="True">0</asp:ListItem>
                    <asp:ListItem>1</asp:ListItem>
                    <asp:ListItem>2</asp:ListItem>
                </asp:DropDownList>
                <asp:ImageButton runat="server" ImageUrl="~/image/find.png" OnClick="Unnamed_Click" style="float:left;height:43px;" />
            </div>
        </div>
    </div>
    </form>
</body>
</html>
