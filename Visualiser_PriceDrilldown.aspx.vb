Imports System.Data.OleDb
Imports System.Runtime.CompilerServices
Imports System.Security.Cryptography

Public Class Visualiser_PriceDrilldown
    Inherits System.Web.UI.Page
    Dim sConn As String = ConfigurationManager.ConnectionStrings("sConn").ConnectionString
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


        If Session("priceTarget") Is Nothing Then
            Response.Write("ERROR: no target data, please close")
            Exit Sub
        End If

        Response.Write(Session("priceTarget"))

        If Not Page.IsPostBack Then
            showDrilldown()
        End If
    End Sub

    Private Sub Visualiser_PriceDrilldown_Init(sender As Object, e As EventArgs) Handles Me.Init
        ViewStateUserKey = Session.SessionID
    End Sub
    Sub showDrilldown()
        Dim rc() As String = Split(Session("priceTarget"), ",")
        If rc.Length <> 2 Then
            Response.Write("ERROR: incorrect target, please close")
            Exit Sub
        End If

        Dim oConn As New OleDb.OleDbConnection(sConn)
        Dim oDS As New DataSet
        Dim oDA As New OleDbDataAdapter("Select * FROM qryVisualiser WHERE rowSeq=@p1", oConn)
        If IsNumeric(rc(1)) Then
            '*** numeric column, further limit dataset
            oDA = New OleDbDataAdapter("Select * FROM qryVisualiser WHERE rowSeq=@p1 And MapToOrdinal=@p2", oConn)
        End If
        oDA.SelectCommand.Parameters.AddWithValue("@p1", rc(0))
        oDA.SelectCommand.Parameters.AddWithValue("@p2", rc(1))

        oDA.Fill(oDS)

        '** for NRE revenue in the PCMrevenue column, multiply this up by 36
        For Each myR As DataRow In oDS.Tables(0).Rows
            If myR("ChargeType") = "NRE" Then myR("PCMrevenue") *= 36
        Next


        gvDrilldown.DataSource = oDS.Tables(0)
        gvDrilldown.DataBind()
        oConn.Dispose()



    End Sub
End Class