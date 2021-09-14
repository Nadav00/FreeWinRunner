<%@ Page Title="Home Page" Language="C#"  MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="FWR_Web._Default" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
	<div class="jumbotron">
		<h1>FWR Queue Runs Database</h1>
		<p class="lead">View/filter past and current queues run</p>
		<p><asp:Button runat="server" ID="btnLogin" Text="Log In" OnClick="btnLogin_Click" /></p>
		<p><asp:GridView ID="RunsGridView" runat="server"></asp:GridView> 
	</div>

    <div><asp:ListView runat="server" ID="headerListVeiw"></asp:ListView>	</div>

	<script src="\Scripts\jquery-3.3.1.js" type="text/javascript"></script>
	<script  type="text/javascript">
		$('.header').click(function(){

$(this).nextUntil('tr.header').slideToggle(1000);
});
	</script>
</asp:Content>
