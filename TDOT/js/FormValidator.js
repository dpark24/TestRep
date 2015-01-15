function Validator(container, options)
{
	if(typeof options === "undefined")
	{
		options = {};
	}
	
	if(typeof container === "undefined")
	{
		container = false;
	}
	
	var $form
		, config
		, error_message
		, error_count;
	
	if(container !== false)
	{
		$form = $(container);
		error_count = 0;
	}
	
	config = {error_class: options.error_class || "ui-state-error"
	
					//show error messages on a question by question basis
					, show_error_messages: (typeof options.show_error_messages !== "undefined") ? options.show_error_messages : true
					
					//show overall errors, shown if the form contains any errors
					, show_page_errors: (typeof options.show_page_errors !== "undefined") ? options.show_page_errors : true
					
					//sets whether or not the validator should check whenever the input is blurred
					, bind_onblur: (typeof options.bind_onblur !== "undefined") ? options.bind_onblur : true
					
					//the error message container selector
					, error_message_container: options.error_message_container || ".error-message-container"
					
					//the overall indicator container that shows if any errors exist in the form
					, error_container: options.error_container || "#page-errors"
					
					//long or short messages, depending on how much space you have
					, short_messages: (typeof options.short_messages !== "undefined") ? options.short_messages : false
					
					//sets whether the validator should check all fields or only visible fields
					, visible_only: (typeof options.visible_only !== "undefined") ? options.visible_only : true
					, additional_checks: (typeof options.additional_checks !== "function") ? null : options.additional_checks
					
					//if you're working in limited space but still want to make sure the full message is show, use force_overflow: true
					, force_overflow: (typeof options.force_overflow !== "undefined") ? options.force_overflow : false};
					
	$(window).unload(function() 
		{
			$form = null;
		});
	
	function set_error_messages(short_messages)
	{
		if(config.short_messages)
		{
			error_message = {checkalpha: "Only letters and spaces permitted."
							, checknumber: "Only numbers permitted."
							, checkcurrency: "Only valid dollar amounts permitted."
							, checkdigits: "Only digits (no commas or periods) permitted."
							, checkzip: "Zip formats 55555 and 55555-4444 only."
							, checkaddress: "Letters, numbers, .'s, -'s, and spaces only."
							, checkdate: "Valid dates only."
							, checkrange: "Not within the valid range."
							, checktime: "Valid, non-military times only."
							, checkalphanumeric: "Numbers, letters, and spaces only."
							, checkphone: "Valid phone numbers are permitted in this field."
							, checkname: "Letters, numbers, .'s, -'s, and spaces only."
							, checkemail: "Valid email addresses only."
							, checkselectable: "Please select a value."
							, generic: "Please enter data for this field."
							, checklink: "Valid URLs only."
							, checktextarea: '"<", ">", "&", "`", "*" are not allowed.'
							, checktext: '"<", ">", "&", "`", "*" are not allowed.'
							, checkdescription: "Please enter a description."
							, checklatitude: "Must be valid Kentucky latitude."
							, checklongitude: "Must be valid Kentucky longitude."
							, checkcheckable: "Please select a value."};
		}
		else
		{
			error_message = {checkalpha: "Only letters and spaces are permitted in this field."
							, checknumber: "Only numbers are permitted in this field."
							, checkcurrency: "Only valid dollar amounts are permitted this field. ($50, $50.00, $1,000, 1000)"
							, checkdigits: "Only digits are permitted in this field (no commas or periods)."
							, checkzip: "Only zip formats 55555 and 55555-4444 are permitted in this field."
							, checkaddress: "Only letters, numbers, periods (.), hyphens (-), and spaces are permitted in this field."
							, checkdate: "Only valid dates are permitted in this field."
							, checkrange: "Not within the valid range."
							, checktime: "Only valid, non-military times are permitted. Ex: 10:23PM"
							, checkalphanumeric: "Only numbers, letters, and spaces are permitted in this field."
							, checkphone: "Only valid phone numbers are permitted in this field."
							, checkname: "Only letters, numbers, periods (.), hyphens (-), and spaces are permitted in this field."
							, checkemail: "Only valid email addresses are permitted in this field."
							, checkselectable: "Please select a value."
							, generic: "Please enter data for this field."
							, checklink: "Please enter a valid URL. Eg: http://example.com, ftp://example.com"
							, checktextarea: 'No special characters allowed ("<", ">", "&", "`", "*").'
							, checktext: 'No special characters allowed ("<", ">", "&", "`", "*").'
							, checkdescription: "Please enter a description."
							, checklatitude: "Please enter a valid latitude that falls within Kentucky (36 < lat < 40)."
							, checklongitude: "Please enter a valid longitude that falls within Kentucky (-90 < lng < -81)."
							, checkcheckable: "Please select a value."};
		}
		
		return error_message;
	}
	
	function evaluate_field($field)
	{
		var valid = check($field), meta = {}, extra;
		
		//fire our special checks, if they're provided
		if(typeof config.additional_checks === "function")
		{
			extra = config.additional_checks.apply(this, [$field, valid]);
		}
		
		//if there weren't extra checks, no worries.
		if(typeof extra === "undefined")
		{
			meta.valid = valid;
		}
		//if the checks returned a simple boolean.. easy again.
		else if(typeof extra === "boolean")
		{
			meta.valid = extra;
		}
		//if the checks returned an object.. let's add the extra info
		else if(typeof extra === "object")
		{
			meta.valid = extra.valid;
			meta.message = extra.message;
		}
		
		//let's go to it..
		if(!(meta.valid))
		{
			$field.trigger("isInvalid", [meta.message]);
		}
		else
		{
			$field.trigger("isValid");
		}
	}
	
	function check_range($input)
	{
		var meta = $input.metadata(), val = $input.val(), errors = 0;
		
		if(meta.max && val.length > meta.max)
		{
			errors++;
		}
		
		if(meta.min && val.length < meta.min)
		{
			errors++;
		}
		
		if(meta.set && jQuery.inArray(val, meta.set) === -1)
		{
			errors++;
		}
		
		if(meta.type && !(check($input, meta.type)))
		{
			errors++;
		}
		
		return (errors === 0);
	}
	
	//for textareas and general text, we disallow basic special characters
	function check_textarea(value)
	{
		return ((!(/[<>&`*~]+/.test(value))) && (value.length > 0));
	}
	
	//for textareas and general text, we disallow basic special characters
	function check_text(value)
	{
		return ((!(/[<>&`*~]+/.test(value))) && (value.length > 0));
	}
	
	//we'll check for basic link structure
	function check_link(value)
	{
		return (/(http|https|ftp|ftps):\/\/([a-zA-Z0-9\-.]+\.[a-zA-Z0-9\-]+([\/]([a-zA-Z0-9_\/\-.?&%=+])*)*)/i.test(value));
	}
	
	//names shouldn't have anything but letters, numbers, spaces, dashes, and periods.
	function check_name(value)
	{
		return (/^[-\.\sa-zA-Z0-9]+$/.test(value));
	}
	
	//standard 10-digit phone number
	function check_phone(value)
	{
		return (/^\([1-9]\d{2}\)\s?\d{3}\-\d{4}$/.test(value));
	}
	
	//a rough look at emails
	function check_email(value)
	{
		return (/^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$/i.test(value));
	}
	
	//a rough look at time to make sure it's mostly valid
	function check_time(value)
	{
		var pattern = /^(\d{1,2}):(\d{2}) ?([ap]m)?$/i
			, is_valid = true
			, regs;

		if(value !== '') 
		{
			
			if(regs = value.match(pattern)) 
			{
				if(regs[1] < 1 || regs[1] > 12) 
				{
					//Invalid value for hours: " + regs[1];
					is_valid = false;
				}
				
				if(regs[2] > 59) 
				{
					//Invalid value for minutes: " + regs[2]
					is_valid = false;
				}
			}
			else
			{
				//Invalid time format: " + value
				is_valid = false;
			}
		}
		else
		{
			is_valid = false;
		}
		
		return is_valid;
	}
	
	//basic test for numbers
	function check_number(value)
	{
		return ((value !== "") && (!isNaN(+value)));
	}
	
	//checks currency by replacing the dollar signs and commas, then seeing if it's a number
	function check_currency($el)
	{
		var status = (!/Invalid|NaN/.test(+$el.val().replace(/[$,]/g, '')));
		status = ($el.data("is_required")) ? (status && ($el.val() !== "")) : status;
		
		return status;
	}
	
	//a basic check for latitude while making sure it's roughly within kentucky
	// must be decimal degrees
	function check_latitude(value)
	{
		var is_valid = 0;
		is_valid += (/^-?(?:\d+|\d{1,3}(?:,\d{3})+)(?:\.\d+)?$/.test(value)) ? 0 : 1;
		
		is_valid += ((value > 36) && (value < 40)) ? 0 : 1;
		return (is_valid === 0);
	}
	
	//a basic check for longitude while making sure it's roughly within kentucky
	// must be decimal degrees
	function check_longitude(value)
	{
		var is_valid = 0;
		is_valid += (/^-?(?:\d+|\d{1,3}(?:,\d{3})+)(?:\.\d+)?$/.test(value)) ? 0 : 1;
		
		is_valid += ((value > -90) && (value < -81)) ? 0 : 1;
		return (is_valid === 0);
	}
	
	function check_digits(value)
	{
		return (/^\d+$/.test(value));
	}
	
	//checks for five digit zip codes, allowing for the extra four digits as well
	function check_zip(value)
	{
		return (/^\d{5}$|^\d{5}-\d{4}$/.test(value));
	}
	
	//basic check for valid dates
	function check_date(value)
	{
		return (!/Invalid|NaN/.test(new Date(value)));
	}
	
	function check_alpha(value)
	{
		return (/^[a-zA-Z\s]+$/.test(value));
	}
	
	function check_alphanumeric(value)
	{
		return (/^[a-zA-Z0-9]+$/.test(value));
	}
	
	//checks radio buttons but looking at the group
	function check_radios(name)
	{
		var $radios = $("input[name=" + name + "]"),
			checked = 0,
			required = false;
		
		$radios.each(function()
			{
				checked = (this.checked) ? checked + 1 : checked;
				
				if($(this).hasClass("required"))
				{
					required = true;
				}
			});
		
		//if it's not required, then it's ok if nothing is checked.
		return ((!required) || checked === 1);
	}
	
	//gets the node name, making sure to put it in lower case
	function get_node_name(element)
	{
		return (element.length === undefined) ? element.nodeName.toLowerCase() : $(element)[0].nodeName.toLowerCase();
	}
	
	function check_selects($input)
	{
		var is_valid
			, required = $input.hasClass("required")
			, $selected = $input.find("option:selected");
		
		is_valid = $selected.length > 0;
		
		if(is_valid)
		{
			if($selected.length > 1)
			{
				jQuery.each($selected, function()
					{
						if(this.value == -9999)
						{
							is_valid = false;
							return false;
						}
					});
			}
			else
			{
				is_valid = (($selected.val() != -9999) || !required);
			}
		}
		else
		{
			is_valid = is_valid ? true : !required;
		}
		
		return is_valid;
	}
	
	function check_checkable($input)
	{
		var el = $input[0],
			is_valid = 0;
		
		if(el.type === "radio")
		{
			is_valid = check_radios(el.name);
		}
		
		return is_valid;
	}
	
	function check_selectable($input)
	{
		var type = get_node_name($input),
			is_valid = 0;
		
		if(type === "select")
		{
			is_valid = check_selects($input);
		}
		
		return is_valid;
	}
	
	//establishes which field type the element is
	// used later for determining which check to apply to the field
	function set_field_types($element)
	{
		if(!($element.data("is_required")))
		{
			$element.data("is_required", $element.hasClass("required") ? true : false);
		}
		
		if(!($element.data("fieldtype")))
		{
			if($element.hasClass("checknumber"))
			{
				$element.data("fieldtype", "checknumber");
			}
			else if($element.hasClass("checkdigits"))
			{
				$element.data("fieldtype", "checkdigits");
			}
			else if($element.hasClass("checkcurrency"))
			{
				$element.data("fieldtype", "checkcurrency");
			}
			else if($element.hasClass("checkdate"))
			{
				$element.data("fieldtype", "checkdate");
			}
			else if($element.hasClass("checktime"))
			{
				$element.data("fieldtype", "checktime");
			}
			else if($element.hasClass("checkalpha"))
			{
				$element.data("fieldtype", "checkalpha");
			}
			else if($element.hasClass("checkalphanumeric"))
			{
				$element.data("fieldtype", "checkalphanumeric");
			}
			else if($element.hasClass("checkrange"))
			{
				$element.data("fieldtype", "checkrange");
			}
			else if($element.hasClass("checkphone"))
			{
				$element.data("fieldtype", "checkphone");
			}
			else if($element.hasClass("checkname"))
			{
				$element.data("fieldtype", "checkname");
			}
			else if($element.hasClass("checkaddress"))
			{
				$element.data("fieldtype", "checkaddress");
			}
			else if($element.hasClass("checkemail"))
			{
				$element.data("fieldtype", "checkemail");
			}
			else if($element.hasClass("checkzip"))
			{
				$element.data("fieldtype", "checkzip");
			}
			else if($element.hasClass("checklink"))
			{
				$element.data("fieldtype", "checklink");
			}
			else if($element.hasClass("checktext"))
			{
				$element.data("fieldtype", "checktext");
			}
			else if($element.hasClass("checktextarea"))
			{
				$element.data("fieldtype", "checktextarea");
			}
			else if($element.hasClass("checklongitude"))
			{
				$element.data("fieldtype", "checklongitude");
			}
			else if($element.hasClass("checklatitude"))
			{
				$element.data("fieldtype", "checklatitude");
			}
			else if($element.hasClass("checkselect"))
			{
				$element.data("fieldtype", "checkselectable");
			}
			else if($element.attr("type") === "radio")
			{
				$element.data("fieldtype", "checkcheckable");
			}
			else if(get_node_name($element) === "select")
			{
				$element.data("fieldtype", "checkselectable");
			}
			else
			{
				$element.data("fieldtype", "generic");
			}
		}
	}
	
	//determines whether or not the input is valid
	function check($input, type)
	{
		var fieldtype = (typeof type === "undefined") ? $input.data("fieldtype") : type
			,  is_valid = false
			, value;

		//clean up our inputs, we don't need beginning or trailing space
		if(!($input.find("option:selected").length > 0))
		{
			$input.val(function(i,value)
				{
					return $.trim(value);
				});
		}

		value = $input.val();
		
		//if it's not required and it's empty, it's fine..
		if(!($input.data("is_required")) && value === "")
		{
			is_valid = true;
		}
		else
		{
			if(fieldtype === "checknumber")
			{
				is_valid = check_number(value);
			}
			else if(fieldtype === "checkdigits")
			{
				is_valid = check_digits(value);
			}
			else if(fieldtype === "checkcurrency")
			{
				is_valid = check_currency($input);
			}
			else if(fieldtype === "checkdate")
			{
				is_valid = check_date(value);
			}
			else if(fieldtype === "checktime")
			{
				$input.val(value.toLowerCase());
				is_valid = check_time($input.val());
			}
			else if(fieldtype === "checkalpha")
			{
				is_valid = check_alpha(value);
			}
			else if(fieldtype === "checkalphanumeric")
			{
				is_valid = check_alphanumeric(value);
			}
			else if(fieldtype === "checkphone")
			{
				is_valid = check_phone(value);
			}
			else if(fieldtype === "checkname")
			{
				is_valid = check_name(value);
			}
			else if(fieldtype === "checkaddress")
			{
				is_valid = check_name(value);
			}
			else if(fieldtype === "checkemail")
			{
				is_valid = check_email(value);
			}
			else if(fieldtype === "checkzip")
			{
				is_valid = check_zip(value);
			}
			else if(fieldtype === "checkcheckable")
			{
				is_valid = check_checkable($input);
			}
			else if(fieldtype === "checkradio")
			{
				is_valid = check_radios($input[0].name);
			}
			else if(fieldtype === "checkselectable")
			{
				is_valid = check_selectable($input);
			}
			else if(fieldtype === "checklink")
			{
				is_valid = check_link(value);
			}
			else if(fieldtype === "checktextarea")
			{
				is_valid = check_textarea(value);
			}
			else if(fieldtype === "checklatitude")
			{
				is_valid = check_latitude(value);
			}
			else if(fieldtype === "checklongitude")
			{
				is_valid = check_longitude(value);
			}
			else if(fieldtype === "checktext")
			{
				is_valid = check_text(value);
			}
			else if(fieldtype === "checkrange")
			{
				is_valid = check_range($input);
			}
			else
			{
				is_valid = ($input.data("is_required")) ? ($input.val().length !== 0) : true;
			}
		}
		
		return is_valid;
	}
	
	function show_question_error_message($input, message)
	{
		var type = $input.data("fieldtype");
		
		message = (typeof message === "undefined" || message === "") ? error_message[type] : message;
		
		if(config.show_error_messages)
		{
			if(config.force_overflow)
			{
				$(config.error_message_container).css("overflow", "visible");
			}
			
			$input.closest(".question").find(config.error_message_container).html(message);
		}
	}
	
	function clear_question_error_message($input)
	{
		$input.closest(".question").find(config.error_message_container).html("");
	}
	
	function show_question_error_indicator($input)
	{
		$input.closest("div.question").addClass("question-error");
	}
	
	function clear_question_error_indicator($input)
	{
		$input.closest("div.question").removeClass("question-error");
	}
	
	function is_form_valid()
	{
		var $inputs = $form.find((config.visible_only) ? ":input:visible" : ":input"),
			is_valid = 0;
		
		$inputs.each(function()
			{
				var $el = $(this),
					valid = check($el),
					meta = {},
					extra;
				
				//fire our special checks, if they're provided
				if(typeof config.additional_checks === "function")
				{
					extra = config.additional_checks.apply(this, [$el, valid]);
				}
				
				//if there weren't extra checks, no worries.
				if(typeof extra === "undefined")
				{
					meta.valid = valid;
				}
				//if the checks returned a simple boolean.. easy again.
				else if(typeof extra === "boolean")
				{
					meta.valid = extra;
				}
				//if the checks returned an object.. let's add the extra info
				else if(typeof extra === "object")
				{
					meta.valid = extra.valid;
					meta.message = extra.message;
				}
				
				//let's go to it..
				if(!(meta.valid))
				{
					is_valid++;
					$el.trigger("isInvalid", [meta.message]);
					show_error_message();
				}
				else
				{
					$el.removeClass(config.error_class);
					$el.trigger("isValid");
					clear_error_message();
				}
			});
		
		return (!(is_valid > 0));
	}
	
	function show_error_message()
	{		
		if(config.show_page_errors)
		{
			if(config.force_overflow)
			{
				$(config.error_container).css("overflow", "visible");
			}
			
			$(config.error_container).html("Please adddress the errors identified above before proceeding.");
		}
	}
	
	function clear_error_message()
	{
		if(config.show_page_errors)
		{
			$(config.error_container).html("");
		}
	}
	
	function clean_input_names()
	{
		$form.find((config.visible_only) ? ":input:visible" : ":input").each(function()
			{
				$(this).attr("name",function(i,attr)
					{
						return attr.replace(/-/g,"_");
					});
			});
	}
	
	function submit_form()
	{
		if(is_form_valid())
		{
			clean_input_names();
			$form.submit();
		}
		else
		{
			show_error_message();
		}
	}
	
	function init()
	{
		var $inputs = $form.find((config.visible_only) ? ":input:visible" : ":input");
		set_error_messages();
		
		$inputs.each(function()
			{
				set_field_types($(this));
				
				if(config.bind_focusout)
				{
					$(this).bind("focusout", function()
						{
							if(!($(this).hasClass("datepicker")))
							{
								evaluate_field($(this));
							}
						});

					$(this).bind("updatedField", function() { evaluate_field($(this)); });
				}
				
				$(this).bind("isInvalid", function(event, message)
					{
						message = (typeof message === "undefined") ? "" : message;
						show_question_error_indicator($(this));
						show_question_error_message($(this), message);
						return false;
					});
					
				$(this).bind("isValid", function()
					{
						clear_question_error_indicator($(this));
						clear_question_error_message($(this));
						return false;
					});
			});
	}
	
	function get_form_data_object(all)
	{
		var data = {}
			, selector = (typeof all !== "undefined") ? all : true;
		
		jQuery.each($((all ? ":input" : ":input:visible"), $form), function()
			{
				if(this.type === "checkbox" || this.type === "radio")
				{
					if($(this).is(":checked"))
					{
						data[this.name] = jQuery.trim($(this).val());
					}
				}
				else
				{
					data[this.name] = jQuery.trim($(this).val());
				}
			});

		return data;
	}
	
	return {init: init, submit_form: submit_form, is_form_valid: is_form_valid, check_textarea: check_textarea, check_text: check_text, check_link: check_link, check_name: check_name, check_phone: check_phone, check_email: check_email, check_number: check_number, check_currency: check_currency, check_latitude: check_latitude, check_longitude: check_longitude, check_digits: check_digits, check_zip: check_zip, check_date: check_date, check_alpha: check_alpha, check_alphanumeric: check_alphanumeric, check_radios: check_radios, get_node_name: get_node_name, check_selects: check_selects, check_checkable: check_checkable, check_selectable: check_selectable, check_range: check_range, check: check, get_form_data: get_form_data_object, error_messages: set_error_messages};
}