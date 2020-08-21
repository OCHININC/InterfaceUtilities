<%@ Page Title="Epic Maintentance" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EpicMaintenance.aspx.cs" Inherits="org.ochin.interoperability.OCHINInterfaceUtilities.EpicMaintenance" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <script>
    function showModalConfirm(msg)
    {
        $(".modal .message").innerHtml(msg);
        $(".modal").Show();
    }
    </script>
    <div class="row col-md-12">
        <a href="https://wiki.ochin.org/display/INTM/Interface+Epic+Maintenance+Process" target="_blank">WIKI - Epic Maintenance Process</a>
    </div>
    <div class="row col-md-12">
        <asp:Label ID="lblStatusMsg" runat="server" Text=""></asp:Label>
    </div>
    <div class="row">
        <div class="col-md-12">
            <h2>Mirth</h2>
        </div>
        <div class="col-md-4">
            <div class="card my-2">
                <div class="card-header">Environment</div>
                <div class="card-body">
                    <asp:RadioButtonList ID="rblistMirthEnvs" runat="server" RepeatDirection="Horizontal" CellPadding="5">
                    </asp:RadioButtonList>
                </div>
            </div>
            <div class="card my-2">
                <div class="card-header">Credentials</div>
                <div class="card-body">
                    <div class="py-md-1">
                        <asp:Label runat="server" Text="Username: "></asp:Label>
                        <asp:TextBox ID="tbMirthUsername" runat="server"></asp:TextBox>
                    </div>
                    <div class="py-md-1">
                        <asp:Label runat="server" Text="Password: "></asp:Label>
                        <asp:TextBox ID="tbMirthPassword" runat="server" TextMode="Password"></asp:TextBox>
                    </div>
                    <div>
                        <asp:Button CssClass="btn btn-dark" Text="Login" runat="server" ID="btnMirthLogin" OnClick="btnMirthLogin_Click" />
                        <asp:Button CssClass="btn btn-dark" Text="Logout" runat="server" ID="btnMirthLogout" OnClick="btnMirthLogout_Click" Enabled="false" />
                    </div>
                </div>
            </div>
        </div>
        <div class="col-md-8">
            <div class="card my-2">
                <div class="card-header">Channels</div>
                <div class="card-body">
                    <div>
                        <asp:Label runat="server" ID="lblChannelsSummary"></asp:Label>
                    </div>
                    <div style="overflow:auto;max-height:400px">
                        <asp:GridView ID="gridChannels" runat="server" AllowSorting="True" SelectedRowStyle-BackColor="#00CC00" AutoGenerateColumns="False" OnRowDataBound="gridChannels_RowDataBound">
                            <Columns>
                                <asp:TemplateField HeaderText="Select">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="cbSelect" runat="server"></asp:CheckBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="state" HeaderText="State"></asp:BoundField>
                                <asp:BoundField DataField="server" HeaderText="Server"></asp:BoundField>
                                <asp:BoundField DataField="name" HeaderText="Name"></asp:BoundField>
                                <asp:BoundField DataField="id" HeaderText="ID"></asp:BoundField>
                            </Columns>
                        </asp:GridView>
                    </div>
                    <asp:Button CssClass="btn btn-primary my-3" ID="btnRefreshChannels" runat="server" Text="Query Channels" Enabled="false" OnClick="btnRefreshChannels_Click" />
                    <br />
                    <div>
                        <asp:CheckBoxList ID="cblistDeployed" runat="server" RepeatDirection="Horizontal" CellSpacing="10" Visible="false">
                            <asp:ListItem Selected="True" Enabled="False">Deployed</asp:ListItem>
                            <asp:ListItem Enabled="False">Undeployed</asp:ListItem>
                        </asp:CheckBoxList>
                        <asp:CheckBoxList ID="cbListSourceType" runat="server" RepeatDirection="Horizontal" CellSpacing="10" Visible="false">
                            <asp:ListItem>Channel Reader</asp:ListItem>
                            <asp:ListItem Selected="True">HTTP Listener</asp:ListItem>
                            <asp:ListItem Selected="True">TCP Listener</asp:ListItem>
                            <asp:ListItem Selected="True">JavaScript Reader</asp:ListItem>
                        </asp:CheckBoxList>
                        <asp:CheckBoxList ID="cblistState" runat="server" RepeatDirection="Horizontal" CellSpacing="10" OnSelectedIndexChanged="cblistState_SelectedIndexChanged" AutoPostBack="True">
                            <asp:ListItem Selected="True">Started</asp:ListItem>
                            <asp:ListItem Selected="False">Stopped</asp:ListItem>
                        </asp:CheckBoxList>
                    </div>
                </div>
            </div>
            <div class="card my-2">
                <div class="card-header">Actions</div>
                <div class="card-body">
                    <asp:Button CssClass="btn btn-outline-danger" Text="Stop Selected Channels" runat="server" ID="btnStopChannels" OnClick="btnStopChannels_Click" Enabled="false" OnClientClick="return confirm('Stop all the selected channels?');" />
                    <asp:Button CssClass="btn btn-outline-info" Text="Start Selected Channels" runat="server" ID="btnStartChannels" OnClick="btnStartChannels_Click" Enabled="false" />
                </div>
            </div>
        </div>
    </div>
    <hr />
    <div class="row">
        <div class="col-md-12">
            <h2>Pentra</h2>
        </div>
        <div class="col-md-4">
            <div class="card my-2">
                <div class="card-header">Environment</div>
                <div class="card-body">
                    <asp:RadioButtonList ID="rblistPentraEnvs" runat="server" RepeatDirection="Horizontal" CellPadding="5">
                    </asp:RadioButtonList>
                </div>
            </div>
            <div class="card my-2">
                <div class="card-header">Credentials</div>
                <div class="card-body">
                    <div class="py-md-1">
                        <asp:Label runat="server" Text="Username: "></asp:Label>
                        <asp:TextBox ID="tbPentraUsername" runat="server"></asp:TextBox>
                    </div>
                    <div class="py-md-1">
                        <asp:Label runat="server" Text="Password: "></asp:Label>
                        <asp:TextBox ID="tbPentraPassword" runat="server" TextMode="Password"></asp:TextBox>
                    </div>
                    <div>
                        <asp:Button CssClass="btn btn-dark" Text="Login" runat="server" ID="btnPentraLogin" OnClick="btnPentraLogin_Click" />
                        <asp:Button CssClass="btn btn-dark" Text="Logout" runat="server" ID="btnPentraLogout" OnClick="btnPentraLogout_Click" Enabled="false" />
                    </div>
                </div>
            </div>
        </div>
        <div class="col-md-8">
            <div class="card my-2">
                <div class="card-header">Pentra Gateway Log</div>
                <div class="card-body">
                    <asp:TextBox ID="tbPentraGatewayLog" runat="server" ReadOnly="True" TextMode="MultiLine" Width="100%" Height="250px"></asp:TextBox>
                    <asp:Label runat="server" Text="Last # of lines: " />
                    <asp:TextBox ID="tbPentraGatewayLogLines" Text="20" runat="server" TextMode="Number"></asp:TextBox>
                    <asp:Button CssClass="btn btn-outline-info" Text="Refresh Logs" runat="server" ID="btnRefreshPentraGatewayLogs" OnClick="btnRefreshPentraGatewayLogs_Click" Enabled="false" />
                </div>
            </div>
            <div class="card my-2">
                <div class="card-header">Actions</div>
                <div class="card-body">
                    <asp:Button CssClass="btn btn-outline-danger" Text="Stop Pentra Gateway" runat="server" ID="btnStopPentra" OnClick="btnStopPentra_Click" Enabled="false" OnClientClick="return confirm('Stop the Pentra Gateway?');" />
                    <asp:Button CssClass="btn btn-outline-info" Text="Start Pentra Gateway" runat="server" ID="btnStartPentra" OnClick="btnStartPentra_Click" Enabled="false" />
                </div>
            </div>
            <div>
                <asp:Label runat="server" ID="lblPentraCmd" Font-Italic="True" Font-Size="Small" ForeColor="Blue" />
            </div>
        </div>
    </div>

</asp:Content>
