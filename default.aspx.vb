Imports System.Data.OleDb

Public Class _default
    Inherits System.Web.UI.Page
    Dim sConn As String = ConfigurationManager.ConnectionStrings("sConn").ConnectionString

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Page.IsPostBack Then
            'showDrilldownIframe()
            showPrice()
        Else
            showPrice()
        End If
    End Sub
    Sub showDrilldownIframe()
        '*** not in use.  this method uses an iframe as a popup window to the detail.
        Dim rc() As String = Split(hdnPrice.Value, ",")
        If rc.Length <> 2 Then Exit Sub
        Dim fhc As DataControlFieldHeaderCell = gvPrice.HeaderRow.Controls(rc(1)) 'holds MapToOrdinal
        Dim fc As DataControlFieldCell = gvPrice.Rows(rc(0)).Controls(0)  'holds rowSeq

        '*** invoke the popup and seed its values
        idPrice.Src = "Visualiser_PriceDrilldown.aspx"
        Session.Add("priceTarget", fc.Text & "," & fhc.Text)
        idPrice.Style.Item("display") = "inherit"

    End Sub




    Sub showPrice()
        '*** cannot invoke MSaccess queries that themselves have embedded functions...
        '*** so we have to do it properly, and open a dataset and then progressively modify that to get to desired data
        '*** and then we have to cross tab that... but how.  would need to write a dataset to crosstab function. (i did this with IDRanalytics)

        Dim dt As DataTable = TryCast(Session("xtab"), DataTable)
        If dt Is Nothing Then

            Dim oConn As New OleDb.OleDbConnection(sConn)
            Dim oDS As New DataSet
            Dim oda As New OleDbDataAdapter("Select * FROM qryVisualiser", oConn)
            oda.Fill(oDS, "xtab")

            '*** multiply NRE values by 36
            For Each myR As DataRow In oDS.Tables("xtab").Rows
                If myR("ChargeType") & String.Empty = "NRE" Then
                    If IsNumeric(myR("PCMrevenue")) Then myR("PCMrevenue") *= 36
                End If
            Next


            '*** we should cache this xtab dataset because its not going to change.  you might sort it etc but unless we are going to mask in/out individual services
            '*** it won't change

            dt = datatableToCrosstab(oDS.Tables("xtab"), "MapToOrdinal", "PCMRevenue", {"RowSeq", "LocationFinal", "Country"})
            Session.Add("xtab", dt)
        End If

        Dim myView As DataView = dt.DefaultView
        myView.Sort = "rowSeq"
        gvPrice.DataSource = myView
        gvPrice.DataBind()

    End Sub

    Function datatableToCrosstab(dt As DataTable, columnHeading As String, valueField As String, rowHeading() As String) As DataTable
        'transforms data to a cross tab.  assume valuefield will be SUM for now

        'problem is you also want to specify order by, but I guess you can do this on the transformed data
        'you'd always want cols to be sorted alpha

        '2022-10-05 need to cope with null entries for one/more of the rowHeadings

        ' Try


        '1/ select distinct columnHeading and sort ASC. Have to use a dataview to effect the sorting
        Dim dtColumnsView As DataView = dt.DefaultView
            dtColumnsView.Sort = columnHeading
            Dim dtColumns As DataTable = dtColumnsView.ToTable(True, columnHeading)


            '2/ select distinct rowHeading based off param array
            Dim dtRows As DataTable = (dt.DefaultView).ToTable(True, rowHeading)

            '3/ build the crosstab output table
            Dim dtCrosstab As New DataTable
            '*** add the rowHeadings as strings, even though they may in fact be typed differently...
            For Each colName As String In rowHeading
                '** find source column, copy to dtCrosstab.  There's no copy/clone function so create with samename and type
                'note: datatable merge can be used to merge one table into another
                Dim myC As New DataColumn(colName, dt.Columns(colName).DataType)
                dtCrosstab.Columns.Add(myC)

            Next

            '*** now add output columns, yeah its confusing, we have rows of columns...
            For Each col As DataRow In dtColumns.Rows
            '** add a column of the columnHeading datatype but named as one of the rows of columns :-)
            '** note that col(columnHeading) may be null if the underlying data is a left join and has null outputs
            If col(columnHeading) & String.Empty = String.Empty Then
                '*** deal with null
                ' Dim myC As New DataColumn("<>", dt.Columns(columnHeading).DataType)
                ' dtCrosstab.Columns.Add(myC)
                ' Trace.Warn("col " & myC.ColumnName)
            Else
                Dim myC As New DataColumn(col(columnHeading), dt.Columns(columnHeading).DataType)
                dtCrosstab.Columns.Add(myC)
                Trace.Warn("col " & myC.ColumnName)
            End If


        Next

            'run sum compute on the data to calculate the value and build the return dataset
            '*** add a row of data
            Trace.Warn("step3 complete")
        'gvPrice.DataSource = dtRows
        'gvPrice.DataBind()
        'Exit Function
        'debug dump dtRows



        '4/ run the compute to populate
        For Each myR In dtRows.Rows

                Dim newR As DataRow = dtCrosstab.NewRow
                Dim sbFilter As New StringBuilder
                '*** populate the rowHEadings
                For Each colName As String In rowHeading
                    newR(colName) = myR(colName)
                sbFilter.Append(colName)

                '*** refactored
                '2022-10-05 bug fix, detect null values and use isNull rather than =
                If (myR(colName)).ToString.Trim = String.Empty Then
                    sbFilter.Append(" IS NULL")
                ElseIf newR(colName).GetType = GetType(String) Then
                    sbFilter.Append("='")
                    sbFilter.Append(myR(colName))
                    sbFilter.Append("'")
                Else
                    sbFilter.Append("=")
                    sbFilter.Append(myR(colName))
                End If

                sbFilter.Append(" AND ")

                Next
                Dim sbFilterLen As Long = sbFilter.Length

            'want to capture sbFilter length at this point so we can keep varying it


            '** and inner loop for the values, using compute
            For Each col As DataRow In dtColumns.Rows

                '*** BUG, we added all the cols but now it cannot find col 18. huh
                '*** yeah you need to cast to string, else it uses col(columnHeading) as a numeric index
                sbFilter.Append(columnHeading)
                If col(columnHeading).ToString.Trim = String.Empty Then
                    sbFilter.Append(" IS NULL")

                ElseIf newR(col(columnHeading).ToString).GetType = GetType(String) Then
                    sbFilter.Append("='")
                    sbFilter.Append(col(columnHeading))
                    sbFilter.Append("'")
                Else
                    sbFilter.Append("=")
                    sbFilter.Append(col(columnHeading))
                End If


                'Trace.Warn("sbFilter=" & sbFilter.ToString)

                '** not sure why we need this test...
                If Not col(columnHeading).ToString.Trim = String.Empty Then
                    '*** only compute if we have a valid target column
                    newR(col(columnHeading).ToString) = dt.Compute("sum(" & valueField & ")", sbFilter.ToString)
                End If




                'filter is where rh1,rh2 etc = their values and columnheading_field=col(columnheading) 
                'but its made more complex because you need to ensure types are properly dealt with
                'e.g. if f1 is string, f2 double then f1='this' AND f2=99

                '*** compute probably supports max, min, sum, avg. we could accept a defining param and test and throw and error if bad

                sbFilter.Remove(sbFilterLen, sbFilter.Length - sbFilterLen)

            Next
            dtCrosstab.Rows.Add(newR)
            Next
            Return dtCrosstab

        ' Catch ex As Exception
        'Trace.Warn(ex.ToString)
        'Return Nothing
        ' End Try
    End Function

    Private Sub gvPrice_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles gvPrice.RowDataBound
        '*** Make the meta table entries active hyperlinks to allow drilldown
        '*** note, the column count is zero because the table has not been rendered yet...instead we have to iterate the controls

        If e.Row.RowType <> DataControlRowType.DataRow Then Exit Sub

        For Each fc As DataControlFieldCell In e.Row.Controls
            If Not String.IsNullOrWhiteSpace(HttpUtility.HtmlDecode(fc.Text)) Then
                Dim lb As New LinkButton
                '*** you can suppress the hyperlink colouration as below
                'lb.Style.Add("text-decoration", "none")
                'lb.Style.Add("color", "inherit")
                lb.Text = fc.Text
                '*** send row@column reference when clicked
                '*** dynamic controls won't raise events or pass arguements, you need to handle via client script
                '*** but they do raise postbacks

                '*** we have to detect the zero ordinal entry and flag this as BAN
                '*** use ClientID rather than the cell value
                'If e.Row.Controls.IndexOf(fc) = 0 Then
                'lb.OnClientClick = "document.getElementById('hdnPrice').value='SEQ" & HttpUtility.HtmlDecode(fc.ID) & "'"
                'Else

                '*** return row, column position in the gridview
                ' lb.OnClientClick = "document.getElementById('hdnPrice').value='" & e.Row.RowIndex & "," & e.Row.Controls.IndexOf(fc) & "'"

                '*** return the rowSeq an column ordinal descriptor via commandArguement
                lb.CommandArgument = gvPrice.DataKeys(e.Row.RowIndex).Item("rowSeq") & "," & TryCast(gvPrice.HeaderRow.Controls(e.Row.Controls.IndexOf(fc)), DataControlFieldHeaderCell).Text


                'End If
                fc.Controls.Add(lb)
            End If
        Next
    End Sub

    Private Sub gvPrice_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles gvPrice.RowCommand
        Trace.Warn("rowCommand " & e.CommandArgument)

        '*** highlight the gvPrice row
        Dim gvr As GridViewRow = TryCast(CType(e.CommandSource, Control).NamingContainer, GridViewRow)   '*** cast to GridViewRow
        If gvr Is Nothing Then
            '*** gvr will be nothing if you click on sort, for example because the control will be the gridview and the namingcontainer the page.
            '*** This happens if you are in the gridview rowCommand event, because standard events like sort, and button columns will bubble through this
            '*** on their way to their own handlers
            Trace.Warn("gvr nothing")
        Else
            gvPrice.SelectedIndex = gvr.RowIndex
        End If



        Dim rc() As String = Split(e.CommandArgument, ",")
        If rc.Length <> 2 Then
            statusBar.InnerText = "ERROR: incorrect target"
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
        Session.Add("detail", oDS.Tables(0))


        '** for NRE revenue in the PCMrevenue column, multiply this up by 36
        For Each myR As DataRow In oDS.Tables(0).Rows
            If myR("ChargeType") = "NRE" Then myR("PCMrevenue") *= 36
        Next


        gvDrilldown.DataSource = oDS.Tables(0)
        gvDrilldown.DataBind()
        divDetail.Visible = True
        oConn.Dispose()


    End Sub

    Protected Sub lbDetail_Click(sender As Object, e As ImageClickEventArgs) Handles lbDetail.Click
        exportRFC4180table(TryCast(Session("detail"), DataTable), "detail.csv", False)


    End Sub
End Class