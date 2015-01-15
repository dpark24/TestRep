function DocumentManager(options)
{
	if(typeof options !== 'object')
	{
		options = {};
	}
	
	var config = {ajax: options.ajax || 'ajax.aspx'
					, dialog_defaults: options.dialog_defaults || {}
				/*	, uploadify_initialized: false
					, uploadify_container_id: options.uploadify_container_id || 'upload-doc'
					, uploadify_id: options.uploadify_id || 'uploadify'*/
					, basepath: options.basepath || window.location.pathname.substr(0, (window.location.pathname.lastIndexOf("/") + 1))
				/*	, swf_uploader: options.swf_uploader || 'js/lib/uploadify.swf'*/
					, action_name: options.action_name || 'upload-document'
					, trigger_element: options.trigger_element || '#main'
				/*	, library_id: options.library_id || 'library-manager'*/
				}
		, $jq = { uploadify_container: $("#" + config.uploadify_container_id), uploadify: $("#uploadify") }
		, upload_meta = null
		, $dialogs = {}
		, doc_types = { all: ["Documents and Files", null]
							, images: ["Images", "*.jpg;*.png;*.gif;*.jpeg"]
							, documents: ["Documents", "*.pdf;*.doc;*.ppt;*.docx;*.pptx;*.txt;*.xls;*.xlsx"]
							, presentations: ["Presentation Files", "*.pdf;*.ppt;*.pptx"]
						}
		/*, $library;*/

	config.dialog_defaults = {autoOpen: false
									, buttons: { Ok: function() { $(this).dialog("close"); } }
									, height: "auto"
									, width: 600
									, maxWidth: 600
									, maxHeight: 500
									, modal: false
									, show: "fade"
								};

	/*$library = $('#' + config.library_id);*/
	
	$(window).unload(function() 
		{
			$jq = null;
			config = null;
			$dialogs = null;
		});
	
	/**
	 * Adds a given number of days to a given date
	 * @author Paul Vidal, Stantec
	 * @param {Date} base_date Prefered to be a Date object but can also be a string, mm/dd/yyyy
	 * @param {Integer} days The number of days to add to the given date
	 * @returns The new Date object is returned
	 */
	function add_days(base_date, days)
	{
		var d = (base_date.getMonth) ? new Date(base_date) : new Date(base_date.toString());
		d.setDate(base_date.getDate() + days);
		return d;
	}
	
	/**
	 * Takes a given date or date string and makes it a "normal" slashed date
	 * @author Paul Vidal, Stantec
	 * @param {Date} base_date Prefered to be a Date object but can also be a string
	 * @returns The slashed date string is returned
	 */
	function to_slashed_date(d)
	{
		d = (d.getMonth) ? d : new Date(d);
		return (/Invalid|NaN/.test(d)) ? '' : (d.getMonth() + 1) + '/' + d.getDate() + '/' + d.getFullYear();
	}
	
	/**
	 * Takes a given date or date string and makes it a "nice", long date
	 * @author Paul Vidal, Stantec
	 * @param {Date} base_date Prefered to be a Date object but can also be a string
	 * @returns The nicely formatted date string is returned
	 */
	function to_nice_date(d)
	{
		d = (d.getMonth) ? d : new Date(d);
		return (/Invalid|NaN/.test(d)) ? '' : d.toLocaleDateString();
	}
	
	/**
	 * Displays a generic UI dialog with the given message and options
	 * @author Paul Vidal, Stantec
	 * @param {String} message The message to display to users
	 * @param {Object} options An object of options to customize the display.
	 * @returns Nothing is returned
	 */
	function generic_dialog(message, opts)
	{
		if(typeof opts === "undefined")
		{
			opts = {};
		}
		
		if(message === "close")
		{
			$dialogs.base.dialog("close");
			return false;
		}
		
		opts.modal = (typeof opts.modal !== "undefined") ? opts.modal : true;
		opts.title = (typeof opts.title !== "undefined") ? opts.title : "System Alert";
		opts.width = (typeof opts.width !== "undefined") ? opts.width : 300;
		opts.height = (typeof opts.height !== "undefined") ? opts.height : "auto";
		opts.buttons = (typeof opts.buttons !== "undefined") ? opts.buttons : {Ok: function() { $(this).dialog("close"); }};
		
		$dialogs.base.find("div:first").html(message).find(".datepicker").datepicker();
		$dialogs.base.dialog("option", opts);
		$dialogs.base.dialog("open");
		
		return $dialogs.base;
	}
	
	/**
	 * Displays a generic UI dialog with the given message and options
	 * @author Paul Vidal, Stantec
	 * @param {String} message The message to display to users
	 * @param {Object} opts An object of options to customize the display.
	 * @returns Nothing is returned
	 */
	function info_dialog(message, opts)
	{
		if(typeof opts === "undefined")
		{
			opts = {};
		}
		
		opts.modal = (typeof opts.modal !== "undefined") ? opts.modal : false;
		return generic_dialog(message, opts);
	}
	
	function initialize_dialogs()
	{
		$dialogs.base = $("#base-dialog").dialog(config.dialog_defaults);
		
		$dialogs.upload = $("#dialog-upload-document").dialog({autoOpen:false, modal: true, width: 600, minHeight: 500, closeOnEscape: true, buttons:
				{
				    Cancel: function () {
				        $(this).dialog("close");
				    }
				}
				, open: function () {
				    var $buttons = $(".ui-dialog-buttonpane:visible");
				    TDOTFiles.Uploader.initUploader();

				    //reset our form each time
				    jQuery.each($(":input:visible", this), function () {
				        var node_type = this.nodeName.toLowerCase();

				        if (node_type === "select") {
				            this.selectedIndex = 0;
				        }
				        else if (node_type === "input") {
				            if (this.type === "radio" || this.type === "checkbox") {
				                $(this).attr("checked", false);
				            }
				            else {
				                this.value = "";
				            }
				        }
				        else if (node_type === "textarea") {
				            $(this).val("");
				        }
				    });

				    //some nice styles
				    $buttons.find("button:first").addClass("simple-button simple-action simple-red");
				    $buttons.find("button:last").addClass("simple-button simple-action simple-grey");
				}

                , title: "Upload File"
		    /*, beforeclose: function () { destroy_uploadify(); }*/
                , beforeclose: function () {
                    TDOTFiles.Uploader.destroyUploader();
                    //$('.filename').text("");
                    $('.submit-file').addClass('disabled');
                    $(".progress-animated").css({ "width": "0%" });
                }
		});
			
		$dialogs.edit_document = $("#dialog-edit-document").dialog({autoOpen: false, modal: true, width: 600, minHeight: 500, closeOnEscape: false, buttons:
				{
					Save: function()
						{
							var meta = {}
								, status
								, form_validator
								, $d = $(this);
							
							//add our user's data to the data object
							if(typeof Validator === "function")
							{
								form_validator = new Validator(this, {force_overflow: true, short_messages: true, bind_onblur: false});
								
								form_validator.init();
								if(form_validator.is_form_valid())
								{
									meta = form_validator.get_form_data(false);
								}
								else
								{
									return false;
								}
							}
							else
							{
								jQuery.each($(":input:visible", this), function()
									{
										meta[this.name] = $.trim($(this).val());
									});
							}
							
							meta.action = "edit-document-metadata";
							meta.file_id = $("#hidden-file-id").val();
							
							generic_dialog("Please sit tight while the system updates..");
							
							$.post(config.ajax, meta, function(data)
								{
									generic_dialog("close");
									$(config.trigger_element).trigger("documentEdited", [$d, meta, data]);
								}, "json");
							
						},
					Cancel: function()
						{
							$(this).dialog("close");
						}
				}
				, open: function()
					{
						var $buttons = $(".ui-dialog-buttonpane:visible");
						
						//some nice styles
						$buttons.find("button:first").addClass("simple-button simple-action simple-red");
						$buttons.find("button:last").addClass("simple-button simple-action simple-grey");
					}
        });

        $dialogs.delete_document = $("#dialog-delete-document").dialog({ autoOpen: false, modal: true, width: 500, minHeight: 500, closeOnEscape: false, buttons:
				{
				    Delete: function () {
				        var meta = {}
								, status
								, form_validator
								, $d = $(this);

				        //add our user's data to the data object
				        if (typeof Validator === "function") {
				            form_validator = new Validator(this, { force_overflow: true, short_messages: true, bind_onblur: false });

				            form_validator.init();
				            if (form_validator.is_form_valid()) {
				                meta = form_validator.get_form_data(false);
				            }
				            else {
				                return false;
				            }
				        }
				        else {
				            jQuery.each($(":input:visible", this), function () {
				                meta[this.name] = $.trim($(this).val());
				            });
				        }

				        meta.action = "delete-file";
				        console.log(meta);
				        meta.file_id = $("#file-id").val();

				        generic_dialog("Please sit tight while the system updates..");
				        $.post(config.ajax, meta, function (data) {
				            generic_dialog("close");
				            $(config.trigger_element).trigger("documentEdited", [$d, meta, data]);
				        }, "json");
				    },
				    Cancel: function () {
				        $(this).dialog("close");
				    }
				}
				, open: function () {
				    var $buttons = $(".ui-dialog-buttonpane:visible");

				    //some nice styles
				    $buttons.find("button:first").addClass("simple-button simple-action simple-red");
				    $buttons.find("button:last").addClass("simple-button simple-action simple-grey");
				}
        });
	}
	
	function add_document_dialog(opts, data)
	{
		var key;
		
		opts = opts || {};
		data = data || {};
		
		for(key in data)
		{
			if(data.hasownProperty(key))
			{
				$(key).val(data[key]);
			}
		}
		
		$dialogs.upload.dialog("option", opts);
		$dialogs.upload.dialog("open");
		return this;
	}
	
	/**
	 * Constructor. Initiates the jQuery UI dialogs
	 * @author Paul Vidal, Stantec
	 * @returns Nothing
	 */
	function init()
	{
		//start off our dialogs
		initialize_dialogs();
	}
	
	/**
	 * Function to check if the document type is valid. If it's not, it allows all documents.
	 * @author Paul Vidal, Stantec
	 * @param {String} type Checks to see if the provided document type is in the doc_types variable, indicating if it's valid.
	 * @returns If "type" is in "doc_types", then the provided "type" is return, otherwise the string "all" is returned.
	 */
	function valid_doctype(type)
	{
		//check if it's a valid document type
		//if not, we'll default to the all option
		return (type.hasOwnProperty(doc_types)) ? type : "all";
	}
	
/*	function intialize_uploadify()
	{
		var doctype = valid_doctype($("#upload-doctype").val());
		
		$(config.trigger_element).trigger("uploadifyBeforeInitialized");
		
		if($jq.uploadify !== "")
		{
			$jq.uploadify.remove();
		}
		//get the container and add the input
		$jq.uploadify_container.html('<input type="file" name="uploadify" id="' + config.uploadify_id + '" />');
		
		//set the config variable
		$jq.uploadify = $("#" + config.uploadify_id);
		
		//initialize uploadify
		$jq.uploadify.uploadify({uploader: config.swf_uploader
				, auto: false
				, script: config.ajax
				, scriptData: upload_meta || {action: config.action_name, token: config.token, AUTHID: $("#fc").val(), ASPSESSID: $("#si").val()}
				, multi: false
				, fileDesc: doc_types[doctype][0]
				, fileExt: doc_types[doctype][1]
				, queueSizeLimit: 1
				, cancelImg: "images/cancel.png"
				, onAllComplete: function(event, data)
					{
						var meta = $jq.uploadify.uploadifySettings("scriptData");
						
						if(data.filesUploaded > 0 && data.errors === 0)
						{
							$dialogs.upload.dialog("close");
							$(config.trigger_element).trigger("documentUploadSuccessful", [meta, data]);
						}
						else
						{
							generic_dialog("There was an error uploading your document. If this problem persists, please contact a system administrator.", {title: "File Upload Error"});
							$(config.trigger_element).trigger("documentUploadFailed");
						}
					}
			});
		
		config.uploadify_initialized = true;
	}
	
	function destroy_uploadify()
	{
		config.uploadify_initialized = false;
		$jq.uploadify.remove();
		return this;
	}
	
	function set_uploadify_settings(setting,data)
	{
		if(config.uploadify_initialized)
		{
			$jq.uploadify.uploadifySettings(setting, data);
		}
		return this;
	}
	
	function set_uploadify_script_data(data)
	{
		set_uploadify_settings("scriptData", data);
		upload_meta = data;
		return this;
	} */
	
	function edit_document(meta)
	{
		var key
			, $element, $el
			, $inputs
			, value
			, i, len;
		
		$inputs = $(':input', $dialogs.edit_document);
		
		if(typeof meta === "object")
		{
			jQuery.each($inputs, function()
				{
					if(this.type === "radio" || this.type === "checkbox")
					{
						$(this).attr("checked", false);
					}
					else if(this.type.search(/select/) !== -1)
					{
						$(this).find("option").attr("selected", false);
					}
					else
					{
						$(this).val("");
					}
				});
				
			for(key in meta)
			{
				if(meta.hasOwnProperty(key))
				{
					$element = $('[name="' + key + '"]', $dialogs.edit_document);
					
					if($element.length === 1)
					{
						$element.val(meta[key]);
					}
					else if ($element.length > 1)
					{
						value = meta[key];
						
						if($.isArray(value))
						{
							len = value.length;
							for(i=0;i<len;i++)
							{
								$el = $element.filter("[value=" + value[i] + "]");
								
								if($el[0].type === "radio" || $el[0].type === "checkbox")
								{
									$el.attr("checked", true);
								}
								else if($[0].type.search(/select/) !== -1)
								{
									$el.attr("selected", true);
								}
							}
						}
						else
						{
							//lazy.. but should work
							$element.filter("[value=" + value + "]").attr("checked", true).attr("selected", true);
						}
					}
					
				}
			}
			$dialogs.edit_document.dialog("open");
		}
		else if(typeof meta === "string")
		{
			$dialogs.edit_document.dialog(meta);
		}
		
		return this;
	}

	function delete_document(meta) {
	    var key;

	    if (typeof meta === "object") {
	        for (key in meta) {
	            $('[name="' + key + '"]', $dialogs.delete_document).val(meta[key]);
	        }
	    }
	    console.log("opening delete file dialog");
	    $dialogs.delete_document.dialog("open");
	    return this;
	}

	function document_details_dialog()
	{
		return this;
	}
	
	function refresh_documents(meta)
	{
		var $container = (typeof meta.target === "string") ? $("#" + meta.target) : meta.target;
		meta.target = '';
		$.post(config.ajax, meta, function(data) 
			{
				if(data && data.status != -1)
				{
					$container.html(data.html);
					$("ul.filetree", $container).each(function() { $(this).treeview({animated: 400}); });
				}
			});
		return this;
	}
	
	function get_library()
	{
		return $library;
	}
	
	function associate_file()
	{
		return this;
	}
	
	return {ajax: config.ajax
			, add: add_document_dialog
			, associate: associate_file
			, init: init
			, dialogs: $dialogs
			, library: get_library
			, upload_meta: upload_meta
			, details: document_details_dialog
			, edit: edit_document
            , delete_file: delete_document
			, refresh: refresh_documents
		};
}