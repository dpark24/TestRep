//custom plugins
(function($) 
{
	$.fn.action_panel = function(options) 
	{
		var defaults = {}, action, opts;

		if(typeof options === "object" || typeof options === "undefined")
		{
			opts = $.extend({}, defaults, options);
			action = "initialize";
		}
		else if(typeof options === "string")
		{
			action = options;
		}
		
		return this.each(function()
		{
			var full_width, button_width, height
				, $self = $(this)
				, $button = $(".action-button", this)
				, $message = $(".action-message", this)
				, $message_text = $(".message", $message);
			
			if(action === "initialize")
			{
				height = $button.outerHeight();
				if($self.hasClass("action-panel-inline"))
				{
					full_width = $self.outerWidth();
					button_width = $button.outerWidth();
					
					$message.width(full_width - button_width);
				}
				else if($self.hasClass("action-panel-stacked"))
				{
					button_width = $("a", $button).outerWidth();
					
					$message.width(button_width);
				}
				
				//$message.height(height);
				//$message.css("line-height", height + "px");
				
				$self.bind("loading", function(event, options)
					{
						$self.addClass("action-panel-loading");
						$("a", $button).addClass("disabled");
						$message_text.text("Loading..");
					});
					
				$self.bind("loaded", function(event, options)
					{
						$self.removeClass("action-panel-loading").addClass("action-panel-loaded");
						$("a", $button).removeClass("disabled");
						$message_text.text("");
					});
					
				$self.bind("idle", function(event, options)
					{
						$self.removeClass("action-panel-loading action-panel-loaded");
						$("a", $button).removeClass("disabled");
						$message_text.text("");
					});
					
				$self.bind("success", function(event, options)
					{
						$self.removeClass("action-panel-loading action-panel-loaded");
						$("a", $button).removeClass("disabled");
						$message_text.text("Loaded");
					});
					
				$self.bind("failure", function(event, options)
					{
						$self.removeClass("action-panel-loading action-panel-loaded");
					});
			}
			else
			{
				$(this).trigger(action);
			}
		});
	};
})(jQuery);

var TDOTFiles = {};

/**
 * Function to show the selected tab panel, hide the other panels, and show the panel tab as selected.
 * @author Paul Vidal, Stantec
 * @param {Object} $list The DOM UL element
 * @param {String} class_name The class name of the order number element
 * @returns Nothing is returned as it acts on the wrappers directly
 */
function update_list_order($list, class_name)
{
	if(typeof class_name === "undefined")
	{
		class_name = "order";
	}
	
	$list.find("li ." + class_name).each(function(i, val)
		{
			$(this).html(i + 1);
		});
		
	return $list;
}

function show_tab($container, tab, options) 
{
	if (typeof options === "undefined" || jQuery.isEmptyObject(options)) 
	{
		options = {};
	}

	//check if the options are set and, if not, use the defaults.
	var $wrapper = $(".tabs-container")
		, change_links = (options.change_links !== undefined) ? options.change_links : true
		, $current;

	if (change_links) 
	{
		$("a", $container).removeClass("active");

		//sets the selected tab as selected (black)
		$("a[name='tab_" + tab + "']", $container).addClass("active");
		$("a[data-tab='" + tab + "']", $container).addClass("active");
	}

	//loop through the panels and show only the indicated panel
	jQuery.each($wrapper.find(".tab-content"), function () 
		{
			if ($(this).data("tab") === tab) 
			{
				$(this).removeClass("hidden");
				$current = $(this);
			}
			else 
			{
				if (!($(this).hasClass("hidden"))) 
				{
					$(this).addClass("hidden");
					$(this).trigger("tabHidden");
				}
			}
		});
	
	$container.trigger("tabChanged", [tab]);
	$current.trigger("tabShown");
}

//Gets all of the url parameters as an object {parameter name: parameter value}
function get_url_parameters()
{
	var params = {};
	window.location.search.replace(/[?&]+([^=&]+)=([^&]*)/gi, function (str, key, value)
		{
			params[key] = value;
		});
	return params;
}

// Gets the value of a specified parameter
function get_url_parameter(name)
{
	var regex_string, regex, results;
	
	name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
	regex_string = "[\\?&]" + name + "=([^&#]*)";
	regex = new RegExp(regexS);
	results = regex.exec(window.location.href);
	
	return (results == null) ? "" : results[1];
}

