Option Strict On
Option Explicit On

Imports Microsoft.VisualBasic.ApplicationServices

Namespace My
    Partial Friend Class MyApplication
        Private Sub MyApplication_ApplyApplicationDefaults(
            sender As Object,
            e As ApplyApplicationDefaultsEventArgs
        ) Handles Me.ApplyApplicationDefaults
            e.Font = Core.AppTheme.FontBody
            e.HighDpiMode = HighDpiMode.PerMonitorV2
        End Sub
    End Class
End Namespace
