require(['/js/common.js'],
    function(common) {
        require(['jquery', 'tables', 'utils', 'gallery'],
            function ($, tables, utils, gallery) {
                $(document).ready(function() {

                    $('#quickactions>[data-toggle="tooltip"]').tooltip({
                        container: "#quickactions"
                    });

                    $('[data-toggle="tooltip"]').tooltip();

                    $(document).on('click', '[data-toggle="lightbox"]', function (event) {
                        event.preventDefault();
                        $(this).ekkoLightbox({
                            alwaysShowClose: true
                        });
                    });

                    
                });
            }
        );
    }
);