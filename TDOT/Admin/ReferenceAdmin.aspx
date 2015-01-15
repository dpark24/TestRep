<%@ Page Title="TDOT File Reference Admin" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ReferenceAdmin.aspx.cs" Inherits="ReferenceAdmin" %>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="Body">
<div class="row">
	<div class="column grid-20">
		<div id="management-panel">
			<h1>Manage Site Elements</h1>
			<div id="manage-categories" class="management-widget" data-element_type="Category">
				<div class="row">
					<div class="column grid-10">
						<div class="main-controls float-left">
							<h2>File Categories</h2>
							<div class="question">
								<label for="add-file-category">Add a new File Category option</label>
								<div class="example-text">Enter a name for a new File Category Name.</div>
								<input type="text" class="checkname w300" name="add_file_category" id="add-file-category" value="" placeholder="File Category Name" />
								<div class="error-message-container redtext"></div>
							</div>
							<div class="control-toolbar mv8">
								<a href="#" class="sexybutton sexysimple sexygreen submit-item" title="Add New Option">Add Option &raquo;</a>
								<div class="fixed-line-20px center-text mv8">
								</div>
							</div>
						</div>
					</div>
					<div class="column grid-10">
						<div class="current-options float-left">
							<div class="float-right p2 trash-box cursor-pointer">
								<%//<span class="ui-icon ui-icon-trash tooltip-dark-left" title="Previously Deleted Items - Click here to manage their restoration"></span>%>
							</div>
							<h3>Active Options</h3>
							<ul data-options="category" class="sort-options">
								<%=get_reference_table("FileCategory")%>
							</ul>
						</div>
					</div>
				</div>
			</div>
			<div id="Div1" class="management-widget" data-element_type="Organization">
				<div class="row">
					<div class="column grid-10">
						<div class="main-controls float-left">
							<h2>Organizations</h2>
							<div class="question">
								<label for="add-organization">Add a new Organization option</label>
								<div class="example-text">Enter a name for a new Organization.</div>
								<input type="text" class="checkname w300" name="add_organization" id="add-organization" value="" placeholder="Organization Name" />
								<div class="error-message-container redtext"></div>
							</div>
							<div class="control-toolbar mv8">
								<a href="#" class="sexybutton sexysimple sexygreen submit-item" title="Add New Option">Add Option &raquo;</a>
								<div class="fixed-line-20px center-text mv8">
								</div>
							</div>
						</div>
					</div>
					<div class="column grid-10">
						<div class="current-options float-left">
							<div class="float-right p2 trash-box cursor-pointer">
								<%//<span class="ui-icon ui-icon-trash tooltip-dark-left" title="Previously Deleted Items - Click here to manage their restoration"></span>%>
							</div>
							<h3>Active Options</h3>
							<ul data-options="category" class="sort-options">
								<%=get_reference_table("Organization")%>
							</ul>
						</div>
					</div>
				</div>
			</div>
		</div>
	</div>
</div>
</asp:Content>

<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="HiddenData">
	<div id="item-trash-container">
		<div id="trash-wrapper">
			<h2>Previously Trashed Items</h2>
		</div>
		<div class="trash-content">
			Please hold on just a moment while we get the latest data from our systems..
			<div class="fixed-line-20px center-text status-bar"></div>
			<ul class="nice-list">
				<li>Loading data..</li>
			</ul>
		</div>
	</div>
</asp:Content>
