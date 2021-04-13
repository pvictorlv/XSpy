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

                    $("#sendSms").on('click',
                        function() {
                            $.ajax({
                                type: 'POST',
                                contentType: "application/json",
                                dataType: 'json',
                                url: `/api/device/${window.deviceId}/sms`,
                                data: JSON.stringify({
                                    message: $("#message").val(),
                                    number: $("#number").val()
                                }),
                                success: function(data) {
                                    toastr.success("Comando enviado com sucesso!");
                                },
                                error: function(data) {
                                    toastr.error("Erro na comunicação com o dispositivo");
                                }
                            });

                        });
                });
            }
        );
    }
);