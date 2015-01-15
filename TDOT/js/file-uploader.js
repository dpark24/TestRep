function documentFileUploader() {
    
    var $upload_container = $("#fileupload", $(".fileupload-buttonbar"))
        , upload_meta = {}
        , file_types = { all: ["Document and Image Files", "*.jpg;*.png;*.gif;*.pdf;*.jpeg;*.doc;*.ppt;*.docx;*.pptx;*.txt;*.xls;*.xlsx"]
                            , images: ["Images", "*.jpg;*.png;*.gif;*.jpeg"]
                            , documents: ["Documents", "*.pdf;*.doc;*.ppt;*.docx;*.pptx;*.txt;*.xls;*.xlsx"]
                            , presentations: ["Presentation Files", "*.pdf;*.ppt;*.pptx"]
                        };
        
    /**
     * Function to check if the filetype is valid. If it's not, it allows all files.
     * @author Paul Vidal, Stantec
     * @param {String} type Checks to see if the provided filetype is in the file_types variable, indicating if it's valid.
     * @returns If "type" is in "file_types", then the provided "type" is return, otherwise the string "all" is returned.
     */
    function valid_filetype(type) {
        //check if it's a valid file type
        //if not, we'll default to the all option
        return (type in file_types) ? type : "all";
    }
    
    function setUploadData(params) {
        
        if (typeof params === 'object')
        {
            upload_meta = params;
        }
        return this;
    }

    function initUploader() {
        var $form, upload_meta, form_validator, filetype = valid_filetype($("#upload-filetype").val());
        
        $("#filename").html("");
        $form = $('#upload-file-meta');
        if($upload_container.fileupload() !== "") {
            destroyUploader();
        }
        
        $upload_container.fileupload({
            dataType: 'json',
            formData: upload_meta,
            url: 'ajax.aspx',
            add: function (e, data) {
                if (data.files.length > 1) {
                    alert('uh-oh mulitple files selected!');
                } else {
                
                    $.each(data.files, function (index, file) {
    
                        $('.filename').text(file.name);
                        $('.submit-file').removeClass('disabled');
                        
                        $('.submit-file', $(".fileupload-buttonbar")).off().on('click', function(e) {
                            if (!$('.submit-file').hasClass('disabled')) {
                                
                                //add our user's data to the data object
                                if(typeof Validator === "function") {
                                    form_validator = new Validator($form, {force_overflow: true, short_messages: true, bind_onblur: false});
                                    
                                    form_validator.init();
                                    if(form_validator.is_form_valid()) {
                                        upload_meta = form_validator.get_form_data(false);
                                    } else {
                                        return false;
                                    }
                                } else {
                                    jQuery.each($(":input:visible", $form), function() {
                                            upload_meta[this.name] = $.trim($form.val());
                                        });
        
                                }
                                
                                upload_meta.action = "upload-document";
                                data.formData = upload_meta;
                                
                                $(".fileupload-loading").show();
                                e.preventDefault();
                                data.submit();
                            }
                        });
                    });
                
                }
                return false;
            },
            progress: function (e, data) {
                var progress = parseInt(data.loaded / data.total * 100, 10);
                $(".progress-animated").css({"width": progress + "%"});
            },
            done: function (e, data) {
                $(".progress-animated").css({"width":"100%"});
                $(".fileupload-loading").hide();
                $('#dialog-upload-document').dialog("close");
        
            }
        });

    }

    function destroyUploader() {
        try {
            $upload_container.fileupload('destroy');
        } catch(e) {
        }
        
        return this;
    }
    
    function fileSelected(e, data) {
        
    }

    return { setUploadData: setUploadData,
        initUploader : initUploader,
        destroyUploader : destroyUploader
    };
}
