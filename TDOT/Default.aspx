<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<asp:Content ID="AdminBodyContent" runat="server" ContentPlaceHolderID="Body">
<div class="row">
	<div class="column grid-20">
        <div class="row">
            <div class="column grid-4">
                <h2>File Library</h2>
            </div>
            <div class="column grid-16">
                <%
                if (HttpContext.Current.User.IsInRole("Administrator"))
                {
                    %>
				    <div class="action-panel">
					    <div class="action-button">
						    <a title="Add File" class="simple-button simple-action simple-red add-document" href="#">Add File »</a>
					    </div>
				    </div>
                    <%
                }
                %>
            </div>
        </div>
        <div class="row" id="filter-controls">
            <div class="column grid-4">
                <div class="question">
                    <label for="year-filter">Monitoring Year</label>
                    <div class="example-text">Filter based on a selected year.</div>
                    <select id="year-filter" class="w150">
                        <option value="-9999">- Select -</option>
                        <option value="N/A">N/A</option>
                        <option value="2007">2007</option>
                        <option value="2008">2008</option>
                        <option value="2009">2009</option>
                        <option value="2010">2010</option>
                        <option value="2011">2011</option>
                        <option value="2012">2012</option>
                        <option value="2013">2013</option>
                        <option value="2014">2014</option>
                    </select>
                </div>
            </div>
            <div class="column grid-4">
                <div class="question">
                    <label for="region-filter">Region</label>
                    <div class="example-text">Filter based on a selected region.</div>
                    <select id="region-filter" class="w150">
                        <option value="-9999">- Select -</option>
                        <%=get_regions() %>
                    </select>
                </div>
            </div>
            <div class="column grid-4">
                <div class="question">
                    <label for="county-filter">County</label>
                    <div class="example-text">Filter based on a selected county.</div>
                    <select id="county-filter" class="w150">
                        <option value="-9999">Select a Region</option>
                    </select>
                </div>
            </div>
            <div class="column grid-4">
                <div class="question">
                    <label for="contractor-filter">Contractor</label>
                    <div class="example-text">Filter based on a selected contractor.</div>
                    <select id="contractor-filter" class="w150">
                        <option value="-9999">- Select -</option>
                        <%=get_organization() %>
                    </select>
                </div>
            </div>
            <div class="column grid-4">
                <div class="question">
                    <label for="category-filter">File Category</label>
                    <div class="example-text">Filter based on a selected category.</div>
                    <select id="category-filter" class="w150">
                        <option value="-9999">- Select -</option>
                        <%=get_categories()%>
                    </select>
                </div>
            </div>
        </div>
		<div class="row">
			<div class="column grid-10">
                &nbsp;
			</div>
			<div class="column grid-10">
				<div class="center-text bg-cccccc package-download">
					<strong>Download Package</strong><br />
					<span class="download-package">Select files to add to your download package.</span>
				</div>
			</div>
		</div>
		<div class="row">
			<div class="column grid-20">
				<div id="table-editor-panel" class="hidden"></div>
				<table class="wide100pct sortable-ajax selectable" id="file-management-table">
					<thead>
						<tr>
							<th class="col1 dont-sort center-text"><input type="checkbox" class="column-select-all" title="Select all files" /></th>
							<th class="col4 left-text primary">File Name and Description</th>
                            <th class="col2 center-text" title="Monitoring Year">Mon. Year</th>
                            <th class="col1 center-text">Region</th>
                            <th class="col2 center-text">County</th>
                            <th class="col2 center-text">PIN</th>
                            <th class="col2 center-text">Contractor</th>
							<th class="col2 center-text">Category</th>
							<th class="col2 center-text">Upload Date</th>
                            <th class="col2 center-text">Uploaded By</th>
						</tr>
					</thead>
					<tbody><%=TDOT.DocumentManagement.GetDocumentsTable()%></tbody>
				</table>
			</div>
		</div>
	</div>
