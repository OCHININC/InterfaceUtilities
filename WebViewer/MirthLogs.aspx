﻿<%@ Page Title="Mirth Logs" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MirthLogs.aspx.cs" Inherits="org.ochin.interoperability.OCHINInterfaceUtilities.MirthLogs" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row col-md-12">
        <a href="https://wiki.ochin.org/display/INTM/How+to+pull+HL7+messages+from+the+Mirth+archive" target="_blank">WIKI - How to pull HL7 messages from the Mirth archive</a>
    </div>
    <div class="row col-md-12">
        <asp:Label ID="lblStatusMsg" runat="server" Text=""></asp:Label>
    </div>
    <div class="row">
        <div class="card col-md-4">
            <div class="card-header">Credentials</div>
            <div class="card-body">
                <div class="py-md-1">
                    <asp:Label runat="server" Text="Username: "></asp:Label>
                    <asp:TextBox ID="tbUsername" runat="server"></asp:TextBox>
                </div>
                <div class="py-md-1">
                    <asp:Label runat="server" Text="Password: "></asp:Label>
                    <asp:TextBox ID="tbPassword" runat="server" TextMode="Password"></asp:TextBox>
                </div>
                <div>
                    <asp:Button CssClass="btn btn-dark" Text="Login" runat="server" ID="btnLogin" OnClick="btnLogin_Click" />
                    <asp:Button CssClass="btn btn-dark" Text="Logout" runat="server" ID="btnLogout" OnClick="btnLogout_Click" Enabled="false" />
                </div>
            </div>
        </div>
        <div class="card col-md-8">
            <div class="card-body">
                <div>
                    <asp:Label runat="server" Text="Text to search for: " />
                    <asp:TextBox ID="tbSearchText" runat="server"></asp:TextBox>
                    <asp:CheckBox ID="cbIgnoreCase" runat="server" Checked="false" Text="Ignore Case" CssClass="px-md-2" />
                    <asp:CheckBox ID="cbRegEx" runat="server" Checked="false" Text="Regular Expression" CssClass="px-md-2" />
                </div>
                <div>
                    <asp:Label runat="server" Text="Files to search: " />
                    <input ID="tbSearchFiles" runat="server" style="max-width:500px;width:100%" placeholder="ie. Lab_Quest_Hub_Orders202005*" />
                </div>
                <div>
                    <asp:Button CssClass="btn btn-outline-info" ID="btnSearch" runat="server" Text="Search" Enabled="false" OnClick="btnSearch_Click" />
                    <asp:Label runat="server" ID="lblSearchCmd" Font-Italic="True" Font-Size="Small" ForeColor="Blue" />
                </div>
                <asp:Panel runat="server" GroupingText="Search Results">
                    <div class="col-md-12">
                        <asp:TextBox ID="tbSearchResults" runat="server" ReadOnly="True" TextMode="MultiLine" Width="100%" Height="250px"></asp:TextBox>
                    </div>
                </asp:Panel>
            </div>
        </div>
    </div>
</asp:Content>
