<%@ Page Title="Mirth Configuration Map" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MirthConfigMap.aspx.cs" Inherits="org.ochin.interoperability.OCHINInterfaceUtilities.MirthConfigMap" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <script>
        function filterMirthConfigMap() {
            // Declare variables
            var filterKey, filterValue, filterComment;
            var table, tr, td, txtValue;
            filterKey = document.getElementById("tbFilterMirthConfigMapKey").value.toUpperCase();
            filterValue = document.getElementById("tbFilterMirthConfigMapValue").value.toUpperCase();
            filterComment = document.getElementById("tbFilterMirthConfigMapComment").value.toUpperCase();

            table = document.getElementById("MainContent_gridMirthConfigMap");
            tr = table.getElementsByTagName("tr");
            var filters = [[filterKey], [filterValue], [filterComment]];

            // Loop through all table rows (except for the header row), and hide those who don't match the search query
            for (var i = 1; i < tr.length; i++) {
                td = tr[i].getElementsByTagName("td");

                // Loop through filters
                var found = true;
                for (var j = 0; j < filters.length; j++) {
                    var f = filters[j];

                    if (f != undefined && f.length > 0 && td[j]) {;
                        txtValue = td[j].textContent || td[j].innerText;

                        if (txtValue.toUpperCase().indexOf(f[0]) < 0) {
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

        function listDisplayedMirthConfigMap() {
            var text = "";

            // Loop through Lab Accounts table to gather all displayed rows
            var table, tr, td, i, j;

            table = document.getElementById("MainContent_gridMirthConfigMap");
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

            document.getElementById("MainContent_hfMirthConfigMap").value = text;
        }

        function confirmAddRemitInboundSA() {
            var sa = document.getElementById("MainContent_tbAddRemitInboundSASA").value;

            if (sa == '') {
                alert('A value must be specified for [SA]');
                return false;
            }

            return confirm('Add SA' + sa + ' to the Trizetto inbound remit list?');
        }
    </script>
    <div class="row">
        <div class="col-md-4">
            <div class="card my-2">
                <div class="card-header">Environment</div>
                <div class="card-body">
                    <asp:RadioButtonList ID="rblistMirthEnvs" runat="server" RepeatDirection="Horizontal" CellPadding="5" RepeatColumns="6">
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
                    <div class="py-md-1">
                        <asp:Label ID="lblStatusMsg" runat="server" Text="" ForeColor="Red"></asp:Label>
                    </div>
                </div>
            </div>
        </div>
        <div class="col-md-8">
            <div class="card my-2">
                <div class="card-header">Mirth Configuration Map</div>
                <div class="card-body">
                    <div class="my-1">
                        <input type="text" placeholder="Key" id="tbFilterMirthConfigMapKey" />
                        <input type="text" placeholder="Value" id="tbFilterMirthConfigMapValue" />
                        <input type="text" placeholder="Comment" id="tbFilterMirthConfigMapComment" />
                        <asp:Button CssClass="btn btn-secondary" ID="btnFilterMirthConfigMap" runat="server" Text="Apply Filters" OnClientClick="return filterMirthConfigMap();" />
                    </div>
                    <div style="overflow:auto;max-height:400px">
                        <asp:GridView ID="gridMirthConfigMap" runat="server" AllowSorting="True" SelectedRowStyle-BackColor="#00CC00" AutoGenerateColumns="False"
                            OnSorting="gridMirthConfigMap_Sorting" DataKeyNames="key">
                            <Columns>
                                <asp:BoundField DataField="key" HeaderText="Key" SortExpression="key" />
                                <asp:BoundField DataField="value" HeaderText="Value" SortExpression="value" />
                                <asp:BoundField DataField="comment" HeaderText="Comment" SortExpression="comment" />
                            </Columns>
                        </asp:GridView>
                    </div>
                    <div class="my-1">
                        <asp:Button CssClass="btn btn-outline-info" ID="btnGetMirthConfigMap" runat="server" Text="Get Mirth Configuration Map" OnClick="btnGetMirthConfigMap_Click" />
                        <asp:LinkButton CssClass="pl-2" ID="lbtnDownloadMirthConfigMap" runat="server" Text="Download Mirth Configuration Map as CSV" OnClick="lbtnDownloadMirthConfigMap_Click" OnClientClick="listDisplayedMirthConfigMap();" />
                        <asp:HiddenField ID="hfMirthConfigMap" runat="server" />
                    </div>
                    <div class="my-1" hidden="hidden">
                        <input type="text" runat="server" placeholder="Key" id="tbAddMirthConfigMapKey" />
                        <input type="text" runat="server" placeholder="Value" id="tbAddMirthConfigMapValue" />
                        <input type="text" runat="server" placeholder="Comment" id="tbAddMirthConfigMapComment" />
                        <asp:Button CssClass="btn btn-secondary" ID="btnAddMirthConfigMapEntry" runat="server" Text="Add Entry" OnClick="btnAddMirthConfigMapEntry_Click" />
                    </div>
                    <div class="my-1">
                        <button runat="server" class="btn btn-secondary" type="button" data-toggle="collapse" data-target="#addRemitInboundSA" aria-expanded="false" aria-controls="addRemitInboundSA" id="btnAddTrizettoRemitInboundSA">
                            Add Trizetto remit inbound SA
                        </button>
                        <div class="collapse" id="addRemitInboundSA">
                            <div class="card card-body">
                                <input type="text" runat="server" placeholder="SA" id="tbAddRemitInboundSASA" />
                                <input type="text" runat="server" placeholder="Name" id="tbAddRemitInboundSAName" />
                                <input type="text" runat="server" placeholder="Contacts (optional)" id="tbAddRemitInboundSAContacts" />
                                <asp:Button CssClass="btn btn-secondary" ID="btnAddRemitInboundSAAdd" runat="server" Text="Add SA" OnClick="btnAddRemitInboundSAAdd_Click" OnClientClick="confirmAddRemitInboundSA();" />
                            </div>
                        </div>
                    </div>
                    <div class="py-md-1">
                        <asp:Label ID="lblMirthConfigMapStatus" runat="server" Text="" ForeColor="Red"></asp:Label>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
