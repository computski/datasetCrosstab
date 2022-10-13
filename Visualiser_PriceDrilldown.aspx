<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Visualiser_PriceDrilldown.aspx.vb" Inherits="Visualiser.Visualiser_PriceDrilldown" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>visualiser drilldown</title>
      <link href="Styles.css?v=<%=ConfigurationManager.AppSettings("Ver")%>" rel="stylesheet" type="text/css" />
         <script src="javascript/prototype.js?v=<%=ConfigurationManager.AppSettings("Ver")%>" type="text/javascript"></script>
         <script src="javascript/behaviour3.js?v=<%=ConfigurationManager.AppSettings("Ver")%>" type="text/javascript"></script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
             <input type="button" value="close" onclick="parent.document.getElementById('idPrice').hide();" />

            <!--rowSeq	ID	Alerts	Product	ServiceGroupID	PriceQuoteReferenceID	ServiceID	ExistingServiceStatus	ServiceTermEndDate	PBLIID	LocationID	Site	MasterSiteName	QuoteLocationName	InvoiceLocationName	Feature	Configuration	QuoteID	eRateEligible	ICBExists	ICBReference Note	Qty	ChargeType	ListPriceUSD	ListDiscount	ListNetPrice	ProposedDiscount	ProposedNetPriceUSD	RevenueUSD	CustomerCostUSD	DirectCostUSD	SharedCostUSD	CustomerMargin	DirectMargin	SharedMargin	LineItemCurrency	PQRI	AccessSpeed	AccessProvider	AccessSpeedUpload	UploadBandwidth	DownloadBandwidth	VendorProvider	nickname	MapToOrdinal	country	exclude	LocationRemapped	PCMrevenue	LocationFinal
17-->

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
        <asp:BoundField HeaderText="AccessSpeed" DataField="AccessSpeed" />
         <asp:BoundField HeaderText="AccessProvider" DataField="AccessProvider" />
 <asp:BoundField HeaderText="AccessSpeedUpload" DataField="AccessSpeedUpload" />
 <asp:BoundField HeaderText="UploadBandwidth" DataField="UploadBandwidth" />
 <asp:BoundField HeaderText="DownloadBandwidth" DataField="DownloadBandwidth" />
 <asp:BoundField HeaderText="VendorProvider" DataField="VendorProvider" />

    </Columns>



</asp:GridView>

        </div>
    </form>
</body>
</html>
