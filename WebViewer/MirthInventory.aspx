<%@ Page Title="Mirth Inventory" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MirthInventory.aspx.cs" Inherits="org.ochin.interoperability.OCHINInterfaceUtilities.MirthInventory" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <script>
    function showModalConfirm(msg) {
        $(".modal .message").innerHtml(msg);
        $(".modal").Show();
    }

    function filterMirthInventory() {
        // Declare variables
        var filterName, filterTags, filterServer, filterState;
        var table, tr, td, i, j, txtValue;
        filterName = document.getElementById("tbFilterMirthInventoryName").value.toUpperCase();
        filterTags = document.getElementById("tbFilterMirthInventoryTags").value.toUpperCase();
        filterServer = document.getElementById("tbFilterMirthInventoryServer").value.toUpperCase();
        filterState = document.getElementById("tbFilterMirthInventoryState").value.toUpperCase();

        table = document.getElementById("MainContent_gridMirthInventory");
        tr = table.getElementsByTagName("tr");

        // Loop through all table rows (except for the header row), and hide those who don't match the search query
        for (i = 1; i < tr.length; i++) {
            td = tr[i].getElementsByTagName("td");

            // Loop through filters
            var filters = [filterName, filterTags, filterServer, filterState];

            var found = true;
            for (j = 0; j < filters.length; j++) {
                var f = filters[j];
                if (f != "" && td[j]) {
                    txtValue = td[j].textContent || td[j].innerText;
                    if (txtValue.toUpperCase().indexOf(f) < 0) {
                        found = false;
                        break;
                    }
                }
            }

            if (found) {
                tr[i].style.display = "";
            } else {
                tr[i].style.display = "none";
            }
        }

        return false;
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
                <div class="card-header">Mirth Inventory</div>
                <div class="card-body">
                    <div class="py-1">
                        <input type="text" placeholder="Name" id="tbFilterMirthInventoryName" />
                        <input type="text" placeholder="Tags" id="tbFilterMirthInventoryTags" />
                        <input type="text" placeholder="Server" id="tbFilterMirthInventoryServer" />
                        <input type="text" placeholder="State" id="tbFilterMirthInventoryState" />
                        <asp:Button CssClass="btn btn-secondary" ID="btnFilterMirthInventory" runat="server" Text="Apply Filters" OnClientClick="return filterMirthInventory();" />
                    </div>
                    <div style="overflow:auto;max-height:400px">
                        <asp:GridView ID="gridMirthInventory" runat="server" AllowSorting="True" SelectedRowStyle-BackColor="#00CC00" AutoGenerateColumns="False">
                            <Columns>
                                <asp:BoundField DataField="name" HeaderText="Name"></asp:BoundField>
                                <asp:BoundField DataField="tags" HeaderText="Tags" />
                                <asp:BoundField DataField="server" HeaderText="Server"></asp:BoundField>
                                <asp:BoundField DataField="state" HeaderText="State"></asp:BoundField>
                                <asp:BoundField DataField="description" HeaderText="Description"></asp:BoundField>
                            </Columns>
                        </asp:GridView>
                    </div>
                    <div class="my-1">
                        <asp:Button CssClass="btn btn-outline-info" ID="btnGetMirthInventory" runat="server" Text="Get Mirth Inventory" OnClick="btnGetMirthInventory_Click" />
                        <asp:LinkButton CssClass="pl-2" ID="lbtnDownloadMirthInventory" runat="server" Text="Download Mirth Inventory as CSV" OnClick="lbtnDownloadMirthInventory_Click" OnClientClick="listDisplayedMirthInventory();" />
                        <asp:HiddenField ID="hfMirthInventory" runat="server" />
                    </div>
                    <div class="my-1">
                        <asp:CheckBox ID="cbIncludeDesc" Text="Include Description (may be slow)" Checked ="false" runat="server" Enabled="False" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
