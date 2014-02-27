Imports System.Xml

Public Class Form1

    Private RootPath As String = "c:\"

#Region "Methods"
    Private Sub LsvFilesItemCountChanged()
        If Not Me.lsvFiles.Items.Count = 0 Then
            Me.tssFileCount.Text = Me.lsvFiles.Items.Count & " adet dosya vard�r.."
        Else
            Me.tssFileCount.Text = ""
        End If
    End Sub

    Private Sub ReadConfig(ByVal sender As Object, ByVal e As EventArgs)
        'Yap�lan ayarlar okunuyor..Program kapatul�rken kaydedilen form boyutu, varsay�lan dosya ekleme konumu, listeleme t�r� okunup uygulan�yor..
        Dim doc As XmlDocument = New XmlDocument
        'XML d�k�man haf�zaya al�n�r..Bir DataTable gibi..
        doc.Load(Application.StartupPath & "\config.xml")

        'Root Node..
        Dim root As XmlElement = doc.DocumentElement

        'Root Node '�n t�m child Node'lar� okunur..Program nas�l halde b�rak�ld�ysa o �ekilde ayarlan�r..Bu k�s�m dahada zenginle�tirilebilir.
        For Each child As XmlElement In root.ChildNodes
            Select Case child.Name
                Case "FormWidth"
                    Me.Width = CInt(child.InnerText)
                Case "FormHeight"
                    Me.Height = CInt(child.InnerText)
                Case "RootPath"
                    RootPath = child.InnerText
                Case "ListType"
                    Select Case child.InnerText
                        Case "View.LargeIcon"
                            SimgeToolStripMenuItem_Click(sender, e)
                        Case "View.List"
                            ListeToolStripMenuItem_Click(sender, e)
                        Case Else
                            SimgeToolStripMenuItem_Click(sender, e)
                    End Select
            End Select
        Next

        'XML d�k�man haf�zadan silinir..
        doc = Nothing
    End Sub

    Private Sub WriteConfig()
        'Program kapat�l�rken yap�lan dei�iklikler kaydedilir..Form boyutu, listeleme t�r�, varsay�lan dosya a�ma konumu..XML d�k�man� "ReadConfig" sub '�ndaki a��klama �eklinde okunur ve gerekli ayarlar yap�l�r..
        Dim doc As XmlDocument = New XmlDocument
        doc.Load(Application.StartupPath & "\config.xml")

        Dim root As XmlElement = doc.DocumentElement

        For Each child As XmlElement In root.ChildNodes
            Select Case child.Name
                Case "FormWidth"
                    child.InnerText = Me.Width
                Case "FormHeight"
                    child.InnerText = Me.Height
                Case "RootPath"
                    child.InnerText = RootPath
                Case "ListType"
                    child.InnerText = Me.lsvFiles.View
            End Select
        Next

        'Haf�zaya al�nan XML d�k�man�,XML dosyas�na kaydedilir..
        doc.Save(Application.StartupPath & "\config.xml")
        doc = Nothing
    End Sub
#End Region

