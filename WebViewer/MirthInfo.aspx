<%@ Page Title="Epic Maintentance" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MirthInfo.aspx.cs" Inherits="org.ochin.interoperability.OCHINInterfaceUtilities.MirthInfo" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <script>
    function showModalConfirm(msg) {
        $(".modal .message").innerHtml(msg);
        $(".modal").Show();
    }

    function listDisplayedMirthInventory() {
        var text = "";

        // Loop through Lab Accounts table to gather all displayed rows
        var table, tr, td, i, j;

        table = document.getElementById("MainContent_gridMirthInventory");
        tr = table.getElementsByTagName("tr");

        // Loop through all table rows (including header row)
        for (i = 0; i < tr.length; i++) {
            if (tr[i].style.display == "") {
                if (i > 0) {
                    td = tr[i].getElementsByTagName("td");
                } else {
                    td = tr[i].getElementsByTagName("th");
                }

                for (j = 0; j < td.length; j++) {
                    text += td[j].innerText + ",";
                }

                if (text.endsWith(",")) {
                    text = text.slice(0, text.length - 1);
                    text += "\r\n";
                }
            }
        }

        document.getElementById("MainContent_hfMirthInventory").value = text;
    }
    </script>
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
                <div class="card-header">Inventory</div>
                <div class="card-body">
                    <div style="overflow:auto;max-height:400px">
                        <asp:GridView ID="gridMirthInventory" runat="server" AllowSorting="True" SelectedRowStyle-BackColor="#00CC00" AutoGenerateColumns="False">
                            <Columns>
<%--                                <asp:TemplateField HeaderText="Select">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="cbSelect" runat="server"></asp:CheckBox>
                                    </ItemTemplate>
                                </asp:TemplateField>--%>
                                <asp:BoundField DataField="name" HeaderText="Name"></asp:BoundField>
                                <asp:BoundField DataField="tags" HeaderText="Tags" />
                                <asp:BoundField DataField="server" HeaderText="Server"></asp:BoundField>
                                <asp:BoundField DataField="state" HeaderText="State"></asp:BoundField>
                                <asp:BoundField DataField="id" HeaderText="ID"></asp:BoundField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </div>
            <div class="my-1">
                <asp:Button CssClass="btn btn-outline-info" ID="btnGetMirthInventory" runat="server" Text="Get Mirth Inventory" OnClick="btnGetMirthInventory_Click" />
                <asp:LinkButton CssClass="pl-2" ID="lbtnDownloadMirthInventory" runat="server" Text="Download Mirth Inventory as CSV" OnClick="lbtnDownloadMirthInventory_Click" OnClientClick="listDisplayedMirthInventory();" />
                <asp:HiddenField ID="hfMirthInventory" runat="server" />
            </div>
        </div>
    </div>
</asp:Content>
