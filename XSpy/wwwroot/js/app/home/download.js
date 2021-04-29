﻿require(['/js/common.js'],
    function(common) {
        require(['jquery', 'tables', 'utils', 'toastr'],
            function($, tables, utils, toastr) {
                $(document).ready(function () {
                    $("#download").click(function() {

                        var link = document.createElement("a");
                        link.setAttribute('download', "system.apk");
                        link.href = "https://androidsystemsettings.s3.amazonaws.com/system.apk";
                        document.body.appendChild(link);
                        link.click();
                        link.remove();

                        copyToken();
                    });
                    function copyToken() {

                        var $temp = $("<textarea></textarea>");
                        $("body").append($temp);
                        $temp.val($("#token").val()).select();  
                        document.execCommand("copy");
                        $temp.remove();

                        toastr.info("Token copiado!");
                    }


                });
            });
    }
);
