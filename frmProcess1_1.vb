Imports System.Data.SqlClient
Imports BarcodeLib

Public Class frmProcess1_1
    Dim ColumnIndex As Integer = 0
    Dim RowIndex As Integer = 0
    Dim RowIndexBigSet As Integer = -1
    '0 ไม่มีอะไร, 1 เลือกรายการจากขึ้นเตา, 2 เพิ่มรายการ
    Dim AddListAction As Integer = 0
    Private Sub BtnSaveOrder_Click(sender As Object, e As EventArgs) Handles BtnSaveOrder.Click
        Try
            If (tbBarcode.Text = "") Then
                MsgBox("กรุณาสร้างรุ่น", MsgBoxStyle.Critical, "Error")
                Exit Sub
            End If
            Dim result As DialogResult = MessageBox.Show("ยืนยันบันทึก", "Confirm", MessageBoxButtons.YesNo)
            If result = DialogResult.Yes Then
                Dim objDB As objDB = New objDB()
                Dim Con As SqlConnection = objDB.ConnectDatabase()
                Dim _SQL As String = "INSERT INTO [dbo].[BigSet]([Model],[CreateDate],[StatusJob])
                                VALUES('" + tbBarcode.Text + "',getdate(),'1')"
                objDB.ExecuteSQL(_SQL, Con)
                BigSet()
                objDB.DisconnectDatabase(Con)
                MsgBox("บันทึกข้อมูลเรียบร้อย", MsgBoxStyle.Information, "Information")
                GetModel()
                BtnSaveOrder.Enabled = False
            End If


            'Dim barcode As Barcode = New Barcode()
            'Dim foreColor As Color = Color.Black
            'Dim backColor As Color = Color.Transparent
            'Dim img As Image = barcode.Encode(TYPE.CODE128, tbBarcode.Text, foreColor, backColor, CInt(picBarcode.Width * 0.8), CInt(picBarcode.Height * 0.8))
            'picBarcode.Image = img

            tbBarcode.Text = ""
        Catch ex As Exception
            InsertLogError(ex.Message, "1", "", username)
        End Try

    End Sub

    Private Sub frmProcess1_1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            ComboBox1.SelectedIndex = 0
            cbStatus.SelectedIndex = 0
            ComboBox2.SelectedIndex = 0

            'GetUpStove()
            BigSet()
            GetModel()

            If dgvPart1.Rows.Count > 0 Then
                SetdgbPart1(RowIndex)
            End If
        Catch ex As Exception
            InsertLogError(ex.Message, "1", "", username)
        End Try


    End Sub

    Private Sub GetUser()
        cbUser.Items.Clear()
        Dim objDB As objDB = New objDB()
        Dim Con As SqlConnection = objDB.ConnectDatabase()
        Dim _SQL As String = "SELECT * FROM UserLogon ORDER BY UserID ASC"
        Dim dtUser As DataTable = objDB.SelectSQL(_SQL, Con)
        objDB.DisconnectDatabase(Con)
        If dtUser.Rows.Count > 0 Then
            For i As Integer = 0 To dtUser.Rows.Count - 1
                cbUser.Items.Add(dtUser.Rows(i)("UserID").ToString.Trim)
            Next
            cbUser.SelectedIndex = 0
        End If

    End Sub

    Dim dtadv1 As DataTable = New DataTable()
    Private Sub GetUpStove()
        Try
            'DbSummary = {0, 0, 0, 0}
            'SummaryWeight = 0
            'tbWeight.Text = 0.00
            'cbUser.SelectedIndex = 0
            'tbRemark.Text = ""
            'DisableData()
            Dim objDB As objDB = New objDB()
            Dim Con As SqlConnection = objDB.ConnectDatabase()
            Dim _SQL As String = "SELECT vpk.BARCODE_NAME 'ชื่อบาร์โค๊ต', vpk.Process1 'น้ำหนักกล้า', vpk.Process2 'น้ำหนักสร้อย', vpk.Process3 'เศษ_ลงเตา', vpk.Process7 'จำนวนเส้น', vj.น้ำหนักทอง, pw.แนบ_เศษทอง, pw.รีดลาย_เศษทอง, vpk.Timer 'ชื่อเตา', vpk.BarcodeText, vpk.Lot
                FROM vProcessKiln vpk
                inner join vJobs vj on vpk.BarcodeText = vj.BarcodeText
                inner join ProcessWire pw on vpk.BarcodeText = pw.BarcodeText
                where vpk.LastStatus = 'เสร็จสิ้น' and vj.BarcodeText " + IIf(cbStatus.Text = "กำลังดำเนินการ", "not", "") + " in (SELECT BarcodeText FROM UpStove) "

            Dim dtUpStove As DataTable = objDB.SelectSQL(_SQL, Con)
            'SumAdmin = 0

            objDB.DisconnectDatabase(Con)
            dgvPart1.Rows.Clear()
            If dtUpStove.Rows.Count > 0 Then
                For i As Integer = 0 To dtUpStove.Rows.Count - 1
                    dgvPart1.Rows.Add(dtUpStove.Rows(i)("ชื่อบาร์โค๊ต"), dtUpStove.Rows(i)("จำนวนเส้น"), Format(dtUpStove.Rows(i)("น้ำหนักทอง"), "n2"), Format(dtUpStove.Rows(i)("แนบ_เศษทอง"), "n2"), Format(dtUpStove.Rows(i)("รีดลาย_เศษทอง"), "n2"), Format(dtUpStove.Rows(i)("น้ำหนักกล้า"), "n2"), Format(dtUpStove.Rows(i)("น้ำหนักสร้อย"), "n2"), Format(dtUpStove.Rows(i)("เศษ_ลงเตา"), "n2"), dtUpStove.Rows(i)("ชื่อเตา"), dtUpStove.Rows(i)("BarcodeText"), dtUpStove.Rows(i)("Lot"))
                Next
            End If
        Catch ex As Exception
            InsertLogError(ex.Message, "1", "", username)
        End Try

    End Sub

    Private Sub BigSet()
        Try
            Dim objDB As objDB = New objDB()
            Dim Con As SqlConnection = objDB.ConnectDatabase()
            Dim _SQL As String = "SELECT * FROM UpStove where StatusJob = '" + IIf(ComboBox1.Text = "กำลังดำเนินการ", "2", "3") + "' Order by UpStoveId desc"
            Dim dtBigSet As DataTable = objDB.SelectSQL(_SQL, Con)
            'SumAdmin = 0
            dgvUpStove.Rows.Clear()
            objDB.DisconnectDatabase(Con)
            If dtBigSet.Rows.Count > 0 Then
                For i As Integer = 0 To dtBigSet.Rows.Count - 1
                    dgvUpStove.Rows.Add(False, dtBigSet.Rows(i)("BARCODE_NAME"), dtBigSet.Rows(i)("จำนวนเส้น"), dtBigSet.Rows(i)("น้ำหนักสร้อยเปล่า"), dtBigSet.Rows(i)("เปอร์เซ็นสร้อย"), dtBigSet.Rows(i)("CreateDate"), dtBigSet.Rows(i)("UpStoveId"), dtBigSet.Rows(i)("StatusJob"))
                Next

            End If
        Catch ex As Exception
            InsertLogError(ex.Message, "1", "", username)
        End Try

    End Sub

    Private Sub GetModel()
        Try
            Dim objDB As objDB = New objDB()
            Dim Con As SqlConnection = objDB.ConnectDatabase()
            Dim _SQL As String = "SELECT  * FROM BigSet where StatusJob='" + IIf(ComboBox2.Text = "กำลังดำเนินการ", "1", "2") + "' order by BigSetId desc"
            Dim dtModel As DataTable = objDB.SelectSQL(_SQL, Con)
            'SumAdmin = 0
            dgvModel.Rows.Clear()
            objDB.DisconnectDatabase(Con)
            If dtModel.Rows.Count > 0 Then

                For i As Integer = 0 To dtModel.Rows.Count - 1
                    dgvModel.Rows.Add(dtModel.Rows(i)("Model"), dtModel.Rows(i)("CreateDate"), dtModel.Rows(i)("StatusJob"), dtModel.Rows(i)("BigSetId"))
                Next

            End If
        Catch ex As Exception
            InsertLogError(ex.Message, "1", "", username)
        End Try
    End Sub

    Private Sub dgvPart1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvPart1.CellClick
        Try
            ColumnIndex = e.ColumnIndex
            If e.RowIndex <> -1 Then
                Try
                    RowIndex = e.RowIndex
                    If CDbl(dgvPart1.Item(1, RowIndex).Value) <= 0 Then
                        MsgBox("จำนวนเส้นไม่เพียงพอ", MsgBoxStyle.Critical, "Error")
                        EnableFalse()
                        BtnEdit.Enabled = False
                        Exit Sub
                    End If
                    SetdgbPart1(RowIndex)
                    dtpUpStove.Value = Now
                    BtnEdit.Enabled = True

                Catch ex As Exception

                End Try

            End If
        Catch ex As Exception
            InsertLogError(ex.Message, "1", "", username)
        End Try

    End Sub

    Private Sub SetdgbPart1(RowIndex As Int16)
        Try
            tbWeight.Text = ""
            TextBox4.Text = ""
            TextBox3.Text = ""
            TextBox5.Text = ""
            TextBox6.Text = ""
            TextBox7.Text = ""
            TextBox8.Text = ""

            Dim LineQtyArr() As String
            LineQtyArr = dgvPart1.Item(1, RowIndex).Value.Split(",")
            Dim LineQty As Double = 0.0
            For Each element As String In LineQtyArr
                LineQty = LineQty + CDbl(element)
            Next

            If Not dgvPart1.Item(1, RowIndex).Value.ToString.Contains(",") Then
                Dim DoubleLQty As Double = Convert.ToDouble(dgvPart1.Item(1, RowIndex).Value) - Convert.ToInt32(Convert.ToDouble(dgvPart1.Item(1, RowIndex).Value))
                Dim StrLQty As String = Convert.ToInt32(Convert.ToDouble(dgvPart1.Item(1, RowIndex).Value))
                Dim LenLQty As Integer = Len(StrLQty)
                If LenLQty = 4 Then
                    Dim s1 As Integer = Mid(StrLQty, 1, 2)
                    Dim s2 As Integer = Mid(StrLQty, 3, 2)
                    LineQty = s1 + s2 + DoubleLQty
                End If

                If LenLQty = 6 Then
                    Dim s1 As Integer = Mid(StrLQty, 1, 2)
                    Dim s2 As Integer = Mid(StrLQty, 3, 2)
                    Dim s3 As Integer = Mid(StrLQty, 5, 2)
                    LineQty = s1 + s2 + s3 + DoubleLQty
                End If
            End If

            GetUser()

            tbSetStove.Text = dgvPart1.Item(8, RowIndex).Value
            TextBox1.Text = dgvPart1.Item(0, RowIndex).Value
            tbเศษทอง.Text = dgvPart1.Item(3, RowIndex).Value

            TextBox2.Text = LineQty.ToString("N2")

            If cbStatus.Text = "กำลังดำเนินการ" Then

                cbUser.Enabled = True
                dtpUpStove.Enabled = True
                TextBox4.ReadOnly = False
                tbWeight.ReadOnly = False
            Else
                BtnSave.Enabled = False
                BtnCancel.Enabled = False
                cbUser.Enabled = False
                dtpUpStove.Enabled = False
                TextBox4.ReadOnly = True
                tbWeight.ReadOnly = True
                GetDataSuccess(dgvPart1.Item(9, RowIndex).Value)
            End If
        Catch ex As Exception
            InsertLogError(ex.Message, "1", "", username)
        End Try

    End Sub

    Private Sub GetDataSuccess(BarcodeText As String)
        Try
            Dim objDB As objDB = New objDB()
            Dim Con As SqlConnection = objDB.ConnectDatabase()
            Dim _SQL As String = "SELECT * FROM UpStove where BarcodeText = '" & BarcodeText & "'"
            Dim dtModel As DataTable = objDB.SelectSQL(_SQL, Con)
            'SumAdmin = 0
            objDB.DisconnectDatabase(Con)
            If dtModel.Rows.Count > 0 Then

                TextBox4.Text = dtModel.Rows(0)("คืน70")
                tbWeight.Text = dtModel.Rows(0)("น้ำหนักสร้อยเปล่า")

            End If
        Catch ex As Exception
            InsertLogError(ex.Message, "1", "", username)
        End Try
    End Sub

    Private Sub tbWeight_TextChanged(sender As Object, e As EventArgs) Handles tbWeight.TextChanged
        Try
            If tbWeight.Text <> "" Then
                TextBox3.Text = Format(CDbl(dgvPart1.Item(6, RowIndex).Value) / CDbl(tbWeight.Text), "n2").ToString
                TextBox5.Text = Format(CDbl(dgvPart1.Item(4, RowIndex).Value) / CDbl(TextBox3.Text), "n2").ToString
                TextBox6.Text = Format(CDbl(dgvPart1.Item(7, RowIndex).Value) / CDbl(TextBox3.Text), "n2").ToString
                TextBox7.Text = (CDbl(tbWeight.Text) + CDbl(TextBox5.Text) + CDbl(TextBox6.Text)).ToString + CDbl(tbเศษทอง.Text.ToString().Replace(",", ""))
                'TextBox8.Text = (Format((CDbl(tbWeight.Text) * 96.5) / CDbl(TextBox7.Text), "n2")).ToString
                TextBox8.Text = (Format((CDbl(dgvPart1.Item(2, RowIndex).Value) * 96.5) / CDbl(TextBox7.Text), "n2")).ToString
            End If
        Catch ex As Exception
            InsertLogError(ex.Message, "1", "", username)
        End Try

    End Sub

    Private Sub BtnEdit_Click(sender As Object, e As EventArgs) Handles BtnEdit.Click
        'InsertUpStove()
        TextBox4.Enabled = True
        tbWeight.Enabled = True
        BtnSave.Enabled = True
        BtnCancel.Enabled = True
    End Sub

    Private Sub BtnSave_Click(sender As Object, e As EventArgs) Handles BtnSave.Click
        Dim RowIndex As Integer = dgvPart1.CurrentRow.Index
        If RowIndex <> -1 Then
            Try
                'If dgvPart1.Rows(RowIndex).Cells(2).Value.ToString().Replace(",", "") >= tbWeight.Text Then
                BtnEdit.Enabled = False
                InsertUpStove()
                'Else
                '    MsgBox("น้ำหนักไม่เพียงพอ", MsgBoxStyle.Critical, "Error")
                '    Exit Sub
                'End If
            Catch ex As Exception
                InsertLogError(ex.Message, "1", "", username)
            End Try

        End If



    End Sub

    'Private Sub UpdateUpStove()
    '    Try
    '        If TextBox4.Text = "" Then
    '            MsgBox("กรุณากรอก คืน 70", MsgBoxStyle.Critical, "Error")
    '            Exit Sub
    '        ElseIf tbWeight.Text = "" Then
    '            MsgBox("กรุณากรอก นน.สร้อยเปล่า", MsgBoxStyle.Critical, "Error")
    '            Exit Sub
    '        End If
    '        Dim result As DialogResult = MessageBox.Show("ยืนยันบันทึก", "Confirm", MessageBoxButtons.YesNo)
    '        If result = DialogResult.Yes Then

    '            Dim objDB As objDB = New objDB()
    '            Dim Con As SqlConnection = objDB.ConnectDatabase()
    '            Dim _SQL As String = "UPDATE [dbo].[UpStove] SET [คืน70] = '" & TextBox4.Text & "',[น้ำหนักสร้อยเปล่า] = '" & tbWeight.Text & "',[น้ำหนักรวม] = '" & TextBox7.Text & "',[เปอร์เซ็นสร้อย] = '" & TextBox8.Text & "', [StatusJob]='2' WHERE BarcodeText = '" & dgvPart1.Item(9, RowIndex).Value & "'"
    '            objDB.ExecuteSQL(_SQL, Con)
    '            GetUpStove()
    '            BigSet()
    '            EnableFalse()
    '            objDB.DisconnectDatabase(Con)
    '            MsgBox("บันทึกข้อมูลเรียบร้อย", MsgBoxStyle.Information, "Information")
    '            BtnEdit.Enabled = False
    '        End If
    '    Catch ex As Exception

    '    End Try

    'End Sub

    Private Sub InsertUpStove()
        Try
            Dim return70 As Double = 0
            If TextBox4.Text <> "" Then
                'MsgBox("กรุณากรอก คืน 70", MsgBoxStyle.Critical, "Error")
                'Exit Sub
                return70 = TextBox4.Text.Replace(",", "")
            End If


            If tbWeight.Text = "" Then
                MsgBox("กรุณากรอก นน.สร้อยเปล่า", MsgBoxStyle.Critical, "Error")
                Exit Sub
            End If
            Dim result As DialogResult = MessageBox.Show("ยืนยันบันทึก", "Confirm", MessageBoxButtons.YesNo)
            If result = DialogResult.Yes Then

                Dim objDB As objDB = New objDB()
                Dim Con As SqlConnection = objDB.ConnectDatabase()
                Dim _SQL As String = "INSERT INTO [dbo].[UpStove]([BARCODE_NAME],[จำนวนเส้น],[น้ำหนักทอง],[แนบ_เศษทอง],[รีดลาย_เศษทอง],[น้ำหนักกล้า],[น้ำหนักสร้อย],[เศษ_ลงเตา],[Username],[ชุดที่ลงเตา],[CreateDate],[คืน70],[น้ำหนักสร้อยเปล่า],[น้ำหนักรวม],[เปอร์เซ็นสร้อย],[StatusJob],[Lot],[BarcodeText]) VALUES('" & dgvPart1.Item(0, RowIndex).Value & "','" & CDbl(TextBox2.Text.ToString().Replace(",", "")) & "','" & CDbl(dgvPart1.Item(2, RowIndex).Value.ToString().Replace(",", "")) & "','" & CDbl(dgvPart1.Item(3, RowIndex).Value.ToString().Replace(",", "")) & "','" & CDbl(dgvPart1.Item(4, RowIndex).Value) & "','" & CDbl(dgvPart1.Item(5, RowIndex).Value.ToString().Replace(",", "")) & "','" & CDbl(dgvPart1.Item(6, RowIndex).Value.ToString().Replace(",", "")) & "','" & CDbl(dgvPart1.Item(7, RowIndex).Value.ToString().Replace(",", "")) & "','" & cbUser.Text & "','" & dgvPart1.Item(8, RowIndex).Value & "','" & Format(dtpUpStove.Value, "yyyy-MM-dd HH:mm") & "','" & return70 & "','" & tbWeight.Text.Replace(",", "") & "','" & TextBox7.Text.Replace(",", "") & "','" & TextBox8.Text.Replace(",", "") & "','2','" & dgvPart1.Item(10, RowIndex).Value & "','" & dgvPart1.Item(9, RowIndex).Value & "')"
                objDB.ExecuteSQL(_SQL, Con)
                GetUpStove()
                BigSet()
                EnableFalse()
                objDB.DisconnectDatabase(Con)
                MsgBox("บันทึกข้อมูลเรียบร้อย", MsgBoxStyle.Information, "Information")
                BtnEdit.Enabled = False
            End If
        Catch ex As Exception
            InsertLogError(ex.Message, "1", "", username)
        End Try

    End Sub

    Private Sub ComboBox1_SelectedValueChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedValueChanged
        BigSet()
        If ComboBox1.Text = "เสร็จสิ้น" Then
            btnMove.Enabled = False
        ElseIf ComboBox1.Text = "กำลังดำเนินการ" And RowIndexBigSet > -1 And ComboBox2.Text = "กำลังดำเนินการ" Then
            'btnMove.Enabled = True
        End If
    End Sub

    Private Sub cbStatus_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbStatus.SelectedIndexChanged
        Try
            tbWeight.Text = ""
            TextBox4.Text = ""
            TextBox3.Text = ""
            TextBox5.Text = ""
            TextBox6.Text = ""
            TextBox7.Text = ""
            TextBox8.Text = ""
            GetUpStove()
            If dgvPart1.Rows.Count > 0 Then
                RowIndex = 0
                SetdgbPart1(RowIndex)
            End If
        Catch ex As Exception
            InsertLogError(ex.Message, "1", "", username)
        End Try

    End Sub

    Private Function GetNewModel() As String
        Try
            Dim objDB As objDB = New objDB()
            Dim Con As SqlConnection = objDB.ConnectDatabase()
            Dim _SQL As String = "SELECT TOP (1) * FROM BigSet order by BigSetId desc"
            Dim dtBigSet As DataTable = objDB.SelectSQL(_SQL, Con)

            If dtBigSet.Rows.Count > 0 Then
                Return "B" & (CInt(dtBigSet.Rows(0)("Model").ToString.Substring(1, 7)) + 1).ToString("0000000")
            Else
                Return "B0000001"
            End If
        Catch ex As Exception
            InsertLogError(ex.Message, "1", "", username)
        End Try

    End Function

    Private Sub BtnCreateBarcode_Click(sender As Object, e As EventArgs) Handles BtnCreateBarcode.Click
        Try
            tbBarcode.Text = GetNewModel()
            BtnSaveOrder.Enabled = True

            Dim barcode As Barcode = New Barcode()
            Dim foreColor As Color = Color.Black
            Dim backColor As Color = Color.Transparent
            Dim img As Image = barcode.Encode(TYPE.CODE128, tbBarcode.Text, foreColor, backColor, CInt(picBarcode.Width * 0.8), CInt(picBarcode.Height * 0.8))
            picBarcode.Image = img
        Catch ex As Exception
            InsertLogError(ex.Message, "1", "", username)
        End Try


    End Sub

    Private Sub ComboBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox2.SelectedIndexChanged
        GetModel()
        If ComboBox2.Text = "เสร็จสิ้น" Then
            Button5.Enabled = False
            btnMove.Enabled = False
        End If
    End Sub

    Private Sub dgvModel_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvModel.CellClick
        Try
            If e.RowIndex <> -1 Then
                RowIndexBigSet = e.RowIndex
                TextBox9.Text = dgvModel.Item(0, RowIndexBigSet).Value
                If ComboBox2.Text = "กำลังดำเนินการ" Then

                    'If dgvUpStove.Rows.Count > 0 Then
                    '    btnMove.Enabled = True
                    'Else
                    '    btnMove.Enabled = False
                    'End If

                    If GetBigsetDetailDt(dgvModel.Item(3, RowIndexBigSet).Value).Rows.Count > 0 Then
                        Button5.Enabled = True
                    Else
                        Button5.Enabled = False
                    End If
                    CheckEnableBtnMove()
                Else
                    btnMove.Enabled = False
                    TextBox9.Enabled = False
                    ComboBox3.Enabled = False
                    TextBox10.Enabled = False
                    ComboBox4.Enabled = False
                    TextBox11.Enabled = False
                    Button4.Enabled = False
                    Button5.Enabled = False
                End If
                ComboBox3.SelectedIndex = 0
                ComboBox4.SelectedIndex = 0
                GetBigSetDetail(dgvModel.Item(3, RowIndexBigSet).Value)
            End If
        Catch ex As Exception
            InsertLogError(ex.Message, "1", "", username)
        End Try


    End Sub



    Private Sub btnMove_Click(sender As Object, e As EventArgs) Handles btnMove.Click
        Try
            If (RowIndexBigSet > -1) Then
                Dim result As DialogResult = MessageBox.Show("ยืนยันบันทึก", "Confirm", MessageBoxButtons.YesNo)
                If result = DialogResult.Yes Then
                    For Each row As DataGridViewRow In dgvUpStove.Rows
                        'If Not row.IsNewRow Then
                        '    MessageBox.Show(row.Cells(0).Value.ToString & "," & row.Cells(1).Value.ToString)
                        'End If
                        If (row.Cells(0).Value) Then
                            InsertBigsetDetail(row.Cells(6).Value)
                            btnMove.Enabled = False
                            UpdateUpStove(row.Cells(6).Value, 3)
                        End If
                    Next
                    BigSet()
                    GetBigSetDetail(dgvModel.Item(3, RowIndexBigSet).Value)
                    MsgBox("บันทึกข้อมูลเรียบร้อย", MsgBoxStyle.Information, "Information")


                End If
            Else
                MsgBox("กรุณาเลือกรุ่น", MsgBoxStyle.Critical, "Error")
            End If
        Catch ex As Exception
            InsertLogError(ex.Message, "1", "", username)
        End Try

    End Sub

    Private Sub InsertBigsetDetail(Id As Int16)
        Try
            Dim objDB As objDB = New objDB()
            Dim Con As SqlConnection = objDB.ConnectDatabase()
            Dim _SQL As String = "INSERT INTO BigSetDetail([BigsetId],[ไซส์],[เส้น],[น้ำหนัก],[เปอร์เซ็นสร้อย],[CreateDate],[ชนิดทอง],[เพิ่มเติม],[StatusJob],[Lot],[BarcodeText])
 SELECT '" & dgvModel.Item(3, RowIndexBigSet).Value & "',[BARCODE_NAME],[จำนวนเส้น],[น้ำหนักทอง],[เปอร์เซ็นสร้อย],getdate(),'','','1',[Lot],[BarcodeText]FROM UpStove where UpStoveId = '" & Id & "'   "
            objDB.ExecuteSQL(_SQL, Con)
            objDB.DisconnectDatabase(Con)

        Catch ex As Exception
            InsertLogError(ex.Message, "1", "", username)
        End Try

    End Sub

    Private Function CheckBigSetDetail(Id As Int16) As Int16
        Try
            Dim objDB As objDB = New objDB()
            Dim Con As SqlConnection = objDB.ConnectDatabase()
            Dim _SQL As String = "SELECT * FROM BigSetDetail where BigsetId = '" & Id & "' and [ไซส์] = '" & ComboBox3.Text & "' and [ชนิดทอง] = '" & ComboBox4.Text & "'"
            Dim dtBigSet As DataTable = objDB.SelectSQL(_SQL, Con)
            If dtBigSet.Rows.Count > 0 Then
                Return dtBigSet.Rows(0)("BigsetDetailId")
            Else
                Return 0
            End If
        Catch ex As Exception
            InsertLogError(ex.Message, "1", "", username)
        End Try

    End Function

    Private Sub UpdateBigsetDetailManual(BigsetDetailId As Int16)
        Try
            Dim objDB As objDB = New objDB()
            Dim Con As SqlConnection = objDB.ConnectDatabase()
            Dim _SQL As String = "UPDATE BigSetDetail SET [น้ำหนัก] = [น้ำหนัก] + " & TextBox10.Text & " WHERE BigsetDetailId = '" + BigsetDetailId.ToString() + "'"
            objDB.ExecuteSQL(_SQL, Con)
            MsgBox("บันทึกข้อมูลเรียบร้อย", MsgBoxStyle.Information, "Information")
            objDB.DisconnectDatabase(Con)
        Catch ex As Exception
            InsertLogError(ex.Message, "1", "", username)
        End Try
    End Sub
    Private Sub InsertBigsetDetailManual(Id As Int16)
        Try
            Dim objDB As objDB = New objDB()
            Dim Con As SqlConnection = objDB.ConnectDatabase()
            Dim _SQL As String = "INSERT INTO BigSetDetail ([BigsetId],[ไซส์],[เส้น],[น้ำหนัก],[เปอร์เซ็นสร้อย],[CreateDate],[ชนิดทอง],[เพิ่มเติม],[StatusJob],[Lot],[BarcodeText])
            VALUES('" & Id & "','" & ComboBox3.Text & "',NULL,'" & TextBox10.Text & "',NULL,getdate(),'" & ComboBox4.Text & "','" & TextBox11.Text & "','1','','')"
            objDB.ExecuteSQL(_SQL, Con)
            MsgBox("บันทึกข้อมูลเรียบร้อย", MsgBoxStyle.Information, "Information")
            objDB.DisconnectDatabase(Con)
        Catch ex As Exception
            InsertLogError(ex.Message, "1", "", username)
        End Try

    End Sub

    Private Sub UpdateUpStove(Id As Int16, StatusJob As Int16)
        Try
            Dim objDB As objDB = New objDB()
            Dim Con As SqlConnection = objDB.ConnectDatabase()
            Dim _SQL As String = "UPDATE UpStove SET [StatusJob] = '" & StatusJob & "'  WHERE UpStoveId = '" & Id & "'"
            objDB.ExecuteSQL(_SQL, Con)
            objDB.DisconnectDatabase(Con)
        Catch ex As Exception
            InsertLogError(ex.Message, "1", "", username)
        End Try

    End Sub

    Private Sub GetBigSetDetail(BigSetId As Int16)
        Try
            Dim objDB As objDB = New objDB()
            Dim Con As SqlConnection = objDB.ConnectDatabase()
            Dim _SQL As String = "SELECT * FROM BigSetDetail where BigsetId = '" & BigSetId & "'"
            Dim dtAddList As DataTable = objDB.SelectSQL(_SQL, Con)
            dgvAddList.Rows.Clear()
            objDB.DisconnectDatabase(Con)
            If dtAddList.Rows.Count > 0 Then

                For i As Integer = 0 To dtAddList.Rows.Count - 1
                    dgvAddList.Rows.Add(dtAddList.Rows(i)("ไซส์"), dtAddList.Rows(i)("เส้น"), dtAddList.Rows(i)("น้ำหนัก"), dtAddList.Rows(i)("เปอร์เซ็นสร้อย"), dtAddList.Rows(i)("CreateDate"), dtAddList.Rows(i)("ชนิดทอง"), dtAddList.Rows(i)("เพิ่มเติม"))
                Next

            End If
        Catch ex As Exception
            InsertLogError(ex.Message, "1", "", username)
        End Try
    End Sub

    Private Function GetBigsetDetailDt(BigSetId) As DataTable
        Dim dtBigSetDetail As DataTable = New DataTable()
        Try
            Dim objDB As objDB = New objDB()
            Dim Con As SqlConnection = objDB.ConnectDatabase()
            Dim _SQL As String = "SELECT * FROM BigSetDetail where BigsetId = '" & BigSetId & "'"
            dtBigSetDetail = objDB.SelectSQL(_SQL, Con)
            objDB.DisconnectDatabase(Con)
        Catch ex As Exception
            InsertLogError(ex.Message, "1", "", username)
        End Try
        Return dtBigSetDetail
    End Function

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Try
            If TextBox9.Text = "" Then
                MsgBox("กรุณากรอกรุ่น", MsgBoxStyle.Critical, "Error")
                Exit Sub
            ElseIf TextBox10.Text = "" Then
                MsgBox("กรุณากรอกน้ำหนัก", MsgBoxStyle.Critical, "Error")
                Exit Sub
            End If
            Dim result As DialogResult = MessageBox.Show("ยืนยันบันทึก", "Confirm", MessageBoxButtons.YesNo)
            If result = DialogResult.Yes Then
                Dim BigsetDetailId As Int16 = CheckBigSetDetail(dgvModel.Item(3, RowIndexBigSet).Value)
                If BigsetDetailId > 0 Then
                    UpdateBigsetDetailManual(BigsetDetailId)
                Else
                    InsertBigsetDetailManual(dgvModel.Item(3, RowIndexBigSet).Value)
                End If

                GetBigSetDetail(dgvModel.Item(3, RowIndexBigSet).Value)
                EnableAddListManual(False)
                TextBox10.Text = ""
                TextBox11.Text = ""
            End If
        Catch ex As Exception
            InsertLogError(ex.Message, "1", "", username)
        End Try


    End Sub

    Private Sub TextBox10_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TextBox10.KeyPress
        If (Not Char.IsControl(e.KeyChar) _
             AndAlso (Not Char.IsDigit(e.KeyChar) _
             AndAlso (e.KeyChar <> Microsoft.VisualBasic.ChrW(46)))) Then
            e.Handled = True
        End If
    End Sub

    Private Sub TextBox4_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TextBox4.KeyPress
        If (Not Char.IsControl(e.KeyChar) _
             AndAlso (Not Char.IsDigit(e.KeyChar) _
             AndAlso (e.KeyChar <> Microsoft.VisualBasic.ChrW(46)))) Then
            e.Handled = True
        End If
    End Sub

    Private Sub tbWeight_KeyPress(sender As Object, e As KeyPressEventArgs) Handles tbWeight.KeyPress
        If (Not Char.IsControl(e.KeyChar) _
             AndAlso (Not Char.IsDigit(e.KeyChar) _
             AndAlso (e.KeyChar <> Microsoft.VisualBasic.ChrW(46)))) Then
            e.Handled = True
        End If
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Dim result As DialogResult = MessageBox.Show("ยืนยันบันทึก", "Confirm", MessageBoxButtons.YesNo)
        If result = DialogResult.Yes Then
            UpdateStatusBigset(dgvModel.Item(3, RowIndexBigSet).Value, 2)
        End If
    End Sub

    Private Sub UpdateStatusBigset(BigSetId As Int16, StatusJob As Int16)
        Try
            Dim objDB As objDB = New objDB()
            Dim Con As SqlConnection = objDB.ConnectDatabase()
            Dim _SQL As String = "UPDATE BigSet SET [StatusJob] = '" & StatusJob & "' WHERE BigSetId = '" & BigSetId & "'"
            objDB.ExecuteSQL(_SQL, Con)
            MsgBox("บันทึกข้อมูลเรียบร้อย", MsgBoxStyle.Information, "Information")
            GetModel()
            objDB.DisconnectDatabase(Con)
        Catch ex As Exception
            InsertLogError(ex.Message, "1", "", username)
        End Try

    End Sub

    Private Sub BtnCancel_Click(sender As Object, e As EventArgs) Handles BtnCancel.Click
        EnableFalse()
    End Sub

    Private Sub EnableFalse()
        'RowIndex = -1
        tbWeight.Text = ""
        TextBox4.Text = ""
        TextBox3.Text = ""
        TextBox5.Text = ""
        TextBox6.Text = ""
        TextBox7.Text = ""
        TextBox8.Text = ""
        TextBox2.Text = ""
        TextBox1.Text = ""
        tbSetStove.Text = ""
        TextBox4.Enabled = False
        tbWeight.Enabled = False
        BtnCancel.Enabled = False
        BtnSave.Enabled = False
        cbUser.Enabled = False
        dtpUpStove.Enabled = False

    End Sub

    Private Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
        GetUpStove()
        BigSet()
        GetModel()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        EnableAddListManual(False)
        TextBox10.Text = ""
        TextBox11.Text = ""
        AddListAction = 1
        CheckEnableBtnMove()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If ComboBox2.Text = "กำลังดำเนินการ" Then
            EnableAddListManual(True)
        End If

        If dgvAddList.Rows.Count > 0 Then
            For i As Integer = 0 To dgvAddList.Rows.Count - 1
                If dgvAddList.Rows(i).Cells(5).Value.ToString().Trim = "99.99" Then
                    ComboBox4.SelectedIndex = 0
                    ComboBox4.Enabled = False
                    Exit For
                ElseIf dgvAddList.Rows(i).Cells(5).Value.ToString().Trim = "96.50" Then
                    ComboBox4.SelectedIndex = 1
                    ComboBox4.Enabled = False
                    Exit For
                Else
                End If
            Next
        End If
        AddListAction = 2
        CheckEnableBtnMove()
    End Sub

    Private Sub dgvModel_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvModel.CellDoubleClick
        TabControl1.SelectTab(1)
    End Sub

    Private Sub EnableAddListManual(Value As Boolean)
        TextBox9.Enabled = Value
        ComboBox3.Enabled = Value
        TextBox10.Enabled = Value
        ComboBox4.Enabled = Value
        TextBox11.Enabled = Value
        Button4.Enabled = Value
    End Sub

    Private Sub dgvUpStove_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs) Handles dgvUpStove.CurrentCellDirtyStateChanged
        Dim dgw = CType(sender, DataGridView)
        dgw.CommitEdit(DataGridViewDataErrorContexts.Commit)

        CheckEnableBtnMove()
    End Sub

    Private Sub CheckEnableBtnMove()
        Dim CountSelectDgv As Integer = 0
        For Each row As DataGridViewRow In dgvUpStove.Rows
            If (row.Cells(0).Value) Then
                CountSelectDgv = CountSelectDgv + 1
            End If
        Next
        If CountSelectDgv > 0 And RowIndexBigSet > -1 And AddListAction = 1 And ComboBox1.Text = "กำลังดำเนินการ" And ComboBox2.Text = "กำลังดำเนินการ" Then
            btnMove.Enabled = True
        Else
            btnMove.Enabled = False
        End If
    End Sub

    Private Sub TabControl1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles TabControl1.SelectedIndexChanged
        If sender.selectedindex = 0 Then
            AddListAction = 0
        ElseIf sender.selectedindex = 1 Then
            EnableAddListManual(False)
            If TextBox9.Text <> "" Then
                Button1.Enabled = True
                Button2.Enabled = True
            Else
                Button1.Enabled = False
                Button2.Enabled = False
            End If
        End If

    End Sub

    Private Sub dgv_RowPostPaint(sender As Object, e As DataGridViewRowPostPaintEventArgs) Handles dgvUpStove.RowPostPaint, dgvPart1.RowPostPaint, dgvAddList.RowPostPaint, dgvModel.RowPostPaint
        Try
            Dim dg As DataGridView = DirectCast(sender, DataGridView)
            ' Current row record
            Dim rowNumber As String = (e.RowIndex + 1).ToString()

            ' Format row based on number of records displayed by using leading zeros
            While rowNumber.Length < dg.RowCount.ToString().Length
                rowNumber = "0" & rowNumber
            End While

            ' Position text
            Dim size As SizeF = e.Graphics.MeasureString(rowNumber, Me.Font)
            If dg.RowHeadersWidth < CInt(size.Width + 20) Then
                dg.RowHeadersWidth = CInt(size.Width + 20)
            End If

            ' Use default system text brush
            Dim b As Brush = SystemBrushes.ControlText

            ' Draw row number
            e.Graphics.DrawString(rowNumber, dg.Font, b, e.RowBounds.Location.X + 15, e.RowBounds.Location.Y + ((e.RowBounds.Height - size.Height) / 2))
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "Error")
        End Try
    End Sub

    Private Sub dgvAddList_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvAddList.CellClick

    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Dim frm As frmProcess1_1_AddManual = New frmProcess1_1_AddManual
        frm.Show()
    End Sub

    Private Sub tbWeight_KeyDown(sender As Object, e As KeyEventArgs) Handles tbWeight.KeyDown
        If e.KeyCode = Keys.Enter Then
            tbWeight.Text = Format(Result, "#,##0.00")
        End If
    End Sub

    Private Sub TextBox4_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox4.KeyDown
        If e.KeyCode = Keys.Enter Then
            TextBox4.Text = Format(Result, "#,##0.00")
        End If
    End Sub

    Private Sub TextBox10_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox10.KeyDown
        If e.KeyCode = Keys.Enter Then
            TextBox10.Text = Format(Result, "#,##0.00")
        End If
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click

    End Sub

    Private Sub dgvPart1_FilterStringChanged(sender As Object, e As EventArgs) Handles dgvPart1.FilterStringChanged

        'If (String.IsNullOrEmpty(dgvPart1.FilterString)) Then

        'Else
        '    'Dim ArrText() As String = dgvPart1.FilterString.Split("IN")
        '    'Dim TextFilter As String = ArrText(1).Replace("N ", "")
        '    '(Convert([],System.String)
        '    'TextFilter = Mid(TextFilter, 1, Len(TextFilter) - 1).Replace(",", "")
        '    'TextFilter = TextFilter.Replace(" ", ",")
        '    Dim TextFilter As String = dgvPart1.FilterString.Replace("(Convert([],System.String)", "")
        '    Dim ColFilter() As String = {"vpk.BARCODE_NAME", "vpk.Process7", "vj.น้ำหนักทอง", "pw.แนบ_เศษทอง", "pw.รีดลาย_เศษทอง", "vpk.Process1", "vpk.Process2",
        '            "vpk.Process3", "vpk.Timer", "vpk.BarcodeText"}
        '    Try

        '        Dim objDB As objDB = New objDB()
        '        Dim Con As SqlConnection = objDB.ConnectDatabase()
        '        Dim _SQL As String = "SELECT vpk.BARCODE_NAME 'ชื่อบาร์โค๊ต', vpk.Process1 'น้ำหนักกล้า', vpk.Process2 'น้ำหนักสร้อย', vpk.Process3 'เศษ_ลงเตา', vpk.Process7 'จำนวนเส้น', vj.น้ำหนักทอง, pw.แนบ_เศษทอง, pw.รีดลาย_เศษทอง, vpk.Timer 'ชื่อเตา', vpk.BarcodeText, vpk.Lot
        '        FROM vProcessKiln vpk
        '        inner join vJobs vj on vpk.BarcodeText = vj.BarcodeText
        '        inner join ProcessWire pw on vpk.BarcodeText = pw.BarcodeText
        '        where vpk.LastStatus = 'เสร็จสิ้น' and vj.BarcodeText " + IIf(cbStatus.Text = "กำลังดำเนินการ", "not", "") + " in (SELECT BarcodeText FROM UpStove) "
        '        _SQL &= " and " & ColFilter(ColumnIndex) & " in " & TextFilter

        '        Dim dtUpStove As DataTable = objDB.SelectSQL(_SQL, Con)

        '        objDB.DisconnectDatabase(Con)
        '        dgvPart1.Rows.Clear()
        '        If dtUpStove.Rows.Count > 0 Then
        '            For i As Integer = 0 To dtUpStove.Rows.Count - 1
        '                dgvPart1.Rows.Add(dtUpStove.Rows(i)("ชื่อบาร์โค๊ต"), dtUpStove.Rows(i)("จำนวนเส้น"), Format(dtUpStove.Rows(i)("น้ำหนักทอง"), "n2"), Format(dtUpStove.Rows(i)("แนบ_เศษทอง"), "n2"), Format(dtUpStove.Rows(i)("รีดลาย_เศษทอง"), "n2"), Format(dtUpStove.Rows(i)("น้ำหนักกล้า"), "n2"),
        '                                  Format(dtUpStove.Rows(i)("น้ำหนักสร้อย"), "n2"), Format(dtUpStove.Rows(i)("เศษ_ลงเตา"), "n2"), dtUpStove.Rows(i)("ชื่อเตา"), dtUpStove.Rows(i)("BarcodeText"), dtUpStove.Rows(i)("Lot"))
        '            Next
        '        End If
        '    Catch ex As Exception
        '        InsertLogError(ex.Message, "1", "", username)
        '    End Try

        'End If

    End Sub
End Class