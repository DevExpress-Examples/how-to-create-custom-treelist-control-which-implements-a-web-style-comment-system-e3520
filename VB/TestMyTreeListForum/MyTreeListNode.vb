﻿Imports System
Imports System.Windows.Forms
Imports System.Collections.Generic
Imports DevExpress.XtraTreeList.Nodes
Imports System.Drawing
Imports DevExpress.XtraEditors.Controls




Namespace DevExpress.MyControl
	Public Enum MyNodeAction
		[Nothing]
		Add
		Remove
	End Enum



	Public Class MyTreeListNode
        Inherits TreeListNode
        Private buttonsField As MyToolButtonCollection
        Private Const INTERVALbutton As Integer = 3
        Private focusedButton As MyToolButton
        Private avatarSize As Size
        Private Const AVATARCaptionWidth As Integer = 46



        Protected Friend Sub New(ByVal id As Integer, ByVal owner As TreeListNodes)
            MyBase.New(id, owner)
            isPostedField = False
            buttonsField = CreateButtonCollection()
            UpdateButtons()
            SelectImageIndex = 0
            avatarSize = New Size(20, 25)
            avatarImageBoundsField = Rectangle.Empty
            avatarCaptionBoundsField = Rectangle.Empty
        End Sub

        Private avatarImageBoundsField As Rectangle
        Public ReadOnly Property AvatarImageBounds() As Rectangle
            Get
                Return avatarImageBoundsField
            End Get
        End Property


        Private avatarCaptionBoundsField As Rectangle
        Public ReadOnly Property AvatarCaptionBounds() As Rectangle
            Get
                Return avatarCaptionBoundsField
            End Get
        End Property


        Private isPostedField As Boolean
        Friend Property IsPosted() As Boolean
            Get
                Return isPostedField
            End Get
            Set(ByVal value As Boolean)
                If isPostedField = value Then
                    Return
                End If
                isPostedField = value
                UpdateButtons()
                TreeList.LayoutChanged()
            End Set
        End Property



        Public ReadOnly Property Buttons() As MyToolButtonCollection
            Get
                Return buttonsField
            End Get
        End Property



        Protected Overridable Function CreateButtonCollection() As MyToolButtonCollection
            Return New MyToolButtonCollection()
        End Function



        Protected Overridable Sub UpdateButtons()
            focusedButton = Nothing
            Buttons.Clear()
            If Not IsPosted Then
                Buttons.Add(New MyToolButton(MyButtonType.Cancel))
                Buttons.Add(New MyToolButton(MyButtonType.Commit))
            Else
                Buttons.Add(New MyToolButton(MyButtonType.Link))
            End If
        End Sub



        Public ReadOnly Property Interval() As Integer
            Get
                Return INTERVALbutton
            End Get
        End Property



        Public Overridable Sub HitTest(ByVal pt As Point)
            If focusedButton IsNot Nothing Then
                If focusedButton.ViewInfo.Bounds.Contains(pt) Then
                    Return
                End If
                focusedButton.ViewInfo.UnderCursor = False
                InvalidateButton(focusedButton)
                focusedButton = Nothing
            End If


            Dim btn As MyToolButton
            For i As Integer = 0 To Buttons.Count - 1
                btn = Buttons(i)
                If btn.ViewInfo.Bounds.Contains(pt) Then
                    btn.ViewInfo.UnderCursor = True
                    focusedButton = btn
                    InvalidateButton(focusedButton)
                    Exit For
                End If
            Next i
        End Sub



        Protected Overridable Sub InvalidateButton(ByVal btn As MyToolButton)
            TreeList.Invalidate(btn.ViewInfo.Bounds)
        End Sub



        Friend Function MouseDownAction(ByVal e As MouseEventArgs) As MyNodeAction
            Dim res As MyNodeAction = MyNodeAction.Nothing
            If e.Button <> MouseButtons.Left OrElse focusedButton Is Nothing Then
                Return res
            End If

            Select Case focusedButton.ButtonType
                Case MyButtonType.Link
                    res = MyNodeAction.Add
                    Exit Select
                Case MyButtonType.Cancel
                    res = MyNodeAction.Remove
                    Exit Select
                Case MyButtonType.Commit
                    IsPosted = True
                    res = MyNodeAction.Nothing
                    Exit Select

            End Select

            Return res
        End Function



        Friend Sub CalcAvatarBounds(ByVal rect As Rectangle)
            avatarImageBoundsField = New Rectangle(New Point((rect.Width - avatarSize.Width) \ 2 + rect.X, rect.Y + 2), avatarSize)
            avatarCaptionBoundsField = New Rectangle(New Point((rect.Width - AVATARCaptionWidth) \ 2 + rect.X, avatarImageBoundsField.Bottom + 2), New Size(AVATARCaptionWidth, rect.Bottom - avatarImageBoundsField.Bottom))
        End Sub



		Public ReadOnly Property AvatarCaption() As String
			Get
				Dim mtl As MyTreeList = TryCast(TreeList, MyTreeList)
				If mtl.AvatarCaptionFieldName = String.Empty Then
					Return String.Empty
				End If
				Return TryCast(GetValue(TreeList.Columns(mtl.AvatarCaptionFieldName).AbsoluteIndex), String)
			End Get
		End Property



		Public ReadOnly Property AvatarImage() As Image
			Get
				Return GetImage()
			End Get
		End Property



		Protected Overridable Function GetImage() As Image
			Dim img As Image = Nothing
			Dim mtl As MyTreeList = TryCast(TreeList, MyTreeList)
			If mtl.AvatarImageFieldName = String.Empty Then
				Return img
			End If

			Try
				Dim col As DevExpress.XtraTreeList.Columns.TreeListColumn = TreeList.Columns(mtl.AvatarImageFieldName)
				Dim buffer() As Byte = DirectCast(GetValue(TreeList.Columns.IndexOf(col)), Byte())
				img = ByteImageConverter.FromByteArray(buffer)
			Catch
				img = Nothing
			End Try

			Return img
		End Function
	End Class
End Namespace
