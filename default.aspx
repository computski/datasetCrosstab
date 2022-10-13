<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="default.aspx.vb" Inherits="Visualiser._default"  Trace="false"%>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Bid visualiser</title>
        <link href="Styles.css?v=<%=ConfigurationManager.AppSettings("Ver")%>" rel="stylesheet" type="text/css" />
         <script src="javascript/prototype.js?v=<%=ConfigurationManager.AppSettings("Ver")%>" type="text/javascript"></script>
         <script src="javascript/behaviour3.js?v=<%=ConfigurationManager.AppSettings("Ver")%>" type="text/javascript"></script>

</head>
<body>
    <form id="form1" runat="server">
        <div>

             
     
                  
             <div id="bannerApp"><span id="spanProject" runat="server">project</span>&nbsp;&nbsp;
                 <span style="FONT-VARIANT: normal; FONT-SIZE: 11px; FONT-WEIGHT: normal" id="spanLogin" runat="server">login credentials</span> &nbsp;&nbsp;&nbsp;&nbsp;Bid visualiser&nbsp;</div>
    <div id="statusBar" runat="server" EnableViewState="false">&nbsp;</div>

<!--drilldown view-->           
            <asp:GridView ID="gvDrilldown" runat="server" AutoGenerateColumns="false">
    <Columns>
        <asp:BoundField HeaderText="rowSeq" DataField="rowSeq" />
        <asp:BoundField Headertext="ColOrdinal" DataField="mapToOrdinal" />
        <asp:BoundField HeaderText="Cux ref final" DataField="locationFinal" ItemStyle-Wrap="false" />

        <asp:BoundField HeaderText="Country" DataField="country" />
        <asp:BoundField HeaderText="Nickname" DataField="nickname" />
<asp:BoundField HeaderText="Quote ID" DataField="QuoteID" />
<asp:BoundField HeaderText="ServiceGroupID" DataField="ServiceGroupID" />
<asp:BoundField HeaderText="Site" DataField="Site" />
        <asp:BoundField HeaderText="Feature" DataField="Feature" />
        <asp:BoundField HeaderText="Configuration" DataField="Configuration" />
        <asp:BoundField HeaderText="ChargeType" DataField="ChargeType" />
        <asp:BoundField HeaderText="PCM revenue" DataField="PCMrevenue" />
       

    </Columns>
</asp:GridView>

<!--temporarily remove these fields
     <asp:BoundField HeaderText="AccessSpeed" DataField="AccessSpeed" />
         <asp:BoundField HeaderText="AccessProvider" DataField="AccessProvider" />
 <asp:BoundField HeaderText="AccessSpeedUpload" DataField="AccessSpeedUpload" />
 <asp:BoundField HeaderText="UploadBandwidth" DataField="UploadBandwidth" />
 <asp:BoundField HeaderText="DownloadBandwidth" DataField="DownloadBandwidth" />
 <asp:BoundField HeaderText="VendorProvider" DataField="VendorProvider" />
    -->


<asp:Panel ID="divDetail" runat="server" Visible="false">
    <asp:imagebutton id="lbDetail" ImageUrl="./img/XLdownload.gif" runat="server" AlternateText="unique" style="width: 16px" />&nbsp;download lineitem detail
</asp:Panel>
     

                        <br />

            <br />
            Click on a price to see the PQ records behind it.&nbsp; rowSeq is the XL row on the customer sheet, the numeric column headers are the XL colums on the customer sheet (aka ColOrdinal)<br />
                        <br />
             
            <asp:GridView ID="gvPrice" DataKeyNames="rowSeq" runat="server"  SelectedRowStyle-BackColor="YellowGreen" />
                           

            <asp:HiddenField ID="hdnPrice" runat="server" />

            <!-- we don't need an iframe-->
             <iframe id="idPrice" src="MOTD.htm" runat="server" visible="false"></iframe>


          </div>
        
        


      </form>
</body>
</html>