</div>
</asp:Content>
<asp:Content ID="HiddenDialogs" ContentPlaceHolderID="HiddenData" runat="server">
    <input type="hidden" id="fc" name="fc" class="hidden" value="<%=Request.Cookies[FormsAuthentication.FormsCookieName]==null ? string.Empty : Request.Cookies[FormsAuthentication.FormsCookieName].Value %>" />
	<input type="hidden" id="si" name="si" class="hidden" value="<%=Session.SessionID %>" />
    <%
        if (HttpContext.Current.User.Identity.IsAuthenticated)
        {
		    %>
		    <div id="dialog-upload-document" title="Upload File">
			    <div>
				    <h1>Upload File</h1>
				    <p>
					    Please enter the appropriate data below. Once the file's data has been entered, select the file you would like to add by clicking "Browse" below. When you've selected the desired file, click Save to finish and exit.
				    </p>
				    <div id="upload-file-meta">
                        <div class="fileupload-loading"></div>
					    <div class="columns-2">
						    <div class="column-1">
							    <div class="question">
								    <label for="upload-file-title">Document Title</label>
								    <div class="example-text">E.g.,: Meeting Minutes</div>
								    <input type="text" id="upload-file-title" name="upload_file_title" class="required checktext w250" value="" placeholder="File Title" />
								    <div class="error-message-container redtext"></div>
							    </div>
							    <div class="question">
								    <label for="upload-file-category">File Category</label>
								    <div class="example-text">Select the appropriate category for the file.</div>
								    <select id="upload-file-category" name="upload_file_category" class="required checkselect w250">
									    <option value="-9999">- Select -</option>
                                        <%=get_categories()%>
								    </select>
								    <div class="error-message-container redtext"></div>
							    </div>
                                <div class="question">
								    <label for="upload-file-region">Region</label>
								    <div class="example-text">Select the appropriate TDOT Region for the file.</div>
								    <select id="upload-file-region" name="upload_file_region" class="required checkselect w250">
									    <option value="-9999">- Select -</option>
                                        <%=get_regions()%>
								    </select>
								    <div class="error-message-container redtext"></div>
							    </div>
                                <div class="question">
								    <label for="upload-file-county">County</label>
								    <div class="example-text">Select the appropriate county for the file.</div>
								    <select id="upload-file-county" name="upload_file_county" class="required checkselect w250">
									    <option value="-9999">- Select -</option>
								    </select>
								    <div class="error-message-container redtext"></div>
							    </div>
                                <div class="question">
								    <label for="upload-file-description">File Description</label>
								    <div class="example-text">Provide a description of the file's contents.</div>
								    <textarea cols="40" rows="5" id="upload-file-description" name="upload_file_description" class="required checktext w250" placeholder="File Description"></textarea>
								    <div class="error-message-container redtext"></div>
							    </div>
						    </div>
						    <div class="column-2">
                                <div class="question">
								    <label for="upload-file-pin">PIN</label>
								    <div class="example-text">Enter the PIN number, e.g., 101000.01.</div>
								    <input type="text" id="upload-file-pin" name="upload_file_pin" class="required checktext w250" value="" placeholder="PIN number" />
								    <div class="error-message-container redtext"></div>
							    </div>
                                <div class="question">
								    <label for="upload-file-monitoring-year">Monitoring Year</label>
								    <div class="example-text">Select the appropriate monitoring year for the file.</div>
								    <select id="upload-file-monitoring-year" name="upload_file_monitoring_year" class="required checkselect w250">
									    <option value="-9999">- Select -</option>
                                        <option value="N/A">N/A</option>
                                        <option value="2007">2007</option>
                                        <option value="2008">2008</option>
                                        <option value="2009">2009</option>
                                        <option value="2010">2010</option>
                                        <option value="2011">2011</option>
                                        <option value="2012">2012</option>
                                        <option value="2013">2013</option>
                                        <option value="2014">2014</option>
								    </select>
								    <div class="error-message-container redtext"></div>
							    </div>
                                <div class="question">
								    <label for="upload-file-organization">Contractor</label>
								    <div class="example-text">Select the appropriate contractor.</div>
								    <select id="upload-file-organization" name="upload_file_organization" class="required checkselect w250">
									    <option value="-9999">- Select -</option>
                                        <%=get_organization()%>
								    </select>
								    <div class="error-message-container redtext"></div>
							    </div>
                                <div class="question">
								    <label for="upload-file-tdec">TDEC Permit Number (NRS)</label>
								    <div class="example-text">Enter the NRS number, e.g., NRS 07.342.</div>
								    <input type="text" id="upload-file-tdec" name="upload_file_tdec" class="required checktext w250" value="" placeholder="TDEC Permit Number" />
								    <div class="error-message-container redtext"></div>
							    </div>
                                <div class="question">
								    <label for="upload-file-usace">USACE Permit Number</label>
								    <div class="example-text">Enter the USACE number, e.g., NWP #12; 200701629.</div>
								    <input type="text" id="upload-file-usace" name="upload_file_usace" class="required checktext w250" value="" placeholder="USACE Permit Number" />
								    <div class="error-message-container redtext"></div>
							    </div>
						    </div>
						    <input type="hidden" value="all" id="upload-doctype" name="upload_doctype" />
					    </div>
					    <hr />
					    <div>
						   <!-- <div id="upload-doc" class="mv10"></div>
						    <p>Once you have selected a file and entered the appropriate information, please click "Save" to upload the file.</p> -->
					        <div class="fileupload-buttonbar">
                                <span class="fileinput-button">
                                    <span class="sexybutton sexysimple sexygreen">Select File</span>
                                    <input type="file" name="files[]" id="fileupload">
                                </span>
                                <a class="submit-file sexybutton sexysimple sexyblue disabled" href="#">Submit</a>
                                <div class="filename mv6"></div>
                                <div id="progress">
                                    <div style="width: 0%;" class="progress-animated"></div>
                                </div>
                            </div>
                        </div>
				    </div>
			    </div>
		    </div>
            <div id="dialog-edit-document" title="Edit File">
			    <div>
				    <h1>Edit File</h1>
				    <p>
					    Edit the file's data below. 
				    </p>
				    <div id="edit-file-meta">
					    <div class="columns-2">
						    <div class="column-1">
							    <div class="question">
								    <label for="edit-file-title">Document Title</label>
								    <div class="example-text">E.g.,: Meeting Minutes</div>
								    <input type="text" id="edit-file-title" name="edit_file_title" class="required checktext w250" value="" placeholder="File Title" />
								    <div class="error-message-container redtext"></div>
							    </div>
							    <div class="question">
								    <label for="edit-file-category">File Category</label>
								    <div class="example-text">Select the appropriate category for the file.</div>
								    <select id="edit-file-category" name="edit_file_category" class="required checkselect w250">
									    <option value="-9999">- Select -</option>
                                        <%=get_categories()%>
								    </select>
								    <div class="error-message-container redtext"></div>
							    </div>
                                <div class="question">
								    <label for="edit-file-region">Region</label>
								    <div class="example-text">Select the appropriate TDOT Region for the file.</div>
								    <select id="edit-file-region" name="edit_file_region" class="required checkselect w250">
									    <option value="-9999">- Select -</option>
                                        <%=get_regions()%>
								    </select>
								    <div class="error-message-container redtext"></div>
							    </div>
                                <div class="question">
								    <label for="edit-file-county">County</label>
								    <div class="example-text">Select the appropriate county for the file.</div>
								    <select id="edit-file-county" name="edit_file_county" class="required checkselect w250">
									    <option value="-9999">- Select -</option>
								    </select>
								    <div class="error-message-container redtext"></div>
							    </div>                                
							    <div class="question">
								    <label for="edit-file-description">File Description</label>
								    <div class="example-text">Provide a description of the file's contents.</div>
								    <textarea cols="40" rows="5" id="edit-file-description" name="edit_file_description" class="required checktext w250" placeholder="File Description"></textarea>
								    <div class="error-message-container redtext"></div>
							    </div>
						    </div>
						    <div class="column-2">
                                <div class="question">
								    <label for="edit-file-pin">PIN</label>
								    <div class="example-text">Enter the PIN number, e.g., 101000.01.</div>
								    <input type="text" id="edit-file-pin" name="edit_file_pin" class="required checktext w250" value="" placeholder="PIN number" />
								    <div class="error-message-container redtext"></div>
							    </div>
                                <div class="question">
								    <label for="edit-file-monitoring-year">Monitoring Year</label>
								    <div class="example-text">Select the appropriate monitoring year for the file.</div>
								    <select id="edit-file-monitoring-year" name="edit_file_monitoring_year" class="required checkselect w250">
									    <option value="-9999">- Select -</option>
                                        <option value="N/A">N/A</option>
                                        <option value="2007">2007</option>
                                        <option value="2008">2008</option>
                                        <option value="2009">2009</option>
                                        <option value="2010">2010</option>
                                        <option value="2011">2011</option>
                                        <option value="2012">2012</option>
                                        <option value="2013">2013</option>
                                        <option value="2014">2014</option>
								    </select>
								    <div class="error-message-container redtext"></div>
							    </div>
                                <div class="question">
								    <label for="edit-file-organization">Contractor</label>
								    <div class="example-text">Select the appropriate contractor.</div>
								    <select id="edit-file-organization" name="edit_file_organization" class="required checkselect w250">
									    <option value="-9999">- Select -</option>
                                        <%=get_organization()%>
								    </select>
								    <div class="error-message-container redtext"></div>
							    </div>
                                 <div class="question">
								    <label for="edit-file-tdec">TDEC Permit Number (NRS)</label>
								    <div class="example-text">Enter the NRS number, e.g., NRS 07.342.</div>
								    <input type="text" id="edit-file-tdec" name="edit_file_tdec" class="checktext w250" value="" placeholder="TDEC Permit Number" />
								    <div class="error-message-container redtext"></div>
							    </div>
                                <div class="question">
								    <label for="edit-file-usace">USACE Permit Number</label>
								    <div class="example-text">Enter the USACE number, e.g., NWP #12; 200701629.</div>
								    <input type="text" id="edit-file-usace" name="edit_file_usace" class="checktext w250" value="" placeholder="USACE Permit Number" />
								    <div class="error-message-container redtext"></div>
							    </div>
						    </div>
						    <input type="hidden" value="-1" id="hidden-file-id" name="hidden_file_id" />
					    </div>
				    </div>
			    </div>
		    </div>
            <%
     }
     %>
</asp:Content>