if(typeof $.fn.dataTableExt !== "undefined")
{
	/*
	 * Function: fnAddTr
	 * Purpose:  Add a TR element to a table
	 * Returns:  -
	 * Inputs:   object:oSettings - automatically passed by DataTables
	 *           node:nTr - TR element to add to table
	 *           array:sort_on - optional - should the column(s) to sort on after redrawing - second row, ascending.
	 * Usage:    var row = '<tr class="gradeX"><td>1</td><td>2</td><td>3</td><td>4</td><td>5</td></tr>';
						 oTable.fnAddTr( $(row)[0] );
	 */
	$.fn.dataTableExt.oApi.fnAddTr = function (oSettings, nTr, sort_on) 
	{
		var row, attributes = {}, cached_row, key
			, nTds
			, aData = []
			, iIndex, i, len;
			
		if(typeof nTr === 'string')
		{
			row = $(nTr);
			$.each(row[0].attributes, function(index, attr) 
				{
					attributes[attr.name] = attr.value;
				});
			row = row[0];
		}
		
		nTds = row.getElementsByTagName('td')
		if(typeof sort_on === 'undefined')
		{
			sort_on = [[1, 'asc']];
		}
		
		len = nTds.length
		if(len != oSettings.aoColumns.length)
		{
			//console.debug('Warning: not adding new TR - columns and TD elements must match');
			return;
		}
		
		aData = [];
		for (i=0;i<len;i++)
		{
			aData.push(nTds[i].innerHTML);
		}
		
		/* Add the data and then replace DataTable's generated TR with ours */
		iIndex = this.oApi._fnAddData(oSettings, aData);
		
		oSettings.aoData[iIndex].row = row;
		cached_row = oSettings.aoData[iIndex].nTr;
		
		//add all the attributes to the row
		for(key in attributes)
		{
			if(attributes.hasOwnProperty(key))
			{
				$(cached_row).attr(key, attributes[key]);
			}
		}
		
		oSettings.aiDisplay = oSettings.aiDisplayMaster.slice();
		
		this.oApi._fnReDraw(oSettings);
		
		//sort it back on what we want
		//TODO: Sort based on what the table is currently sorted on
		if(sort_on)
		{
			this.fnSort(sort_on);
		}
	}

	$.fn.dataTableExt.oApi.fnGetFilteredNodes = function (oSettings)
		{
			var anRows = [], i, len = oSettings.aiDisplay.length, row;
			
			for (i=0;i<len;i++)
			{
				row = oSettings.aoData[oSettings.aiDisplay[i]].nTr;
				anRows.push(row);
			}
			return anRows;
		};
}

