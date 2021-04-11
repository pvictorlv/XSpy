require(['/js/common.js'],
    function(common) {
        require(['jquery', 'tables', 'utils', 'toastr'],
            function($, tables, utils, toastr) {
                $(document).ready(function () {

                    $('#quickactions>[data-toggle="tooltip"]').tooltip({
                        container: "#quickactions"
                    });

                    $('#dataTable_wrapper').find('[data-toggle="tooltip"]').tooltip({
                        container: "#dataTable_wrapper"
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