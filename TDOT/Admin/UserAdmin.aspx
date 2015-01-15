<%@ Page Title="User Admin" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="UserAdmin.aspx.cs" Inherits="UserAdmin" %>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="Body">
<div class="row">
	<div class="column grid-20">
		<div id="management-panel">
			<h1>Manage Users</h1>
			<a title="Create User" class="simple-button simple-action simple-red create-user" href="#">Create New User</a>
			<div id="manage-users">
				<table class="wide100pct sortable selectable center-all" id="user-management-table">
					<thead>
						<tr>
							<th>Delete</th>
							<th>Approved</th>
							<th>User Name</th>
							<%=get_user_table_headers() %>
							<th>Last Login</th>
						</tr>
					</thead>
					<tbody><%=TDOT.Utilities.GetUserTable()%></tbody>
				</table>
			</div>
		</div>
	</div>
</div>
</asp:Content>

<asp:Content ID="DialogContent" runat="server" ContentPlaceHolderID="HiddenData">
	<div id="create-user-dialog">
		<div class="dialog-header">
			<h2>Create New User</h2>
			<p>
				Use the form below to create a new user that will have access to the site.
			</p>
		</div>
		<div class="dialog-content">
			<div class="question">
				<label for="username">Username</label>
				<div class="example-text">Enter a memorable and meaningful username.</div>
				<input type="text" id="username" name="username" class="required checkname w300" placeholder="Username" />
				<div class="error-message-container redtext"></div>
			</div>
			<div class="question">
				<label for="password">Password</label>
				<div class="example-text">Enter a password at least <%= Membership.MinRequiredPasswordLength %> characters in length.</div>
				<input type="text" id="password" name="password" class="required checktext w300" placeholder="Password" />
				<div class="error-message-container redtext"></div>
			</div>
			<div class="question">
				<label for="email">Email</label>
				<div class="example-text">Enter the user's email address.</div>
				<input type="text" id="email" name="email" class="required checkemail w300" placeholder="Email" />
				<div class="error-message-container redtext"></div>
			</div>
		</div>
		<div class="dialog-footer"></div>
	</div>
</asp:Content>
