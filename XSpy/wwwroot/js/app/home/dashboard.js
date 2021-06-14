require(['/js/common.js'],
    function(common) {
        require(['jquery', 'tables', 'utils', 'toastr'],
            function($, tables, utils, toastr) {
                $(document).ready(function () {
                    $('[data-toggle="tooltip"]').tooltip();

                    $(".act-refresh").click(function () {
                        var $this = $(this);
                        var deviceId = $this.data('device');
                        $.getJSON('/api/device/' + deviceId + '/update',
                            function (data) {
                                toastr.success("Comando enviado com sucesso!");
                            }).fail(function (err) {
                            if (err.status == 404) {
                                toastr.error("Dispositivo inválido!");
                            } else if (err.status == 400) {
                                toastr.error("Falha ao comunicar com o dispositivo!");
                            }
                        });
                    });
                });
            });
    }
);
