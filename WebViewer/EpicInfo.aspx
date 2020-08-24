<%@ Page Title="Mirth Logs" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EpicInfo.aspx.cs" Inherits="org.ochin.interoperability.OCHINInterfaceUtilities.EpicInfo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row col-md-12">
        <asp:Label ID="lblStatusMsg" runat="server" Text=""></asp:Label>
    </div>
    <div class="row card my-2">
        <div class="card-header">Environment</div>
        <div class="card-body">
            <asp:RadioButtonList ID="rblistEpicICEnvs" runat="server" RepeatDirection="Horizontal" CellPadding="5">
            </asp:RadioButtonList>
        </div>
    </div>
    <div class="row card my-2">
        <div class="card-header">Department List</div>
        <div class="card-body">
            <div class="py-1">
                <asp:Label runat="server" Text="SA: " />
                <asp:TextBox ID="tbSA" runat="server"></asp:TextBox>
                <asp:Label runat="server" Text="IIT: " CssClass="pl-2" />
                <asp:TextBox ID="tbIIT" runat="server"></asp:TextBox>
            </div>
            <div class="py-1">
                <asp:Button CssClass="btn btn-outline-info" ID="btnGetDepList" runat="server" Text="Get DEP List" OnClick="btnGetDepList_Click" />
                <asp:LinkButton CssClass="pl-2" ID="lbtnDownloadDepList" runat="server" Text="Download DEP List as CSV" OnClick="lbtnDownloadDepList_Click" />
            </div>
            <div class="py-1 pre-scrollable">
                <asp:Table ID="tblDepList" runat="server" GridLines="Both" CssClass="table-dark"></asp:Table>
            </div>
        </div>
    </div>
    <div class="row card my-2">
        <div class="card-header">Lab Accounts (AIF-500000)</div>
        <div class="card-body">
            <div class="py-1">
                <input type="text" placeholder="DEP" id="tbFilterTblLabAcctsDep" />
                <input type="text" placeholder="SA" id="tbFilterTblLabAcctsSa" />
                <input type="text" placeholder="TYPE" id="tbFilterTblLabAcctsType" />
                <input type="text" placeholder="ACCT" id="tbFilterTblLabAcctsAcct" />
                <input type="text" placeholder="FPL" id="tbFilterTblLabAcctsFpl" />
                <input type="text" placeholder="LLB" id="tbFilterTblLabAcctsLlb" />
                <asp:Button CssClass="btn btn-secondary" ID="btnFilterLabAccts" runat="server" Text="Apply Filters" OnClientClick="return filterTblLabAccts();" />
            </div>
            <div class="py-1 pre-scrollable">
                <asp:Table ID="tblLabAccts" runat="server" GridLines="Both" CssClass="table-dark"></asp:Table>
                <asp:HiddenField ID="hfLabAccts" runat="server" />
            </div>
            <div class="py-1">
                <asp:Button CssClass="btn btn-outline-info" ID="btnGetLabAccts" runat="server" Text="Get Lab Accounts" OnClick="btnGetLabAccts_Click" />
                <asp:LinkButton CssClass="pl-2" ID="lbtnDownloadLabAccts" runat="server" Text="Download Lab Accounts as CSV" OnClick="lbtnDownloadLabAccts_Click" OnClientClick="listDisplayedLabAccts();" />
            </div>
            <script>
                //var input = document.getElementById("btnFilterLabAccts");
                //input.addEventListener("keydown", function (event) {
                //    // Number 13 is the "Enter" key on the keyboard
                //    if (event.keyCode === 13) {
                //        // Cancel the default action, if needed
                //        event.preventDefault();

                //        filterTblLabAccts();
                //    }
                //});

                function filterTblLabAccts() {
                    // Declare variables
                    var filterDep, filterSa, filterType, filterAcct, filterFpl, filterLlb;
                    var table, tr, td, i, j, txtValue;
                    filterDep = document.getElementById("tbFilterTblLabAcctsDep").value.toUpperCase();
                    filterSa = document.getElementById("tbFilterTblLabAcctsSa").value.toUpperCase();
                    filterType = document.getElementById("tbFilterTblLabAcctsType").value.toUpperCase();
                    filterAcct = document.getElementById("tbFilterTblLabAcctsAcct").value.toUpperCase();
                    filterFpl = document.getElementById("tbFilterTblLabAcctsFpl").value.toUpperCase();
                    filterLlb = document.getElementById("tbFilterTblLabAcctsLlb").value.toUpperCase();

                    table = document.getElementById("MainContent_tblLabAccts");
                    tr = table.getElementsByTagName("tr");

                    // Loop through all table rows (except for the header row), and hide those who don't match the search query
                    for (i = 1; i < tr.length; i++) {
                        td = tr[i].getElementsByTagName("td");

                        // Loop through filters
                        var filters = ["", filterDep, filterSa, filterType, filterAcct, filterFpl, filterLlb];
                        
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

                function listDisplayedLabAccts() {
                    var text = "";

                    // Loop through Lab Accounts table to gather all displayed rows
                    var table, tr, td, i, j;

                    table = document.getElementById("MainContent_tblLabAccts");
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

                    document.getElementById("MainContent_hfLabAccts").value = text;
                }
            </script>
        </div>
    </div>
</asp:Content>
