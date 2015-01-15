<%@ Page Title="Log Out" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="Logout.aspx.cs" Inherits="Account_Logout" %>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="Body">
<div class="row">
	<div class="column grid-20">
		<h2>
			You have been successfully logged out.
		</h2>
		<p>
			To log in again, please enter your username and password.
		</p>
		<asp:Login ID="LoginUser" runat="server" EnableViewState="false" RenderOuterTable="false">
			<LayoutTemplate>
				<div class="failureNotification redtext bold">
					<asp:Literal ID="FailureText" runat="server"></asp:Literal>
				</div>
				<asp:ValidationSummary ID="LoginUserValidationSummary" runat="server" CssClass="failureNotification" 
					 ValidationGroup="LoginUserValidationGroup"/>
				<div class="accountInfo">
					<h4>Account Information</h4>
					<div class="question">
						<asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName">Username</asp:Label><br />
						<asp:TextBox ID="UserName" runat="server" CssClass="textEntry w200" placeholder="Username"></asp:TextBox>
						<asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName" 
								CssClass="failureNotification" ErrorMessage="User Name is required." ToolTip="User Name is required." 
								ValidationGroup="LoginUserValidationGroup">*</asp:RequiredFieldValidator>
					</div>
					<div class="question">
						<asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password">Password</asp:Label><br />
						<asp:TextBox ID="Password" runat="server" CssClass="passwordEntry w200" TextMode="Password" placeholder="Password"></asp:TextBox>
						<asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password" 
								CssClass="failureNotification" ErrorMessage="Password is required." ToolTip="Password is required." 
								ValidationGroup="LoginUserValidationGroup">*</asp:RequiredFieldValidator>
					</div>
					<div>
						<asp:CheckBox ID="RememberMe" runat="server"/>
						<asp:Label ID="RememberMeLabel" runat="server" AssociatedControlID="RememberMe" CssClass="inline">Keep me logged in</asp:Label>
					</div>
					<div class="submitButton">
						<asp:Button ID="LoginButton" runat="server" CommandName="Login" Text="Log In &raquo;" ValidationGroup="LoginUserValidationGroup" CssClass="simple-button simple-action simple-red" />
					</div>
				</div>
			</LayoutTemplate>
		</asp:Login>
	</div>
</div>
</asp:Content>