#Region "toolStrip"
    Private Sub OpenToolStripButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OpenToolStripButton.Click
        'Klas�r a�ma butonuna bas�l�nca varsay�lan ,daha �nceden a��lm�� konum neyse oradan FolderBrowserDialog a��l�r..
        Me.FolderBrowserDialog1.SelectedPath = RootPath

        'E�er bir klas�r se�ilir ve OK 'e bas�l�rsa..
        If Me.FolderBrowserDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            'Se�ilen klas�r yolu varsay�lan olarak de�i�kene aktar�l�r..
            RootPath = Me.FolderBrowserDialog1.SelectedPath
            'Se�ilen konumdaki t�m dosyalar okunur..
            For Each file As String In System.IO.Directory.GetFiles(Me.FolderBrowserDialog1.SelectedPath)

                'Herbir dosyadan FileDetails objesi yarat�l�r..
                Dim fd As FileDetails = New FileDetails(file)
                'Bu obje ListView 'a eklenir..
                Me.lsvFiles.Items.Add(fd)
            Next

            'ListView'daki nesne say�s�na g�re ListView altdaki StatusStrip de ListView'da ka� nesne oldu�unu belirtiyoruz..
            LsvFilesItemCountChanged()
        End If
    End Sub

    Private Sub NewToolStripButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NewToolStripButton.Click
        'Listview temizlenir..
        Me.lsvFiles.Items.Clear()
        'ListView'daki nesne bilgisi g�sterilir..
        LsvFilesItemCountChanged()
    End Sub

    Private Sub SaveToolStripButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveToolStripButton.Click
        'ListView'daki dosyalar�n konumlar� bir XML dosyas�na yaz�larak Taslak olarak kaydedilir..
        Me.SaveFileDialog1.Filter = "XML Files(*.xml)|*.xml"

        If Me.SaveFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            'Bir datatable belirtiriz..
            Dim DT As DataTable = New DataTable
            Dim DR As DataRow
            'Dosya Ad� ve Dosya Konumu kolonlar� olan..
            DT.Columns.Add("Dosya Ad�")
            DT.Columns.Add("Dosya Konumu")

            'Listview 'daki check edilmi� t�m nesneler bu datatable 'a kaydedilir..
            For Each lsvi As ListViewItem In Me.lsvFiles.CheckedItems
                'Herbir nesne FileDetails nesnesine aktar�l�r.Property'lerine eri�ilir..
                Dim fd As FileDetails = CType(lsvi, FileDetails)
                'Yeni sat�r olu�turulur..
                DR = DT.NewRow()
                'Olu�turulan sat�rdaki ilk kolona Dosya ad� yaz�l�r..
                DR.Item(0) = fd.Text
                'Olu�turulan ikinci konuma Dosya konumu yaz�l�r..
                DR.Item(1) = fd.FilePath
                'Sat�r datatable 'a eklenir..
                DT.Rows.Add(DR)
            Next

            'Burada daha �nceden yazd���m�z DLL dosyas�n� kullan�yoruz..Bu DLL komutlar�na ula�mak i�in �nceki makalemizi okuyabilirsiniz..(Makale ismi:DataTable'dan XML dosya Olu�turma ve XML dosyadan DataTable Olu�turma..)
            Dim SaveXml As WinCreatingXMLFileFromDataTable.XMLFileCreating = New WinCreatingXMLFileFromDataTable.XMLFileCreating

            'Bu DLL deki CreateXMLFile metodunu kullanarak DataTable'�m�z� XML olarak kaydediyoruz..
            SaveXml.CreateXMLFile(DT, Me.SaveFileDialog1.FileName)
        End If
    End Sub

    Private Sub OpenSaveFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OpenSaveFile.Click
        'Daha �nce kaydedilmi� taslaklar� a�ma..
        'OpenFileDialog'un Filter �zelli�i ile sadece .xml uzant�l� dosyalr�n gr�nmesini sa�l�yoruz..
        Me.OpenFileDialog1.Filter = "XML Files(*.xml)|*.xml"

        If Me.OpenFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            'Daha �nceden kaydedilmi� taslak dosyas� se�ilir..Se�ilen XML Doc nesnesine y�klenir..Oldu�u gibi haf�zaya al�n�r..
            Dim Doc As XmlDocument = New XmlDocument
            Doc.Load(Me.OpenFileDialog1.FileName)

            'Root node elde edilir..
            Dim root As XmlElement = Doc.DocumentElement

            'Root node'�n t�m child node'lar� i�in okuma yap�l�r..
            For Each child As XmlElement In root.ChildNodes
                'E�er childnode '�n Dosyakonumu alan� bo� de�ilse..
                If Not child.ChildNodes(1).InnerText Is Nothing Then
                    'ChildNode 'dan FileDetails nesneis olu�turulur..
                    'Dosya ad� ve konumu bu nesnenin property'lerine new metodu ile yaz�l�r..
                    Dim fd As FileDetails = New FileDetails(child.ChildNodes(1).InnerText)
                    'ListView 'a nesne eklenir..
                    Me.lsvFiles.Items.Add(fd)
                End If
            Next

            'XMl d�k�mani haf�zadan silinir..
            Doc = Nothing
        End If
    End Sub

    Private Sub AddFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddFile.Click
        Me.OpenFileDialog1.Filter = "T�m Dosyalar(*.*)|*.*"
        Me.OpenFileDialog1.Multiselect = True
        Me.OpenFileDialog1.FileName = ""
        Me.OpenFileDialog1.InitialDirectory = RootPath

        If Me.OpenFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            RootPath = Me.OpenFileDialog1.FileName.Substring(0, Me.OpenFileDialog1.FileName.LastIndexOf("\"))
            For Each file As String In Me.OpenFileDialog1.FileNames
                Dim fd As FileDetails = New FileDetails(file)
                Me.lsvFiles.Items.Add(fd)
            Next
        End If
    End Sub

    Private Sub SimgeToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SimgeToolStripMenuItem.Click
        'E�er simge �eklinde listeleme t�r� se�ilirse..ListView 'in View �zelli�i ona g�re ayarlan�r.Yani LargeIcon modunda ve "ToolStripDropDownButton1" 'in image �zelli�ine "SimgeToolStripMenuItem" '�n image '� eklenir..G�rsellik olsun biraz demi!!
        Me.lsvFiles.View = View.LargeIcon
        Me.ToolStripDropDownButton1.Image = Me.SimgeToolStripMenuItem.Image
    End Sub

    Private Sub ListeToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ListeToolStripMenuItem.Click
        'E�er liste �eklinde listeleme t�r� se�ilirse..ListView 'in View �zelli�i ona g�re ayarlan�r.Yani List modunda ve "ToolStripDropDownButton1" 'in image �zelli�ine "ListeToolStripMenuItem" '�n image '� eklenir..G�rsellik yap�yoz..
        Me.lsvFiles.View = View.List
        Me.ToolStripDropDownButton1.Image = Me.ListeToolStripMenuItem.Image
    End Sub

    Private Sub ToolStripButton2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripButton2.Click
        'ListView'dan se�ili nesnelerin silinmesi i�lemi..Se�ili her bir listviewItem i�in "Remove" metodu kullan�l�r..
        For Each lsvi As ListViewItem In Me.lsvFiles.CheckedItems
            lsvi.Remove()
        Next
    End Sub

    Private Sub ToolStripButton1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripButton1.Click
        'ListView'da bulunan fakat dosya sabit diskte bulunmuyorsa,bu nesneler silinir.Bu nesnelerin imageIndex'ini 19 olarak ekletti�imiz i�in kolayca ula��yoruz..
        For Each lsvi As ListViewItem In Me.lsvFiles.Items
            If lsvi.ImageIndex = 19 Then
                lsvi.Remove()
            End If
        Next
    End Sub

    Private Sub PrintToolStripButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PrintToolStripButton.Click

        If Not System.IO.Directory.Exists(Application.StartupPath & "\apps\SourceData") = True Then
            MkDir(Application.StartupPath & "\apps\SourceData")
        End If

        Dim DT As DataTable = New DataTable
        Dim DR As DataRow

        DT.Columns.Add("Dosya Ad�")
        DT.Columns.Add("Dosya Konumu")

        For Each lsvi As ListViewItem In Me.lsvFiles.CheckedItems
            Dim fd As FileDetails = CType(lsvi, FileDetails)

            DR = DT.NewRow()
            DR.Item(0) = fd.Text
            DR.Item(1) = fd.FilePath

            DT.Rows.Add(DR)
        Next

        Dim SaveXml As WinCreatingXMLFileFromDataTable.XMLFileCreating = New WinCreatingXMLFileFromDataTable.XMLFileCreating

        SaveXml.CreateXMLFile(DT, Application.StartupPath & "\apps\SourceData\Dosyalar.xml")

        MessageBox.Show("L�tfen tablolar k�sm�ndan tablonuzu se�iniz..Gerekli d�zenlemeden sonra " & Chr(34) & "Kaydet" & Chr(34) & " yaparak olu�an dosyay� yazd�rabilirsiniz..", "Yazd�rma", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Process.Start(Application.StartupPath & "\apps\WinHtmlTableOptionsForm.exe")
    End Sub

    Private Sub HelpToolStripButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles HelpToolStripButton.Click
        Dim frm As frmHakkinda = New frmHakkinda
        frm.Show()
    End Sub
#End Region


    Private Sub lsvFiles_ItemChecked(ByVal sender As Object, ByVal e As System.Windows.Forms.ItemCheckedEventArgs) Handles lsvFiles.ItemChecked
        If Not Me.lsvFiles.CheckedItems.Count = 0 Then
            Me.tssCheckedItemsCount.Text = Me.lsvFiles.CheckedItems.Count & " adet dosya se�ilmi�tir.."
        Else
            Me.tssCheckedItemsCount.Text = ""
        End If
    End Sub

    Private Sub lsvFiles_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles lsvFiles.KeyDown
        Select Case e.KeyCode
            Case Keys.Delete
                ToolStripButton2_Click(sender, e)
            Case Keys.PageUp
                Me.lsvFiles.Items(Me.lsvFiles.Items.Count - 1).Focused = True
            Case Keys.PageDown
                Me.lsvFiles.Items(0).Focused = True
        End Select
    End Sub

    Private Sub lsvFiles_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lsvFiles.SelectedIndexChanged

        'MsgBox(Me.lsvFiles.FocusedItem.Index)
        Dim fd As FileDetails
        fd = CType(Me.lsvFiles.FocusedItem, FileDetails)

        Me.txtFileName.Text = fd.Text
        Me.txtFilePath.Text = fd.FilePath.ToString

        If fd.ImageIndex = 7 OrElse fd.ImageIndex = 8 OrElse fd.ImageIndex = 9 Then
            Me.PictureBox1.Image = Image.FromFile(fd.FilePath)
        Else
            Me.PictureBox1.Image = Image.FromFile(Application.StartupPath & "\Empty.bmp")
        End If
    End Sub

    Private Sub btnSelectAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSelectAll.Click
        For i As Integer = 0 To Me.lsvFiles.Items.Count - 1
            Me.lsvFiles.Items(i).Checked = True
        Next
    End Sub

    Private Sub btnDeselectAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDeselectAll.Click
        For i As Integer = 0 To Me.lsvFiles.Items.Count - 1
            Me.lsvFiles.Items(i).Checked = False
        Next
    End Sub

    Private Sub btnCopyFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCopyFile.Click
        'Bu k�s�mdaki kodlar dosyalar� ta��ma ile ayn�d�r..Tek fark� ta��ma i�leminde dosyay� kopyalad�ktan sonra eski kopyalanan dosyay� silmek gerekmektedir..(Bu iki k�s�m tek sub olarak yaz�labilir.)
        If Me.FolderBrowserDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            Dim Kopyalanan As Integer = 0
            Dim Kopyalanamayan As Integer = 0
            Dim KopyalanmayanRapor As String = ""
            Dim rapor As String = "...::: Dosya Kopyalama Raporu :::..." & vbCrLf & "====================================" & vbCrLf & vbCrLf

            'Checked edilen item kadar d�n..
            For i As Integer = 0 To Me.lsvFiles.CheckedItems.Count - 1
                'Checked edilen item FileDetails objesine d�n��t�r�lerek property'lerine ula��l�r..
                Dim fd As FileDetails = CType(Me.lsvFiles.CheckedItems(i), FileDetails)
                'E�er i�aretlenmi� dosya belirtilen konumda ise,yani varsa.
                If System.IO.File.Exists(fd.FilePath) = True Then
                    'Dosya kopyalan�r..
                    FileCopy(fd.FilePath, Me.FolderBrowserDialog1.SelectedPath & "\" & fd.FilePath.Substring(fd.FilePath.LastIndexOf("\") + 1))
                    'Kopyalanan dosya adetini elde etmek i�in..
                    Kopyalanan += 1
                Else
                    'E�er kopyalanacak dosya belirtilen konumda bulunamazsa; kopyalanamayanRapor de�i�kenine kaydedilir..Ayr�ca kopyalanamayan dosya adeti elde etmek i�in Kopyalanamayan de�i�keni kullan�yoruz..
                    KopyalanmayanRapor &= Chr(34) & fd.Text & Chr(34) & vbCrLf
                    Kopyalanamayan += 1
                End If
            Next

            'Rapor metni olu�turulur..
            KopyalanmayanRapor &= vbCrLf & "   Yukardaki dosyalar bulunamad��� ya da kullan�mda oldu�u i�in kopyalanamam��t�r.." & vbCrLf
            rapor &= "Kopyalanan Dosya Adeti : " & Kopyalanan & vbCrLf

            'E�er Kopyalanamayan dosya varsa "Kopyalanamayan Dosya adeti" metni eklenir..
            If Kopyalanamayan <> 0 Then
                rapor &= "Kopyalanamayan Dosya Adeti : " & Kopyalanamayan & vbCrLf & vbCrLf & KopyalanmayanRapor
            End If

            'Rapor G�r�nt�lenir..
            MessageBox.Show(rapor, "RAPOR !", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1)
        End If
    End Sub

    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        'Programda yap�lan ayarlar program kapan�rken kaydedilir..
        'Form'un width,height 'i, klas�r ekleme veya dosya ekleme butonlar�na bas�l�nca a��lacak varsay�lan konum ve listemele t�r� kaydedilir..
        WriteConfig()
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Programda yap�lan ayarlar program ba�larken okunur..
        'Form'un width,height 'i, klas�r ekleme veya dosya ekleme butonlar�na bas�l�nca a��lacak varsay�lan konum ve listemele t�r� okunur..
        ReadConfig(sender, e)
    End Sub

    Private Sub btnSelectFileMove_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSelectFileMove.Click
        'Bu k�s�mdaki kodlar dosyalar� kopyalama ile ayn�d�r..Tek fark� ta��ma i�leminde dosyay� kopyalad�ktan sonra eski kopyalanan dosyay� silmek gerekmektedir..Bu iki k�s�m tek sub olarak yaz�labilir.
        If Me.FolderBrowserDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            Dim Tasinan As Integer = 0
            Dim Tasinamayan As Integer = 0
            Dim TasinamayanRapor As String = ""
            Dim rapor As String = "...::: Dosya Ta��ma Raporu :::..." & vbCrLf & "====================================" & vbCrLf & vbCrLf

            'Checked edilen item kadar d�n..
            For i As Integer = 0 To Me.lsvFiles.CheckedItems.Count - 1
                'Checked edilen item FileDetails objesine d�n��t�r�lerek property'lerine ula��l�r..
                Dim fd As FileDetails = CType(Me.lsvFiles.CheckedItems(i), FileDetails)
                'E�er i�aretlenmi� dosya belirtilen konumda ise,yani varsa.
                If System.IO.File.Exists(fd.FilePath) = True Then
                    'Dosya kopyalan�r..
                    FileCopy(fd.FilePath, Me.FolderBrowserDialog1.SelectedPath & "\" & fd.FilePath.Substring(fd.FilePath.LastIndexOf("\") + 1))
                    'Dosyalar� kopyalama yapmay�p kesme yapman�n fark�..
                    'Kopyalanan dosya silinerek ta��ma i�lemi o dosya i�in tamamlan�r..
                    Kill(fd.FilePath)
                    'Ta��nan dosya adetini elde etmek i�in..
                    Tasinan += 1
                Else
                    'E�er ta��nacak dosya belirtilen konumda bulunamazsa; ta��namayan dosya ismi kaydedilir..Ayr�ca ta��namayan dosya adeti elde etmek i�in ta��namayan de�i�keni kullan�yoruz..
                    TasinamayanRapor &= Chr(34) & fd.Text & Chr(34) & vbCrLf
                    Tasinamayan += 1
                End If
            Next

            'Rapor metni olu�turulur..
            TasinamayanRapor &= vbCrLf & "   Yukardaki dosyalar bulunamad��� ya da kullan�mda oldu�u i�in ta��namam��t�r.." & vbCrLf
            rapor &= "Ta��nan Dosya Adeti : " & Tasinan & vbCrLf

            'E�er ta��namayan dosya varsa "Ta��namayan Dosya adeti" metni eklenir..
            If Tasinamayan <> 0 Then
                rapor &= "Ta��namayan Dosya Adeti : " & Tasinamayan & vbCrLf & vbCrLf & TasinamayanRapor
            End If

            'Rapor G�r�nt�lenir..
            MessageBox.Show(rapor, "RAPOR !", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1)
        End If
    End Sub
End Class
