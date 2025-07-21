Imports SlideShowWPFTransition.SchiebenWischen.SlideShowWPFTransition.SchiebenWischen

Module DummySubMain

    <STAThread>
        Sub Main()
            Dim win As New WpfTransitionWindow()
            win.Left = 100
            win.Top = 100
            win.Width = 800
            win.Height = 600
            win.ShowDialog()
        End Sub

End Module
