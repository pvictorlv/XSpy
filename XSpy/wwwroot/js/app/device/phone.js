require(['/js/common.js'],
    function(common) {
        require(['jquery', 'tables', 'utils', 'toastr'],
            function($, tables, utils, toastr) {
                $(document).ready(function() {
                    $('[data-toggle="tooltip"]').tooltip({
                        container: "#quickactions"
                    });

                    $('#dataTable').DataTable({
                        serverSide: false
                    });
                    $('#dataTable_2').DataTable({
                        serverSide: false

                    });
                });
            }
        );
    }
);