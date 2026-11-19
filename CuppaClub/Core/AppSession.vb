Option Strict On
Option Explicit On

Imports CuppaClub.Models

Namespace Core
    Public NotInheritable Class AppSession
        Private Sub New()
        End Sub

        Private Shared _currentUser As UserProfile

        Public Shared Event SessionChanged As EventHandler

        Public Shared ReadOnly Property CurrentUser As UserProfile
            Get
                Return _currentUser
            End Get
        End Property

        Public Shared ReadOnly Property IsAuthenticated As Boolean
            Get
                Return _currentUser IsNot Nothing
            End Get
        End Property

        Public Shared ReadOnly Property IsStaff As Boolean
            Get
                Return _currentUser IsNot Nothing AndAlso _currentUser.IsStaff
            End Get
        End Property

        Public Shared Sub SetUser(profile As UserProfile)
            _currentUser = profile
            RaiseEvent SessionChanged(Nothing, EventArgs.Empty)
        End Sub

        Public Shared Sub Clear()
            _currentUser = Nothing
            RaiseEvent SessionChanged(Nothing, EventArgs.Empty)
        End Sub
    End Class
End Namespace
