<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="org.ochin.interoperability.OCHINInterfaceUtilities.Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="card my-2 w-100">
            <div class="card-header">Epic</div>
            <div class="card-body">
                <a href="EpicInfo.aspx" class="mx-5 d-block float-left text-center">
                    <img src="Images/outline_info_black_48dp.png" alt="EpicInfo" />
                    <p>Information</p>
                </a>
                <a href="EpicMaintenance.aspx" class="mx-5 d-block float-left text-center">
                    <img src="Images/outline_build_black_48dp.png" alt="EpicMaintenance" />
                    <p>Maintenance</p>
                </a>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="card my-2 w-100">
            <div class="card-header">Mirth</div>
            <div class="card-body">
                <a href="MirthInventory.aspx" class="mx-5 d-block float-left text-center">
                    <img src="Images/outline_ballot_black_48dp.png" alt="MirthInventory" />
                    <p>Inventory</p>
                </a>
                <a href="MirthLogs.aspx" class="mx-5 d-block float-left text-center">
                    <img src="Images/outline_find_in_page_black_48dp.png" alt="MirthLogsSearch" />
                    <p>Logs Search</p>
                </a>
                <a href="MirthConfigMap.aspx" class="mx-5 d-block float-left text-center">
                    <img src="Images/outline_text_snippet_black_48dp.png" alt="MirthConfigMap" />
                    <p>Config Map</p>
                </a>
            </div>
        </div>
    </div>
</asp:Content>