jQuery(document).ready(function()
	{
		var $base_dialog = $("#base-dialog")
			, config = { base_path: "/TDOT/"
							, ajax: "/TDOT/ajax.aspx"
						}
			, last_checked;

		TDOTFiles.Documents = new DocumentManager({trigger_element: "#content"});
		TDOTFiles.Documents.init();
        TDOTFiles.Uploader = new documentFileUploader();
		
		$("#Body_LoginUser_UserName").focus();
		
		if($("#daily-reports-masonry").length > 0)
		{
			$(".masonry-container").masonry({
					// options
					itemSelector : '.masonry-item'
				});
		}
		
		function generic_dialog(message, options)
		{
			if(typeof options === "undefined")
			{
				options = {};
			}
			
			if(message === "close")
			{
				$base_dialog.dialog("close");
				return false;
			}
			
			options.modal = (typeof options.modal !== "undefined") ? options.modal : true;
			options.show = (typeof options.show !== "undefined") ? options.show : "slide";
			options.title = (typeof options.title !== "undefined") ? options.title : "System Alert";
			options.width = (typeof options.width !== "undefined") ? options.width : 300;
			options.height = (typeof options.height !== "undefined") ? options.height : "auto";
			options.buttons = (typeof options.buttons !== "undefined") ? options.buttons : {Ok: function() { $(this).dialog("close"); }};
			
			$base_dialog.find("div:first").html(message).find(".datepicker").datepicker();
			$base_dialog.dialog("option", options);
			$base_dialog.dialog("open");
			
			return $base_dialog;
		}
		
		$("#create-user-dialog").dialog({autoOpen: false
				, height: "auto"
				, width: 375
				, maxWidth: 400
				, maxHeight: 500
				, modal: true
				, show: "fade"
				, buttons: {
					"Create User": function()
						{
							var meta = {}, $d = $(this);
							
							meta.action = "create-user-account";
							meta.username = $.trim($("#username").val());
							meta.password = $.trim($("#password").val());
							meta.email = $.trim($("#email").val());
							
							$(".dialog-footer", this).text("Please wait while the system processes your request..");
							
							$.post(config.ajax, meta, function(data)
								{
									var $table = $("#user-management-table");
									
									if(data && data.status == 1)
									{
										$table.dataTable().fnClearTable();
										$("tbody", $table).html(data.user_table);
										$table.trigger("refresh");
										$d.dialog("close");
										$(".dialog-footer", $d).text("");
									}
									else if(data && data.status == -2)
									{
										generic_dialog(data.reason);
									}
									else
									{
										generic_dialog("The system experienced an unexpected error. Please contact the system administrator if the problem persists.");
									}
								}, "json");
						}
					, Cancel: function() { $(this).dialog("close"); } 
				}
				, open: function()
					{

						var $buttons = $(".ui-dialog-buttonpane:visible");
						jQuery.each($("input", this), function()
							{
								$(this).val("");
							});
						
						//some nice styles
						$buttons.find("button:first").addClass("simple-button simple-action simple-red");
						$buttons.find("button:last").addClass("simple-button simple-action simple-grey");
					}
			});
		
		$(".create-user").click(function()
			{
				$("#create-user-dialog").dialog("open");
				return false;
			});
			
		$("#change-password-dialog").dialog({autoOpen: false
				, height: "auto"
				, width: 300
				, maxWidth: 400
				, maxHeight: 500
				, modal: true
				, show: "fade"
				, buttons: {
					"Save New Password": function()
						{
							var meta = {}, $d = $(this);
							
							meta.action = "change-user-password";
							meta.old_password = $.trim($("#old-password").val());
							meta.new_password = $.trim($("#new-password").val());
							meta.confirm_password = $.trim($("#confirm-password").val());
							
							
							if(meta.new_password.length >= 8 && (meta.new_password === meta.confirm_password))
							{
								$(".dialog-footer", this).text("Please wait while the system processes your request..");
								$.post(config.ajax, meta, function(data)
									{
										if(data && data.status == 1)
										{
											$d.dialog("close");
											generic_dialog('Your password has been successfully changed. It\'s recommended that you <a href="#" class="logout" title="Logout of the portal">logout</a> and log in again to confirm the changes have been properly applied.');
										}
										else if(data && data.status == 1)
										{
											generic_dialog('The system experienced an error processing your request. Please confirm your information and try again.');
											
										}
										else
										{
											generic_dialog("The system experienced an unexpected error. Please contact the system administrator if the problem persists.");
										}
										$(".dialog-footer", $d).text("");
									}, "json");
								//*/
							}
							else if(meta.new_password.length < 8)
							{
								generic_dialog("Please enter a new password of eight (8) characters or more.");
							}
							else if(meta.new_password !== meta.confirm_password)
							{
								generic_dialog("Your new password and confirmation password do not match. Please make sure the two fields are exactly the same.");
							}							
						}
					, Cancel: function() { $(this).dialog("close"); } 
				}
				, open: function()
					{
						var $buttons = $(".ui-dialog-buttonpane:visible");
						jQuery.each($("input", this), function()
							{
								$(this).val("");
							});
						
						//some nice styles
						$buttons.find("button:first").addClass("simple-button simple-action simple-red");
						$buttons.find("button:last").addClass("simple-button simple-action simple-grey");
					}
			});
		
		$(".change-password").click(function()
			{
				$("#change-password-dialog").dialog("open");
				return false;
			});
			
		$("body").on("click", "a.logout", function()
			{
				__doPostBack('ctl00$HeadLoginView$HeadLoginStatus$ctl00', '');
				return false;
			});
		$(".navigation-menu").supersubs({minWidth: 12, maxWidth: 20, extraWidth: 1}).superfish();
		$(":input.required").closest(".question").find("label:first").attr("title", "Required").append('<sup class="redtext reqtext" title="Required">*Req<\/sup>');
		
		$("#content").on("click", ".delete-file", function()
			{
				var meta = {}
					, $row = $(this).closest("tr");
				
				if(confirm("Are you sure you want to delete this file?"))
				{
					meta.action = "delete-file";
					meta.file_id = $row.data("file_id");
					
					$.post(config.ajax, meta, function(data)
						{
							if(data && data.status == 1)
							{
								$("#file-management-table").dataTable().fnDeleteRow($row[0]);
							}
							else
							{
								generic_dialog("Sorry, the system encountered an error.");
							}
						}, "json");
				}
				
				return false;
			});
		
		$("#content").on("click", ".dt-clear-filters", function()
			{
				var i, len, settings
					, DTable = $(".sortable-ajax").dataTable();
				
				settings = DTable.fnSettings();
				len = settings.aoPreSearchCols.length;
				
				for(i=0;i<len;i++)
				{
					settings.aoPreSearchCols[i].sSearch = '';
				}
				
				settings.oPreviousSearch.sSearch = '';
				DTable.fnDraw();
				
				jQuery.each($("#filter-controls select"), function()
					{
						this.selectedIndex = 0;
					});
					
				$(".dataTables_filter input").val("");

				return false;
			});
		
		//reference items
		$(".sort-options").sortable({update: function() { update_list_order($(this)) }});
		
		//javascript function
		$("#content").on("click", ".edit-file", function () 
			{
				var meta = {}
					, info = {}
					, file_data = $(this).data("File")
					, file_id
					, $tr = $(this).closest("tr");

				if (typeof file_data === "undefined") 
				{
					file_id = +$tr.data("file_id");
					meta.action = "get-file-information";
					meta.file_id = file_id;

					$.post(config.ajax, meta, function (data)
						{
							var File
								, field_date;

							if (data && data.status == 1) 
							{
								File = data.file_information;
								
								info.edit_file_title = File.Title;
								info.edit_file_description = File.Description;
								info.edit_file_category = File.CategoryId;
								info.edit_file_organization = File.OrganizationId;
								info.edit_file_pin = File.Pin;
								info.edit_file_tdec = File.TdecPermit;
								info.edit_file_usace = File.USACEPermit;
								info.edit_file_monitoring_year = File.MonitoringYear;
								info.edit_file_county = File.CountyId;
								
								if(typeof File.Meta.region_id !== "undefined")
								{
									info.edit_file_region = File.Meta.region_id;
								}

								info.hidden_file_id = meta.file_id;
								//calls the edit method in DocumentManager.js and populates files based on the element "name"
								TDOTFiles.Documents.edit(info);
								
								if(typeof File.Meta.region_id !== "undefined")
								{
									$("#edit-file-region").trigger("change", [File.CountyId]);
								}
							}
						}, "json");
				}
				
				return false;
			});
			
		$("#content").bind("documentEdited", function(event, dialog, meta, data)
			{
				TDOTFiles.Documents.edit("close");
				$(".sortable-ajax").dataTable().fnDraw();
				return false;
			});
		
		$("#daily-report-selection").change(function()
			{
				var meta = {};
				
				meta.action = "get-slider-data";
				meta.field_date = this.value;
				
				$.post(config.ajax, meta, function(data)
					{
						var $container = $("#daily-reports-masonry div:first");
						if(data && data.status == 1)
						{
							$(".masonry-container").masonry("destroy");
							
							$container.html(data.SliderImages);
							$(".masonry-container").masonry({
									// options
									itemSelector: '.masonry-item'
								});
						}
						else
						{
						}
					}, "json");
			});
		
		$(".column-select-all").click(function()
			{
				var $table = $(this).closest("table")
					, $boxes = $(".add-to-download-package", $table.dataTable().fnGetFilteredNodes())
					, checked = $(this).is(":checked");
				
				$.each($boxes, function()
					{
						$(this).attr("checked", checked);
					});
					
				$("tbody", $table).trigger("selectionUpdated");
			});
		
		$("#file-management-table tbody").bind("selectionUpdated", function()
			{
				var $table = $(this).closest("table")
					, $boxes = $(".add-to-download-package", $table.dataTable().fnGetNodes())
					, count = 0
					, file_ids = []
					, download_url = 'GetFile.aspx?files=';
				
				$.each($boxes, function()
					{
						if($(this).is(":checked"))
						{
							file_ids.push(this.value);
							count++;
						}
					});
				
				if(count > 0)
				{
					$(".download-package").html('There are currently ' + count + ' files awaiting download. <a href="' + download_url + file_ids.join(',') + '">Download your files here.</a>');
				}
				else
				{
					$(".download-package").html("Select files to add to your download package.");
				}
				
				return false;
			});

		$(".selectable [type='checkbox']").live("click", function(event) 
			{
				var start, end
				//this should be the same as the original selector statement
				, $checkboxes = $(".selectable [type='checkbox']");

				//this is the first time they're clicking on something..
				// dies in this block if it is the first click
				if(!last_checked) 
				{
					last_checked = this;
					$(this).closest("tbody").trigger("selectionUpdated");
					return;
				}

				//if they're holding down the shift key, let's check everything in between
				if(event.shiftKey) 
				{
					start = $checkboxes.index(this);
					end = $checkboxes.index(last_checked);

					//some fun array stuff to get only the first item clicked, the last one, and everything in between
					// since we were originally dealing with checkboxes, the "checked" attribute was set
					// that can be changed to do whatever, including trigger an event all items in the array
					$checkboxes.slice(Math.min(start, end), Math.max(start, end) + 1).attr('checked', last_checked.checked);
				}

				//make sure we remember where we started
				last_checked = this;
				$(this).closest("tbody").trigger("selectionUpdated");
			});
		
		if ($(".tabber").length > 0) 
		{
			$(".tabber").live("changeTab", function (event, tab_name) 
				{
					show_tab($(this), tab_name);
					return false;
				});

			$(".tabber a").live("click", function () 
				{
					var tab_name = '';

					if (typeof $(this).data("tab") !== "undefined") 
					{
						$(".tabber").trigger("changeTab", $(this).data("tab"));
					}
					else 
					{
						$(".tabber").trigger("changeTab", this.name.split("_")[1]);
					}

					return false;
				});
		}
		
		//opens specified links in new windows
		$("body").on("click", "a.external", function()
			{
				window.open(this.href);
				return false;
			});
		
		$(".show-advanced-search").click(function()
			{
				$("#advanced-repair-search").toggle();
				return false;
			});
					
		$(".document-meta").on("click", ".edit-document", function()
			{
			    //TDOTFiles.Documents.edit({ id: 3, description: "aasdf", title: "This Title" });
				return false;
			});
		
		//qtips are appended to the main body
		$("body").on("click", ".add-document", function()
			{
				//console.debug($(this).data());
			    TDOTFiles.Documents.add({});
				return false;
			});
		
		$("#user-management-table").on("click", ".delete-user-account", function()
			{
				var meta = {}
					$self = $(this);
				
				meta.action = 'delete-user-account';
				meta.user_name = $(this).data('user_name');
				
				if(confirm('Are you sure you want to delete this user? This cannot be undone.'))
				{
					$.post(config.ajax, meta, function(data)
						{
							var $tr = $self.closest('tr')
								,  $table = $tr.closest('table');
							if(data && data.status == 1)
							{
								$table.dataTable().fnDeleteRow($tr[0]);
							}
							else
							{
								generic_dialog('Sorry, we experienced an error. Please let the system administrators know about your trouble.');
							}
						}, 'json');
				}
				
				return false;
			});
		
		$("#user-management-table").on("click", ".toggle-user-approval", function()
			{
				var meta = {}
					$self = $(this);
				
				meta.action = 'toggle-user-approval';
				meta.user_name = $(this).data('user_name');
				
				$.post(config.ajax, meta, function(data)
					{
						var $span = $("span", $self);
						if(data && data.status == 1)
						{
							if($span.hasClass('ui-icon-notice'))
							{
								$span.removeClass('ui-icon-notice').addClass('ui-icon-check');
							}
							else
							{
								$span.removeClass('ui-icon-check').addClass('ui-icon-notice');
							}
						}
						else
						{
						}
					}, 'json');
				
				return false;
			});
		
		$("#user-management-table").on("click", ".access-level-toggle", function()
			{
				var meta = {}
					$self = $(this);
				
				meta.action = 'toggle-user-role';
				meta.user_name = $self.closest("tr").data('username');
				meta.role_level = $self.closest("td").data('role_level');
				
				$.post(config.ajax, meta, function(data)
					{
						if(data && data.status == 1)
						{
							if($self.hasClass('sprite-accept'))
							{
								$self.removeClass('sprite-accept').addClass('sprite-delete');
							}
							else
							{
								$self.removeClass('sprite-delete').addClass('sprite-accept');
							}
						}
						else
						{
						}
					}, 'json');
				
				return false;
			});
		
		$("#file-management-table").on("click", "a.toggle-full-text", function()
			{
				var $td = $(this).closest("td");
				
				$(".excerpt", $td).toggle();
				$(".full-text", $td).toggle();
				
				if($(".full-text", $td).is(":visible"))
				{
					$(this).text("Show less");
				}
				else
				{
					$(this).text("Show more");
				}
				
				return false;
			});
		
		$("#upload-file-region, #edit-file-region").change(function(event, county)
			{
				var meta = {};
				
				meta.action = "get-region-counties";
				meta.region_id = $(this).val();
				
				if(meta.region_id > 0)
				{
					$("#upload-file-county").html('<option value="-9999">Loading Options..</option>');
					$("#edit-file-county").html('<option value="-9999">Loading Options..</option>');
					$.post(config.ajax, meta, function(data)
						{
							if(data && data.status == 1)
							{
								$("#upload-file-county").html('<option value="-9999">- Select -</option>' + data.options);
								$("#edit-file-county").html('<option value="-9999">- Select -</option>' + data.options);
								
								//only required for the edit dialog
								if(typeof county !== "undefined")
								{
									$("#edit-file-county").val(county);
								}
							}
							else
							{
								//todo: alert users
							}
						}, "json");
				}
			});
			
		$("#region-filter").change(function()
			{
				var meta = {};
				
				meta.action = "get-region-counties";
				meta.region_id = $(this).val();
				
				if(meta.region_id > 0)
				{
					$("#county-filter").html('<option value="-9999">Loading Options..</option>');
					$.post(config.ajax, meta, function(data)
						{
							if(data && data.status == 1)
							{
								$("#county-filter").html('<option value="-9999">- Select -</option>' + data.options);
							}
							else
							{
								//todo: alert users
							}
						}, "json");
				}
			});
		
		$("#content").bind("documentUploadSuccessful", function(event, meta)
			{
				var html = ''
					, $table = $("#file-management-table")
					, meta = {action: 'get-document-table'};
				
				
				$.post(config.ajax, meta, function(data)
					{
						if(data && data.status == 1)
						{
							$table.dataTable().fnClearTable();
							$("tbody", $table).html(data.table);
							$table.trigger("refresh");
						}
						else
						{
							//alert('issues');
						}
						
					}, "json");
				
				return false;
			});

		$(".tooltip").qtip({style: { classes: "ui-tooltip-shadow"}
											, position: {my: "top center"
														, at: "bottom center"
													}
										});
		
		$(".tooltip-red-left").qtip({style: { classes: "ui-tooltip-dark ui-tooltip-shadow"}
											, position: {my: "right center"
														, at: "left center"
													}
										});
		$(".tooltip-red-top").qtip({style: { classes: "ui-tooltip-dark ui-tooltip-shadow"}
											, position: {my: "bottom center"
														, at: "top center"
													}
										});
			
		$(".datepicker").datepicker({changeMonth: true, changeYear: true, onClose: function(text, input) { $(this).trigger("updatedField"); }});
		$base_dialog.dialog({autoOpen: false
									, buttons: { Ok: function() { $(this).dialog("close"); } }
									, height: "auto"
									, width: 600
									, maxWidth: 600
									, maxHeight: 600
									, modal: false
									, show: "fade"
								});

		
		$(".sortable").on("mouseover", "tbody tr", function()
			{
				
			});
		
		//initiates the tablesorter plugin on all tables with the sortable class
		if(typeof $.fn.dataTableExt !== "undefined")
		{
			//initiates the tablesorter plugin on all tables with the sortable class
			$.fn.dataTableExt.sErrMode = 'throw';

			jQuery.fn.dataTableExt.aTypes.push(  
					function (sData)  
					{  
						var sValidChars = "0123456789.-,"
							, character
							, i;  
						   
						/* Check the numeric part */ 
						for (i=1;i<sData.length;i++)   
						{   
							character = sData.charAt(i);   
							if(sValidChars.indexOf(character) === -1)   
							{  
								return null;  
							}  
						}  
						   
						/* Check prefixed by currency */ 
						if(sData.charAt(0) === '$')  
						{  
							return 'currency';  
						}  
						return null;  
					}  
				);
			
			$.fn.dataTableExt.oApi.fnFilterClear  = function (oSettings, col_index)
				{
					var feature, i, len;
					
					/* Remove global filter */
					oSettings.oPreviousSearch.sSearch = "";
					  
					/* Remove the text of the global filter in the input boxes */
					if(typeof oSettings.aanFeatures.f != 'undefined')
					{
						n = oSettings.aanFeatures.f;
						len = n.length;
						for(i=0;i<len;i++)
						{
							$('input', n[i]).val('');
						}
					}
					
					if(typeof col_index === "number")
					{
						oSettings.aoPreSearchCols[col_index].sSearch = "";						
					}
					else
					{
						/* Remove the search text for the column filters - NOTE - if you have input boxes for these
						 * filters, these will need to be reset
						 */
						len = oSettings.aoPreSearchCols.length;
						for (i=0;i<iLen;i++)
						{
							oSettings.aoPreSearchCols[i].sSearch = "";
						}
					}

					/* Redraw */
					oSettings.oApi._fnReDraw(oSettings);
				};
			
			jQuery.fn.dataTableExt.aTypes.unshift(
					function (sData)
					{
						var sValidChars = "0123456789-,"
							, character
							, is_decimal = false
							, i, len;
						 
						/* Check the numeric part */
						len = sData.length;
						for(i=0;i<len;i++)
						{
							character = sData.charAt(i);
							if (sValidChars.indexOf(character) == -1)
							{
								return null;
							}
							 
							/* Only allowed one decimal place... */
							if(character == "0")
							{
								if(is_decimal)
								{
									return null;
								}
								
								is_decimal = true;
							}
						}
						 
						return 'numeric-comma';
					}
				);
			 
			jQuery.fn.dataTableExt.oSort['currency-asc'] = function(a, b) 
				{
					/* Remove any commas (assumes that if present all strings will have a fixed number of d.p) */
					var x = a == "-" ? 0 : a.replace(/[$,]/g, "")
						, y = b == "-" ? 0 : b.replace(/[$,]/g, "");

					/* Parse and return */
					x = parseFloat(x);
					y = parseFloat(y);
					return x - y;
				};
			 
			jQuery.fn.dataTableExt.oSort['currency-desc'] = function(a, b) 
				{
					/* Remove any commas (assumes that if present all strings will have a fixed number of d.p) */
					var x = a == "-" ? 0 : a.replace(/[$,]/g, "" )
						, y = b == "-" ? 0 : b.replace(/[$,]/g, "" );

					/* Parse and return */
					x = parseFloat(x);
					y = parseFloat(y);
					return y - x;
				};
				 
			jQuery.fn.dataTableExt.oSort['numeric-comma-asc']  = function(a, b) 
				{
					var x = (a == "-") ? 0 : a.replace(/,/g, "")
						, y = (b == "-") ? 0 : b.replace(/,/g, "");
						
					x = parseFloat(x);
					y = parseFloat(y);
					return ((x < y) ? -1 : ((x > y) ?  1 : 0));
				};
			 
			jQuery.fn.dataTableExt.oSort['numeric-comma-desc'] = function(a, b) 
				{
					var x = (a == "-") ? 0 : a.replace(/,/g, "")
						, y = (b == "-") ? 0 : b.replace(/,/g, "");
					
					x = parseFloat(x);
					y = parseFloat(y);
					return ((x < y) ?  1 : ((x > y) ? -1 : 0));
				};
		}
		
		$(".clear-dt-filter").click(function()
			{
				var i, len, settings
					, DTable = $(".sortable").dataTable();
				
				/*/
				var search_params = [];
				jQuery.each($("table.sortable thead th"), function()
					{
						search_params.push(null);
					});
				$(".sortable").dataTable().fnFilter("");
				//*/
				
				settings = DTable.fnSettings();
				len = settings.aoPreSearchCols.length;
				for(i=0;i<len;i++)
				{
					settings.aoPreSearchCols[i].sSearch = '';
				}
				
				settings.oPreviousSearch.sSearch = '';
				DTable.fnDraw();
				
				$(this).closest("div").html("&nbsp;");
				
				return false;
			});
		
		/*
		 * Filters
		 */
		 $("#year-filter").change(function()
			{
				var $file_table = $("#file-management-table")
					, year = $(this).val();
				
				$file_table.dataTable().fnFilter(year, 2);
				if(year !== "-9999")
				{
					$file_table.dataTable().fnFilter(year, 2);
				}
				else
				{
					$file_table.dataTable().fnFilterClear(2);
				}
			});
		
		$("#region-filter").change(function()
			{
				var $file_table = $("#file-management-table")
					, region = $(this).val();
				
				if(region > 0)
				{
					$file_table.dataTable().fnFilter(region, 3);
				}
				else
				{
					$file_table.dataTable().fnFilterClear(3);
				}
			});
			
		$("#county-filter").change(function()
			{
				var $file_table = $("#file-management-table")
					, county = $(this).val();
				
				$file_table.dataTable().fnFilter(county, 4);
			});
			
		$("#contractor-filter").change(function()
			{
				var $file_table = $("#file-management-table")
					, org = $(this).val();
				
				$file_table.dataTable().fnFilter(org, 6);
			});
			
		$("#category-filter").change(function()
			{
				var $file_table = $("#file-management-table")
					, category = $(this).val();
				
				$file_table.dataTable().fnFilter(category, 7);
			});
		
		jQuery.each($(".sortable-ajax"), function()
			{
				var server_side_options = {bSortClasses: false
							, bProcessing: true
							, bServerSide: true
							, sPaginationType: "full_numbers"
							, bJQueryUI: false
							, sDom: '<"dt-toolbars"<"dt-top-toolbar"lfr<"clear">><"dt-bottom-toolbar"ip<"clear">>>t'
							, oLanguage: {sEmptyTable: "No data available in table", sZeroRecords: "No records to display"}
							, iDisplayLength: 25
							, bAutoWidth: true
							, sAjaxSource: config.ajax
							, fnServerParams: function (aoData) 
								{
									aoData.push( { name: "action", value : "get-file-management-table" } );
								}
							, fnRowCallback: function (row) 
								{
									var $row = $(row);
									
									$row.data("file_id", row.id.split("-")[1]);
									
									$("td:eq(0)", $row).addClass("center-text");
									$("td:gt(1)", $row).addClass("center-text");
									
									return row;
								}
						}
					, primary_sorting = $("thead th.primary", this).index()
					, show = $(".toggle-row-display .selected").data("show")
					, search_params = []
					, $ignore_sorting = $("thead th.dont-sort", this)
					, cols = [];
				
				if($ignore_sorting.length > 0)
				{
					jQuery.each($("thead th", this), function()
						{
							var ignore = $(this).hasClass("dont-sort");
							cols.push((ignore) ? {bSortable: false} : null);
						});
						
					server_side_options.aoColumns = cols;
				}
				
				if(primary_sorting >= 0)
				{
					server_side_options.aaSorting = [[primary_sorting, "asc"]];
				}
				
				jQuery.each($("thead th", this), function()
					{
						var filter = $(this).data("filter");
						if(typeof filter === "undefined" || filter === "")
						{
							search_params.push(null);
						}
						else
						{
							search_params.push({"sSearch" : $(this).data("filter")});
						}
					});
				
				server_side_options.aoSearchCols = search_params;
			  
				if(typeof $(this).data("initial_display") !== "undefined")
				{
					server_side_options.iDisplayLength = +$(this).data("initial_display");
					
				}
			  
				$(this).dataTable(server_side_options);

				$("#datatables-search-input").live("keyup", function(e) 
					{
						if (e.keyCode == 27)
						{
							this.value = "";

							//reset the field and induce the table to reset by triggering the keyup again
							$(this).blur();
							$(this).trigger("keyup");
						}
					});
			});
			
		jQuery.each($(".sortable"), function()
			{
				var base_options = {bSortClasses: false
									, bProcessing: true
									, sPaginationType: "full_numbers"
									, bJQueryUI: false
									, sDom: '<"dt-toolbars"<"dt-top-toolbar"lfr<"clear">><"dt-bottom-toolbar"ip<"clear">>>t'
									, oLanguage: {sEmptyTable: "No data available in table", sZeroRecords: "No records to display"}
									, iDisplayLength: 25
									, bAutoWidth: false}
					, primary_sorting = $("thead th.primary", this).index()
					, $ignore_sorting = $("thead th.dont-sort", this)
					, cols = [];
				
				if($ignore_sorting.length > 0)
				{
					jQuery.each($("thead th", this), function()
						{
							var ignore = $(this).hasClass("dont-sort");
							cols.push((ignore) ? {bSortable: false} : null);
						});
						
					base_options.aoColumns = cols;
				}				
				
				if(primary_sorting >= 0)
				{
					base_options.aaSorting = [[primary_sorting, "asc"]];
				}

				if(typeof $(this).data("initial_display") !== "undefined")
				{
					base_options.iDisplayLength = +$(this).data("initial_display");
				}

				$(this).dataTable(base_options);
				
				$(this).bind("refresh", function()
					{
						var base_options = {bSortClasses: false
											, bProcessing: true
											, sPaginationType: "full_numbers"
											, sDom: '<"dt-toolbars"<"dt-top-toolbar"lfr<"clear">><"dt-bottom-toolbar"ip<"clear">>>t'
											, bDestroy: true
											, oLanguage: {sEmptyTable: "No data available in table", sZeroRecords: "No records to display"}
											, iDisplayLength: 25
											, bJQueryUI: false}
							, primary_sorting = $("thead th.primary", this).index()
							, $ignore_sorting = $("thead th.dont-sort", this)
							, cols = [];
						
						if($ignore_sorting.length > 0)
						{
							jQuery.each($("thead th", this), function()
								{
									var ignore = $(this).hasClass("dont-sort");
									cols.push((ignore) ? {bSortable: false} : null);
								});
								
							base_options.aoColumns = cols;
						}
						
						if(primary_sorting >= 0)
						{
							base_options.aaSorting = [[primary_sorting, "asc"]];
						}

						if(typeof $(this).data("initial_display") !== "undefined")
						{
							base_options.iDisplayLength = +$(this).data("initial_display");
						}

						$(this).dataTable().fnDestroy();
						$(this).dataTable(base_options);
						return false;
					});
				
				$("#datatables-search-input").live("keyup", function(e) 
					{
						if (e.keyCode == 27)
						{
							this.value = "";
							
							//reset the field and induce the table to reset by triggering the keyup again
							$(this).blur();
							$(this).trigger("keyup");
						}
					});
			});
	});