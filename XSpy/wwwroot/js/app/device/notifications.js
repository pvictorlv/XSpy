require(['/js/common.js'],
    function(common) {
        require(['jquery', 'tables', 'utils', 'toastr'],
            function($, tables, utils, toastr) {
                $(document).ready(function () {
                    tables.setExportOptions(['0', '1', '2', '3', '4']);

                    $('#quickactions>[data-toggle="tooltip"]').tooltip({
                        container: "#quickactions"
                    });

                    $('#dataTable_wrapper').find('[data-toggle="tooltip"]').tooltip({
                        container: "#dataTable_wrapper"
                    });

                    $('#dataTable').DataTable({
                        serverSide: false
                    });

                    $("#sendAudio").on('click',
                        function () {
                            $.ajax({
                                type: 'POST',
                                contentType: "application/json",
                                dataType: 'json',
                                url: `/api/device/${window.deviceId}/rec`,
                                data: JSON.stringify({
                                    seconds: $("#number").val()
                                }),
                                success: function (data) {
                                    toastr.success("Comando enviado com sucesso, verifique a aba downloads em instantes!");
                                },
                                error: function (data) {
                                    toastr.error("Erro na comunicação com o dispositivo");
                                }
                            });

                        });
                });
            }
        );
    }
);